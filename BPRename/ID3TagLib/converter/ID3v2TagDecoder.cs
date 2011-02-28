using System;
using System.IO;

namespace ID3TagLib {

	internal static class ID3v2TagDecoder {
		
		internal const int TagHeaderSize = 10;
		
		/*
		 * TagHeader:
		 *
		 * ID3			Tag identifier
		 * xx		 	Major version byte
		 * xx			Minor version byte; always zero
		 * xx			Flag byte
		 * xx xx xx xx	Tag size as unsync int, excluding tag header and footer but including ext. header.
		 * Tag data
		 *
		 */
		internal static ID3v2Tag DecodeTag(Stream s, ReadMode mode) {
			ID3v2Tag tag;
			byte[] tagHeader = new byte[TagHeaderSize];
			byte flag;
			int tagSize, wholeTagSize;
			
			try {
				Util.ReadFully(s, tagHeader);
				if (!HasID3v2Tag(tagHeader)) {
					return null;
				}
				flag = tagHeader[5];
				tagSize = NumberConverter.ReadInt32(tagHeader, 6, 4, true);
				wholeTagSize = GetID3v2TagSize(tagHeader);
				
				switch (tagHeader[3]) {
					case 0x02:
						tag = DecodeID3v2_2Tag(s, flag, tagSize, mode);
						break;
					
					case 0x03:
						tag = DecodeID3v2_3Tag(s, flag, tagSize, mode);
						break;
						
					case 0x04:
						tag = DecodeID3v2_4Tag(s, flag, tagSize, mode);
						break;
					
					default:
						throw new ID3ParseException("Invalid ID3v2 tag version.");
				}
				
				tag.PaddingMode = PaddingMode.PadToSize;
				tag.PaddingSize = wholeTagSize;
				
				return tag;
				
			} catch (ID3TagException) {
				throw;
			} catch (IOException e) {
				throw new ID3TagException("IO Exception occured while parsing the tag.", e);
			} catch (Exception e) {
				throw new ID3TagException("Exception occured while parsing the tag.", e);
			}
		}
		
		internal static bool HasID3v2Tag(byte[] tagHeader) {
			return tagHeader[0] == 'I' && tagHeader[1] == 'D' && tagHeader[2] == '3' && 
				   (tagHeader[3] >= 0x02 && tagHeader[3] <= 0x04) && tagHeader[4] < 0xFF &&
				   tagHeader[6] < 0x80 && tagHeader[7] < 0x80 && tagHeader[8] < 0x80 && tagHeader[9] < 0x80;
		}
		
		internal static int GetID3v2TagSize(byte[] tagHeader) {
			int tagSize;
			
			if (!HasID3v2Tag(tagHeader)) {
				return -1;
			}
			
			tagSize = NumberConverter.ReadInt32(tagHeader, 6, 4, true);
			
			switch (tagHeader[3]) {
				case 0x02:
				case 0x03:
					return tagSize + TagHeaderSize;
				
				case 0x04:
					if ((tagHeader[5] & 0x10) != 0) {
						/* footer present */
						return tagSize + 2 * TagHeaderSize;
					} else {
						return tagSize + TagHeaderSize;
					}
				
				default:
					throw new ID3ParseException("Invalid ID3v2 tag version.");
			}
		}
		
		/*
		 * Tag header
		 * {Frames}
		 * [padding]
		 *
		 * Note: unsync is applied on tag level(after header); no ext. header or compression
		 */
		private static ID3v2Tag DecodeID3v2_2Tag(Stream s, byte tagFlag, int tagSize, ReadMode mode) {
			byte[] tagContent;
			ID3v2Tag tag;
			
			tag = new ID3v2Tag();
			tag.Version = ID3v2Version.ID3v2_2;
            tag.ExtendedHeader = null;
			tag.Flags = ID3v2TagFlags.None;
			
			tagContent = new byte[tagSize];
			Util.ReadFully(s, tagContent);
			
			/* handle tag flags */
			if ((tagFlag & 0x80) != 0) {
				tag.Flags |= ID3v2TagFlags.Unsync;
				tagContent = Util.RemoveUnsynchronization(tagContent);
			}
            if ((tagFlag & 0x40) != 0) {
                throw new ID3ParseException("Compression bit set although no compression schema defined in 2.2.");
            }
            if ((tagFlag & 0x3F) != 0) {
                /* some other undefined flags set */
                throw new ID3ParseException("Undefined tag flag(s) set.");
            }
			
			ReadFrames(tagContent, 0, tag, mode);
			
			return tag;
		}
		
