using System;

namespace ID3TagLib {
    
    [FlagsAttribute]
    public enum FrameFlags {
        None = 0x00,
        ReadOnly = 0x01,
        PreserveFileAltered = 0x02,
        PreserveTagAltered = 0x04,
        Compress = 0x08,
        Encrypt = 0x10
    }
}