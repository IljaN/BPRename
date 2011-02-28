using System;

namespace ID3TagLib {
	
	[Serializable]
	public struct SyllableEntry: IEquatable<SyllableEntry> {
		
		private string syllable;
		private long timestamp;
		
		public SyllableEntry(string syllable, long timestamp) {
			CheckSyllable(syllable);
			this.syllable = syllable;
			this.timestamp = timestamp & 0xFFFFFFFF;
		}
		
		public string Syllable {
			get {
				return syllable;
			}
			
			set {
				CheckSyllable(value);
				syllable = value;
			}
		}
		
		private static void CheckSyllable(string value) {
			if (value == null) {
				throw new ArgumentNullException("value");
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
			if (obj is SyllableEntry) {
				return Equals((SyllableEntry)obj);
			} else {
				return false;
			}
		}
		
		public bool Equals(SyllableEntry other) {
			return syllable.Equals(other.syllable) && timestamp == other.timestamp;
		}
		
		public override int GetHashCode() {
			return syllable.GetHashCode() ^ timestamp.GetHashCode();
		}

		public override string ToString() {
			return String.Format("<{0}:{1:X8}>", syllable, timestamp);
        }
	}
}