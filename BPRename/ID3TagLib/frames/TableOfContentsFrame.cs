using System;
using System.IO;
using System.Text;

namespace ID3TagLib {
    
	[Serializable]
    public class TableOfContentsFrame: Frame, IEquatable<TableOfContentsFrame> {
		
		private string elementId;
		private bool topmost;
		private bool ordered;
		private StringCollection childElementIds;
		private FrameCollection embeddedFrames;
		
		protected internal TableOfContentsFrame(string frameID, FrameFlags flags) : base(frameID, flags) {
            elementId = String.Empty;
			childElementIds = new StringCollection();
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
		
		public bool Topmost {
			get {
				return topmost;
			}
			
			set {
				topmost = value;
			}
		}
		
		public bool Ordered {
			get {
				return ordered;
			}
			
			set {
				ordered = value;
			}
		}
		
		public StringCollection ChildElementIds {
			get {
				return childElementIds;
			}
		}
		
		public FrameCollection EmbeddedFrames {
			get {
				return embeddedFrames;
			}
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as TableOfContentsFrame);
		}
		
		public bool Equals(TableOfContentsFrame other) {
			if (other == null) {
				return false;
			}
			
			if (base.Equals(other) || !elementId.Equals(other.elementId) || topmost != other.topmost || ordered != other.ordered ||
				childElementIds.Count != other.childElementIds.Count || embeddedFrames.Count != other.embeddedFrames.Count) {
			
				return false;
			}
			
			for (int i = 0; i < childElementIds.Count; i++) {
				if (!childElementIds[i].Equals(other.childElementIds[i])) {
					return false;
				}
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
			hashCode ^= topmost.GetHashCode();
			hashCode ^= ordered.GetHashCode();
			
			foreach (string s in childElementIds) {
				hashCode ^= s.GetHashCode();
			}
			
			foreach (Frame f in embeddedFrames) {
				hashCode ^= f.GetHashCode();
			}
			
			return hashCode;
		}
		
		/* Note: does not copy the frames in the collection. */
		public override object Clone() {
			TableOfContentsFrame obj = (TableOfContentsFrame)base.Clone();
			
			obj.elementId = elementId;
			obj.topmost = topmost;
			obj.ordered = ordered;
			obj.childElementIds = new StringCollection();
			foreach (string s in childElementIds) {
				obj.childElementIds.Add(s);
			}
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
			
			sBuild.Append("Topmost: ");
			sBuild.AppendLine(topmost.ToString());
			
			sBuild.Append("Ordered: ");
			sBuild.AppendLine(ordered.ToString());
			
			sBuild.AppendLine("Child element IDs: ");
			for (int i = 0; i < childElementIds.Count; i++) {
				sBuild.AppendFormat("Child ID[{0}]:", i);
				sBuild.AppendLine(childElementIds[i].ToString());
			}
			
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
			int stringLength, currentIndex;
			byte[] stringRawData;
			MemoryStream frameRawData = new MemoryStream(4096); 
			
			/* Compute string length */
			stringLength = StringEncoder.ByteCount(elementId, latin1, true);
			foreach (string s in childElementIds) {
				stringLength += StringEncoder.ByteCount(s, latin1, true);
			}
			
			stringRawData = new byte[stringLength + 2];//flag byte, Entry count
			
			/* encode elementID */
			currentIndex = StringEncoder.WriteString(stringRawData, 0, elementId, latin1, true);
			
			/* encode flags */
			if (topmost) {
				stringRawData[currentIndex] |= 0x01;
			}
			if (ordered) {
				stringRawData[currentIndex] |= 0x02;
			}
			currentIndex++;
			
			/* encode Entry count */
			if (childElementIds.Count > 0xFF) {
				throw new ID3ParseException("Not more than 255 Child element IDs allowed.");
			}
			stringRawData[currentIndex++] = (byte)childElementIds.Count;
			
			/* encode Child element IDs */
			foreach (string s in childElementIds) {
				currentIndex += StringEncoder.WriteString(stringRawData, currentIndex, s, latin1, true);
			}
			
			/* write current content plus embedded frames to a MemoryStream */
			frameRawData.Write(stringRawData, 0, stringRawData.Length);
			foreach (Frame f in embeddedFrames) {
				FrameEncoder.EncodeFrame(frameRawData, f, tag);
			}
			
			return frameRawData.ToArray();
		}
		
		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			int bytesRead, currentIndex, entryCount;
			Encoding latin1 = StringDecoder.GetEncoding(TextEncoding.IsoLatin1);
			
			/* read elementID */
			elementId = StringDecoder.DecodeString(rawData, 0, -1, latin1, out bytesRead);
			if (rawData.Length < bytesRead + 2) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			/* parse flags */
			currentIndex = bytesRead;
			if (rawData[currentIndex] > 0x03) {
				throw new ID3ParseException("Invalid flags set.");
			}
			topmost = (rawData[currentIndex] & 0x01) == 0x01;
			ordered = (rawData[currentIndex] & 0x02) == 0x02;
			currentIndex++;
			
			/* read Child element IDs */
			entryCount = rawData[currentIndex++];
			childElementIds.Clear();
			while (childElementIds.Count < entryCount && currentIndex < rawData.Length) {
				childElementIds.Add(StringDecoder.DecodeString(rawData, currentIndex, -1, latin1, out bytesRead));
				currentIndex += bytesRead;
			}
			if (entryCount != childElementIds.Count) {
				throw new ID3ParseException("Error while parsing frame.");
			}
			
			/* read embedded frames */
			embeddedFrames.Clear();
			MemoryStream ms = new MemoryStream(rawData, currentIndex, rawData.Length - currentIndex, false);
			while (ms.Position < ms.Length) {
				embeddedFrames.Add(FrameDecoder.DecodeFrame(ms, tag));
			}
		}
	}
}