using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class PlayCounterFrame: Frame, IEquatable<PlayCounterFrame> {
		
		private long counter;
		
		protected internal PlayCounterFrame(string frameID, FrameFlags flags) : base(frameID, flags) { }
		
		public long Counter {
			get {
				return counter;
			}
			
			set {
				if (value < 0L) {
					throw new ArgumentException("Only nonnegative values allowed.");
				}
				counter = value;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as PlayCounterFrame);
		}
		
		public bool Equals(PlayCounterFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && counter == other.counter;
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ unchecked((int)counter);
		}
		
		public override object Clone() {
			PlayCounterFrame obj = (PlayCounterFrame)base.Clone();
			
			obj.counter = counter;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Counter: ");
			sBuild.AppendLine(counter.ToString(NumberFormatInfo.InvariantInfo));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[NumberConverter.ByteCount(counter, 4)];
			
			NumberConverter.WriteInt(counter, rawData, 0, rawData.Length, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			if (rawData.Length < 4) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			counter = NumberConverter.ReadInt64(rawData, 0, rawData.Length, false);
		}
	}
}