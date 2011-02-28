using System;
using System.Collections.ObjectModel;

namespace ID3TagLib {
	
	[Serializable]
	public class EventCollection: Collection<EventEntry> {
		
		public void Add(byte eventType, long timestamp) {
			Add(new EventEntry(eventType, timestamp));
		}
	}
}