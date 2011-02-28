using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class OwnershipFrame: TextFrame, ISelectableTextEncoding, IEquatable<OwnershipFrame> {
		
		private TextEncoding encoding;
		private string price;
		private string purchaseDate;
		
		protected internal OwnershipFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			price = String.Empty;
			purchaseDate = String.Empty;
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
		
		/* An 8 character date string in the format YYYYMMDD. */
		public string PurchaseDate {
			get {
				return purchaseDate;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				if (value.Length > 8) {
					throw new ArgumentException("PurchaseDate may not have more than 8 characters.");
				}
				if (!Util.IsNumeric(value)) {
					throw new ArgumentException("PurchaseDate may only contain numeric values.");
				}
				purchaseDate = value;
			}
		}
		
		/* returns DateTime.MinValue on error. */
		public DateTime ParsePurchaseDate() {
			return FrameUtil.ParseDateString(purchaseDate);
		}

		public void SetPurchaseDate(DateTime value) {
			purchaseDate = FrameUtil.GetDateString(value);
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as OwnershipFrame);
		}

		public bool Equals(OwnershipFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && encoding == other.encoding && price.Equals(other.price) && purchaseDate.Equals(other.purchaseDate);
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= (int)encoding;
			hashCode ^= price.GetHashCode();
			hashCode ^= purchaseDate.GetHashCode();
			
			return hashCode;
		}
		
		public override object Clone() {
			OwnershipFrame obj = (OwnershipFrame)base.Clone();

			obj.encoding = encoding;
			obj.price = price;
			obj.purchaseDate = purchaseDate;

			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());

			sBuild.Append("Price: ");
			sBuild.AppendLine(price);

			sBuild.Append("Purchase date: ");
			sBuild.AppendLine(purchaseDate);

			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData;
			int rawDataLength, currentIndex;
			Encoding e, latin1;
			
			e = StringEncoder.GetEncoding(encoding);
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			
			rawDataLength = 1;//textencoding
			rawDataLength += StringEncoder.ByteCount(price, latin1, true);
			rawDataLength += 8;//purchaseDate
			rawDataLength += StringEncoder.ByteCount(Text, e, false);
			
			rawData = new byte[rawDataLength];
			
			rawData[0] = (byte)encoding;
			currentIndex = 1;
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, price, latin1, true);
			StringEncoder.WriteString(rawData, currentIndex, purchaseDate, latin1, false);
			currentIndex += 8;
			StringEncoder.WriteString(rawData, currentIndex, Text, e, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1, e;
			int currentIndex, bytesRead;
			
			latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			if (rawData.Length < 10) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			price = StringDecoder.DecodeString(rawData, 1, -1, latin1, out bytesRead);
			currentIndex = 1 + bytesRead;
			
			purchaseDate = StringDecoder.DecodeString(rawData, currentIndex, 8, latin1, out bytesRead);
			currentIndex += 8;
			
			StringDecoder.ParseEncodingByte(rawData, 0, currentIndex, out e, out encoding);
			Text = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out bytesRead);
		}
	}
}