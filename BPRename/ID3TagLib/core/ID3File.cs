using System;
using System.ComponentModel;
using System.Text;
using System.IO;

namespace ID3TagLib {
	
	[Serializable]
	public class ID3File {

        private ID3v1Tag id3v1Tag;
        private ID3v2Tag id3v2Tag;
		
		private object tag;
		
		public ID3File() { }
		
		public ID3File(string fileName) {
			Load(fileName);
		}
		
		public ID3File(FileInfo fileInfo) {
			Load(fileInfo);
		}
		
		public ID3File(Stream s) {
			Load(s);
		}

        public ID3v1Tag ID3v1Tag {
            get {
                return id3v1Tag;
            }
            
            set {
                id3v1Tag = value;
            }
        }

        public ID3v2Tag ID3v2Tag {
            get {
                return id3v2Tag;
            }
            
            set {
                id3v2Tag = value;
            }
        }
		
		public object Tag {
			get {
				return tag;
			}
			
			set {
				tag = value;
			}
		}
		
		public void Load(string fileName) {
			Load(fileName, ReadMode.SkipCorruptParts);
		}
		
		public void Load(string fileName, ReadMode mode) {
			Stream s = null;
			
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			
			try {
				s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				Load(s, mode);
			} finally {
				if (s != null) s.Close();
			}
		}
		
		public void Load(FileInfo fileInfo) {
			Load(fileInfo, ReadMode.SkipCorruptParts);
		}
		
		public void Load(FileInfo fileInfo, ReadMode mode) {
			Stream s = null;
			
			if (fileInfo == null) {
				throw new ArgumentNullException("fileInfo");
			}
			
			try {
				s = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
				Load(s, mode);
			} finally {
				if (s != null) s.Close();
			}
		}
		
		/* sucht in stream s an der derzeitigen Position nach einem v2 Tag und
		   falls s.CanSeek true ist, so wird an das Streamende gegangen und ein
		   v1 Tag gesucht. s.Position wird dabei nicht wieder zurückgesetzt!
		 */
        public void Load(Stream s) {
			Load(s, ReadMode.SkipCorruptParts);
		}
		
		public void Load(Stream s, ReadMode mode) {
			if (s == null) {
				throw new ArgumentNullException("s");
			}
			if (!s.CanRead) {
				throw new ArgumentException("Cannot read from Stream.", "s");
			}
			if (!Enum.IsDefined(typeof(ReadMode), mode)) {
                throw new InvalidEnumArgumentException("mode", (int)mode, typeof(ReadMode));
            }
			
			try {
				id3v2Tag = ID3v2TagDecoder.DecodeTag(s, mode);
			} catch {
				if (mode == ReadMode.ThrowOnError) {
					throw;
				}
				id3v2Tag = null;
			}
			
			if (s.CanSeek && s.Length >= ID3v1Tag.TagLength) {
				s.Seek(-ID3v1Tag.TagLength, SeekOrigin.End);
				try {
					id3v1Tag = ID3v1TagDecoder.DecodeTag(s);
				} catch {
					if (mode == ReadMode.ThrowOnError) {
						throw;
					}
					id3v1Tag = null;
				}
			} else {
				id3v1Tag = null;
			}
        }
		
		public void Save(string fileName) {
			Stream s = null;
			
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			
			try {
				s = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
				Save(s);
			} finally {
				if (s != null) s.Close();
			}
		}
		
		public void Save(FileInfo fileInfo) {
			Stream s = null;
			
			if (fileInfo == null) {
				throw new ArgumentNullException("fileInfo");
			}
			
			try {
				s = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
				Save(s);
			} finally {
				if (s != null) s.Close();
			}
		}

        public void Save(Stream s) {
			if (s == null) {
				throw new ArgumentNullException("s");
			}
			if (!s.CanWrite) {
				throw new ArgumentException("Cannot write to Stream.", "s");
			}
			if (!s.CanSeek) {
				throw new ArgumentException("Cannot seek in Stream.", "s");
			}
			
			Writev2Tag(s, id3v2Tag);
			Writev1Tag(s, id3v1Tag);
        }
		
