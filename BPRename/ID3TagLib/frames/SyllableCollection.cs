using System;
using System.Collections.ObjectModel;

namespace ID3TagLib {
	
	[Serializable]
	public class SyllableCollection: Collection<SyllableEntry> {
		
		public void Add(string syllable, long timestamp) {
			Add(new SyllableEntry(syllable, timestamp));
		}
		
		protected override void InsertItem(int index, SyllableEntry item) {
			if (item.Syllable == null) {
				throw new ArgumentNullException("item.Syllable");
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, SyllableEntry item) {
			if (item.Syllable == null) {
				throw new ArgumentNullException("item.Syllable");
			}
			base.SetItem(index, item);
		}
	}
}