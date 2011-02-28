using System;
using System.Text;
using System.ComponentModel;

namespace ID3TagLib {
	/// <summary><b>ID3v1Tag</b> holds information about an ID3v1 tag.</summary>
	/// <remarks>
	///		<b>ID3v1Tag</b> only consists of properties to access and manipulate the fields
	///		in an ID3v1 tag. Still some error checking is done, because the size of an ID3v1 tag is
	///		limited to 128 bytes. To obtain the length of all the fields <b>ID3v1Tag</b> has some constants
	///		defined. To retrieve and save an ID3v1 tag to a file use the methods
	///		defined in <see cref="ID3File" />(see example).
	/// </remarks>
    [Serializable]
    public class ID3v1Tag : ICloneable, IEquatable<ID3v1Tag> {
        
		private ID3v1Genre genre;
        private string title;
        private string album;
        private string artist;
        private string comment;
        private string year;
        private string trackNumber;

		/// <summary>Defines the maximum <see cref="Title" /> String length.</summary>
        /// <remarks>A title may have up to 30 characters.</remarks>
		public const int MaxTitleLength = 30;
        /// <summary>Defines the maximum <see cref="Album" /> String length.</summary>
		/// <remarks>An album name may have up to 30 characters.</remarks>
		public const int MaxAlbumLength = 30;
        /// <summary>Defines the maximum <see cref="Artist" /> String length.</summary>
		/// <remarks>An artists name may have up to 30 characters.</remarks>
		public const int MaxArtistLength = 30;
        /// <summary>Defines the maximum <see cref="Year" /> String length.</summary>
		/// <remarks>A year string may have up to 4 characters.</remarks>
		public const int MaxYearLength = 4;
        /// <summary>Defines the maximum <see cref="TrackNumber" /> value.</summary>
		/// <remarks>The tracknumber values ranges from 1 to 255.</remarks>
		public const int MaxTracknumberValue = 255;
        /// <summary>Defines the maximum <see cref="Comment" /> String length when using no tracknumber.</summary>
		public const int MaxCommentLengthVersion1_0 = 30;
        /// <summary>Defines the maximum <see cref="Comment" /> String length when using a tracknumber.</summary>
		public const int MaxCommentLengthVersion1_1 = 28;
		/// <summary>Defines the ID3v1Tag size in bytes.</summary>
		/// <remarks>An ID3v1Tag has always 128 bytes.</remarks>
		public const int TagLength = 128;

		public ID3v1Tag() {
            title = String.Empty;
            album = String.Empty;
            artist = String.Empty;
            comment = String.Empty;
            genre = ID3v1Genre.None;
            year = String.Empty;
            trackNumber = String.Empty;
        }
		
		public ID3v1Tag(ID3v1Tag tag) {
			if (tag == null) {
				throw new ArgumentNullException("tag");
			}
			title = tag.title;
			album = tag.album;
			artist = tag.artist;
			comment = tag.comment;
			genre = tag.genre;
			year = tag.year;
			trackNumber = tag.trackNumber;
		}

		/// <summary>Gets or sets the title.</summary>
		/// <value>A <see cref="string" /> representing the title of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <remarks>
		///		<b>Note:</b> ID3 version 1 does not supports Unicode chars. When encoding the tag
		///		the upper byte of the char is ignored. Title is limited to a maximum of
		///		<see cref="MaxTitleLength" /> chars.
		/// </remarks>
		public string Title {
            get {
                return title;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                title = value;
            }
        }

		/// <summary>Gets or sets the album name.</summary>
		/// <value>A <see cref="string" /> representing the album name of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <remarks>
		///		<b>Note:</b> ID3 version 1 does not supports Unicode chars. When encoding the tag
		///		the upper byte of the char is ignored. Album is limited to a maximum of
		///		<see cref="MaxAlbumLength" /> chars.
		/// </remarks>
        public string Album {
            get {
                return album;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                album = value;
            }
        }

		/// <summary>Gets or sets the artists name.</summary>
		/// <value>A <see cref="string" /> representing the artists name of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <remarks>
		///		<b>Note:</b> ID3 version 1 does not supports Unicode chars. When encoding the tag
		///		the upper byte of the char is ignored. Artist is limited to a maximum of
		///		<see cref="MaxArtistLength" /> chars.
		/// </remarks>
        public string Artist {
            get {
                return artist;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                artist = value;
            }
        }

