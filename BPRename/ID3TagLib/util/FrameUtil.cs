using System;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace ID3TagLib {
	
	internal static class FrameUtil {
		
		internal static bool ArrayEquals(byte[] arr1, byte[] arr2) {
			if (arr1 == null && arr2 != null) {
				return false;
			} else if (arr1 != null && arr2 == null) {
				return false;
			} else if (arr1 == null && arr2 == null) {
				return true;
			} else if (arr1.Length != arr2.Length) {
				return false;
			}
			
			int i = 0;
			while (i < arr1.Length && arr1[i] == arr2[i]) {
				i++;
			}

			return i == arr1.Length;
		}
		
		internal static int ComputeArrayHashCode(byte[] arr) {
			int hashCode = 0;
			
			if (arr == null) {
				return 0;
			}
			
			foreach (byte val in arr) {
				hashCode <<= 8;
				hashCode |= val;
			}
			
			return hashCode;
		}
		
		internal static string GetStringRepresentation(byte[] arr) {
			StringBuilder sBuild = new StringBuilder();
			
			if (arr == null) {
				sBuild.Append("<Empty>");
			} else {
				for (int i = 0; i < arr.Length; i++) {
					if (i != 0 && i % 10 == 0) {
						sBuild.AppendLine();
					}
					sBuild.Append(String.Format("{0:X2}({1}) ", arr[i], GetPrintableChar(arr[i])));
				}
			}
			
			return sBuild.ToString();
		}
		
		private static char GetPrintableChar(byte b) {
			if (b >= 32 && b <= 126) {
				return (char)b;
			} else {
				return '.';
			}
		}
		
		internal static string GetMimeType(ImageFormat imgFormat) {
			if (imgFormat.Equals(ImageFormat.Jpeg)) {
				return "image/jpeg";
			} else if (imgFormat.Equals(ImageFormat.Png)) {
				return "image/png";
			} else if (imgFormat.Equals(ImageFormat.Bmp)) {
				return "image/bmp";
			} else if (imgFormat.Equals(ImageFormat.Gif)) {
				return "image/gif";
			} else if (imgFormat.Equals(ImageFormat.Tiff)) {
				return "image/tiff";
			} else if (imgFormat.Equals(ImageFormat.Icon)) {
				return "image/x-icon";
			} else {
				return "image/";
			}
		}
		
		internal static void CheckLanguage(string value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			if (value.Length > 3) {
				throw new ArgumentException("Language must have three characters(ISO/FDIS 639-2).");
			}
		}
		
		internal static DateTime ParseDateString(string value) {
			DateTime result;

			DateTime.TryParseExact(value, "yyyyMMdd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result);
			return result;
		}

		internal static string GetDateString(DateTime value) {
			return value.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
		}
	}
}