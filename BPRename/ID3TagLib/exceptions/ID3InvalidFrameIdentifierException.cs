using System;

namespace ID3TagLib {
		
	public class ID3InvalidFrameIdentifierException : ID3TagException {
        public ID3InvalidFrameIdentifierException(string msg) : base(msg) { }
        public ID3InvalidFrameIdentifierException(string msg, Exception e) : base(msg, e) { }
	}
}