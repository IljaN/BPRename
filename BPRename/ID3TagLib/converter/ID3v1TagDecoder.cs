using System;
using System.Text;
using System.IO;

namespace ID3TagLib {
    
	/// <summary>This helper class is used to deserialize an <see cref="ID3v1Tag" /> from a <see cref="Stream" />.</summary>
	internal static class ID3v1TagDecoder {
        
		internal const int TagHeaderSize = 3;
		
        internal static ID3v1Tag DecodeTag(Stream s) {
            byte[] buffer = new byte[ID3v1Tag.TagLength];
			ID3v1Tag tag;
            
			Util.ReadFully(s, buffer);
			
            if (!HasID3v1Tag(buffer)) {
                return null;
            }
            
            tag = new ID3v1Tag();
			tag.Title = StringDecoder.DecodeLatin1String(buffer, 3, ID3v1Tag.MaxTitleLength);
            tag.Artist = StringDecoder.DecodeLatin1String(buffer, 33, ID3v1Tag.MaxArtistLength);
            tag.Album = StringDecoder.DecodeLatin1String(buffer, 63, ID3v1Tag.MaxAlbumLength);
            tag.Year = StringDecoder.DecodeLatin1String(buffer, 93, ID3v1Tag.MaxYearLength);
			if (buffer[125] == 0x00 && buffer[126] != 0x00) {
				/* ID3v1.1 tag */
				tag.Comment = StringDecoder.DecodeLatin1String(buffer, 97, ID3v1Tag.MaxCommentLengthVersion1_1);
				tag.TrackNumber = (buffer[126] == 0x00 ? String.Empty : Convert.ToString(buffer[126]));
			} else {
				/* ID3v1.0 tag */
				tag.Comment = StringDecoder.DecodeLatin1String(buffer, 97, ID3v1Tag.MaxCommentLengthVersion1_0);
			}
            tag.Genre = (ID3v1Genre)buffer[127];
            
            return tag;
        }
        
        internal static bool HasID3v1Tag(byte[] arr) {
            return arr[0] == 'T' && arr[1] == 'A' && arr[2] == 'G';
        }
    }//end class ID3v1TagDecoder
}