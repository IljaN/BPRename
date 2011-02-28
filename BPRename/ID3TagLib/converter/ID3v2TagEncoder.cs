using System;
using System.IO;

namespace ID3TagLib {

	internal static class ID3v2TagEncoder {
		
		internal static byte[] EncodeTag(ID3v2Tag tag) {
			using (MemoryStream ms = new MemoryStream(8096)) {
				switch (tag.Version) {
					case ID3v2Version.ID3v2_2:
						EncodeID3v2_2Tag(ms, tag);
						break;
					
					case ID3v2Version.ID3v2_3:
						EncodeID3v2_3Tag(ms, tag);
						break;
					
					case ID3v2Version.ID3v2_4:
						EncodeID3v2_4Tag(ms, tag);
						break;
				}
				return ms.ToArray();
			}
		}
		
		/*
		 * ID3			Tag Identifier
		 * 02 00		2 bytes Version information
		 * xx			Tag flags
		 * xx xx xx xx	Tag size excluding header as sync safe integer
		 * {Frames}
		 * [Padding]	included in tag size
		 *
		 * Note: Unsync is applied on tag level; compression, encryption, footer,
		 *		 ExtendedHeader and experimental are not allowed.
		 *
		 */
		private static void EncodeID3v2_2Tag(Stream s, ID3v2Tag tag) {
			StringEncoder.WriteLatin1String(s, "ID3", 3);
			s.WriteByte(0x02);
			s.WriteByte(0x00);
			/* write flag byte */
			if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
				s.WriteByte((byte)0x80);
			} else {
				s.WriteByte((byte)0x00);
			}
			/* skip tag size */
			s.Position += 4L;
			
			WriteContentToStream(s, tag);
			s.SetLength(s.Length + ComputePaddingSize(s.Length, tag));
			
			/* update tag size */
			s.Position = 6L;
			NumberConverter.WriteInt(s.Length - 10L, s, 4, true);
		}
		
		/*
		 * ID3			Tag Identifier
		 * 03 00		2 bytes Version information
		 * xx			Tag flags
		 * xx xx xx xx	Tag size excluding header but including extended header as sync safe integer
		 * [Extended header]
		 * {Frames}
		 * [Padding]	included in tag size
		 *
		 * Note: Unsync is applied on tag level; footer is not allowed.
		 *
		 */
		private static void EncodeID3v2_3Tag(Stream s, ID3v2Tag tag) {
			long paddingLen;
			byte flags = 0;
			
			const long TagHeaderSize = 10L;
			const long ExtHeaderSizeWithCrc = 14L;
			const long ExtHeaderSizeWithoutCrc = 10L;
			
			/* generate flag byte */
			if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
				flags |= 0x80;
			}
			if (tag.ExtendedHeader != null) {
				flags |= 0x40;
			}
			if ((tag.Flags & ID3v2TagFlags.Experimental) == ID3v2TagFlags.Experimental) {
				flags |= 0x20;
			}
			
			/* write tag header */
			StringEncoder.WriteLatin1String(s, "ID3", 3);
			s.WriteByte(0x03);
			s.WriteByte(0x00);
			s.WriteByte(flags);
			
			/* skip size */
			s.Position += 4;
			
			if (tag.ExtendedHeader != null) {
				long crc;
				byte[] frameData = GetFrameRawData(tag);
				
				if ((tag.ExtendedHeader.Flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent) {
					crc = Crc32.ComputeCrc(frameData);
					paddingLen = ComputePaddingSize(frameData.Length + TagHeaderSize + ExtHeaderSizeWithCrc, tag);
				} else {
					crc = -1L;
					paddingLen = ComputePaddingSize(frameData.Length + TagHeaderSize + ExtHeaderSizeWithoutCrc, tag);
				}
				
				if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
					UnsyncFilterStream unsyncFilterStream = new UnsyncFilterStream(s, UnsyncMode.Write, true);
					
					EncodeID3v2_3ExtendedHeader(unsyncFilterStream, tag.ExtendedHeader, paddingLen, crc);
					unsyncFilterStream.Write(frameData, 0, frameData.Length);
					unsyncFilterStream.ApplyFinalization();
					
				} else {
					EncodeID3v2_3ExtendedHeader(s, tag.ExtendedHeader, paddingLen, crc);
					s.Write(frameData, 0, frameData.Length);
				}
				
			} else {
				/* no extended header */
				WriteContentToStream(s, tag);
				paddingLen = ComputePaddingSize(s.Length, tag);
			}
			
