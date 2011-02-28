using System;
using System.Text;
using System.Collections.ObjectModel;

namespace ID3TagLib {

    [Serializable]
	public class LinkedInformationFrame: Frame, IEquatable<LinkedInformationFrame> {
		
		private string frameIdentifier;
		private string url;
		private StringCollection additionalData;

		protected internal LinkedInformationFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            frameIdentifier = String.Empty;
			url = String.Empty;
			additionalData = new StringCollection();
        }
		
		/* Note: when decoding a 2.2 frame FrameIdentifer contains a three letter id else a 4 letter one.
		   When encoding the frame, the value is adjusted from 2.2 to 2.3 or 2.3 to 2.2 frame id.
		 */
		public string FrameIdentifier {
			get {
				return frameIdentifier;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				frameIdentifier = value;
			}
		}

		public string Url {
			get {
				return url;
			}

			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				url = value;
			}
		}

		public StringCollection AdditionalData {
			get {
				return additionalData;
			}
		}

		public override bool Equals(object obj) {
			return Equals(obj as LinkedInformationFrame);
		}

		public bool Equals(LinkedInformationFrame other) {
			if (other == null) {
				return false;
			}

			if (!base.Equals(other) || !frameIdentifier.Equals(other.frameIdentifier) ||
				!url.Equals(other.url) || additionalData.Count != other.additionalData.Count) {
				return false;
			}

			for (int i = 0; i < additionalData.Count; i++) {
				if (!additionalData[i].Equals(other.additionalData[i])) {
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode() {
			int hashCode = base.GetHashCode();

			hashCode ^= frameIdentifier.GetHashCode();
			hashCode ^= url.GetHashCode();

			foreach (string s in additionalData) {
				hashCode ^= s.GetHashCode();
			}

			return hashCode;
		}

		public override object Clone() {
			LinkedInformationFrame obj = (LinkedInformationFrame)base.Clone();

			obj.frameIdentifier = frameIdentifier;
			obj.url = url;
			obj.additionalData = new StringCollection();
			foreach (string s in additionalData) {
				obj.additionalData.Add(s);
			}

			return obj;
		}

		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.Append("Frame identifier: ");
			sBuild.AppendLine(frameIdentifier);

			sBuild.Append("Url: ");
			sBuild.AppendLine(url);

			sBuild.AppendLine("Additional data:");
			foreach (string s in additionalData) {
				sBuild.AppendLine(s);
			}

			return sBuild.ToString();
        }

		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
            int dataLength, currentIndex;
			byte[] rawData;
			string last = (additionalData.Count == 0) ? String.Empty : additionalData[additionalData.Count - 1];

			/* compute rawdata length */
			dataLength = (tag.Version == ID3v2Version.ID3v2_2) ? 3 : 4;
			dataLength += StringEncoder.ByteCount(url, latin1, true);
			for (int i = 0; i < additionalData.Count - 1; i++) {
                dataLength += StringEncoder.ByteCount(additionalData[i], latin1, true);
            }
			dataLength += StringEncoder.ByteCount(last, latin1, false);

			rawData = new byte[dataLength];

			/* write data */
			currentIndex = (tag.Version == ID3v2Version.ID3v2_2) ? 3 : 4;
			StringEncoder.WriteString(rawData, 0, ConvertFrameId(frameIdentifier, tag.Version), latin1, true);
			currentIndex += StringEncoder.WriteString(rawData, currentIndex, url, latin1, true);
			for (int i = 0; i < additionalData.Count - 1; i++) {
                currentIndex += StringEncoder.WriteString(rawData, currentIndex, additionalData[i], latin1, true);
            }
			StringEncoder.WriteString(rawData, currentIndex, last, latin1, false);

			return rawData;
		}

		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
            int index, bytesRead;
			int frameIdentiferLength = (tag.Version == ID3v2Version.ID3v2_2) ? 3 : 4;

			if (rawData.Length < frameIdentiferLength + 1) {
				throw new ID3ParseException("Error while parsing frame.");
			}

			frameIdentifier = StringDecoder.DecodeString(rawData, 0, frameIdentiferLength, latin1, out bytesRead);
			index = frameIdentiferLength;

			url = StringDecoder.DecodeString(rawData, index, -1, latin1, out bytesRead);
			index += bytesRead;

			additionalData.Clear();
			while (index < rawData.Length) {
                additionalData.Add(StringDecoder.DecodeString(rawData, index, -1, latin1, out bytesRead));
                index += bytesRead;
            }
		}
		
		private static string ConvertFrameId(string id, ID3v2Version desiredVersion) {
			if (desiredVersion == ID3v2Version.ID3v2_2) {
				if (id.Length == 4) {
					id = FrameFactory.ID3v2_3IdToID3v2_2Id(id);
				}
				
				if (id.Length > 3) {
					id = id.Substring(0, 3);
				}
			} else {
				if (id.Length == 3) {
					id = FrameFactory.ID3v2_2IdToID3v2_3Id(id);
				}
				
				if (id.Length > 4) {
					id = id.Substring(0, 4);
				}
			}
			
			return id;
		}
	}
}