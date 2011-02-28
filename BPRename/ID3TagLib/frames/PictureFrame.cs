using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Net;

namespace ID3TagLib {
    
    [Serializable]
    public class PictureFrame:  Frame, ISelectableTextEncoding, IEquatable<PictureFrame> {
		
		private TextEncoding encoding;
		private string mimeType;
		private PictureType pictureType;
		private string description;
		private byte[] pictureData;
		
		protected internal PictureFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			mimeType = String.Empty;
			pictureType = PictureType.Other;
			description = String.Empty;
			pictureData = new byte[0];
        }
		
		/* sets the TextEncoding for Description. */
		public TextEncoding Encoding {
			get {
				return encoding;
			}
			
			set {
				if (!Enum.IsDefined(typeof(TextEncoding), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(TextEncoding));
				}
				encoding = value;
			}
		}
		
		/* The mime type of the data. The "image/png" or "image/jpeg" picture format
		   should be used when interoperability is wanted. In the event that the 
		   MIME media type name is omitted, "image/" will be implied
		   Note:
		   There is the possibility to put only a link to the image file by using the
		   'MIME type' "-->" and having a complete URL instead of picture data. The 
		   use of linked files should however be used sparingly since there is the 
		   risk of separation of files.
		 */
		public string MimeType {
			get {
				return mimeType;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				mimeType = value;
			}
		}
		
		public PictureType PictureType {
			get {
				return pictureType;
			}
			
			set {
				if (!Enum.IsDefined(typeof(PictureType), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(PictureType));
				}
				pictureType = value;
			}
		}
		
		/* The description has a maximum length of 64 characters, but may be empty. */
		public string Description {
			get {
				return description;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				description = value;
			}
		}
		
		/* The rawdata of the image.
		   Note:
		   There is the possibility to put only a link to the image file by using the
		   'MIME type' "-->" and having a complete URL instead of picture data. The 
		   use of linked files should however be used sparingly since there is the 
		   risk of separation of files.
		 */
		public byte[] PictureData {
			get {
				return (byte[])pictureData.Clone();
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				pictureData = (byte[])value.Clone();
			}
		}
		
		/* Creates an Image out of ImageData. Note that this method downloads the image wehen '-->' is 
		   specified as MimeType.
		   You must call Dispose on the Image when finished using it.
		   When setting an image, SetImage with an ImageFormat of value.RawData is called.
		   Note: The image is immediately serialized to a byte array, so it can be disposed afterwards.
		   Exceptions: WebException: An error occurred while downloading data, or the URl is invalid. 
					   ArgumentException: The stream does not have a valid image format.(Image.FromStream)
		 */
		public Image Picture {
			get {
				Stream s;
				
				if (mimeType.Trim().Equals("-->")) {
					/* pictureData contains an URL to the actual data. */
					s = ReadImageDataFromURL(DecodeUrl(pictureData));
				} else {
					s = new MemoryStream(PictureData, false);//Make a copy of image data to prevent modification
				}
				
				return Image.FromStream(s);
			}
			
			set {
				SetPicture(value, value.RawFormat);
			}
		}
		
		public void SetPicture(Image image, ImageFormat format) {
			if (image == null) {
				throw new ArgumentNullException("image");
			}
			if (format == null) {
				throw new ArgumentNullException("format");
			}
			
			using (MemoryStream buffer = new MemoryStream(16 * 1024)) {
				image.Save(buffer, format);
				mimeType = FrameUtil.GetMimeType(format);
				pictureData = buffer.ToArray();
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as PictureFrame);
		}
		
		public bool Equals(PictureFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && encoding == other.encoding && mimeType.Equals(other.mimeType) &&
				   pictureType == other.pictureType && description.Equals(other.description) &&
				   FrameUtil.ArrayEquals(pictureData, other.pictureData);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ (int)encoding ^ mimeType.GetHashCode() ^ (int)pictureType ^
				   description.GetHashCode() ^ FrameUtil.ComputeArrayHashCode(pictureData);
		}
		
