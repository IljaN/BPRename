namespace ID3TagLib {
    
	public enum ReceivedType: byte {
		Other = 0x00,
		StandardCDAlbum = 0x01,
		CompressedAudioOnCD = 0x02,
		FileOverInternet = 0x03,
		StreamOverInternet = 0x04,
		NoteSheet = 0x05,
		NoteSheetBook = 0x06,
		MusicOnOtherMedia = 0x07,
		NonmusicalMerchandise = 0x08
	}
}