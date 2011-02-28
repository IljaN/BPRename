using System;
using System.IO;
using System.ComponentModel;

namespace ID3TagLib {
	
	internal static class FrameEncoder {
		
		internal static void EncodeFrame(Stream s, Frame frame, ID3v2Tag tag) {
			switch (tag.Version) {
				case ID3v2Version.ID3v2_2:
					EncodeID3v2_2Frame(s, frame, tag);
					break;
					
				case ID3v2Version.ID3v2_3:
					EncodeID3v2_3Frame(s, frame, tag);
					break;
					
				case ID3v2Version.ID3v2_4:
					EncodeID3v2_4Frame(s, frame, tag);
					break;
					
				default:
					throw new InvalidEnumArgumentException("tag.Version", (int)tag.Version, typeof(ID3v2Version));
			}
		}
		
		/*
		 *	xx xx xx Frame identifier
		 *	xx xx xx Frame size excluding frame header
		 *	Frame content
		 *	Note: Unsynchronisation is done on Tag level, no Compression or Encryption schema defined.
		 */
		private static void EncodeID3v2_2Frame(Stream s, Frame frame, ID3v2Tag tag) {
			string id;
			byte[] data;
			
			id = FrameFactory.ID3v2_3IdToID3v2_2Id(frame.FrameId);
			data = frame.EncodeContent(tag);
			
			StringEncoder.WriteLatin1String(s, id, 3);
			NumberConverter.WriteInt(data.Length, s, 3, false);
			s.Write(data, 0, data.Length);
		}
		
		/*
		 *	xx xx xx xx Frame identifier
		 *	xx xx xx xx Frame size excluding frame header
		 *	xx xx		Frame flags
		 *  additinal header bytes(group identifier, encryption, compression)
		 *	Frame content
		 *	Note: Unsynchronisation is done on Tag level, Compression and Encryption on Frame level.
		 */
		private static void EncodeID3v2_3Frame(Stream s, Frame frame, ID3v2Tag tag) {
			byte flagByteHi, flagByteLo, encryptionSymbol = 0;
			byte[] data = frame.EncodeContent(tag);
			int additionalBytes = 0, originalSize = data.Length;
			
			/* generate frame flags */
			flagByteHi = 0;
			if ((frame.Flags & FrameFlags.PreserveTagAltered) == 0) {
				/* discard frame if tag altered is 1! */
				flagByteHi |= 0x80;
			}
			if ((frame.Flags & FrameFlags.PreserveFileAltered) == 0) {
				/* discard frame if file altered is 1! */
				flagByteHi |= 0x40;
			}
			if ((frame.Flags & FrameFlags.ReadOnly) == FrameFlags.ReadOnly) {
				flagByteHi |= 0x20;
			}
			
			flagByteLo = 0;
			if ((frame.Flags & FrameFlags.Compress) == FrameFlags.Compress) {
				/* set bit 0x80 in flag and append 4 bytes of uncompressed size after header */
				flagByteLo |= 0x80;
				additionalBytes += 4;
				data = CompressData(data);
			}
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				/* set bit 0x40 in flag and append 1 byte indicating encryption method after header */
				flagByteLo |= 0x40;
				additionalBytes++;
				data = EncryptData(data, frame, out encryptionSymbol);
			}
			if (frame.GroupIdentifier != Frame.GroupIdNotSet) {
				flagByteLo |= 0x20;
				additionalBytes++;
			}
			
			/* Write Header */
			StringEncoder.WriteLatin1String(s, frame.FrameId, 4);
			NumberConverter.WriteInt(data.Length + additionalBytes, s, 4, false);
			s.WriteByte(flagByteHi);
			s.WriteByte(flagByteLo);
			
			/* Write additional information */
			if ((frame.Flags & FrameFlags.Compress) == FrameFlags.Compress) {
				NumberConverter.WriteInt(originalSize, s, 4, false);
			}
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				s.WriteByte(encryptionSymbol);
			}
			if (frame.GroupIdentifier != Frame.GroupIdNotSet) {
				s.WriteByte((byte)frame.GroupIdentifier);
			}
			
			/* write data */
			s.Write(data, 0, data.Length);
		}
		
		/*
		 *	xx xx xx xx Frame identifier
		 *	xx xx xx xx Frame size excluding frame header as sync safe integer
		 *	xx xx		Frame flags
		 *  additinal header bytes(group identifier, encryption, compression, data length indicator)
		 *	Frame content
		 *	Note: Unsynchronisation, Compression and Encryption is done on Frame level.
		 */
		private static void EncodeID3v2_4Frame(Stream s, Frame frame, ID3v2Tag tag) {
			bool setDataLengthIndicator = false;
			byte flagByteHi, flagByteLo, encryptionSymbol = 0;
			byte[] data = frame.EncodeContent(tag);
			int additionalBytes = 0, originalSize = data.Length;
			
			/* build the flag bytes */
			flagByteHi = 0;
			if ((frame.Flags & FrameFlags.PreserveTagAltered) == 0) {
				flagByteHi |= 0x40;
			}
			if ((frame.Flags & FrameFlags.PreserveFileAltered) == 0) {
				flagByteHi |= 0x20;
			}
			if ((frame.Flags & FrameFlags.ReadOnly) == FrameFlags.ReadOnly) {
				flagByteHi |= 0x20;
			}
			
			flagByteLo = 0;
			if (frame.GroupIdentifier != Frame.GroupIdNotSet) {
				/* add one byte after header(group identifier) */
				flagByteLo |= 0x40;
				additionalBytes++;
			}
			if ((frame.Flags & FrameFlags.Compress) == FrameFlags.Compress) {
				flagByteLo |= 0x08;
				setDataLengthIndicator = true;
				data = CompressData(data);
				
			}
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				flagByteLo |= 0x04;
				additionalBytes++;
				setDataLengthIndicator = true;
				data = EncryptData(data, frame, out encryptionSymbol);
				
			}
			if ((tag.Flags & ID3v2TagFlags.Unsync) == ID3v2TagFlags.Unsync) {
				flagByteLo |= 0x02;
				setDataLengthIndicator = true;
				data = Util.ApplyUnsynchronization(data);
			}
			if (setDataLengthIndicator) {
				flagByteLo |= 0x01;
				additionalBytes += 4;
			}
			
			/* write frame header */
			StringEncoder.WriteLatin1String(s, frame.FrameId, 4);
			NumberConverter.WriteInt(data.Length + additionalBytes, s, 4, true);
			s.WriteByte(flagByteHi);
			s.WriteByte(flagByteLo);
			
			/* write attached data */
			if (frame.GroupIdentifier != Frame.GroupIdNotSet) {
				s.WriteByte((byte)frame.GroupIdentifier);
			}
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				s.WriteByte(encryptionSymbol);
			}
			if (setDataLengthIndicator) {
				NumberConverter.WriteInt(originalSize, s, 4, true);
			}
			
			s.Write(data, 0, data.Length);
		}
		
		private static byte[] CompressData(byte[] data) {
			throw new NotImplementedException("Compression is not yet implemented");
		}
		
		private static byte[] EncryptData(byte[] frameData, Frame f, out byte encryptionSymbol) {
			throw new NotImplementedException("Encrytion is not yet implemented");
		}
    }//end class FrameEncoder
}