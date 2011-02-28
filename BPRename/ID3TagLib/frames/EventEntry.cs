using System;

namespace ID3TagLib {
	
	[Serializable]
	public struct EventEntry: IEquatable<EventEntry> {
		
		public const byte Padding = 0x00;
		public const byte EndOfInitialSilence = 0x01;
		public const byte IntroStart = 0x02;
		public const byte MainpartStart = 0x03;
		public const byte OutroStart = 0x04;
		public const byte OutroEnd = 0x05;
		public const byte VerseStart = 0x06;
		public const byte RefrainStart = 0x07;
		public const byte InterludeStart = 0x08;
		public const byte ThemeStart = 0x09;
		public const byte VariationStart = 0x0A;
		public const byte KeyChange = 0x0B;
		public const byte TimeChange = 0x0C;
		public const byte MomentaryNoise = 0x0D;
		public const byte SustainedNoise = 0x0E;
		public const byte SustainedNoiseEnd = 0x0F;
		public const byte IntroEnd = 0x10;
		public const byte MainpartEnd = 0x11;
		public const byte VerseEnd = 0x12;
		public const byte RefrainEnd = 0x13;
		public const byte ThemeEnd = 0x14;
		public const byte AudioEnd = 0xFD;
		public const byte AudioFileEnd = 0xFE;
		public const byte UserDefindedEventBegin = 0xE0;
		public const byte UserDefindedEventEnd = 0xEF;
		
		private byte eventType;
		private long timestamp;
		
		public EventEntry(byte eventType, long timestamp) {
			this.eventType = eventType;
			this.timestamp = timestamp & 0xFFFFFFFF;
		}
		
		public byte EventType {
			get {
				return eventType;
			}
			
			set {
				eventType = value;
			}
		}
		
		public long Timestamp {
			get {
				return timestamp;
			}
			
			set {
				timestamp = value & 0xFFFFFFFF;
			}
		}
		
		public override bool Equals(object obj) {
			if (obj is EventEntry) {
				return Equals((EventEntry)obj);
			} else {
				return false;
			}
		}
		
		public bool Equals(EventEntry other) {
			return eventType == other.eventType && timestamp == other.timestamp;
		}
		
		public override int GetHashCode() {
			return eventType.GetHashCode() ^ timestamp.GetHashCode();
		}

		public override string ToString() {
			return String.Format("<{0}:{1:X8}>", eventType, timestamp);
        }
	}
}