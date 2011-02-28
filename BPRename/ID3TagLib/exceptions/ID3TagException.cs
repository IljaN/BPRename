using System;

namespace ID3TagLib {

    public class ID3TagException: Exception {
        public ID3TagException(string msg) : base(msg) { }
        public ID3TagException(string msg, Exception e) : base(msg, e) { }
    }
}