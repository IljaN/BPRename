using System;
using System.Globalization;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class AudioEncryptionFrame: Frame, IEquatable<AudioEncryptionFrame> {
		
		private string ownerIdentifier;
		private int previewStart;
		private int previewLength;
		private byte[] encryptionInfo;
		
		protected internal AudioEncryptionFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            ownerIdentifier = String.Empty;
        }
		
		/* A URL containing an email address, or a link to a location where
		   an email address can be found, that belongs to the organisation
           responsible for this specific encrypted audio file. Should not 
		   be Empty!
		*/
		public string OwnerIdentifier {
			get {
				return ownerIdentifier;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				ownerIdentifier = value;
			}
		}
		
		/* A pointer to an unencrypted part of the audio can be specified.
		   The 'Preview start' and 'Preview length' is described in frames.
		   If no part is unencrypted, these fields should be left zeroed.
		   Note only the lower 16 Bits are used.
		 */
		public int PreviewStart {
			get {
				return previewStart;
			}
			
			set {
				previewStart = value & 0xFFFF;
			}
		}
		
		/* A pointer to an unencrypted part of the audio can be specified.
		   The 'Preview start' and 'Preview length' is described in frames.
		   If no part is unencrypted, these fields should be left zeroed.
		   Note only the lower 16 Bits are used.
		 */
		public int PreviewLength {
			get {
				return previewLength;
			}
			
			set {
				previewLength = value & 0xFFFF;
			}
		}
		
		/* An optional data block required for decryption of the audio.
		   Note: may be null.
		 */
		public byte[] EncryptionInfo {
			get {
				return (encryptionInfo == null) ? null : (byte[])encryptionInfo.Clone();
			}
			
			set {
				encryptionInfo = (value != null) ? (byte[])value.Clone() : null;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as AudioEncryptionFrame);
		}
		
		public bool Equals(AudioEncryptionFrame other) {
			if (other == null) {
				return false;
			}
			
			return base.Equals(other) && ownerIdentifier.Equals(other.ownerIdentifier) &&
			       previewStart == other.previewStart && previewLength == other.previewLength &&
				   FrameUtil.ArrayEquals(encryptionInfo, other.encryptionInfo);
		}
		
		public override int GetHashCode() {
			return base.GetHashCode() ^ ownerIdentifier.GetHashCode() ^ previewStart ^ 
				   previewLength ^ FrameUtil.ComputeArrayHashCode(encryptionInfo);
		}
		
		public override object Clone() {
			AudioEncryptionFrame obj = (AudioEncryptionFrame)base.Clone();
			
			obj.ownerIdentifier = ownerIdentifier;
			obj.previewLength = previewLength;
			obj.previewStart = previewStart;
			obj.encryptionInfo = (encryptionInfo == null) ? null : (byte[])encryptionInfo.Clone();
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("OwnerIdentifier: ");
			sBuild.AppendLine(ownerIdentifier);
			sBuild.Append("PreviewLength: ");
			sBuild.AppendLine(previewLength.ToString(NumberFormatInfo.InvariantInfo));
			sBuild.Append("previewStart: ");
			sBuild.AppendLine(previewStart.ToString(NumberFormatInfo.InvariantInfo));
			sBuild.AppendLine("EncryptionInfo:");
			sBuild.AppendLine(FrameUtil.GetStringRepresentation(encryptionInfo));
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			int stringLen;
			byte[] rawData;
			Encoding e = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			
			stringLen = StringEncoder.ByteCount(ownerIdentifier, e, true);
			
			rawData = new byte[2 + 2 + stringLen + ((encryptionInfo == null) ? 0 : encryptionInfo.Length)];
			
			StringEncoder.WriteString(rawData, 0, ownerIdentifier, e, true);
			
			rawData[stringLen] = (byte)((previewStart >> 8) & 0xFF);
			rawData[stringLen +1] = (byte)(previewStart & 0xFF);
			
			rawData[stringLen +2] = (byte)((previewLength >> 8) & 0xFF);
			rawData[stringLen +3] = (byte)(previewLength & 0xFF);
			
			if (encryptionInfo != null) {
				Buffer.BlockCopy(encryptionInfo, 0, rawData, stringLen + 4, encryptionInfo.Length);
			}
			
			return rawData;
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			int stringLen;
			
			ownerIdentifier = StringDecoder.DecodeString(rawData, 0, -1, StringDecoder.GetEncoding(TextEncoding.IsoLatin1), out stringLen);
			if (stringLen + 3 >= rawData.Length) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			previewStart = (rawData[stringLen] << 8) | rawData[stringLen + 1];
			previewLength = (rawData[stringLen + 2] << 8) | rawData[stringLen + 3];
			
			if (stringLen + 4 < rawData.Length) {
				encryptionInfo = new byte[rawData.Length - stringLen - 4];
				Buffer.BlockCopy(rawData, stringLen + 4, encryptionInfo, 0, encryptionInfo.Length);
			} else {
				encryptionInfo = null;
			}
		}
	}
}