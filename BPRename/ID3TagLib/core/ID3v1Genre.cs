namespace ID3TagLib {

    /// <summary>Specifies the genre of an ID3v1Tag</summary>
	/// <remarks>
	///		This enumeration is used by the <see cref="ID3v1Tag.Genre" /> property and also by the
	///		ID3v2Tag frame 'TCON'. The enumeration values directly refers to the numeric value stored
	///		in a ID3v1Tag.
	/// </remarks>
	public enum ID3v1Genre: byte {
		/// <summary>Blues genre.</summary>
		Blues,
		/// <summary>Classic Rock genre.</summary>
		Classic_Rock,
		/// <summary>Country genre.</summary>
		Country,
		/// <summary>Dance genre.</summary>
		Dance,
		/// <summary>Disco genre.</summary>
		Disco,
		/// <summary>Funk genre.</summary>
		Funk,
		/// <summary>Grunge genre.</summary>
		Grunge,
		/// <summary>Hip-Hop genre.</summary>
		Hip_Hop,
		/// <summary>Jazz genre.</summary>
		Jazz,
		/// <summary>Metal genre.</summary>
		Metal,
		/// <summary>New Age genre.</summary>
		New_Age,
		/// <summary>Oldies genre.</summary>
		Oldies,
		/// <summary>Other genre.</summary>
		Other,
		/// <summary>Pop genre.</summary>
		Pop,
		/// <summary>R &amp; B genre.</summary>
		RandB,
		/// <summary>Rap genre.</summary>
		Rap,
		/// <summary>Reggae genre.</summary>
		Reggae,
		/// <summary>Rock genre.</summary>
		Rock,
		/// <summary>Techno genre.</summary>
		Techno,
		/// <summary>Industrial genre.</summary>
		Industrial,
		/// <summary>Alternative genre.</summary>
		Alternative,
		/// <summary>Ska genre.</summary>
		Ska,
		/// <summary>Death Metal genre.</summary>
		Death_Metal,
		/// <summary>Pranks genre.</summary>
		Pranks,
		/// <summary>Soundtrack genre.</summary>
		Soundtrack,
		/// <summary>Euro-Techno genre.</summary>
		Euro_Techno,
		/// <summary>Ambient genre.</summary>
		Ambient,
		/// <summary>Trip-Hop genre.</summary>
		Trip_Hop,
		/// <summary>Vocal genre.</summary>
		Vocal,
		/// <summary>Jazz+Funk genre.</summary>
		Jazz_Funk,
		/// <summary>Fusion genre.</summary>
		Fusion,
		/// <summary>Trance genre.</summary>
		Trance,
		/// <summary>Classical genre.</summary>
		Classical,
		/// <summary>Instrumental genre.</summary>
		Instrumental,
		/// <summary>Acid genre.</summary>
		Acid,
		/// <summary>House genre.</summary>
		House,
		/// <summary>Game genre.</summary>
		Game,
		/// <summary>Sound Clip genre.</summary>
		Sound_Clip,
		/// <summary>Gospel genre.</summary>
		Gospel,
		/// <summary>Noise genre.</summary>
		Noise,
		/// <summary>Alternative Rock genre.</summary>
		Alternative_Rock,
		/// <summary>Bass genre.</summary>
		Bass,
		/// <summary>Soul genre.</summary>
		Soul,
		/// <summary>Punk genre.</summary>
		Punk,
		/// <summary>Space genre.</summary>
		Space,
		/// <summary>Meditative genre.</summary>
		Meditative,
		/// <summary>Instrumental Pop genre.</summary>
		Instrumental_Pop,
		/// <summary>Instrumental Rock genre.</summary>
		Instrumental_Rock,
		/// <summary>Ethnic genre.</summary>
		Ethnic,
		/// <summary>Gothic genre.</summary>
		Gothic,
		/// <summary>Darkwave genre.</summary>
		Darkwave,
		/// <summary>Techno-Industrial genre.</summary>
		Techno_Industrial,
		/// <summary>Electronic genre.</summary>
		Electronic,
		/// <summary>Pop-Folk genre.</summary>
		Pop_Folk,
		/// <summary>Eurodance genre.</summary>
		Eurodance,
		/// <summary>Dream genre.</summary>
		Dream,
		/// <summary>Southern Rock genre.</summary>
		Southern_Rock,
		/// <summary>Comedy genre.</summary>
		Comedy,
		/// <summary>Cult genre.</summary>
		Cult,
		/// <summary>Gangsta genre.</summary>
		Gangsta,
		/// <summary>Top 40 genre.</summary>
		Top_40,
		/// <summary>Christian Rap genre.</summary>
		Christian_Rap,
		/// <summary>Pop/Funk genre.</summary>
		Pop_Funk,
		/// <summary>Jungle genre.</summary>
		Jungle,
		/// <summary>Native US genre.</summary>
		Native_US,
		/// <summary>Cabaret genre.</summary>
		Cabaret,
		/// <summary>New Wave genre.</summary>
		New_Wave,
		/// <summary>Psychadelic genre.</summary>
		Psychadelic,
		/// <summary>Rave genre.</summary>
		Rave,
		/// <summary>Showtunes genre.</summary>
		Showtunes,
		/// <summary>Trailer genre.</summary>
		Trailer,
		/// <summary>Lo-Fi genre.</summary>
		Lo_Fi,
		/// <summary>Tribal genre.</summary>
		Tribal,
		/// <summary>Acid Punk genre.</summary>
		Acid_Punk,
		/// <summary>Acid Jazz genre.</summary>
		Acid_Jazz,
		/// <summary>Polka genre.</summary>
		Polka,
		/// <summary>Retro genre.</summary>
		Retro,
		/// <summary>Musical genre.</summary>
		Musical,
		/// <summary>Rock &amp; Roll genre.</summary>
		Rock_and_Roll,
		/// <summary>Hard Rock genre.</summary>
		Hard_Rock,
		/// <summary>Folk genre.</summary>
		Folk,
		/// <summary>Folk-Rock genre.</summary>
		Folk_Rock,
		/// <summary>National Folk genre.</summary>
		National_Folk,
		/// <summary>Swing genre.</summary>
		Swing,
		/// <summary>Fast Fusion genre.</summary>
		Fast_Fusion,
		/// <summary>Bebob genre.</summary>
		Bebob,
		/// <summary>Latin genre.</summary>
		Latin,
		/// <summary>Revival genre.</summary>
		Revival,
		/// <summary>Celtic genre.</summary>
		Celtic,
		/// <summary>Bluegrass genre.</summary>
		Bluegrass,
		/// <summary>Avantgarde genre.</summary>
		Avantgarde,
		/// <summary>Gothic Rock genre.</summary>
		Gothic_Rock,
		/// <summary>Progressive Rock genre.</summary>
		Progressive_Rock,
		/// <summary>Psychedelic Rock genre.</summary>
		Psychedelic_Rock,
		/// <summary>Symphonic Rock genre.</summary>
		Symphonic_Rock,
		/// <summary>Slow Rock genre.</summary>
		Slow_Rock,
		/// <summary>Big Band genre.</summary>
		Big_Band,
		/// <summary>Chorus genre.</summary>
		Chorus,
		/// <summary>Easy Listening genre.</summary>
		Easy_Listening,
		/// <summary>Acoustic genre.</summary>
		Acoustic,
		/// <summary>Humour genre.</summary>
		Humour,
		/// <summary>Speech genre.</summary>
		Speech,
		/// <summary>Chanson genre.</summary>
		Chanson,
		/// <summary>Opera genre.</summary>
		Opera,
		/// <summary>Chamber Music genre.</summary>
		Chamber_Music,
		/// <summary>Sonata genre.</summary>
		Sonata,
		/// <summary>Symphony genre.</summary>
		Symphony,
		/// <summary>Booty Bass genre.</summary>
		Booty_Bass,
		/// <summary>Primus genre.</summary>
		Primus,
		/// <summary>Porn Groove genre.</summary>
		Porn_Groove,
		/// <summary>Satire genre.</summary>
		Satire,
		/// <summary>Slow Jam genre.</summary>
		Slow_Jam,
		/// <summary>Club genre.</summary>
		Club,
		/// <summary>Tango genre.</summary>
		Tango,
		/// <summary>Samba genre.</summary>
		Samba,
		/// <summary>Folklore genre.</summary>
		Folklore,
		/// <summary>Ballad genre.</summary>
		Ballad,
		/// <summary>Power Ballad genre.</summary>
		Power_Ballad,
		/// <summary>Rhytmic Soul genre.</summary>
		Rhytmic_Soul,
		/// <summary>Freestyle genre.</summary>
		Freestyle,
		/// <summary>Duet genre.</summary>
		Duet,
		/// <summary>Punk Rock genre.</summary>
		Punk_Rock,
		/// <summary>Drum Solo genre.</summary>
		Drum_Solo,
		/// <summary>Acapella genre.</summary>
		Acapella,
		/// <summary>Euro-House genre.</summary>
		Euro_House,
		/// <summary>Dance Hall genre.</summary>
		Dance_Hall,
		/// <summary>Goa genre.</summary>
		Goa,
		/// <summary>Drum &amp; Bass genre.</summary>
		Drum_and_Bass,
		/// <summary>Club-House genre.</summary>
		Club_House,
		/// <summary>Hardcore genre.</summary>
		Hardcore,
		/// <summary>Terror genre.</summary>
		Terror,
		/// <summary>Indie genre.</summary>
		Indie,
		/// <summary>BritPop genre.</summary>
		BritPop,
		/// <summary>Negerpunk genre.</summary>
		Negerpunk,
		/// <summary>Polsk Punk genre.</summary>
		Polsk_Punk,
		/// <summary>Beat genre.</summary>
		Beat,
		/// <summary>Christian Gangsta genre.</summary>
		Christian_Gangsta,
		/// <summary>Heavy Metal genre.</summary>
		Heavy_Metal,
		/// <summary>Black Metal genre.</summary>
		Black_Metal,
		/// <summary>Crossover genre.</summary>
		Crossover,
		/// <summary>Contemporary C genre.</summary>
		Contemporary_C,
		/// <summary>Christian Rock genre.</summary>
		Christian_Rock,
		/// <summary>Merengue genre.</summary>
		Merengue,
		/// <summary>Salsa genre.</summary>
		Salsa,
		/// <summary>Thrash Metal genre.</summary>
		Thrash_Metal,
		/// <summary>Anime genre.</summary>
		Anime,
		/// <summary>JPop genre.</summary>
		JPop,
		/// <summary>SynthPop genre.</summary>
		SynthPop,
		/// <summary>No genre specified.</summary>
		None = 0xFF
    }
}