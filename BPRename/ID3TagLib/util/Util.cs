using System;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace ID3TagLib {
	
	internal static class Util {
		
		internal static void ReadFully(Stream s, byte[] buffer) {
			int offset = 0;
			int count = buffer.Length;
			int bytesRead = 0;
			
			while ((bytesRead = s.Read(buffer, offset, count)) > 0) {
				offset += bytesRead;
				count -= bytesRead;
			}
			
			if (offset != buffer.Length) {
				/* end of Stream reached */
				throw new ID3ParseException("Unexpected end of Stream encountered.");
			}
		}
		
		internal static byte ReadByte(Stream s) {
			int val = s.ReadByte();
			
			if (val == -1) {
				throw new ID3ParseException("Unexpected end of Stream encountered.");
			}
			
			return (byte)val;
		}
		
		internal static byte[] RemoveUnsynchronization(byte[] data) {
			MemoryStream ms = new MemoryStream(data, false);
			UnsyncFilterStream fs = new UnsyncFilterStream(ms, UnsyncMode.Read, true);
			
			byte[] synchronizedData = new byte[data.Length];
			int bytesRead;
			
			bytesRead = fs.Read(synchronizedData, 0, synchronizedData.Length);
			
			// can be == or less 
			if (bytesRead < synchronizedData.Length) {
				byte[] tmp = new byte[bytesRead];
				
				Buffer.BlockCopy(synchronizedData, 0, tmp, 0, bytesRead);
				synchronizedData = tmp;
			}
			
			return synchronizedData;
		}
		
		internal static byte[] ApplyUnsynchronization(byte[] data) {
			MemoryStream ms = new MemoryStream(data.Length * 2);
			UnsyncFilterStream fs = new UnsyncFilterStream(ms, UnsyncMode.Write, true);
			
			fs.Write(data, 0, data.Length);
			fs.ApplyFinalization();
			ms.Close();
			
			return ms.ToArray();
		}
		
		internal static bool IsNumeric(string s) {
			foreach (char c in s) {
				if (!Char.IsDigit(c)) {
					return false;
				}
			}
			
			return true;
		}
    }//end class Util
}