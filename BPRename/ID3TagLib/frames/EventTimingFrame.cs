using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public class EventTimingFrame:  Frame, IEquatable<EventTimingFrame> {
		
		private TimestampFormat timestampFormat;
		private EventCollection events;
		
		protected internal EventTimingFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            timestampFormat = TimestampFormat.AbsoluteTimeInFrames;
			events = new EventCollection();
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
		
		public EventCollection Events {
			get {
				return events;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as EventTimingFrame);
		}
		
		public bool Equals(EventTimingFrame other) {
			if (other == null) {
				return false;
			}
			
			if (!base.Equals(other) || timestampFormat != other.timestampFormat || events.Count != other.events.Count) {
				return false;
			}
			
			for (int i = 0; i < events.Count; i++) {
				if (!events[i].Equals(other.events[i])) {
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode() ^ timestampFormat.GetHashCode();
			
			foreach (EventEntry entry in events) {
				hashCode ^= entry.GetHashCode();
			}
			
			return hashCode;
		}
		
		public override object Clone() {
			EventTimingFrame obj = (EventTimingFrame)base.Clone();
			
			obj.timestampFormat = timestampFormat;
			obj.events = new EventCollection();
			foreach (EventEntry entry in events) {
				obj.events.Add(entry);
			}
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Timestamp format: ");
			sBuild.AppendLine(timestampFormat.ToString());
			
			sBuild.AppendLine("Event codes:");
			foreach (EventEntry entry in events) {
				sBuild.AppendLine(entry.ToString());
			}
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData = new byte[1 + events.Count * 5];
			int currentIndex = 1;
			
			rawData[0] = (byte)timestampFormat;
			foreach (EventEntry entry in events) {
				rawData[currentIndex++] = entry.EventType;
				NumberConverter.WriteInt(entry.Timestamp, rawData, currentIndex, 4, false);
				currentIndex += 4;
			}
			
			return rawData;
		}
		
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			int currentIndex = 1;
			byte eventType;
			long timestamp;
			
			TimestampFormat = (TimestampFormat)rawData[0];
			
			events.Clear();
			while (currentIndex < rawData.Length) {
				eventType = rawData[currentIndex++];
				timestamp = NumberConverter.ReadInt64(rawData, currentIndex, 4, false);
				currentIndex += 4;
				
				events.Add(eventType, timestamp);
			}
		}
	}
}