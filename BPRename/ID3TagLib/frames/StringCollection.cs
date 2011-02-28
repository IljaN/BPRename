using System;
using System.Collections.ObjectModel;

namespace ID3TagLib {

	[Serializable]
	public class StringCollection: Collection<string> {

		protected override void InsertItem(int index, string item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, string item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			base.SetItem(index, item);
		}
	}
}