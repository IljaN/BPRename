using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public class SymbolRegistrationFrame:  Frame, IEquatable<SymbolRegistrationFrame> {
		
		private string ownerIdentifier;
		private byte symbol;
		private byte[] data;
		
		protected internal SymbolRegistrationFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
			ownerIdentifier = String.Empty;
		}
		
		/* URL */
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
		
		public byte Symbol {
			get {
				return symbol;
			}
			
			set {
				symbol = value;
			}
		}
		
		/* Note may be null. */
		public byte[] Data {
			get {
				return (data == null) ? null : (byte[])data.Clone();
			}
			
			set {
				data = (value == null) ? null : (byte[])value.Clone();
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as SymbolRegistrationFrame);
		}
		
		public bool Equals(SymbolRegistrationFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && ownerIdentifier.Equals(other.ownerIdentifier) && symbol == other.symbol &&
				   FrameUtil.ArrayEquals(data, other.data);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ ownerIdentifier.GetHashCode() ^ symbol ^ FrameUtil.ComputeArrayHashCode(data);
		}
		
		public override object Clone() {
			SymbolRegistrationFrame obj = (SymbolRegistrationFrame)base.Clone();
			
			obj.ownerIdentifier = ownerIdentifier;
			obj.symbol = symbol;
			obj.data = (data == null) ? null : (byte[])data.Clone();;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Owner identifier: ");
			sBuild.AppendLine(ownerIdentifier);
			
			sBuild.Append("Symbol: ");
			sBuild.AppendLine(symbol.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(data));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			int dataLen, currentIndex;
			byte[] rawData;
			
			/* Compute rawdata length. */
			dataLen = StringEncoder.ByteCount(ownerIdentifier, latin1, true);
			dataLen += 1;
			dataLen += (data == null) ? 0 : data.Length;
			
			/* write data */
			rawData = new byte[dataLen];
			currentIndex = StringEncoder.WriteString(rawData, 0, ownerIdentifier, latin1, true);
			rawData[currentIndex++] = symbol;
			if (data != null) {
				Buffer.BlockCopy(data, 0, rawData, currentIndex, data.Length);
			}

			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			int currentIndex;
			
			
			ownerIdentifier = StringDecoder.DecodeString(rawData, 0, -1, latin1, out currentIndex);
			if (rawData.Length <= currentIndex) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			symbol = rawData[currentIndex++];
			
			if (currentIndex < rawData.Length) {
				data = new byte[rawData.Length - currentIndex];
				Buffer.BlockCopy(rawData, currentIndex, data, 0, data.Length);
			} else {
				data = null;
			}
		}
	}
}