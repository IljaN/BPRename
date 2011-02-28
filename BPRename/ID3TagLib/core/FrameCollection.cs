using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ID3TagLib {
	
	[Serializable]
	public class FrameCollection: Collection<Frame> {

		internal FrameCollection(): base() { }

		public Frame this[string id] {
            get {
                int index = IndexOf(id);

				return (index == -1 ? null : this[index]);
            }

            set {
                int index = IndexOf(id);

				if (index == -1) {
					Add(value);
				} else {
					this[index] = value;
				}
            }
        }

		public Frame[] GetAll(string id) {
			List<Frame> list = new List<Frame>(3);

			if (id == null) {
				throw new ArgumentNullException("id");
			}
			
			foreach (Frame f in this) {
				if (String.Compare(f.FrameId, id, true, CultureInfo.InvariantCulture) == 0) {
					list.Add(f);
				}
			}

			return list.ToArray();
		}

		public void Remove(string id) {
			int index = IndexOf(id);

			if (index != -1) {
				RemoveAt(index);
			}
		}

		public void RemoveAll(string id) {
			int index = Count - 1;

			while (index >= 0 && (index = LastIndexOf(id, index)) != -1) {
				RemoveAt(index--);
			}
		}

		public bool Contains(string id) {
			return (IndexOf(id) == -1);
		}

		public int IndexOf(string id) {
			return (Count == 0 ? -1 : IndexOf(id, 0));
		}

		public int IndexOf(string id, int startIndex) {
			if (id == null) {
				throw new ArgumentNullException("id");
			}

			if (startIndex < 0 || startIndex >= Count) {
				throw new ArgumentOutOfRangeException("startIndex");
			}

			while (startIndex < Count && String.Compare(this[startIndex].FrameId, id, true, CultureInfo.InvariantCulture) != 0) {
				startIndex++;
			}

			return startIndex == Count ? -1 : startIndex;
		}

		public int LastIndexOf(string id) {
			return (Count == 0 ? -1 : LastIndexOf(id, Count - 1));
		}

		public int LastIndexOf(string id, int startIndex) {
			if (id == null) {
				throw new ArgumentNullException("id");
			}
			
			if (startIndex < 0 || startIndex >= Count) {
				throw new ArgumentOutOfRangeException("startIndex");
			}

			while (startIndex >= 0 && String.Compare(this[startIndex].FrameId, id, true, CultureInfo.InvariantCulture) != 0) {
				startIndex--;
			}

			return startIndex;
		}

		protected override void InsertItem(int index, Frame item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, Frame item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			base.SetItem(index, item);
		}
	}
}