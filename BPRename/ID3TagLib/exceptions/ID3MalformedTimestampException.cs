using System;

namespace ID3TagLib {
		
	public class ID3MalformedTimestampException : ID3TagException {
        
		public ID3MalformedTimestampException(string malformedValue): base(FormatMessage(malformedValue)) { }
        public ID3MalformedTimestampException(string malformedValue, Exception e) : base(FormatMessage(malformedValue), e) { }
	
		private static string FormatMessage(string val) {
			return String.Format("'{0}' is not a correct timestamp.", val);
		}
	}
}