using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class ReverbFrame: Frame, IEquatable<ReverbFrame> {
		
		private int reverbLeft;
		private int reverbRight;
		private byte reverbBouncesLeft;
		private byte reverbBouncesRight;
		private byte reverbFeedbackLeftToLeft;
		private byte reverbFeedbackLeftToRight;
		private byte reverbFeedbackRightToRight;
		private byte reverbFeedbackRightToLeft;
		private byte premixLeftToRight;
		private byte premixRightToLeft;
		
		protected internal ReverbFrame(string frameID, FrameFlags flags) : base(frameID, flags) { }
		
		/* Note: short value! */
		public int ReverbLeft {
			get {
				return reverbLeft;
			}
			
			set {
				reverbLeft = value & 0xFFFF;
			}
		}
		
		/* Note: short value! */
		public int ReverbRight {
			get {
				return reverbRight;
			}
			
			set {
				reverbRight = value & 0xFFFF;
			}
		}
		
		public byte ReverbBouncesLeft {
			get {
				return reverbBouncesLeft;
			}
			
			set {
				reverbBouncesLeft = value;
			}
		}
		
		public byte ReverbBouncesRight {
			get {
				return reverbBouncesRight;
			}
			
			set {
				reverbBouncesRight = value;
			}
		}
		
		public byte ReverbFeedbackLeftToLeft {
			get {
				return reverbFeedbackLeftToLeft;
			}
			
			set {
				reverbFeedbackLeftToLeft = value;
			}
		}
		
		public byte ReverbFeedbackLeftToRight {
			get {
				return reverbFeedbackLeftToRight;
			}
			
			set {
				reverbFeedbackLeftToRight = value;
			}
		}
		
		public byte ReverbFeedbackRightToRight {
			get {
				return reverbFeedbackRightToRight;
			}
			
			set {
				reverbFeedbackRightToRight = value;
			}
		}
		
		public byte ReverbFeedbackRightToLeft {
			get {
				return reverbFeedbackRightToLeft;
			}
			
			set {
				reverbFeedbackRightToLeft = value;
			}
		}
		
		public byte PremixLeftToRight {
			get {
				return premixLeftToRight;
			}
			
			set {
				premixLeftToRight = value;
			}
		}
		
		public byte PremixRightToLeft {
			get {
				return premixRightToLeft;
			}
			
			set {
				premixRightToLeft = value;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as ReverbFrame);
		}
		
		public bool Equals(ReverbFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) &&
				   reverbLeft == other.reverbLeft &&
				   reverbRight == other.reverbRight &&
				   reverbBouncesLeft == other.reverbBouncesLeft &&
				   reverbBouncesRight == other.reverbBouncesRight &&
				   reverbFeedbackLeftToLeft == other.reverbFeedbackLeftToLeft && 
				   reverbFeedbackLeftToRight == other.reverbFeedbackLeftToRight && 
				   reverbFeedbackRightToRight == other.reverbFeedbackRightToRight &&
				   reverbFeedbackRightToLeft == other.reverbFeedbackRightToLeft &&
				   premixLeftToRight == other.premixLeftToRight &&
				   premixRightToLeft == other.premixRightToLeft;
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ 
				   reverbLeft ^
				   reverbRight ^
				   reverbBouncesLeft ^
				   reverbBouncesRight ^
				   reverbFeedbackLeftToLeft ^
				   reverbFeedbackLeftToRight ^
				   reverbFeedbackRightToRight ^
				   reverbFeedbackRightToLeft ^
				   premixLeftToRight ^
				   premixRightToLeft;
		}
		
		public override object Clone() {
			ReverbFrame obj = (ReverbFrame)base.Clone();
			
			obj.reverbLeft = reverbLeft;
			obj.reverbRight = reverbRight;
			obj.reverbBouncesLeft = reverbBouncesLeft;
			obj.reverbBouncesRight = reverbBouncesRight;
			obj.reverbFeedbackLeftToLeft = reverbFeedbackLeftToLeft;
			obj.reverbFeedbackLeftToRight = reverbFeedbackLeftToRight;
			obj.reverbFeedbackRightToRight = reverbFeedbackRightToRight;
			obj.reverbFeedbackRightToLeft = reverbFeedbackRightToLeft;
			obj.premixLeftToRight = premixLeftToRight;
			obj.premixRightToLeft = premixRightToLeft;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Reverb left: ");
			sBuild.AppendLine(reverbLeft.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb right: ");
			sBuild.AppendLine(reverbRight.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb bounces left: ");
			sBuild.AppendLine(reverbBouncesLeft.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb bounces right: ");
			sBuild.AppendLine(reverbBouncesRight.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb feedback left to left: ");
			sBuild.AppendLine(reverbFeedbackLeftToLeft.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb feedback left to right: ");
			sBuild.AppendLine(reverbFeedbackLeftToRight.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb feedback right to right: ");
			sBuild.AppendLine(reverbFeedbackRightToRight.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Reverb feedback right to left: ");
			sBuild.AppendLine(reverbFeedbackRightToLeft.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Premix left to right: ");
			sBuild.AppendLine(premixLeftToRight.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Premix right to left: ");
			sBuild.AppendLine(premixRightToLeft.ToString(NumberFormatInfo.InvariantInfo));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[12];
			
			rawData[0] = (byte)(reverbLeft >> 8);
			rawData[1] = (byte)(reverbLeft & 0xFF);
			rawData[2] = (byte)(reverbRight >> 8);
			rawData[3] = (byte)(reverbRight & 0xFF);
			rawData[4] = reverbBouncesLeft;
			rawData[5] = reverbBouncesRight;
			rawData[6] = reverbFeedbackLeftToLeft;
			rawData[7] = reverbFeedbackLeftToRight;
			rawData[8] = reverbFeedbackRightToRight;
			rawData[9] = reverbFeedbackRightToLeft;
			rawData[10] = premixLeftToRight;
			rawData[11] = premixRightToLeft;
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			if (rawData.Length != 12) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			reverbLeft = (rawData[0] << 8) | rawData[1];
			reverbRight = (rawData[2] << 8) | rawData[3];
			reverbBouncesLeft = rawData[4];
			reverbBouncesRight = rawData[5];
			reverbFeedbackLeftToLeft = rawData[6];
			reverbFeedbackLeftToRight = rawData[7];
			reverbFeedbackRightToRight = rawData[8];
			reverbFeedbackRightToLeft = rawData[9];
			premixLeftToRight = rawData[10];
			premixRightToLeft = rawData[11];
		}
	}
}