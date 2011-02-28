using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class SeekFrame: Frame, IEquatable<SeekFrame> {
		
		private long offset;
		
		protected internal SeekFrame(string frameID, FrameFlags flags) : base(frameID, flags) { }
		
		public long Offset {
			get {
				return offset;
			}
			
			set {
				offset = value & 0xFFFFFFFF;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as SeekFrame);
		}
		
		public bool Equals(SeekFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && offset == other.offset;
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ unchecked((int)offset);
		}
		
		public override object Clone() {
			SeekFrame obj = (SeekFrame)base.Clone();
			
			obj.offset = offset;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Offset: ");
			sBuild.AppendLine(offset.ToString(NumberFormatInfo.InvariantInfo));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[4];
			
			NumberConverter.WriteInt(offset, rawData, 0, 4, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			if (rawData.Length != 4) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			offset = NumberConverter.ReadInt64(rawData, 0, 4, false);
		}
	}
}