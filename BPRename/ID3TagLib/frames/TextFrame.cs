using System;
using System.Text;

namespace ID3TagLib {
    /* Stellt die Basisklasse für alle textuellen Frames dar. Darunter fallen 
	   alle Frames die mit T beginnen, alle URL Frames, User defined Text und URL
	   sowie Comment, Unsync Lyrics und Terms of use.
	   Für alle URL Frames wird diese Klasse verwendet.
	 */
	[Serializable]
    public class TextFrame: Frame, IEquatable<TextFrame> {
        
		private string text;
        
        protected internal TextFrame(string frameId, FrameFlags flags) : base(frameId, flags) {
            text = String.Empty;
		}
        
        public virtual string Text {
            get {
                return text;
            }
            
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
				CheckText(value);
				text = value;
            }
        }
        
		public override bool Equals(object obj) {
			return Equals(obj as TextFrame);
		}
		
		public bool Equals(TextFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && text.Equals(other.text);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ text.GetHashCode();
		}
		
		public override object Clone() {
			TextFrame obj = (TextFrame)base.Clone();
			
			obj.text = text;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Text: ");
			sBuild.AppendLine(text);
			
			return sBuild.ToString();
        }
		
		protected virtual void CheckText(string text) { }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			byte[] rawData = new byte[StringEncoder.ByteCount(text, e, false)];
			
			StringEncoder.WriteString(rawData, 0, text, e, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Text = StringDecoder.DecodeLatin1String(rawData, 0, -1);
		}
    }
}