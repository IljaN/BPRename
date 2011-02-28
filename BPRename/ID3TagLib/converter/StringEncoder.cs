using System;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace ID3TagLib {
	
	/// <summary><b>StringEncoder</b>is a helper class for encoding strings.</summary>
	/// <remarks>
	///		<b>StringEncoder</b> contains static methods which helps encoding a
	///		string into a byte array.
	/// </remarks>
    internal static class StringEncoder {
        
        private static readonly Encoding latin1 = Encoding.GetEncoding("iso-8859-1");
        private static readonly Encoding utf16LE = new UnicodeEncoding(false, true);//little endian; byteordermask=true
        private static readonly Encoding utf16BE = new UnicodeEncoding(true, false);
        private static readonly Encoding utf8 = new UTF8Encoding(false);
        
		/// <summary>
		///		Returns the proper <see cref="Encoding" /> for the given <see cref="TextEncoding" />.
		/// </summary>
        /// <param name="e">The <see cref="TextEncoding" /> you want the <see cref="Encoding" /> for.</param>
		/// <returns>A <see cref="Encoding" /> for the given <see cref="TextEncoding" />.</returns>
		/// <exceptions cref="InvalidEnumArgumentException">
		///		The value specified is outside the range of valid values.
		/// </exceptions>
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
        
		/// <summary>Writes a string with the given Encoding to a byte array.</summary>
		/// <remarks>
		/// 	<b>Note:</b> No array bounds checking is performed. Ensure that the array is
		///		large enough to hold the encoded string.
		/// </remarks>
		/// <param name="arr">The byte array into which the string is written.</param>
		/// <param name="index">
		///		The zero-based byte index in arr at which to begin storing the encoded string.
        /// </param>
		/// <param name="s">The string to be encoded and written into arr.</param>
		/// <param name="e">
		///		The <see cref="Encoding" /> with which <paramref name="s" /> is going to be encoded.
		/// </param>
		/// <param name="terminateString">
		/// 	When <b>true</b> one or two zero bytes depending on the Encoding are appended; else
		///		nothing is appended.
		/// </param>
		/// <returns>The number of bytes written into arr.</returns>
		internal static int WriteString(byte[] arr, int index, string s, Encoding e, bool terminateString) {
            byte[] preamble = e.GetPreamble();
            int bytesWritten = preamble.Length;
            int termSymCount = 0;
            
            if (terminateString) {
                if (e == utf16LE || e == utf16BE) {
                    termSymCount = 2;
                } else {
                    termSymCount = 1;
                }
            }
            
            Buffer.BlockCopy(preamble, 0, arr, index, preamble.Length);
            bytesWritten += e.GetBytes(s, 0, s.Length, arr, index + bytesWritten);
            
            return bytesWritten + termSymCount;
        }
        
		internal static void WriteLatin1String(byte[] arr, int offset, int count, string s) {
			int charCount = Math.Min(count, s.Length);
			
			latin1.GetBytes(s, 0, charCount, arr, offset);
		}
		
		internal static void WriteLatin1String(Stream stream, string s, int count) {
			byte[] buffer = new byte[count];
			
			latin1.GetBytes(s, 0, Math.Min(count, s.Length), buffer, 0);
			stream.Write(buffer, 0, count);
		}
		
		/// <summary>
		///		Computes the number of bytes required to encode the specified string.
		/// </summary>
		/// <remarks>
		/// 	Use this Method to calculate the exact number of bytes that <paramref name="s" />
		///		will have when beeing encoded.
		///		<b>Note:</b> No error checking is performed. Make sure <paramref name="s" /> and 
		///		<paramref name="e" /> are not null.
		/// </remarks>
		/// <param name="s">The <see cref="String" /> to be encoded.</param>
		/// <param name="e">The <see cref="Encoding" /> with which the string will be encoded.</param>
		/// <param name="terminateString">
		/// 	When <b>true</b> one or two zero bytes depending on the Encoding will be appended; else
		///		nothing wil be appended.
		/// </param>
		/// <returns>The number of bytes required to encode <paramref name="s" />.</returns>
        internal static int ByteCount(string s, Encoding e, bool terminateString) {
            int count = 0;
            
            if (terminateString) {
                if (e == utf16LE || e == utf16BE) {
                    count = 2;
                } else {
                    count = 1;
                }
            }
            
            count += e.GetByteCount(s) + e.GetPreamble().Length;
            
            return count;
        }
    }
}