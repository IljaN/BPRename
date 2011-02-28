using System;

namespace ID3TagLib {
    
    [FlagsAttribute]
    public enum ExtendedHeaderFlags {
        None = 0x00,
        UpdateTag = 0x01,
        CrcPresent = 0x02,
        RestrictTag = 0x04
    }
}