		/*
		 * Tagheader
		 * [Extended header]
		 * {Frames}
		 * [Padding]	included in tag size
		 *
		 * Note: Unsync is applied on tag level; footer is not allowed.
		 *
		 */
		private static ID3v2Tag DecodeID3v2_3Tag(Stream s, byte flag, int tagSize, ReadMode mode) {
			byte[] tagContent;
			int offset = 0;
			ID3v2Tag tag;
			
			tag = new ID3v2Tag();
            tag.Version = ID3v2Version.ID3v2_3;
            tag.ExtendedHeader = null;
			tag.Flags = ID3v2TagFlags.None;
			
			tagContent = new byte[tagSize];
			Util.ReadFully(s, tagContent);
			
			/* handle tag flags */
            if ((flag & 0x80) != 0) {
				tag.Flags |= ID3v2TagFlags.Unsync;
				tagContent = Util.RemoveUnsynchronization(tagContent);
			}
			if ((flag & 0x40) != 0) {
                tag.ExtendedHeader = new ExtendedHeader();
				offset = DecodeID3v2_3ExtendedHeader(tagContent, tag.ExtendedHeader);
            }
			if ((flag & 0x20) != 0) {
                tag.Flags |= ID3v2TagFlags.Experimental;
            }
            if ((flag & 0x1F) != 0) {
                /* undefined flags set */
                throw new ID3TagException("Undefined flags set.");
            }
            
            ReadFrames(tagContent, offset, tag, mode);
			
			return tag;
		}
		
		/*
		 * xx xx xx xx		Extended Header size, excluding itself; currently 6 or 10 bytes; not sync safe.
		 * xx xx			Flags
		 * xx xx xx xx		Size of padding; not sync safe
		 * [xx xx xx xx]	4 bytes of CRC Data; not sync safe
		 *
		 */
		private static int DecodeID3v2_3ExtendedHeader(byte[] tagContent, ExtendedHeader extHeader) {
			int extHeaderSize, paddingSize;
			
			CheckLength(tagContent, 10);
			extHeaderSize = NumberConverter.ReadInt32(tagContent, 0, 4, false);
			paddingSize = NumberConverter.ReadInt32(tagContent, 6, 4, false);
			
			if ((tagContent[4] & 0x7F) != 0 || tagContent[5] != 0x00) {
				throw new ID3ParseException("Unknown extended header flag set.");
			}
			
			if (tagContent[4] == 0x80) {
				extHeader.Flags = ExtendedHeaderFlags.CrcPresent;
				if (extHeaderSize != 10) {
					throw new ID3ParseException("Invalid extended header size.");
				}
				CheckLength(tagContent, 14);
				
				long savedCrc = NumberConverter.ReadInt64(tagContent, 10, 4, false);
				int dataLength = tagContent.Length - 14 - paddingSize;
				Crc32 crcCalculator = new Crc32();
				
				if (dataLength < 0) {
					throw new ID3ParseException("Padding size mismatch in extended header.");
				}
				crcCalculator.UpdateCrc(tagContent, 14, dataLength);
				if (savedCrc != crcCalculator.Crc) {
					throw new ID3CrcMismatchException(savedCrc, crcCalculator.Crc);
				}
				extHeader.CrcChecksum = savedCrc;
				
			} else {
				extHeader.Flags = ExtendedHeaderFlags.None;
				if (extHeaderSize != 6) {
					throw new ID3ParseException("Invalid extended header size.");
				}
			}
			
			return extHeaderSize;
		}
		
		/*
		 * Tagheader
		 * [Extended header]
		 * {Frames}
		 * [Padding]
		 * [Footer]
		 *
		 * Note footer is not included in size; unsync is done on frame level
		 */
		private static ID3v2Tag DecodeID3v2_4Tag(Stream s, byte flag, int tagSize, ReadMode mode) {
			byte[] tagContent;
			int offset = 0;
			ID3v2Tag tag;
            
			tag = new ID3v2Tag();
            tag.Version = ID3v2Version.ID3v2_4;
            tag.ExtendedHeader = null;
			tag.Flags = ID3v2TagFlags.None;
            
			tagContent = new byte[tagSize];
			Util.ReadFully(s, tagContent);
			
            if ((flag & 0x80) != 0) {
                /* all frames are really unsychronized */
				tag.Flags = ID3v2TagFlags.Unsync;
            }
            if ((flag & 0x40) != 0) {
                tag.ExtendedHeader = new ExtendedHeader();
				offset = DecodeID3v2_4ExtendedHeader(tagContent, tag.ExtendedHeader);
            }
            if ((flag & 0x20) != 0) {
                tag.Flags |= ID3v2TagFlags.Experimental; 
            }
            if ((flag & 0x10) != 0) {
                tag.Flags |= ID3v2TagFlags.Footer;
				
				const int footerSize = 10;
				byte[] footer = new byte[footerSize];
				
				Util.ReadFully(s, footer);
				if (footer[0] != '3' || footer[1] != 'D' || footer[2] != 'I' || footer[3] != 0x04 ||
					footer[4] != 0x00 || footer[5] != flag || (int)NumberConverter.ReadInt32(footer, 6, 4, true) != tagSize) {
				
					throw new ID3ParseException("Error while parsing footer.");
				}
            }
            if ((flag & 0x0F) != 0) {
                throw new ID3ParseException("Undefined flag(s) set");
            }
			
			ReadFrames(tagContent, offset, tag, mode);
			
			return tag;
		}
		
