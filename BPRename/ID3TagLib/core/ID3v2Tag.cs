using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {

    [Serializable]
    public class ID3v2Tag: ICloneable, IEquatable<ID3v2Tag> {

        private FrameCollection frames;
        private ExtendedHeader extendedHeader;
        private ID3v2Version version;
        private ID3v2TagFlags flags;
        private PaddingMode paddingMode;
        private int paddingSize;

        public ID3v2Tag() {
            frames = new FrameCollection();
            version = ID3v2Version.ID3v2_3;
            flags = ID3v2TagFlags.None;
            paddingMode = PaddingMode.PadToSize;
            paddingSize = 1024;
        }
		
		public FrameCollection Frames {
			get {
				return frames;
			}
		}
		
		public ExtendedHeader ExtendedHeader {
            get {
                return extendedHeader;
            }

            set {
                extendedHeader = value;
            }
        }

		public ID3v2Version Version {
            get {
                return version;
            }

            set {
                if (!Enum.IsDefined(typeof(ID3v2Version), value)) {
                    throw new InvalidEnumArgumentException("Version", (int)value, typeof(ID3v2Version));
                }
                version = value;
            }
        }

		public ID3v2TagFlags Flags {
            get {
                return flags;
            }

            set {
                flags = value;
            }
        }

		public PaddingMode PaddingMode {
            get {
                return paddingMode;
            }

            set {
                if (!Enum.IsDefined(typeof(PaddingMode), value)) {
                    throw new InvalidEnumArgumentException("PaddingMode", (int)value, typeof(PaddingMode));
                }

                paddingMode = value;
            }
        }

		public int PaddingSize {
            get {
                return paddingSize;
            }

            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("value", value, "PaddingSize must not be negative.");
                }

                paddingSize = value;
            }
        }

		public override string ToString() {
            StringBuilder sBuild = new StringBuilder();

            sBuild.Append("Version: ");
            sBuild.AppendLine(version.ToString());
            sBuild.Append("Flags: ");
            sBuild.AppendLine(flags.ToString());
            sBuild.Append("PaddingMode: ");
            sBuild.Append(paddingMode);
            if (paddingMode != PaddingMode.None) {
                sBuild.Append(" (");
                sBuild.Append(paddingSize);
                sBuild.Append(" bytes)");
            }
            sBuild.AppendLine();
            sBuild.Append("ExtendedHeader: ");
			if (extendedHeader == null) {
				sBuild.AppendLine("not set");
			} else {
				sBuild.AppendLine();
				sBuild.AppendLine(extendedHeader.ToString());
			}
            sBuild.AppendLine("Frames:");
            for (int i = 0; i < frames.Count; i++) {
				sBuild.AppendLine(frames[i].ToString());
				if (i < frames.Count - 1) {
					sBuild.AppendLine();
				}
			}

            return sBuild.ToString();
        }
		
		public object Clone() {
			ID3v2Tag obj = (ID3v2Tag)MemberwiseClone();
			
			obj.frames = new FrameCollection();
			foreach (Frame f in frames) {
				obj.frames.Add((Frame)f.Clone());
			}
			obj.extendedHeader = (ExtendedHeader)extendedHeader.Clone();
			
			return obj;
		}
		
		public override bool Equals(object other) {
			return Equals(other as ID3v2Tag);
		}
		
		public bool Equals(ID3v2Tag other) {
			if (other == null) {
				return false;
			}
			
			if (frames.Count != other.frames.Count || !extendedHeader.Equals(other.extendedHeader) ||
				version != other.version || flags != other.flags || paddingMode != other.paddingMode ||
				paddingSize != other.paddingSize) {
			
				return false;
			}
			
			for (int i = 0; i < frames.Count; i++) {
				if (!frames[i].Equals(other.frames[i])) {
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			
			hashCode ^= extendedHeader.GetHashCode();
			hashCode ^= version.GetHashCode();
			hashCode ^= flags.GetHashCode();
			hashCode ^= paddingMode.GetHashCode();
			hashCode ^= paddingSize;
			
			foreach (Frame f in frames) {
				hashCode ^= f.GetHashCode();
			}
			
			return hashCode;
		}
    }
}