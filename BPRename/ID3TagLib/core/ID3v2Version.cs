
namespace ID3TagLib {

	/// <summary>Specifies the version of an ID3v2Tag</summary>
	/// <remarks>
	///		This enumeration is used by the <see cref="ID3v2Tag.Version" /> property.
	///		The default version is ID3v2_3.
	/// </remarks>
    public enum ID3v2Version {
		/// <summary>First version, rarely used.</summary>
        ID3v2_2 = 0x0200,
		/// <summary>The most often used version.</summary>
        ID3v2_3 = 0x0300,
		/// <summary>some tools can't parse v2.4 tags.</summary>
        ID3v2_4 = 0x0400
    }
}