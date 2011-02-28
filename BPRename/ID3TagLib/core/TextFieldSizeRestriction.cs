namespace ID3TagLib {
    
    public enum TextFieldSizeRestriction {
        NoRestrictions = 0x00,
        NotMoreThan1024Chars = 0x08,
        NotMoreThan128Chars = 0x10,
        NotMoreThan30Chars = 0x18
    }
}