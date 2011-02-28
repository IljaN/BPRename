using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public class ExtendedHeader: ICloneable {
	
        private long crcChecksum;
        private ExtendedHeaderFlags flags;
        private TextEncodingRestriction textEncodingRestriction;
        private TagSizeRestriction tagSizeRestriction;
        private TextFieldSizeRestriction textFieldSizeRestriction;
        private ImageEncodingRestriction imageEncodingRestriction;
		private ImageSizeRestriction imageSizeRestriction;
        
        public ExtendedHeader() {
			flags = ExtendedHeaderFlags.None;
            textEncodingRestriction = TextEncodingRestriction.NoRestrictions;
            tagSizeRestriction = TagSizeRestriction.NoMorethan128Frames;
            textFieldSizeRestriction = TextFieldSizeRestriction.NoRestrictions;
			imageEncodingRestriction = ImageEncodingRestriction.NoRestrictions;
            imageSizeRestriction = ImageSizeRestriction.NoRestrictions;
        }
        
        public ExtendedHeaderFlags Flags {
            get {
                return flags;
            }
            
            set {
                flags = value;
            }
        }
        
        public long CrcChecksum {
            get {
                return crcChecksum;
            }
			
			internal set {
				crcChecksum = value;
			}
        }

        public TextEncodingRestriction TextEncodingRestriction {
            get {
                return textEncodingRestriction;
            }
            
            set {
                if (!Enum.IsDefined(typeof(TextEncodingRestriction), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TextEncodingRestriction));
                }
                textEncodingRestriction = value;
            }
        }
        
        public TagSizeRestriction TagSizeRestriction {
            get {
                return tagSizeRestriction;
            }
            
            set {
                if (!Enum.IsDefined(typeof(TagSizeRestriction), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TagSizeRestriction));
                }
                tagSizeRestriction = value;
            }
        }
        
        public TextFieldSizeRestriction TextFieldSizeRestriction {
            get {
                return textFieldSizeRestriction;
            }
            
            set {
                if (!Enum.IsDefined(typeof(TextFieldSizeRestriction), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TextFieldSizeRestriction));
                }
                textFieldSizeRestriction = value;
            }
        }
        
		public ImageEncodingRestriction ImageEncodingRestriction {
			get {
				return imageEncodingRestriction;
			}
			
			set {
				if (!Enum.IsDefined(typeof(ImageEncodingRestriction), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ImageEncodingRestriction));
                }
                imageEncodingRestriction = value;
			}
		}
		
        public ImageSizeRestriction ImageSizeRestriction {
            get {
                return imageSizeRestriction;
            }
            
            set {
                if (!Enum.IsDefined(typeof(ImageSizeRestriction), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ImageSizeRestriction));
                }
                imageSizeRestriction = value;
            }
        }
        
        public override string ToString() {
            StringBuilder sBuild = new StringBuilder();
            
            sBuild.Append("Flags:");
            sBuild.AppendLine(flags.ToString());
			if ((flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent) {
				sBuild.Append("CRC-checksum: ");
				sBuild.AppendLine((crcChecksum == 0 ? "not computed" : crcChecksum.ToString("{0:X}")));
            }
			if ((flags & ExtendedHeaderFlags.RestrictTag) == ExtendedHeaderFlags.RestrictTag) {
                sBuild.AppendLine("Tag restrictions: ");
                sBuild.Append(" ");
				sBuild.AppendLine(textEncodingRestriction.ToString());
                sBuild.Append(" ");
                sBuild.AppendLine(tagSizeRestriction.ToString());
                sBuild.Append(" ");
				sBuild.AppendLine(textFieldSizeRestriction.ToString());
                sBuild.Append(" ");
                sBuild.AppendLine(imageSizeRestriction.ToString());
            }
            
            return sBuild.ToString();
        }
		
		public object Clone() {
			return MemberwiseClone();
		}
		
		public override bool Equals(object other) {
			return Equals(other as ExtendedHeader);
		}
		
		public bool Equals(ExtendedHeader other) {
			if (other == null) {
				return false;
			}
			
			if (flags != other.flags) {
				return false;
			}
			if ((flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent && crcChecksum != other.crcChecksum) {
				return false;
			}
			if ((flags & ExtendedHeaderFlags.RestrictTag) == ExtendedHeaderFlags.RestrictTag) {
				if (textEncodingRestriction != other.textEncodingRestriction ||
					tagSizeRestriction != other.tagSizeRestriction ||
					textFieldSizeRestriction != other.textFieldSizeRestriction ||
					imageEncodingRestriction != other.imageEncodingRestriction ||
					imageSizeRestriction != other.imageSizeRestriction) {
				
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode;
			
			hashCode = flags.GetHashCode();
			if ((flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent) {
				hashCode ^= crcChecksum.GetHashCode();
			}
			if ((flags & ExtendedHeaderFlags.RestrictTag) == ExtendedHeaderFlags.RestrictTag) {
				hashCode ^= textEncodingRestriction.GetHashCode();
				hashCode ^= tagSizeRestriction.GetHashCode();
				hashCode ^= textFieldSizeRestriction.GetHashCode();
				hashCode ^= imageEncodingRestriction.GetHashCode();
				hashCode ^= imageSizeRestriction.GetHashCode();
			}
			
			return hashCode;
		}
    }//end class ExtendedHeader
}