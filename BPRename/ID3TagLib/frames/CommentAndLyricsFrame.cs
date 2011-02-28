using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class CommentAndLyricsFrame: TextFrame, ISelectableTextEncoding, ISelectableLanguage, IEquatable<CommentAndLyricsFrame> {
		
		private TextEncoding encoding;
		private string language;
		private string description;
		
		protected internal CommentAndLyricsFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			language = String.Empty;
			description = String.Empty;
		}
		
		public TextEncoding Encoding {
			get {
				return encoding;
			}

			set {
				if (!Enum.IsDefined(typeof(TextEncoding), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(TextEncoding));
				}
				encoding = value;
			}
		}
		
		public string Language {
			get {
				return language;
			}
			
			set {
				FrameUtil.CheckLanguage(value);
				language = value;
			}
		}
		
		public string Description {
            get {
                return description;
            }
            
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
				description = value;
            }
        }
        
		public override bool Equals(object obj) {
			return Equals(obj as CommentAndLyricsFrame);
		}

		public bool Equals(CommentAndLyricsFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && encoding == other.encoding && language.Equals(other.language) && description.Equals(other.description);
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= (int)encoding;
			hashCode ^= language.GetHashCode();
			hashCode ^= description.GetHashCode();
			
			return hashCode;
		}
		
		public override object Clone() {
			CommentAndLyricsFrame obj = (CommentAndLyricsFrame)base.Clone();

			obj.encoding = encoding;
			obj.language = language;
			obj.description = description;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());

			sBuild.Append("Language: ");
			sBuild.AppendLine(language);
			
			sBuild.Append("Description: ");
			sBuild.AppendLine(description);
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e = StringEncoder.GetEncoding(encoding);
			byte[] rawData;
			int currentIndex, rawDataLength;
			
			rawDataLength = 4;
			rawDataLength += StringEncoder.ByteCount(description, e, true);
			rawDataLength += StringEncoder.ByteCount(Text, e, false);
			
			rawData = new byte[rawDataLength];
			
			rawData[0] = (byte)encoding;
			StringEncoder.WriteLatin1String(rawData, 1, 3, language);
			currentIndex = 4;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, description, e, true);
			StringEncoder.WriteString(rawData, currentIndex, Text, e, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding e;
			int bytesRead;
			
			StringDecoder.ParseEncodingByte(rawData, 0, 4, out e, out encoding);
			language = StringDecoder.DecodeLatin1String(rawData, 1, 3);
			description = StringDecoder.DecodeString(rawData, 4, -1, e, out bytesRead);
			
			if (rawData.Length < 4 + bytesRead) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			Text = StringDecoder.DecodeString(rawData, 4 + bytesRead, -1, e, out bytesRead);
		}
	}	
}