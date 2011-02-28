using System;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class DataFrame: Frame, IEquatable<DataFrame> {
        
		private string ownerIdentifier;
		private byte[] data;
        
        protected internal DataFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            ownerIdentifier = String.Empty;
			data = new byte[0];
        }
        
		/* must not be empty */
        public string OwnerIdentifier {
            get {
                return ownerIdentifier;
            }
            
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
				ownerIdentifier = value;
            }
        }
        
		/* Identifier length must be < 64 */
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
			return Equals(obj as DataFrame);
		}
		
		public bool Equals(DataFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && ownerIdentifier.Equals(other.ownerIdentifier) &&
				   FrameUtil.ArrayEquals(data, other.data);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ FrameUtil.ComputeArrayHashCode(data) ^ ownerIdentifier.GetHashCode();
		}
		
		public override object Clone() {
			DataFrame obj = (DataFrame)base.Clone();
			
			obj.ownerIdentifier = ownerIdentifier;
			obj.data = (byte[])data.Clone();
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("OwnerIdentifier: ");
			sBuild.AppendLine(ownerIdentifier);
			
			sBuild.AppendLine("Data:");
            sBuild.AppendLine(FrameUtil.GetStringRepresentation(data));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			int stringLen = StringEncoder.ByteCount(ownerIdentifier, e, true);
			byte[] rawData;

			rawData = new byte[stringLen + data.Length];
			StringEncoder.WriteString(rawData, 0, ownerIdentifier, e, true);
			Buffer.BlockCopy(data, 0, rawData, stringLen, data.Length);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			int stringLen;
			Encoding e = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			
			if (rawData.Length == 0) {
				throw new ID3ParseException("Error while parsing frame");
			}
			ownerIdentifier = StringDecoder.DecodeString(rawData, 0, -1, e, out stringLen);
			data = new byte[rawData.Length - stringLen];
			Buffer.BlockCopy(rawData, stringLen, data, 0, data.Length);
		}
    }
}