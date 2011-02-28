using System;
using System.Text;
using System.ComponentModel;

namespace ID3TagLib {

    internal static class StringDecoder {
        
        private static readonly Encoding latin1 = Encoding.GetEncoding("iso-8859-1");
        private static readonly Encoding utf16LE = new UnicodeEncoding(false, true);
        private static readonly Encoding utf16BE = new UnicodeEncoding(true, true);
        private static readonly Encoding utf8 = new UTF8Encoding(true);
		
		internal static void ParseEncodingByte(byte[] rawData, int encByteOffset, int stringOffset, out Encoding encoding, out TextEncoding textEncoding) {
			
			if (rawData.Length <= encByteOffset || rawData.Length < stringOffset) {
				throw new ID3ParseException("Frame size mismatch.");
			}
			
			switch (rawData[encByteOffset]) {
				case 0x00:
					encoding = latin1;
                    textEncoding = TextEncoding.IsoLatin1;
					break;
				
				case 0x01:
                    if (rawData.Length < stringOffset + 2) {
						throw new ID3ParseException("Frame size mismatch.");
					}
					
					if (rawData[stringOffset] == 0xFE && rawData[stringOffset + 1] == 0xFF) {
						encoding = utf16BE;
					} else if (rawData[stringOffset] == 0xFF && rawData[stringOffset + 1] == 0xFE) {
						encoding = utf16LE;
					} else {
						throw new ID3ParseException("Invalid Byte Order Mask.");
					}
                    textEncoding = TextEncoding.Utf16;
					break;
				
				case 0x02:
					encoding = utf16BE;
                    textEncoding = TextEncoding.Utf16BE;
					break;
				
				case 0x03:
					encoding = utf8;
                    textEncoding = TextEncoding.Utf8;
					break;
				
				default:
					throw new ID3ParseException("Unknown TextEncoding.");
			}
		}
		
		internal static Encoding GetEncoding(TextEncoding e) {
			switch(e) {
                case TextEncoding.IsoLatin1:
                    return latin1;
                
                case TextEncoding.Utf16:
                    return utf16LE;
                
                case TextEncoding.Utf16BE:
                    return utf16BE;
                
                case TextEncoding.Utf8:
                    return utf8;
                
                default:
                    throw new InvalidEnumArgumentException("e", (int)e, typeof(TextEncoding));
            }
		}
		
        internal static string DecodeString(byte[] arr, int offset, int maxBytes, Encoding e, out int bytesRead) {
            int termSymCount = ((e == utf16LE || e == utf16BE) ? 2 : 1);
            int strLen, index;
            string s;
            
			if (maxBytes == -1) {
				maxBytes = arr.Length;
			}
			
			index = SkipPreamble(arr, offset, maxBytes, e);
            strLen = StringLen(arr, index, maxBytes, termSymCount);
            s = e.GetString(arr, index, strLen);
            bytesRead = index + strLen + termSymCount - offset;
            
            return s;
        }
        
		internal static string DecodeLatin1String(byte[] arr, int offset, int maxBytes) {
			int bytesRead;
			
			return DecodeString(arr, offset, maxBytes, latin1, out bytesRead);
		}
		
        private static int StringLen(byte[] arr, int index, int maxBytes, int termSymCount) {
            int i = index;
            int j = termSymCount - 1;
			int maxIndex = Math.Min(arr.Length - j, index + maxBytes);
			
            while (i < maxIndex && (arr[i] != 0x00 || arr[i + j] != 0x00)) {
                i += termSymCount;
            }
            
            return i - index;
        }
		
		/// <summary>Skips the byte order mask of an string in a byte array.</summary>
		/// <returns>
		/// 	The first byte index after the BOM or index when the BOM is not found or
		/// 	maxBytes is less than BOM.
		/// </returns>
		private static int SkipPreamble(byte[] arr, int index, int maxBytes, Encoding e) {
			byte[] preamble = e.GetPreamble();
			int i;
			
			/* check that the array is long enough to hold a preamble and that we want to read at
			   preamble.Length bytes.
			 */
			if (preamble.Length > maxBytes || preamble.Length + index > arr.Length) {
				return index;
			}
			
			i = 0;
			while (i < preamble.Length && preamble[i] == arr[i + index]) {
				i++;
			}
			
			return (i == preamble.Length ? index + i: index);
		}
    }//end class StringDecoder
}