			/* update tag size */
			s.SetLength(s.Length + paddingLen);
			s.Position = 6;
			NumberConverter.WriteInt(s.Length - TagHeaderSize, s, 4, true);
		}
		
		/*
		 * xx xx xx xx		Extended Header size, excluding itself; currently 6 or 10 bytes; not sync safe.
		 * xx xx			Flags
		 * xx xx xx xx		Size of padding; not sync safe
		 * [xx xx xx xx]	4 bytes of CRC Data; not sync safe
		 *
		 */
		private static void EncodeID3v2_3ExtendedHeader(Stream s, ExtendedHeader extHeader, long paddingLen, long crc) {
			bool writeCRCData = ((extHeader.Flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent);
			int size = (writeCRCData ? 10 : 6);
			
			NumberConverter.WriteInt(size, s, 4, false);
			s.WriteByte((writeCRCData ? (byte)0x80 : (byte)0x00));
			s.WriteByte(0x00);
			NumberConverter.WriteInt(paddingLen, s, 4, false);
			if (writeCRCData) {
				NumberConverter.WriteInt(crc, s, 4, false);
				/* update crc checksum in extended header */
				extHeader.CrcChecksum = crc;
			}
		}
		
		/*
		 * ID3			Tag identifier
		 * 04 00		2 bytes version
		 * xx			Tag flags
		 * xx xx xx xx	Tag size excluding header and footer as syncsafe int
		 * [Extended header]
		 * {Frames}
		 * [Padding]
		 * [Footer]
		 */
		private static void EncodeID3v2_4Tag(Stream s, ID3v2Tag tag) {
			byte flagByte = 0;
			long paddingLen, size;
			
			const long TagHeaderSize = 10L;
			
			if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
				flagByte |= 0x80;
			}
			if (tag.ExtendedHeader != null) {
				flagByte |= 0x40;
			}
			if ((tag.Flags & ID3v2TagFlags.Experimental) == ID3v2TagFlags.Experimental) {
				flagByte |= 0x20;
			}
			if ((tag.Flags & ID3v2TagFlags.Footer) == ID3v2TagFlags.Footer) {
				flagByte |= 0x10;
			}
			
			StringEncoder.WriteLatin1String(s, "ID3", 3);
			s.WriteByte(0x04);
			s.WriteByte(0x00);
			s.WriteByte(flagByte);
			
			/* skip size */
			s.Position += 4;
			
			if (tag.ExtendedHeader != null) {
				byte[] frameData = GetFrameRawData(tag);
				long crc = -1L;
				
				if ((tag.ExtendedHeader.Flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent) {
					/* CRC is calculated on frame data and padding */
					Crc32 crcCalc = new Crc32();
					crcCalc.UpdateCrc(frameData);
					crcCalc.UpdateCrcWithZero(ComputePaddingSize(frameData.Length + TagHeaderSize, tag));
					crc = crcCalc.Crc;
				}
				EncodeID3v2_4ExtendedHeader(s, tag.ExtendedHeader, crc);
				s.Write(frameData, 0, frameData.Length);
				
			} else {	
				foreach (Frame f in tag.Frames) {
					FrameEncoder.EncodeFrame(s, f, tag);
				}
			}
			
			paddingLen = ComputePaddingSize(s.Length, tag);
			s.SetLength(s.Length + paddingLen);
			size = s.Length - TagHeaderSize;
			
			if ((tag.Flags & ID3v2TagFlags.Footer) == ID3v2TagFlags.Footer) {
				s.Position = s.Length - 1L;
				StringEncoder.WriteLatin1String(s, "3DI", 3);
				s.WriteByte(0x04);
				s.WriteByte(0x00);
				s.WriteByte(flagByte);
				NumberConverter.WriteInt(size, s, 4, true);
			}
			
			s.Position = 6L;
			NumberConverter.WriteInt(size, s, 4, true);
		}
		
		/*
		 * xx xx xx xx				Extended header size(min 6) as syncsafe int
		 * xx						Number of flag bytes(currently 1)
		 * xx						Flags
		 * {sizeByte [flagdata]}	For every set bit in flag one entry
		 *
		 */
		private static void EncodeID3v2_4ExtendedHeader(Stream s, ExtendedHeader extHeader, long crc) {
			int size = 6;
			byte flagByte = 0;
			
			/* compute flag byte */
			if ((extHeader.Flags & ExtendedHeaderFlags.UpdateTag) == ExtendedHeaderFlags.UpdateTag) {
				flagByte |= 0x40;
				size += 1;
			}
			if ((extHeader.Flags & ExtendedHeaderFlags.CrcPresent) == ExtendedHeaderFlags.CrcPresent) {
				flagByte |= 0x20;
				size += 6;
			}
			if ((extHeader.Flags & ExtendedHeaderFlags.RestrictTag) == ExtendedHeaderFlags.RestrictTag) {
				flagByte |= 0x10;
				size += 2;
			}
			
			/* write Header */
			NumberConverter.WriteInt(size, s, 4, true);
			s.WriteByte(0x01);
			s.WriteByte(flagByte);
			
			/* write attached data */
			if ((extHeader.Flags & ExtendedHeaderFlags.UpdateTag) != 0) {
				s.WriteByte(0x00);
			}
			if ((extHeader.Flags & ExtendedHeaderFlags.CrcPresent) != 0) {
				s.WriteByte(0x05);
				NumberConverter.WriteInt(crc, s, 5, true);
				/* update crc checksum in extended header */
				extHeader.CrcChecksum = crc;
			}
			if ((extHeader.Flags & ExtendedHeaderFlags.RestrictTag) != 0) {
				byte restrictFlagByte = 0;

				restrictFlagByte |= (byte)extHeader.TagSizeRestriction;
				restrictFlagByte |= (byte)extHeader.TextEncodingRestriction;
				restrictFlagByte |= (byte)extHeader.TextFieldSizeRestriction;
				restrictFlagByte |= (byte)extHeader.TextFieldSizeRestriction;
				restrictFlagByte |= (byte)extHeader.ImageEncodingRestriction;
				restrictFlagByte |= (byte)extHeader.ImageSizeRestriction;
				
				s.WriteByte(0x01);
				s.WriteByte(restrictFlagByte);
			}
		}
		
		private static long ComputePaddingSize(long curSize, ID3v2Tag tag) {
			long paddingLen;
			
			if (tag.PaddingMode == PaddingMode.PadToSize) {
				paddingLen = tag.PaddingSize - curSize;
			} else if (tag.PaddingMode == PaddingMode.PadFixAmount) {
				paddingLen = tag.PaddingSize;
			} else {
				paddingLen = 0;
			}
			
			if ((tag.Flags & ID3v2TagFlags.Footer) == ID3v2TagFlags.Footer) {
				paddingLen -= 10;
			}
			if (paddingLen < 0) {
				paddingLen = 0;
			}
			
			return paddingLen;
		}
		
		private static void WriteContentToStream(Stream s, ID3v2Tag tag) {
			if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
				UnsyncFilterStream unsyncFilterStream = new UnsyncFilterStream(s, UnsyncMode.Write, true);
				
				foreach (Frame f in tag.Frames) {
					FrameEncoder.EncodeFrame(unsyncFilterStream, f, tag);
				}
				unsyncFilterStream.ApplyFinalization();
			} else {
				foreach (Frame f in tag.Frames) {
					FrameEncoder.EncodeFrame(s, f, tag);
				}
			}
		}
		
		private static byte[] GetFrameRawData(ID3v2Tag tag) {
			using (MemoryStream ms = new MemoryStream(1024)) {
				foreach (Frame f in tag.Frames) {
					FrameEncoder.EncodeFrame(ms, f, tag);
				}
				
				return ms.ToArray();
			}
		}
	}// end class ID3v2TagEncoder
}