		/// <summary>Gets or sets the comment string.</summary>
		/// <value>A <see cref="string" /> representing the comment string of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <remarks>
		///		<b>Note:</b> ID3 version 1 does not supports Unicode chars. When encoding the tag
		///		the upper byte of the char is ignored.
		/// </remarks>
        public string Comment {
            get {
                return comment;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                comment = value;
            }
        }

		/// <summary>Gets or sets the ID3v1Tag genre.</summary>
		/// <value>A <see cref="ID3v1Genre" /> that represents the genre of this ID3v1Tag.</value>
		/// <exception cref="InvalidEnumArgumentException">
		///		The value specified is outside the range of valid values.
		/// </exception>
		public ID3v1Genre Genre {
            get {
                return genre;
            }

            set {
                if (!Enum.IsDefined(typeof(ID3v1Genre), value)) {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ID3v1Genre));
                }
                genre = value;
            }
        }

		/// <summary>Gets or sets the recording year.</summary>
		/// <value>A <see cref="string" /> representing the recording year of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <remarks>
		///		Use <c>String.Empty</c> to indicate the absence of a year string
		///		<para><b>Note:</b> Year is limited to a maximum of <see cref="MaxYearLength" /> chars.</para>
		/// </remarks>
        public string Year {
            get {
                return year;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
				if (!Util.IsNumeric(value)) {
					throw new ArgumentException("Year must be a numeric string.", "value");
				}
                year = value;
            }
        }

		/// <summary>Gets or sets the tracknumber.</summary>
		/// <value>A <see cref="string" /> representing the tracknumber of this ID3v1Tag.</value>
        /// <exception cref="ArgumentNullException">Value is a null reference.</exception>
		/// <exception cref="ArgumentException">TrackNumber is not a numeric string.</exception>
		/// <remarks>
		///		Use <c>String.Empty</c> to indicate the absence of a tracknumber.
		/// </remarks>
        public string TrackNumber {
            get {
                return trackNumber;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
				if (!Util.IsNumeric(value)) {
					throw new ArgumentException("TrackNumber must be a numeric string.", "value");
				}
                trackNumber = value;
            }
        }

		/// <summary>Returns the string representation of this ID3v1Tag.</summary>
		/// <returns>A <see cref="string" /> representing this ID3v1Tag.</returns>
		public override string ToString() {
            StringBuilder sBuild = new StringBuilder();

            sBuild.Append("Title  :");
            sBuild.AppendLine(title);
            sBuild.Append("Artist :");
            sBuild.AppendLine(artist);
            sBuild.Append("Album  :");
            sBuild.AppendLine(album);
            sBuild.Append("Year   :");
            sBuild.AppendLine(year);
            sBuild.Append("Comment:");
            sBuild.AppendLine(comment);
            sBuild.Append("TrackNo:");
            sBuild.AppendLine(trackNumber);
            sBuild.Append("Genre  :");
            sBuild.Append(genre.ToString());

            return sBuild.ToString();
        }

		public object Clone() {
			return MemberwiseClone();
        }

		///<summary>
		///		Determines whether this instance of ID3v1Tag and a specified object, which must be a
		///		<b>ID3v1Tag</b>, have the same value.
		///</summary>
		///<param name="obj">An <see cref="object" />.</param>
		///<returns>
		///		<b>true</b> if obj is a <b>ID3v1Tag</b> and its value is the same as this instance;
		///		otherwise, <b>false</b>.
		///</returns>
		public override bool Equals(object other) {
            return Equals(other as ID3v1Tag);
        }
		
		///<summary>
		///		Determines whether this instance of ID3v1Tag and a specified object, which must be a
		///		<b>ID3v1Tag</b>, have the same value.
		///</summary>
		///<param name="obj">An <see cref="object" />.</param>
		///<returns>
		///		<b>true</b> if obj is a <b>ID3v1Tag</b> and its value is the same as this instance;
		///		otherwise, <b>false</b>.
		///</returns>
		public bool Equals(ID3v1Tag other) {
			if (other == null) {
				return false;
			}
			
			return genre == other.genre && title.Equals(other.title) && album.Equals(other.album) &&
				   artist.Equals(other.artist) && comment.Equals(other.comment) && year.Equals(other.year) &&
				   trackNumber.Equals(other.trackNumber);
		}
		
		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() {
            return (int)genre ^ title.GetHashCode() ^ album.GetHashCode() ^ artist.GetHashCode() ^
                   comment.GetHashCode() ^ year.GetHashCode() ^ trackNumber.GetHashCode();
        }
    }//end class ID3v1Tag
}