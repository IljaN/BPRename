namespace ID3TagLib {
    
    public enum TagSizeRestriction {
        NoMorethan128Frames = 0x00,
        NoMoreThan64Frames = 0x40,
        NoMoreThan32FramesAnd40KB = 0x80,
        NoMoreThan32FramesAnd4KB = 0xC0
    }
}