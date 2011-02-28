namespace ID3TagLib {

	/// <summary>Determines how padding is applied on the ID3tag.</summary>
	/// <remarks>
	///		This enumeration is used by the <see cref="ID3v2Tag.PaddingMode" /> property.
	///		The default is PadToSize.
	/// </remarks>
    public enum PaddingMode {
		/// <summary>No padding is applied</summary>
        None,
		/// <summary>
		///		The tag is padded to a fixed size. If the tag is larger than the size specified,
		///		no padding is applied.
		/// </summary>
        PadToSize,
		/// <summary>A fix amount of zero bytes is appended to the tag.</summary>
		PadFixAmount
    }
}