using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class PopularimeterFrame: Frame, IEquatable<PopularimeterFrame> {
		
		private string userIdentifier;
		private byte rating;
		private long counter;
		
		protected internal PopularimeterFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            userIdentifier = String.Empty;
			rating = 1;
			counter = -1;
        }
		
		/* the email address to the user. */
		public string UserIdentifier {
			get {
				return userIdentifier;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				userIdentifier = value;
			}
		}
		
		/* Note: only values in the range 1(worst) - 255(best) are allowed. */
		public byte Rating {
			get {
				return rating;
			}
			
			set {
				rating = value;
			}
		}
		
		/* set to -1 to ommit the counter. */
		public long Counter {
			get {
				return counter;
			}
			
			set {
				if (counter < -1L) {
					throw new ArgumentOutOfRangeException("value", value, "Counter must not be less than -1.");
				}
				counter = value;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as PopularimeterFrame);
		}
		
		public bool Equals(PopularimeterFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && userIdentifier.Equals(other.userIdentifier) && 
				   rating == other.rating && counter == other.counter;
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ userIdentifier.GetHashCode() ^ unchecked((int)counter) ^ rating;
		}
		
		public override object Clone() {
			PopularimeterFrame obj = (PopularimeterFrame)base.Clone();
			
			obj.userIdentifier = userIdentifier;
			obj.rating = rating;
			obj.counter = counter;
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("User identifier: ");
			sBuild.AppendLine(userIdentifier);
			
			sBuild.Append("Rating: ");
			sBuild.AppendLine(rating.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Counter: ");
			if (counter > -1L) {
				sBuild.AppendLine(counter.ToString(NumberFormatInfo.InvariantInfo));
			} else {
				sBuild.AppendLine("<Not available>");
			}
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			byte[] rawData;
			int rawDataLen, currentIndex;
			Encoding latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			
			/* compute rawdata length */
			rawDataLen = StringEncoder.ByteCount(userIdentifier, latin1, true);
			rawDataLen += 1;//rating;
			if (counter > -1L) {
				rawDataLen += NumberConverter.ByteCount(counter, 4);
			}
			
			/* write data */
			rawData = new byte[rawDataLen];
			currentIndex = StringEncoder.WriteString(rawData, 0, userIdentifier, latin1, true);
			rawData[currentIndex++] = rating;
			NumberConverter.WriteInt(counter, rawData, currentIndex, rawData.Length - currentIndex, false);
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			int currentIndex;
			
			userIdentifier = StringDecoder.DecodeString(rawData, 0, -1, latin1, out currentIndex);
			if (rawData.Length <= currentIndex) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			rating = rawData[currentIndex++];
			
			if (currentIndex < rawData.Length) {
				counter = NumberConverter.ReadInt64(rawData, currentIndex, rawData.Length - currentIndex, false);
			} else {
				counter = -1L;
			}
		}
	}
}