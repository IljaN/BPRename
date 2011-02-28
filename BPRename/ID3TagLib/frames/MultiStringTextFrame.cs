using System;
using System.ComponentModel;
using System.Text;
using System.Collections.ObjectModel;

namespace ID3TagLib {
    
	[Serializable]
    public class MultiStringTextFrame: TextFrame, ISelectableTextEncoding, IEquatable<MultiStringTextFrame> {
		
		[Serializable]
		public class StringCollection: Collection<string> {
			
			private MultiStringTextFrame owner;
			
			public StringCollection(MultiStringTextFrame owner) {
				this.owner = owner;
			}
			
			protected override void InsertItem(int index, string item) {
				if (item == null) {
					throw new ArgumentNullException("item");
				}
				owner.CheckText(item);
				base.InsertItem(index, item);
			}
			
			protected override void SetItem(int index, string item) {
				if (item == null) {
					throw new ArgumentNullException("item");
				}
				owner.CheckText(item);
				base.SetItem(index, item);
			}
		}//end class StringCollection
		
		//-------------------------
		
		private TextEncoding encoding;
		private StringCollection strings;
		
		protected internal MultiStringTextFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
			encoding = TextEncoding.IsoLatin1;
			strings = new StringCollection(this);
		}
		
		public TextEncoding Encoding {
			get {
				return encoding;
			}

			set {
				if (!Enum.IsDefined(typeof(TextEncoding), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(TextEncoding));
				}
				encoding = value;
			}
		}
		
		public override string Text {
            get {
                if (strings.Count == 0) {
					/* to work proper with TextFrame return empty string */
					return String.Empty;
				}
				
				return strings[0];
            }
            
            set {
                if (value == null) {
					throw new ArgumentNullException("value");
				}
				/* delete all other strings */
				strings.Clear();
				strings.Add(value);
            }
        }
        
		public StringCollection Strings {
			get {
				return strings;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as MultiStringTextFrame);
		}
		
		public bool Equals(MultiStringTextFrame other) {
			if (other == null) {
				return false;
			}
			
			if (!base.Equals(other) || encoding != other.encoding || strings.Count != other.strings.Count) {
				return false;
			}
			
			for (int i = 0; i < strings.Count; i++) {
				if (!strings[i].Equals(other.strings[i])) {
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();
			
			hashCode ^= (int)encoding;
			foreach (string s in strings) {
				hashCode ^= s.GetHashCode();
			}
			
			return hashCode;
		}
		
		public override object Clone() {
			MultiStringTextFrame obj = (MultiStringTextFrame)base.Clone();
			
			obj.encoding = encoding;
			obj.strings = new StringCollection(obj);
			foreach (string s in strings) {
				obj.strings.Add(s);
			}
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());
			
			if (strings.Count < 2) {
				sBuild.Append("Text: ");
				sBuild.AppendLine(Text);
			} else {
				for (int i = 0; i < strings.Count; i++) {
					sBuild.AppendFormat("Text[{0}]:", i);
					sBuild.AppendLine(strings[i]);
				}
			}
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e = StringEncoder.GetEncoding(encoding);
            int index, dataLen;
            byte[] rawData;
			string last = (strings.Count == 0) ? String.Empty : strings[strings.Count - 1];//preamble may be needed
			
            /* compute raw data length */
            dataLen = 1;
            for (int i = 0; i < strings.Count - 1; i++) {
                dataLen += StringEncoder.ByteCount(strings[i], e, true);
            }
            dataLen += StringEncoder.ByteCount(last, e, false);
            
			/* write the values */
            rawData = new byte[dataLen];
			
            rawData[0] = (byte)encoding;
            index = 1;
			for (int i = 0; i < strings.Count - 1; i++) {
				index += StringEncoder.WriteString(rawData, index, strings[i], e, true);
			}
			StringEncoder.WriteString(rawData, index, last, e, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] data, ID3v2Tag tag) {
			Encoding e;
            int index = 1, bytesRead;
            
			StringDecoder.ParseEncodingByte(data, 0, 1, out e, out encoding);
            strings.Clear();
            
            while (index < data.Length) {
                strings.Add(StringDecoder.DecodeString(data, index, -1, e, out bytesRead));
                index += bytesRead;                
            }
            
			if (strings.Count == 0) {
                strings.Add(String.Empty);
            }
		}
	}
}