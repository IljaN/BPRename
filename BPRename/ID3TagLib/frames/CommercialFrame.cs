using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ID3TagLib {

	[Serializable]
    public class CommercialFrame: Frame, ISelectableTextEncoding, IEquatable<CommercialFrame> {

		private TextEncoding encoding;
		private string price;
		private string validUntil;
		private string contactUrl;
		private ReceivedType receivedType;
		private string sellerName;
		private string description;
		private string logoMimeType;
		private byte[] sellerLogo;

		protected internal CommercialFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			price = String.Empty;
			validUntil = String.Empty;
			contactUrl = String.Empty;
			receivedType = ReceivedType.Other;
			sellerName = String.Empty;
			description = String.Empty;
        }

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

		/* A price is constructed by one three character currency code, encoded according to ISO-4217
		   alphabetic currency code, followed by a numerical value where "." is used as decimal seperator.
		   In the price string several prices may be concatenated, seperated by a "/" character, but there
		   may only be one currency of each type.
		 */
		public string Price {
			get {
				return price;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				price = value;
			}
		}

		/* An 8 character date string in the format YYYYMMDD, describing for how long the price is valid. */
		public string ValidUntil {
			get {
				return validUntil;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				if (value.Length > 8) {
					throw new ArgumentException("ValidUntil may not have more than 8 characters.");
				}
				if (!Util.IsNumeric(value)) {
					throw new ArgumentException("ValueUntil may only contain numeric values.");
				}
				validUntil = value;
			}
		}

		/* returns DateTime.MinValue on error. */
		public DateTime ParseValidUntil() {
			return FrameUtil.ParseDateString(validUntil);
		}

		public void SetValidUntil(DateTime value) {
			validUntil = FrameUtil.GetDateString(value);
		}

		public string ContactUrl {
			get {
				return contactUrl;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				contactUrl = value;
			}
		}

		public ReceivedType ReceivedType {
			get {
				return receivedType;
			}

			set {
				if (!Enum.IsDefined(typeof(ReceivedType), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ReceivedType));
				}

				receivedType = value;
			}
		}

		public string SellerName {
			get {
				return sellerName;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				sellerName = value;
			}
		}

		/* short description of the product. */
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

		/* The LogoMimeType contains information about which picture format is used. In the event that the
		   MIME media type name is omitted, "image/" will be implied. Currently only "image/png" and
		   "image/jpeg" are allowed.
		   LogoMimeType and SellerLogo may be omitted if no picture is to attach.
		   Note: When passing a null reference SellerLogo is set to null too.
		 */
		public string LogoMimeType {
			get {
				return logoMimeType;
			}

			set {
				logoMimeType = value;
				if (logoMimeType == null) {
					sellerLogo = null;
				}
			}
		}

		/* Note: When passing a null reference LogoMimeType is set to null too.
		         returns a copy of the sellerLogo array.
		*/
		public byte[] SellerLogo {
			get {
				return (sellerLogo == null) ? null : (byte[])sellerLogo.Clone();
			}

			set {
				sellerLogo = (value == null) ? null : (byte[])value.Clone();
				if (sellerLogo == null) {
					logoMimeType = null;
				}
			}
		}

		/* Creates an Image out of SellerLogo.
		   You must call Dispose on the Image when finished using it.
		   When setting an image, SetImage with an ImageFormat of value.RawData is called.
		   Note: The image is immediately serialized to a byte array, so it can be disposed afterwards.
		   Exceptions: ArgumentException: The stream does not have a valid image format.(Image.FromStream)
		 */
		public Image SellerPicture {
			get {
				if (sellerLogo == null) {
					return null;
				}
				Stream s = new MemoryStream(SellerLogo, false);//Make a copy of image data to prevent modification

				return Image.FromStream(s);
			}

			set {
				SetSellerLogo(value, value.RawFormat);
			}
		}

		public void SetSellerLogo(Image image, ImageFormat format) {
			if (image == null) {
				throw new ArgumentNullException("image");
			}
			if (format == null) {
				throw new ArgumentNullException("format");
			}

			using (MemoryStream buffer = new MemoryStream(16 * 1024)) {
				image.Save(buffer, format);
				logoMimeType = FrameUtil.GetMimeType(format);
				sellerLogo = buffer.ToArray();
			}
		}

		public override bool Equals(object obj) {
			return Equals(obj as CommercialFrame);
		}

		public bool Equals(CommercialFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && encoding == other.encoding && price.Equals(other.price) &&
				   validUntil.Equals(other.validUntil) && contactUrl.Equals(other.contactUrl) &&
				   receivedType == other.receivedType && sellerName.Equals(other.sellerName) &&
				   description.Equals(other.description) && FrameUtil.ArrayEquals(sellerLogo, other.sellerLogo) &&
				   (logoMimeType == null ? other.logoMimeType == null : logoMimeType.Equals(other.logoMimeType));
		}

		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= (int)encoding;
			hashCode ^= price.GetHashCode();
			hashCode ^= validUntil.GetHashCode();
			hashCode ^= contactUrl.GetHashCode();
			hashCode ^= (int)receivedType;
			hashCode ^= sellerName.GetHashCode();
			hashCode ^= description.GetHashCode();
			if (logoMimeType != null) {
				hashCode ^= logoMimeType.GetHashCode();
			}
			hashCode ^= FrameUtil.ComputeArrayHashCode(sellerLogo);

			return hashCode;
		}

		public override object Clone() {
			CommercialFrame obj = (CommercialFrame)base.Clone();

			obj.encoding = encoding;
			obj.price = price;
			obj.validUntil = validUntil;
			obj.contactUrl = contactUrl;
			obj.receivedType = receivedType;
			obj.sellerName = sellerName;
			obj.description = description;
			obj.logoMimeType = logoMimeType;
			obj.sellerLogo = (sellerLogo == null) ? null : (byte[])sellerLogo.Clone();

			return obj;
		}

		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());

			sBuild.Append("Price: ");
			sBuild.AppendLine(price);

			sBuild.Append("Valid until: ");
			sBuild.AppendLine(validUntil);

			sBuild.Append("Contact URL: ");
			sBuild.AppendLine(contactUrl);

			sBuild.Append("Received as:");
			sBuild.AppendLine(receivedType.ToString());

			sBuild.Append("Seller name:");
			sBuild.AppendLine(sellerName);

			sBuild.Append("Description:");
			sBuild.AppendLine(description);

			sBuild.Append("Logo MIME type:");
			sBuild.AppendLine((logoMimeType == null) ? "<null>" : logoMimeType);

			sBuild.Append("Seller logo:");
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(sellerLogo));

			return sBuild.ToString();
        }

		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding latin1, e;
			int rawDataLength, currentIndex;
			byte[] rawData;
			
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			e = StringEncoder.GetEncoding(encoding);
			
			rawDataLength = 1;//textencoding
			rawDataLength += StringEncoder.ByteCount(price, latin1, true);
			rawDataLength += 8;//valid until
			rawDataLength += StringEncoder.ByteCount(contactUrl, latin1, true);
			rawDataLength += 1;//received type
			rawDataLength += StringEncoder.ByteCount(sellerName, e, true);
			rawDataLength += StringEncoder.ByteCount(description, e, true);
			if (logoMimeType != null) {
				rawDataLength += StringEncoder.ByteCount(logoMimeType, latin1, true);
				rawDataLength += (sellerLogo == null) ? 0 : sellerLogo.Length;
			}
			
			rawData = new byte[rawDataLength];
			
			rawData[0] = (byte)encoding;
			currentIndex = 1;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, price, latin1, true);
			StringEncoder.WriteString(rawData, currentIndex, validUntil, latin1, false);
			currentIndex += 8;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, contactUrl, latin1, true);
			rawData[currentIndex++] = (byte)receivedType;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, sellerName, e, true);
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, description, e, true);
			if (logoMimeType != null) {
				currentIndex += StringEncoder.WriteString(rawData, currentIndex, logoMimeType, latin1, true);
				if (sellerLogo != null) {
					Buffer.BlockCopy(sellerLogo, 0, rawData, currentIndex, sellerLogo.Length);
				}
			}
			
			return rawData;
		}

		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1, e;
			int currentIndex, bytesRead;
			
			latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			EnsureLength(rawData, 1);
			
			price = StringDecoder.DecodeString(rawData, 1, -1, latin1, out bytesRead);
			currentIndex = 1 + bytesRead;
			
			validUntil = StringDecoder.DecodeString(rawData, currentIndex, 8, latin1, out bytesRead);
			currentIndex += 8;
			
			contactUrl = StringDecoder.DecodeString(rawData, currentIndex, -1, latin1, out bytesRead);
			currentIndex += bytesRead;
			
			EnsureLength(rawData, currentIndex);
			ReceivedType = (ReceivedType)rawData[currentIndex++];
			
			StringDecoder.ParseEncodingByte(rawData, 0, currentIndex, out e, out encoding);
			
			sellerName = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out bytesRead);
			currentIndex += bytesRead;
			
			EnsureLength(rawData, currentIndex);
			description = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out bytesRead);
			currentIndex += bytesRead;
			
			if (currentIndex < rawData.Length) {
				logoMimeType = StringDecoder.DecodeString(rawData, currentIndex, -1, latin1, out bytesRead);
				currentIndex += bytesRead;
				
				EnsureLength(rawData, currentIndex);
				sellerLogo = new byte[rawData.Length - currentIndex];
				Buffer.BlockCopy(rawData, currentIndex, sellerLogo, 0, sellerLogo.Length);
			}
		}
		
		private static void EnsureLength(byte[] arr, int minimumLength) {
			if (minimumLength >= arr.Length) {
				throw new ID3ParseException("Error while parsing frame.");
			}
		}
	}
}