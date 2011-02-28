using System;
using System.IO;
using System.ComponentModel;

namespace ID3TagLib {

	internal static class FrameDecoder {
		
		internal static Frame DecodeFrame(Stream s, ID3v2Tag tag) {
			switch (tag.Version) {
				case ID3v2Version.ID3v2_2:
					return DecodeID3v2_2Frame(s, tag);
					
				case ID3v2Version.ID3v2_3:
					return DecodeID3v2_3Frame(s, tag);
					
				case ID3v2Version.ID3v2_4:
					return DecodeID3v2_4Frame(s, tag);
					
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
		private static Frame DecodeID3v2_2Frame(Stream s, ID3v2Tag tag) {
			Frame frame;
			byte[] frameHeader;
			byte[] frameContent;
			string frameID;
			int frameSize;
			
			const int FrameHeaderSize = 6;
			
			frameHeader = new byte[FrameHeaderSize];
			Util.ReadFully(s, frameHeader);
			
			frameID = StringDecoder.DecodeLatin1String(frameHeader, 0, 3);
			CheckFrameID(frameID, 3);
			
			frameSize = NumberConverter.ReadInt32(frameHeader, 3, 3, false);
			CheckSize(frameSize, s);
			
			frameContent = new byte[frameSize];
			Util.ReadFully(s, frameContent);
			
			frame = FrameFactory.GetFrame(FrameFactory.ID3v2_2IdToID3v2_3Id(frameID));
            frame.Flags = FrameFlags.None;
			frame.DecodeContent(frameContent, tag);
			
			return frame;
		}
		
		/*
		 *	xx xx xx xx Frame identifier
		 *	xx xx xx xx Frame size excluding frame header
		 *	xx xx		Frame flags
		 *  additinal header bytes(group identifier, encryption, compression)
		 *	Frame content
		 *	Note: Unsynchronisation is done on Tag level, Compression and Encryption on Frame level.
		 */
		private static Frame DecodeID3v2_3Frame(Stream s, ID3v2Tag tag) {
			Frame frame;
			byte[] frameHeader;
			byte[] frameContent;
			string frameID;
			byte encryptionMethodSymbol = 0;
			int frameSize, uncompressedSize = 0;
			
			const int FrameHeaderSize = 10;
			
			frameHeader = new byte[FrameHeaderSize];
			Util.ReadFully(s, frameHeader);
			
			frameID = StringDecoder.DecodeLatin1String(frameHeader, 0, 4);
			CheckFrameID(frameID, 4);
			
			frameSize = NumberConverter.ReadInt32(frameHeader, 4, 4, false);
			CheckSize(frameSize, s);
			
			frame = FrameFactory.GetFrame(frameID);
			frame.Flags = FrameFlags.None;
			
			/* two flag bytes 8 and 9 */
            if ((frameHeader[8] & 0x80) == 0) {
                frame.Flags |= FrameFlags.PreserveTagAltered;
            }
            if ((frameHeader[8] & 0x40) == 0) {
                frame.Flags |= FrameFlags.PreserveFileAltered;
            }
            if ((frameHeader[8] & 0x20) != 0) {
                frame.Flags |= FrameFlags.ReadOnly;
            }
            if ((frameHeader[8] & 0x1F) != 0) {
                throw new ID3ParseException("Undefined frame flag(s) set.");
            }
			
			if ((frameHeader[9] & 0x80) != 0) {
                /* 4 bytes will be added after header */
				frame.Flags |= FrameFlags.Compress;
				uncompressedSize = NumberConverter.ReadInt32(s, 4, false);
				frameSize -= 4;
            }
            if ((frameHeader[9] & 0x40) != 0) {
                /* one byte will be added after header */
                frame.Flags |= FrameFlags.Encrypt;
				encryptionMethodSymbol = Util.ReadByte(s);
				frameSize--;
				
				throw new NotImplementedException("Encryption is not yet implemented.");
            }
            if ((frameHeader[9] & 0x20) != 0) {
                /* read group identifier byte after header */
                frame.GroupIdentifier = Util.ReadByte(s);
                frameSize--;
            }
            if ((frameHeader[9] & 0x1F) != 0) {
                throw new ID3TagException("Undefined frame flag(s) set.");
            }
			
			frameContent = new byte[frameSize];
			Util.ReadFully(s, frameContent);
			
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				frameContent = DecryptData(frameContent, encryptionMethodSymbol); 
			}
			
			if ((frame.Flags & FrameFlags.Compress) == FrameFlags.Compress) {
				frameContent = DecompressData(frameContent, uncompressedSize);
			}
			
			frame.DecodeContent(frameContent, tag);
			
			return frame;
		}
		
		/*
		 *	xx xx xx xx Frame identifier
		 *	xx xx xx xx Frame size excluding frame header as sync safe integer
		 *	xx xx		Frame flags
		 *  additinal header bytes(group identifier, encryption, compression, data length indicator)
		 *	Frame content
		 *	Note: Unsynchronisation, Compression and Encryption is done on Frame level.
		 */
		private static Frame DecodeID3v2_4Frame(Stream s, ID3v2Tag tag) {
			Frame frame;
			byte[] frameHeader;
			byte[] frameContent;
			string frameID;
			int frameSize, dataLengthIndicator = -1;
			bool unsyncApplied = false;
			byte encryptionMethodSymbol = 0;
			
			const int TagHeaderSize = 10;
			
			frameHeader = new byte[TagHeaderSize];
			Util.ReadFully(s, frameHeader);
			
			frameID = StringDecoder.DecodeLatin1String(frameHeader, 0, 4);
			CheckFrameID(frameID, 4);
			
			frameSize = NumberConverter.ReadInt32(frameHeader, 4, 4, true);
			CheckSize(frameSize, s);
			
			frame = FrameFactory.GetFrame(frameID);
			frame.Flags = FrameFlags.None;
			
			/* two flag bytes 8 and 9 */
            if ((frameHeader[8] & 0x40) == 0) {
                frame.Flags |= FrameFlags.PreserveTagAltered;
            }
            if ((frameHeader[8] & 0x20) == 0) {
                frame.Flags |= FrameFlags.PreserveFileAltered;
            }
            if ((frameHeader[8] & 0x10) != 0) {
                frame.Flags |= FrameFlags.ReadOnly;
            }
            if ((frameHeader[8] & 0x8F) != 0) {
                throw new ID3ParseException("Undefined frame flag(s) set.");
            }
            
            if ((frameHeader[9] & 0x40) != 0) {
                /* read group identifier byte after header */
                frame.GroupIdentifier = Util.ReadByte(s);
                frameSize--;
            }
            if ((frameHeader[9] & 0x08) != 0) {
				/* data length indicator must be set. */
                frame.Flags |= FrameFlags.Compress;
            }
            if ((frameHeader[9] & 0x04) != 0) {
                /* 1 byte will be added after header */
				frame.Flags |= FrameFlags.Encrypt;
				encryptionMethodSymbol = Util.ReadByte(s);
				frameSize--;
            }
            if ((frameHeader[9] & 0x02) != 0) {
                unsyncApplied = true;/* indicates that unsync was applied to this frame */
				tag.Flags |= ID3v2TagFlags.Unsync; /* indicates that at least one frame in the tag is unsynchronized. */
            }
            if ((frameHeader[9] & 0x01) != 0) {
                /* data length indicator added, 4 bytes syncsafe int */
                dataLengthIndicator = NumberConverter.ReadInt32(s, 4, true);
				frameSize -= 4;
            }
            if ((frameHeader[9] & 0xB0) != 0) {
                throw new ID3ParseException("Undefined frame flag(s) set.");
            }
			
			frameContent = new byte[frameSize];
			Util.ReadFully(s, frameContent);
			
			if (unsyncApplied) {
				frameContent = Util.RemoveUnsynchronization(frameContent);
			}
			if ((frame.Flags & FrameFlags.Encrypt) == FrameFlags.Encrypt) {
				frameContent = DecryptData(frameContent, encryptionMethodSymbol);
			}
			if ((frame.Flags & FrameFlags.Compress) == FrameFlags.Compress) {
				if (dataLengthIndicator == -1) {
					throw new ID3ParseException("Frame is compressed but data length indicator bit is not set.");
				}
				frameContent = DecompressData(frameContent, dataLengthIndicator);
			}
			
			frame.DecodeContent(frameContent, tag);
			
			return frame;
		}
		
		private static void CheckFrameID(string frameID, int len) {
			if (frameID.Length != len) {
				throw new ID3ParseException("Error while parsing frame ID.");
			}
			
			for (int i = 0; i < frameID.Length; i++) {
				char c = frameID[i];
				
				if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9')) {
					throw new ID3ParseException("Error while parsing frame ID.");
				}				
			}
			
		}
		
		private static void CheckSize(int size, Stream s) {
			if (s.CanSeek && size > s.Length - s.Position) {
				throw new ID3ParseException("Frame size mismatch.");
			}
		}
		
		private static byte[] DecompressData(byte[] data, int uncompressedSize) {
			throw new NotImplementedException("Compression is not yet implemented.");
		}
		
		private static byte[] DecryptData(byte[] data, byte symbol) {
			throw new NotImplementedException("Encryption is not yet implemented.");
		}
	}
}