using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class SignatureFrame: Frame, IEquatable<SignatureFrame> {
		
		private byte symbol;
		private byte[] signature;
		
		protected internal SignatureFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
			signature = new byte[0];
        }
		
		public byte Symbol {
			get {
				return symbol;
			}
			
			set {
				symbol = value;
			}
		}
		
		/* Note: This property returns a copy of the array! */
		public byte[] Signature {
			get {
				return (byte[])signature.Clone();
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				signature = (byte[])value.Clone();
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as SignatureFrame);
		}
		
		public bool Equals(SignatureFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && symbol == other.symbol && FrameUtil.ArrayEquals(signature, other.signature); 
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ symbol ^ FrameUtil.ComputeArrayHashCode(signature);
		}
		
		public override object Clone() {
			SignatureFrame obj = (SignatureFrame)base.Clone();
			
			obj.symbol = symbol;
			obj.signature = (byte[])signature.Clone();
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Symbol: ");
			sBuild.AppendLine(symbol.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Signature: ");
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(signature));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[1 + signature.Length];
			
			rawData[0] = symbol;
			Buffer.BlockCopy(signature, 0, rawData, 1, signature.Length);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			if (rawData.Length < 1) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			symbol = rawData[0];
			signature = new byte[rawData.Length - 1];
			Buffer.BlockCopy(rawData, 1, signature, 0, signature.Length);
		}
	}
}