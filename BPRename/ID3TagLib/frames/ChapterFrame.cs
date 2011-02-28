using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class ChapterFrame: Frame, IEquatable<ChapterFrame> {
		
		private string elementId;
		private long startTime;
		private long endTime;
		private long startOffset;
		private long endOffset;
		private FrameCollection embeddedFrames;
		
		protected internal ChapterFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            elementId = String.Empty;
			embeddedFrames = new FrameCollection();
        }
		
		public string ElementId {
			get {
				return elementId;
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				elementId = value;
			}
		}
		
		public long StartTime {
			get {
				return startTime;
			}
			
			set {
				startTime = value & 0xFFFFFFFF;
			}
		}
		
		public long EndTime {
			get {
				return endTime;
			}
			
			set {
				endTime = value & 0xFFFFFFFF;
			}
		}
		
		public long StartOffset {
			get {
				return startOffset;
			}
			
			set {
				startOffset = value & 0xFFFFFFFF;
			}
		}
		
		public long EndOffset {
			get {
				return endOffset;
			}
			
			set {
				endOffset = value & 0xFFFFFFFF;
			}
		}
		
		public FrameCollection EmbeddedFrames {
			get {
				return embeddedFrames;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as ChapterFrame);
		}
		
		public bool Equals(ChapterFrame other) {
			if (other == null) {
				return false;
			}
			
			if (base.Equals(other) || !elementId.Equals(other.elementId) || startTime != other.startTime ||
				endTime != other.endTime ||	startOffset != other.startOffset || endOffset != other.endOffset ||
				embeddedFrames.Count != other.embeddedFrames.Count) {
			
				return false;
			}
			
			for (int i = 0; i < embeddedFrames.Count; i++) {
				if (!embeddedFrames[i].Equals(other.embeddedFrames[i])) {
					return false;
				}
			}
			
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();
			
			hashCode ^= elementId.GetHashCode();
			hashCode ^= unchecked((int)startTime);
			hashCode ^= unchecked((int)endTime);
			hashCode ^= unchecked((int)startOffset);
			hashCode ^= unchecked((int)endOffset);
			
			foreach (Frame f in embeddedFrames) {
				hashCode ^= f.GetHashCode();
			}
			
			return hashCode;
		}
		
		/* Note: does not copy the frames in the collection. */
		public override object Clone() {
			ChapterFrame obj = (ChapterFrame)base.Clone();
			
			obj.elementId = elementId;
			obj.startTime = startTime;
			obj.endTime = endTime;
			obj.startOffset = startOffset;
			obj.endOffset = endOffset;
			obj.embeddedFrames = new FrameCollection();
			foreach (Frame f in embeddedFrames) {
				obj.embeddedFrames.Add(f);
			}
			
			return obj;
		}
		
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());
			
			sBuild.Append("Element ID: ");
			sBuild.AppendLine(elementId);
			
			sBuild.Append("Start time: ");
			sBuild.AppendLine(startTime.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("End time: ");
			sBuild.AppendLine(endTime.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("Start offset: ");
			sBuild.AppendLine(startOffset.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.Append("End offset: ");
			sBuild.AppendLine(endOffset.ToString(NumberFormatInfo.InvariantInfo));
			
			sBuild.AppendLine("Embedded Frames: ");
			for (int i = 0; i < embeddedFrames.Count; i++) {
				sBuild.AppendFormat("Frame[{0}]:", i);
				sBuild.AppendLine();
				sBuild.AppendLine(embeddedFrames[i].ToString());
			}
			
			return sBuild.ToString();
        }
		
		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			Encoding latin1 = StringEncoder.GetEncoding(TextEncoding.IsoLatin1);
			MemoryStream frameRawData = new MemoryStream(4096);
			
			byte[] elementIdRaw = new byte[StringEncoder.ByteCount(elementId, latin1, true)];
			StringEncoder.WriteString(elementIdRaw, 0, elementId, latin1, true);
			frameRawData.Write(elementIdRaw, 0, elementIdRaw.Length);
			
			NumberConverter.WriteInt(startTime, frameRawData, 4, false);
			NumberConverter.WriteInt(endTime, frameRawData, 4, false);
			NumberConverter.WriteInt(startOffset, frameRawData, 4, false);
			NumberConverter.WriteInt(endOffset, frameRawData, 4, false);
			
			foreach (Frame f in embeddedFrames) {
				FrameEncoder.EncodeFrame(frameRawData, f, tag);
			}
			
			return frameRawData.ToArray();
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			int bytesRead;
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			
			elementId = StringDecoder.DecodeString(rawData, 0, -1, latin1, out bytesRead);
			if (rawData.Length < bytesRead + 16) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			startTime = NumberConverter.ReadInt64(rawData, bytesRead, 4, false);
			endTime = NumberConverter.ReadInt64(rawData, bytesRead + 4, 4, false);
			startOffset = NumberConverter.ReadInt64(rawData, bytesRead + 8, 4, false);
			endOffset = NumberConverter.ReadInt64(rawData, bytesRead + 12, 4, false);
			
			embeddedFrames.Clear();
			MemoryStream ms = new MemoryStream(rawData, bytesRead + 16, rawData.Length - bytesRead - 16, false);
			while (ms.Position < ms.Length) {
				embeddedFrames.Add(FrameDecoder.DecodeFrame(ms, tag));
			}
		}
	}
}