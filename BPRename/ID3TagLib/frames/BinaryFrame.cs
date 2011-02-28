using System;
using System.Text;

namespace ID3TagLib {

	[Serializable]
    public class BinaryFrame: Frame, IEquatable<BinaryFrame> {

		private byte[] data;

        protected internal BinaryFrame(string frameID, FrameFlags flags): base(frameID, flags) {
			data = new byte[0];
		}
		
		public byte[] Data {
			get {
				return (byte[])data.Clone();
			}
			
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				data = (byte[])value.Clone();
			}
		}
		
		public bool Equals(BinaryFrame other) {
			if (other == null) {
				return false;
			}

			return base.Equals(other) && FrameUtil.ArrayEquals(data, other.data);
		}
		
		public override bool Equals(object obj) {
			return Equals(obj as BinaryFrame);
		}
		
		public override object Clone() {
			BinaryFrame obj = (BinaryFrame)base.Clone();

			obj.data = (byte[])data.Clone();

			return obj;
		}

		public override int GetHashCode() {
			return base.GetHashCode() ^ FrameUtil.ComputeArrayHashCode(data);
		}

		public override string ToString() {
            StringBuilder sBuild = new StringBuilder(base.ToString());

			sBuild.AppendLine("Content:");
            sBuild.AppendLine(FrameUtil.GetStringRepresentation(data));

            return sBuild.ToString();
        }

		protected override byte[] EncodeContentCore(ID3v2Tag tag) {
			return (byte[])data.Clone();
		}

		protected override void DecodeContentCore(byte[] rawData, ID3v2Tag tag) {
			data = (byte[])rawData.Clone();
		}
    }
}