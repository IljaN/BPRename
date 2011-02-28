using System;
using System.IO;

namespace ID3TagLib {
	
	internal static class NumberConverter {
		
		internal static long ReadInt64(byte[] buffer, int offset, int length, bool isSynchronized) {
			long value = 0L;
			int endIndex = Math.Min(buffer.Length, offset + length);
			
			for (int i = offset; i < endIndex; i++) {
				if (isSynchronized) {
					if ((buffer[i] & 0x80) == 0x80) {
						throw new ID3ParseException("Invalid size descriptor.");
					}
					value = (value << 7) | buffer[i];
					
				} else {
					value = (value << 8) | buffer[i];
				}
			}
			
			if (value < 0L) {
				value = 0L;
			}
			
			return value;
		}
		
		internal static long ReadInt64(Stream s, int count, bool isSynchronized) {
			byte[] buffer = new byte[count];
			
			Util.ReadFully(s, buffer);
			return ReadInt64(buffer, 0, count, isSynchronized);
		}
		
		internal static int ReadInt32(byte[] buffer, int offset, int length, bool isSynchronized) {
			int value = 0;
			int endIndex = Math.Min(buffer.Length, offset + length);
			
			for (int i = offset; i < endIndex; i++) {
				if (isSynchronized) {
					if ((buffer[i] & 0x80) == 0x80) {
						throw new ID3ParseException("Invalid size descriptor.");
					}
					value = (value << 7) | buffer[i];
					
				} else {
					value = (value << 8) | buffer[i];
				}
			}
			
			if (value < 0) {
				value = 0;
			}
			
			return value;
		}
		
		internal static int ReadInt32(Stream s, int count, bool isSynchronized) {
			byte[] buffer = new byte[count];
			
			Util.ReadFully(s, buffer);
			return ReadInt32(buffer, 0, count, isSynchronized);
		}
		
		internal static void WriteInt(long value, byte[] buffer, int offset, int length, bool isSynchronized) {
			int maskByte, shift, decrement;
			
			if (isSynchronized) {
				maskByte = 0x7F;
				shift = 7 * (length - 1);
				decrement = 7;
			} else {
				maskByte = 0xFF;
				shift = 8 * (length - 1);
				decrement = 8;
			}
			
			for (int i = 0; i < length; i++) {
				buffer[i + offset] = (byte)((value >> shift) & maskByte);
				shift -= decrement;
			}
		}
		
		internal static void WriteInt(int value, byte[] buffer, int offset, int length, bool isSynchronized) {
			int maskByte, shift, decrement;
			
			if (isSynchronized) {
				maskByte = 0x7F;
				shift = 7 * (length - 1);
				decrement = 7;
			} else {
				maskByte = 0xFF;
				shift = 8 * (length - 1);
				decrement = 8;
			}
			
			for (int i = 0; i < length; i++) {
				buffer[i + offset] = (byte)((value >> shift) & maskByte);
				shift -= decrement;
			}
		}
		
		internal static void WriteInt(long value, Stream s, int length, bool isSynchronized) {
			int maskByte, shift, decrement;
			
			if (isSynchronized) {
				maskByte = 0x7F;
				shift = 7 * (length - 1);
				decrement = 7;
			} else {
				maskByte = 0xFF;
				shift = 8 * (length - 1);
				decrement = 8;
			}
			
			for (int i = 0; i < length; i++) {
				s.WriteByte((byte)((value >> shift) & maskByte));
				shift -= decrement;
			}
		}
		
		internal static void WriteInt(int value, Stream s, int length, bool isSynchronized) {
			int maskByte, shift, decrement;
			
			if (isSynchronized) {
				maskByte = 0x7F;
				shift = 7 * (length - 1);
				decrement = 7;
			} else {
				maskByte = 0xFF;
				shift = 8 * (length - 1);
				decrement = 8;
			}
			
			for (int i = 0; i < length; i++) {
				s.WriteByte((byte)((value >> shift) & maskByte));
				shift -= decrement;
			}
		}
		
		internal static int ByteCount(long number, int minimumByteCount) {
			int length;
			
			if (number > 0xFFFFFFFFFFFFFFL) {
				length = 8;
			} else if (number > 0xFFFFFFFFFFFFL) {
				length = 7;
			} else if (number > 0xFFFFFFFFFFL) {
				length = 6;
			} else if (number > 0xFFFFFFFFL) {
				length = 5;
			} else if (number > 0xFFFFFFL) {
				length = 4;
			} else if (number > 0xFFFFL) {
				length = 3;
			} else if (number > 0xFFL) {
				length = 2;
			} else {
				length = 1;
			}
			if (length < minimumByteCount) {
				length = minimumByteCount;
			}
			
			return length;
		}
		
	}
}