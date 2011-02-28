using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class TermsOfUseFrame: TextFrame, ISelectableTextEncoding, ISelectableLanguage, IEquatable<TermsOfUseFrame> {
		
		private TextEncoding encoding;
		private string language;
		
		protected internal TermsOfUseFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			language = String.Empty;
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
		
		public override bool Equals(object obj) {
			return Equals(obj as TermsOfUseFrame);
		}

		public bool Equals(TermsOfUseFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && encoding == other.encoding && language.Equals(other.language);
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= (int)encoding;
			hashCode ^= language.GetHashCode();
			
			return hashCode;
		}
		
		public override object Clone() {
			TermsOfUseFrame obj = (TermsOfUseFrame)base.Clone();

			obj.encoding = encoding;
			obj.language = language;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());

			sBuild.Append("Language: ");
			sBuild.AppendLine(language);

			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData;
			Encoding e = StringEncoder.GetEncoding(encoding);
			
			rawData = new byte[StringEncoder.ByteCount(Text, e, false) + 4];
			
			rawData[0] = (byte)encoding;
			StringEncoder.WriteLatin1String(rawData, 1, 3, language);
			StringEncoder.WriteString(rawData, 4, Text, e, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding e;
			int bytesRead;
			
			StringDecoder.ParseEncodingByte(rawData, 0, 4, out e, out encoding);
			language = StringDecoder.DecodeLatin1String(rawData, 1, 3);
			Text = StringDecoder.DecodeString(rawData, 4, -1, e, out bytesRead);
		}
	}
}