		private static void Writev2Tag(Stream s, ID3v2Tag id3v2tag) {
			byte[] encodedTag;
			byte[] tagHeader = new byte[ID3v2TagDecoder.TagHeaderSize];
			bool streamHasID3v2Tag;
			int id3v2TagSize;
			
			s.Seek(0, SeekOrigin.Begin);
			s.Read(tagHeader, 0, ID3v2TagDecoder.TagHeaderSize);
			
			streamHasID3v2Tag = ID3v2TagDecoder.HasID3v2Tag(tagHeader);
			id3v2TagSize = ID3v2TagDecoder.GetID3v2TagSize(tagHeader);
			if (id3v2tag != null) {
				/* write tag to stream; check if there is already one. */
				encodedTag = ID3v2TagEncoder.EncodeTag(id3v2tag);
				if (streamHasID3v2Tag) {
					/* stream already contains a tag; check if new tag fits into old one */
					if (encodedTag.Length == id3v2TagSize) {
						s.Seek(0, SeekOrigin.Begin);
					} else {
						/* delete the old tag and leave enough space for the new one */
						StreamCopy(s, id3v2TagSize, encodedTag.Length);
					}
				} else {
					/* stream does not contain a tag; make space for the new one */
					StreamCopy(s, 0, encodedTag.Length);
				}
				s.Write(encodedTag, 0, encodedTag.Length);

			} else {
				/* delete tag in stream if there is one */
				if (streamHasID3v2Tag) {
					StreamCopy(s, id3v2TagSize, 0);
				}
			}
		}
		
		private static void StreamCopy(Stream src, int srcIndex, int additionalBytes) {
			string tmpFileName = Path.GetTempFileName();
			Stream fs = null;
			byte[] buffer = new byte[4096];
			int bytesRead;
			
			try {
				fs = File.Open(tmpFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			
				if (additionalBytes > 0) {
					fs.SetLength(additionalBytes);
					fs.Seek(additionalBytes, SeekOrigin.Begin);
				}
				src.Seek(srcIndex, SeekOrigin.Begin);
				
				/* copy to new file */
				while ((bytesRead = src.Read(buffer, 0, buffer.Length)) > 0) {
					fs.Write(buffer, 0, bytesRead);
				}

				src.Seek(0, SeekOrigin.Begin);
				src.SetLength(fs.Length);
				fs.Seek(0, SeekOrigin.Begin);
				
				/* copy to stream */
				while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0) {
					src.Write(buffer, 0, bytesRead);
				}
				
				src.Seek(0, SeekOrigin.Begin);
				
			} finally {
				if (fs != null) fs.Close();
				if (File.Exists(tmpFileName)) File.Delete(tmpFileName);
			}
		}
		
		private static void Writev1Tag(Stream s, ID3v1Tag id3v1tag) {
			byte[] encodedTag;
			bool streamHasID3v1Tag;
			
			if (s.Length >= ID3v1Tag.TagLength) {
				byte[] tagHeader = new byte[ID3v1TagDecoder.TagHeaderSize];
				s.Seek(-ID3v1Tag.TagLength, SeekOrigin.End);
				s.Read(tagHeader, 0, ID3v1TagDecoder.TagHeaderSize);
				streamHasID3v1Tag = ID3v1TagDecoder.HasID3v1Tag(tagHeader);
			} else {
				streamHasID3v1Tag = false;
			}
			
			if (id3v1tag != null) {
				/* write/overwrite Tag in Stream */
				encodedTag = ID3v1TagEncoder.EncodeTag(id3v1tag);
				
				s.Seek(streamHasID3v1Tag ? -ID3v1Tag.TagLength : 0, SeekOrigin.End);
				s.Write(encodedTag, 0, encodedTag.Length);
			} else {
				/* delete Tag in Stream if exists */
				if (streamHasID3v1Tag) {
					s.SetLength(s.Length - ID3v1Tag.TagLength);
				}
			}
		}
		
		public override string ToString() {
			StringBuilder sBuild = new StringBuilder(1024);
			
			sBuild.AppendLine("ID3v2Tag:");
			sBuild.AppendLine((id3v2Tag == null ? "No ID3v2 Tag available." : id3v2Tag.ToString()));
			sBuild.AppendLine("ID3v1Tag:");
			sBuild.Append((id3v1Tag == null ? "No ID3v1 Tag available." : id3v1Tag.ToString()));
			
			return sBuild.ToString();
		}
    }//end ID3File
}