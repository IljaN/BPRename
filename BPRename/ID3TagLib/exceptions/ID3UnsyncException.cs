using System;

namespace ID3TagLib {
		
	public class ID3UnsyncException : ID3TagException {
        public ID3UnsyncException(string msg) : base(msg) { }
        public ID3UnsyncException(string msg, Exception e) : base(msg, e) { }
	}
}