		/*
		 * xx xx xx xx				Extended header size(min 6) as syncsafe int
		 * xx						Number of flag bytes(currently 1)
		 * xx						Flags
		 * {sizeByte [flagdata]}	For every set bit in flag one entry
		 *
		 */
		private static int DecodeID3v2_4ExtendedHeader(byte[] tagContent, ExtendedHeader extHeader) {
			int extHeaderSize, index;
			
			CheckLength(tagContent, 6);
			extHeaderSize = NumberConverter.ReadInt32(tagContent, 0, 4, true);
			if (extHeaderSize < 6) {
				throw new ID3ParseException("Size mismatch in extended header.");
			}
			if (tagContent[4] != 0x01) {
				throw new ID3ParseException("Flag byte count in extended header does not match.");
			}
			CheckLength(tagContent, extHeaderSize);
			
			/* parse extended header flags */
			extHeader.Flags = ExtendedHeaderFlags.None;
            index = 6;
			if ((tagContent[5] & 0x40) != 0) {
				extHeader.Flags |= ExtendedHeaderFlags.UpdateTag;
				if (index >= extHeaderSize || tagContent[index] != 0x00) {
					throw new ID3ParseException("Error while parsing extended header flag data.");
				}
				index += 1;
			}
			if ((tagContent[5] & 0x20) != 0) {
				extHeader.Flags |= ExtendedHeaderFlags.CrcPresent;
				if (index + 5 >= extHeaderSize || tagContent[index] != 0x05) {
					throw new ID3ParseException("Error while parsing extended header flag data.");
				}
				
				long savedCrc = NumberConverter.ReadInt64(tagContent, index + 1, 5, true);
				Crc32 crcCalculator = new Crc32();
				
				crcCalculator.UpdateCrc(tagContent, extHeaderSize, tagContent.Length - extHeaderSize);
				if (savedCrc != crcCalculator.Crc) {
					throw new ID3CrcMismatchException(savedCrc, crcCalculator.Crc);
				}
				extHeader.CrcChecksum = savedCrc;
				
				index += 6;
			}
			if ((tagContent[5] & 0x10) != 0) {
				extHeader.Flags |= ExtendedHeaderFlags.RestrictTag;
				if (index + 1 >= extHeaderSize || tagContent[index] != 0x01) {
					throw new ID3ParseException("Error while parsing extended header flag data.");
				}
				index += 1;
				extHeader.TagSizeRestriction = (TagSizeRestriction)(tagContent[index] & 0xC0);
				extHeader.TextEncodingRestriction = (TextEncodingRestriction)(tagContent[index] & 0x20);
				extHeader.TextFieldSizeRestriction = (TextFieldSizeRestriction)(tagContent[index] & 0x18);
				extHeader.ImageEncodingRestriction = (ImageEncodingRestriction)(tagContent[index] & 0x4);
				extHeader.ImageSizeRestriction = (ImageSizeRestriction)(tagContent[index] & 0x3);
				index += 1;
			}
			if ((tagContent[5] & 0x8F) != 0) {
				throw new ID3ParseException("Invalid entended header flag set.");
			}
			if (index != extHeaderSize) {
				throw new ID3ParseException("Size mismatch in extended header.");
			}
			
			return extHeaderSize;
		}
		
		private static void ReadFrames(byte[] tagContent, int offset, ID3v2Tag tag, ReadMode mode) {
			MemoryStream s = new MemoryStream(tagContent, offset, tagContent.Length - offset, false);
			
			while (offset + s.Position < tagContent.Length && tagContent[offset + s.Position] != 0x00) {
				try {
					tag.Frames.Add(FrameDecoder.DecodeFrame(s, tag));
				} catch {
					if (mode == ReadMode.ThrowOnError) {
						throw;
					}
				}
			}
			
			s.Close();
		}
		
		private static void CheckLength(byte[] arr, int minLength) {
			if (arr.Length < minLength) {
				throw new ID3ParseException("Size mismatch: unexpected end of tag encountered.");
			}
		}
	}
}