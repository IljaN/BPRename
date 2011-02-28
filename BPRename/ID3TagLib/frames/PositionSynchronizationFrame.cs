using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class PositionSynchronizationFrame: Frame, IEquatable<PositionSynchronizationFrame> {
		
		private TimestampFormat timestampFormat;
		private long position;
		
		protected internal PositionSynchronizationFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            timestampFormat = TimestampFormat.AbsoluteTimeInFrames;
        }
		
		public TimestampFormat TimestampFormat {
			get {
				return timestampFormat;
			}
			
			set {
				if (!Enum.IsDefined(typeof(TimestampFormat), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(TimestampFormat));
				}
				timestampFormat = value;
			}
		}
		
		/* Note is a 32 Bit value */
		public long Position {
			get {
				return position;
			}
			
			set {
				position = value & 0xFFFFFFFFL;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as PositionSynchronizationFrame);
		}
		
		public bool Equals(PositionSynchronizationFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && timestampFormat == other.timestampFormat && position == other.position;
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ (int)timestampFormat ^ position.GetHashCode();
		}
		
		public override object Clone() {
			PositionSynchronizationFrame obj = (PositionSynchronizationFrame)base.Clone();
			
			obj.timestampFormat = timestampFormat;
			obj.position = position;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Time stamp format: ");
			sBuild.AppendLine(timestampFormat.ToString());
			
			sBuild.Append("Position: ");
			sBuild.AppendLine(position.ToString(NumberFormatInfo.InvariantInfo));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[5];
			
			rawData[0] = (byte)timestampFormat;
			NumberConverter.WriteInt(position, rawData, 1, 4, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			if (rawData.Length != 5) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			TimestampFormat = (TimestampFormat)rawData[0];
			position = NumberConverter.ReadInt64(rawData, 1, 4, false);
		}
	}
}