using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class UserDefinedTextFrame: TextFrame, ISelectableTextEncoding, IEquatable<UserDefinedTextFrame> {
		
		private TextEncoding encoding;
		private string description;
		
		private bool isUrlFrame;
		
		protected internal UserDefinedTextFrame(string frameID, FrameFlags flags, bool isUrlFrame) : base(frameID, flags) {
			encoding = TextEncoding.IsoLatin1;
			description = String.Empty;
			this.isUrlFrame = isUrlFrame;
		}
		
		protected internal UserDefinedTextFrame(string frameID, FrameFlags flags) : this(frameID, flags, false) { }
		
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
			return Equals(obj as UserDefinedTextFrame);
		}

		public bool Equals(UserDefinedTextFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && encoding == other.encoding && description.Equals(other.description);
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= (int)encoding;
			hashCode ^= description.GetHashCode();
			
			return hashCode;
		}
		
		public override object Clone() {
			UserDefinedTextFrame obj = (UserDefinedTextFrame)base.Clone();

			obj.encoding = encoding;
			obj.description = description;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());
			
			sBuild.Append("Description: ");
			sBuild.AppendLine(description);
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e, encodingText;
			byte[] rawData;
			int currentIndex, rawDataLength;
			
			e = StringEncoder.GetEncoding(encoding);
			encodingText = isUrlFrame ? StringEncoder.GetEncoding(TextEncoding.IsoLatin1) : e;
			
			rawDataLength = 1;
			rawDataLength += StringEncoder.ByteCount(description, e, true);
			rawDataLength += StringEncoder.ByteCount(Text, encodingText, false);
			
			rawData = new byte[rawDataLength];
			
			rawData[0] = (byte)encoding;
			currentIndex = 1;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, description, e, true);
			StringEncoder.WriteString(rawData, currentIndex, Text, encodingText, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding e, encodingText;
			int bytesRead;
			
			StringDecoder.ParseEncodingByte(rawData, 0, 1, out e, out encoding);
			encodingText = isUrlFrame ? StringDecoder.GetEncoding(TextEncoding.IsoLatin1) : e;
			
			description = StringDecoder.DecodeString(rawData, 1, -1, e, out bytesRead);
			
			if (rawData.Length < 1 + bytesRead) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			Text = StringDecoder.DecodeString(rawData, 1 + bytesRead, -1, encodingText, out bytesRead);
		}
	}
}