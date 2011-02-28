namespace ID3TagLib {
	
    public enum ImageSizeRestriction {
        NoRestrictions = 0x00,
        NotLargerThan256x256 = 0x01,
        NotLargerThan64x64 = 0x02,
        Exactly64x64 = 0x03
    }
}