		public override object Clone() {
			PictureFrame obj = (PictureFrame)base.Clone();
			
			obj.encoding = encoding;
			obj.mimeType = mimeType;
			obj.pictureType = pictureType;
			obj.description = description;
			obj.pictureData = (byte[])pictureData.Clone();
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());
			
			sBuild.Append("MIME type: ");
			sBuild.AppendLine(mimeType);
			
			sBuild.Append("Picturetype: ");
			sBuild.AppendLine(pictureType.ToString());
			
			sBuild.Append("Description: ");
			sBuild.AppendLine(description);
			
			sBuild.Append("Picture data:");
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(pictureData));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding e, latin1;
			int dataLen, currentIndex;
			byte[] rawData;

			e = StringEncoder.GetEncoding(encoding);
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			
			/* compute rawData length */
			dataLen = 1;//TextEncoding
			dataLen += (tag.Version == ID3v2Version.ID3v2_2) ? 3 : StringEncoder.ByteCount(mimeType, latin1, true);
			dataLen += 1;//PictureType
			dataLen += StringEncoder.ByteCount(description, e, true);
			dataLen += pictureData.Length;
			
			rawData = new byte[dataLen];
			
			/* write data */
			rawData[0] = (byte)encoding;
			currentIndex = 1;
			if (tag.Version == ID3v2Version.ID3v2_2) {
				StringEncoder.WriteString(rawData, 1, GetMimeType2_2(mimeType), latin1, false);
				currentIndex += 3;
			} else {
				currentIndex += StringEncoder.WriteString(rawData, 1, mimeType, latin1, true);
			}
			rawData[currentIndex++] = (byte)pictureType;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, description, e, true);
			Buffer.BlockCopy(pictureData, 0, rawData, currentIndex, pictureData.Length);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding e, latin1;
			int stringLen, currentIndex;
			
			latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			currentIndex = 1;
			
			CheckLength(rawData, 3);//min size
			if (tag.Version == ID3v2Version.ID3v2_2) {
				mimeType = StringDecoder.DecodeString(rawData, 1, 3, latin1, out stringLen);
				currentIndex += 3;
			} else {
				mimeType = StringDecoder.DecodeString(rawData, 1, -1, latin1, out stringLen);
				currentIndex += stringLen;
			}
			
			CheckLength(rawData, currentIndex);
			if (!Enum.IsDefined(typeof(PictureType), rawData[currentIndex])) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			pictureType = (PictureType)rawData[currentIndex++];
			
			CheckLength(rawData, currentIndex);
			StringDecoder.ParseEncodingByte(rawData, 0, currentIndex, out e, out encoding);
			description = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out stringLen);
			currentIndex += stringLen;
			
			CheckLength(rawData, currentIndex);
			pictureData = new byte[rawData.Length - currentIndex];
			Buffer.BlockCopy(rawData, currentIndex, pictureData, 0, pictureData.Length);
		}
		
		private static void CheckLength(byte[] arr, int offset) {
			if (offset >= arr.Length) {
				throw new ID3ParseException("Error while parsing frame.");
			}
		}
		
		private static Stream ReadImageDataFromURL(string url) {
			using (WebClient client = new WebClient()) {
				byte[] data = client.DownloadData(url);
				
				return new MemoryStream(data, false);
			}
		}
		
		private static string DecodeUrl(byte[] rawData) {
			int bytesRead;
			Encoding e = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
				
			return StringDecoder.DecodeString(rawData, 0, -1, e, out bytesRead);
		}
		
		private static string GetMimeType2_2(string mimeType) {
			int i;
			string subType;
			
			if (mimeType.Length <= 3) {
				return mimeType;
			}
			
			i = mimeType.IndexOf('/');
			if (i == -1 || i + 1 >= mimeType.Length) {
				return String.Empty;
			}
			
			subType = mimeType.Substring(i + 1, mimeType.Length - i - 1);
			subType = subType.ToUpperInvariant();
			switch (subType) {
				case "JPEG":
				case "JPG":
					return "JPG";
				
				case "PNG":
				case "BMP":
				case "GIF":
					return subType;
					
				default:
					return String.Empty;
			}
		}
	}
}