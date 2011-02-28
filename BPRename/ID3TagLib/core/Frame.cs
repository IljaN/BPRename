using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public abstract class Frame: ICloneable {        
        
		private string frameId;
		private FrameFlags flags;
        private int groupIdentifier;
		
        public const int GroupIdNotSet = -1;

        protected internal Frame(string frameId, FrameFlags flags) {
			this.frameId = frameId;
            this.flags = flags;
            this.groupIdentifier = GroupIdNotSet;
		}
		
		public string FrameId {
            get {
                return frameId;
            }
        }

        public FrameFlags Flags {
            get {
                return flags;
            }
            
            set {
                flags = value;
            }
        }
        
        public int GroupIdentifier {
            get {
                return groupIdentifier;
            }
            
            set {
                groupIdentifier = (value == GroupIdNotSet) ? GroupIdNotSet : (0xFF & value);
            }
        }
        
		public override bool Equals(object obj) {
			Frame other = obj as Frame;
			
			if (other == null) {
				return false;
			}
			
			return frameId.Equals(other.frameId) && flags == other.flags && groupIdentifier == other.groupIdentifier;
		}
		
		public override int GetHashCode() {
			return frameId.GetHashCode() ^ (int)flags ^ groupIdentifier;
		}
		
		public virtual object Clone() {
			return MemberwiseClone();
		}
		
		public override string ToString() {
			StringBuilder sBuild = new StringBuilder();
			
			sBuild.Append("FrameId: ");
			sBuild.AppendLine(frameId);
			sBuild.Append("Flags: ");
			sBuild.AppendLine(flags.ToString());
			sBuild.Append("GroupIdentifier: ");
			sBuild.AppendLine(groupIdentifier.ToString(NumberFormatInfo.InvariantInfo));
			
			return sBuild.ToString();
		}
		
		public byte[] EncodeContent(ID3v2Tag tag) {
			if (tag == null) {
				throw new ArgumentNullException("tag");
			}
			
			return EncodeContentCore(tag);
		}
		
		public void DecodeContent(byte[] data, ID3v2Tag tag) {
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			if (tag == null) {
				throw new ArgumentNullException("tag");
			}
			
			DecodeContentCore(data, tag);
		}
		
		protected abstract byte[] EncodeContentCore(ID3v2Tag tag);
		protected abstract void DecodeContentCore(byte[] rawData, ID3v2Tag tag);
    }
}