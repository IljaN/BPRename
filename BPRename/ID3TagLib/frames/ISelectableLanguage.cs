namespace ID3TagLib {

	public interface ISelectableLanguage {
		
		/* 3 chars wide, use xxx for undefined. */
		string Language {
			get;
			set;
		}
	}
}