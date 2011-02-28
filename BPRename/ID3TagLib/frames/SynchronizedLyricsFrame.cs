using System;
using System.ComponentModel;
using System.Text;

namespace ID3TagLib {
    
    [Serializable]
    public class SynchronizedLyricsFrame:  Frame, ISelectableTextEncoding, ISelectableLanguage, IEquatable<SynchronizedLyricsFrame> {
		
		private TextEncoding encoding;
		private string language;
		private TimestampFormat timestampFormat;
		private ContentType contentType;
		private string description;
		private SyllableCollection syllables;
		
		protected internal SynchronizedLyricsFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            encoding = TextEncoding.IsoLatin1;
			language = String.Empty;
			timestampFormat = TimestampFormat.AbsoluteTimeInFrames;
			contentType = ContentType.Other;
			description = String.Empty;
			syllables = new SyllableCollection();
        }
		
		/* Sets the TextEncoding Description. */
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
		
		public string Language {
			get {
				return language;
			}
			
			set {
				FrameUtil.CheckLanguage(value);
				language = value;
			}
		}
		
		public TimestampFormat TimestampFormat {
			get {
				return timestampFormat;
			}
			
			set {
				if (!Enum.IsDefined(typeof(TimestampFormat), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(TimestampFormat));
				}
				timestampFormat = value;
			}
		}
		
		public ContentType ContentType {
			get {
				return contentType;
			}
			
			set {
				if (!Enum.IsDefined(typeof(ContentType), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ContentType));
				}
				contentType = value;
			}
		}
		
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
		
		public SyllableCollection Syllables {
			get {
				return syllables;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as SynchronizedLyricsFrame);
		}
		
		public bool Equals(SynchronizedLyricsFrame other) {
			if (other == null || !base.Equals(other) || encoding != other.encoding || !language.Equals(other.language) ||
				timestampFormat != other.timestampFormat || contentType != other.contentType ||
				description.Equals(other.description) || syllables.Count != other.syllables.Count) {
				
				return false;
			}
			
			for (int i = 0; i < syllables.Count; i++) {
				if (!syllables[i].Equals(other.syllables[i])) {
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();
			
			hashCode ^= encoding.GetHashCode();
			hashCode ^= language.GetHashCode();
			hashCode ^= timestampFormat.GetHashCode();
			hashCode ^= contentType.GetHashCode();
			hashCode ^= description.GetHashCode();
			
			foreach (SyllableEntry entry in syllables) {
				hashCode ^= entry.GetHashCode();
			}
			
			return hashCode;
		}
		
		public override object Clone() {
			SynchronizedLyricsFrame obj = (SynchronizedLyricsFrame)base.Clone();
			
			obj.encoding = encoding;
			obj.language = language;
			obj.timestampFormat = timestampFormat;
			obj.contentType = contentType;
			obj.description = description;
			obj.syllables = new SyllableCollection();
			foreach (SyllableEntry entry in syllables) {
				obj.syllables.Add(entry);
			}
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Encoding: ");
			sBuild.AppendLine(encoding.ToString());
			
			sBuild.Append("Language: ");
			sBuild.AppendLine(language);
			
			sBuild.Append("Timestamp format: ");
			sBuild.AppendLine(timestampFormat.ToString());
			
			sBuild.Append("Content type: ");
			sBuild.AppendLine(contentType.ToString());
			
			sBuild.Append("Description:");
			sBuild.AppendLine(description);
			
			sBuild.Append("Syllables:");
			foreach (SyllableEntry entry in syllables) {
				sBuild.AppendLine(entry.ToString());
			}
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding latin1, e;
			int rawDataLength, currentIndex;
			byte[] rawData;
			
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			e = StringEncoder.GetEncoding(encoding);
			
			rawDataLength = 6;
			rawDataLength += StringEncoder.ByteCount(description, e, true);
			foreach (SyllableEntry entry in syllables) {
				rawDataLength += StringEncoder.ByteCount(entry.Syllable, e, true);
				rawDataLength += 4;
			}
			
			rawData = new byte[rawDataLength];
			
			rawData[0] = (byte)encoding;
			StringEncoder.WriteString(rawData, 1, language, latin1, false);
			rawData[4] = (byte)timestampFormat;
			rawData[5] = (byte)contentType;
			currentIndex = 6;
			currentIndex += StringEncoder.WriteString(rawData, 6, description, e, true);
			
			foreach (SyllableEntry entry in syllables) {
				currentIndex += StringEncoder.WriteString(rawData, currentIndex, entry.Syllable, e, true);
				NumberConverter.WriteInt(entry.Timestamp, rawData, currentIndex, 4, false);
				currentIndex += 4;
			}
			
			return rawData;
		}
		
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			Encoding latin1, e;
			int bytesRead, currentIndex;
			string syllable;
			long timestamp;
			
			StringDecoder.ParseEncodingByte(rawData, 0, 6, out e, out encoding);
			
			latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			language = StringDecoder.DecodeString(rawData, 1, 3, latin1, out bytesRead);
			
			TimestampFormat = (TimestampFormat)rawData[4];
			ContentType = (ContentType)rawData[5];
			
			description = StringDecoder.DecodeString(rawData, 6, -1, e, out bytesRead);
			currentIndex = 6 + bytesRead;
			
			syllables.Clear();
			bool firstEntry = true;
			while (currentIndex < rawData.Length) {
				syllable = StringDecoder.DecodeString(rawData, currentIndex, -1, e, out bytesRead);
				currentIndex += bytesRead;
				
				if (firstEntry && rawData[currentIndex] != 0x00) {
					//timestamp omitted?
					timestamp = 0L;
				} else {
					timestamp = NumberConverter.ReadInt64(rawData, currentIndex, 4, false);
					currentIndex += 4;
				}
				firstEntry = false;
				
				syllables.Add(syllable, timestamp);
			}
		}
	}
}