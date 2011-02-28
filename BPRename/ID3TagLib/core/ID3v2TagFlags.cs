using System;

namespace ID3TagLib {

	/// <summary>Specifies some general flags for an ID3v2Tag</summary>
	/// <remarks>
	///		This enumeration is used by the <see cref="ID3v2Tag.Flags" /> property.
	///		The default is None.
	/// </remarks>
    [FlagsAttribute]
    public enum ID3v2TagFlags {
		/// <summary>No flags set.</summary>
        None = 0x00,
		/// <summary>Indicates that unsynchronization is applied on the tag.</summary>
        Unsync = 0x01,
		/// <summary>The ID3v2Tag is marked as experimental. Not defined for version 2.2.</summary>
        Experimental = 0x02,
		/// <summary>A Footer is appended to the tag. Only valid for version 2.4.</summary>
        Footer = 0x04
    }
}