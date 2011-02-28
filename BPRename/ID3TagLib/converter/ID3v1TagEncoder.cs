using System;

namespace ID3TagLib {
	
	/// <summary> This helper class is used to serialize an <see cref="ID3v1Tag" /> to a byte array.</summary>
    internal static class ID3v1TagEncoder {
	
		/// <summary>Serialzes an <see cref="ID3v1Tag" /> to a byte array.</summary>
		/// <param name="tag">The <see cref="ID3v1Tag" /> to bew serialized.</param>
		/// <returns>A byte array containing the encoded tag</returns>
        internal static byte[] EncodeTag(ID3v1Tag tag) {
            byte[] buffer = new byte[ID3v1Tag.TagLength];
            
			StringEncoder.WriteLatin1String(buffer, 0, 3, "TAG");
            StringEncoder.WriteLatin1String(buffer, 3, ID3v1Tag.MaxTitleLength, tag.Title);
            StringEncoder.WriteLatin1String(buffer, 33, ID3v1Tag.MaxArtistLength, tag.Artist);
            StringEncoder.WriteLatin1String(buffer, 63, ID3v1Tag.MaxAlbumLength, tag.Album);
            StringEncoder.WriteLatin1String(buffer, 93, ID3v1Tag.MaxYearLength, tag.Year);
			if (String.IsNullOrEmpty(tag.TrackNumber)) {
				/* ID3v1 tag */
				StringEncoder.WriteLatin1String(buffer, 97, ID3v1Tag.MaxCommentLengthVersion1_0, tag.Comment);
			} else {
				/* ID3v1.1 tag */
				StringEncoder.WriteLatin1String(buffer, 97, ID3v1Tag.MaxCommentLengthVersion1_1, tag.Comment);
				buffer[126] = (byte)Convert.ToInt32(tag.TrackNumber);
			}
			buffer[127] = (byte)tag.Genre;
            
            return buffer;
        }
    }//end class ID3v1TagEncoder
}