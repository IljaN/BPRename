using System;

namespace ID3TagLib {
		
	public class ID3ParseException : ID3TagException {
        public ID3ParseException(string msg) : base(msg) { }
        public ID3ParseException(string msg, Exception e) : base(msg, e) { }
	}
}