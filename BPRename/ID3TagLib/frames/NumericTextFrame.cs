using System;

namespace ID3TagLib {
    
	[Serializable]
    public class NumericTextFrame: MultiStringTextFrame, IEquatable<NumericTextFrame> {
		
		protected internal NumericTextFrame(string frameID, FrameFlags flags) : base(frameID, flags) { }
		
		protected override void CheckText(string text) {
			if (!Util.IsNumeric(text)) {
				throw new ID3TagException("Only numeric values allowed.");
			}
		}
		
		public bool Equals(NumericTextFrame other) {
			return base.Equals(other as MultiStringTextFrame);
		}
	}
}