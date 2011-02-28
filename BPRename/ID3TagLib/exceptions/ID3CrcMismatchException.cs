using System;

namespace ID3TagLib {
		
	public class ID3CrcMismatchException : ID3TagException {
        public ID3CrcMismatchException(long expected, long actual):
			base(String.Format("Crc mismatch: expected {0}, but {1} calculated.", expected, actual)) { }
	}
}