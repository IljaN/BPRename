using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public class EncapsulatedObjectFrame:  Frame, ISelectableTextEncoding, IEquatable<EncapsulatedObjectFrame> {
		
		private TextEncoding encoding;
		private string mimeType;
		private string fileName;
		private string contentDescription;
		private byte[] data;
		
		protected internal EncapsulatedObjectFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			mimeType = String.Empty;
			fileName = String.Empty;
			contentDescription = String.Empty;
			data = new byte[0];
        }
		
		/* Sets the TextEncoding for FileName(> 2.2) and ContentDescription. */
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
		
		/* The mime type of the data. May be Emty. */
		public string MimeType {
			get {
				return mimeType;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				mimeType = value;
			}
		}
		
		/* May be Empty. */
		public string FileName {
			get {
				return fileName;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				fileName = value;
			}
		}
		
		/* may not be empty. */
		public string ContentDescription {
			get {
				return contentDescription;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				contentDescription = value;
			}
		}
		
		public byte[] Data {
			get {
				return (byte[])data.Clone();
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				data = (byte[])value.Clone();
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as EncapsulatedObjectFrame);
		}
		
		public bool Equals(EncapsulatedObjectFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && encoding == other.encoding && mimeType.Equals(other.mimeType) &&
				   fileName.Equals(other.fileName) && contentDescription.Equals(other.contentDescription) &&
				   FrameUtil.ArrayEquals(data, other.data);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ (int)encoding ^ mimeType.GetHashCode() ^ fileName.GetHashCode() ^
				   contentDescription.GetHashCode() ^ FrameUtil.ComputeArrayHashCode(data);
		}
		
		public override object Clone() {
			EncapsulatedObjectFrame obj = (EncapsulatedObjectFrame)base.Clone();
			
			obj.encoding = encoding;
			obj.mimeType = mimeType;
			obj.fileName = fileName;
			obj.contentDescription = contentDescription;
			obj.data = (byte[])data.Clone();
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());
			
			sBuild.Append("MIME type: ");
			sBuild.AppendLine(mimeType);
			
			sBuild.Append("Filename: ");
			sBuild.AppendLine(fileName);
			
			sBuild.Append("Content description: ");
			sBuild.AppendLine(contentDescription);
			
			sBuild.Append("Data:");
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(data));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e, latin1;
			Encoding fileNameEncoding;
			int dataLen, currentIndex;
			byte[] rawData;
			
			e = StringEncoder.GetEncoding(encoding);
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			fileNameEncoding = (tag.Version == ID3v2Version.ID3v2_2) ? latin1 : e;//Version 2.2 uses latin1!
			
			/* compute rawData length */
			dataLen = 1;// TextEncoding
			dataLen += StringEncoder.ByteCount(mimeType, latin1, true);
			dataLen += StringEncoder.ByteCount(fileName, fileNameEncoding, true);
			dataLen += StringEncoder.ByteCount(contentDescription, e, true);
			dataLen += data.Length;
			
			rawData = new byte[dataLen];
			
			/* write data */
			rawData[0] = (byte)encoding;
			currentIndex = 1;
			currentIndex += StringEncoder.WriteString(rawData, 1, mimeType, latin1, true);
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, fileName, fileNameEncoding, true);
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, contentDescription, e, true);
			Buffer.BlockCopy(data, 0, rawData, currentIndex, data.Length);
			
			return rawData;
		}
		
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			Encoding e;
			int stringLen, currentIndex;
			
			CheckArrayLength(rawData, 3);//min size
			currentIndex = 1;
			mimeType = StringDecoder.DecodeString(rawData, currentIndex, -1, latin1, out stringLen);
			currentIndex += stringLen;
			CheckArrayLength(rawData, currentIndex);
			
			if (tag.Version == ID3v2Version.ID3v2_2) {
				fileName = StringDecoder.DecodeString(rawData, currentIndex, -1, latin1, out stringLen);
				currentIndex += stringLen;
				
				StringDecoder.ParseEncodingByte(rawData, 0, currentIndex, out e, out encoding);
			} else {
				StringDecoder.ParseEncodingByte(rawData, 0, currentIndex, out e, out encoding);
				
				fileName = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out stringLen);
				currentIndex += stringLen;
			}
			
			CheckArrayLength(rawData, currentIndex);
			contentDescription = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out stringLen);
			currentIndex += stringLen;
			CheckArrayLength(rawData, currentIndex - 1);
			
			data = new byte[rawData.Length - currentIndex];
			Buffer.BlockCopy(rawData, currentIndex, data, 0, data.Length);
		}
		
		private static void CheckArrayLength(byte[] arr, int index) {
			if (index >= arr.Length) {
				throw new ID3ParseException("Error while parsing frame.");
			}
		}
	}
}