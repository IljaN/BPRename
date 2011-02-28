using System;
using System.Reflection;
using System.Collections.Generic;

namespace ID3TagLib {

    public static class FrameFactory {
		
		private static readonly Dictionary<string, string> threeToFour;
        private static readonly Dictionary<string, string> fourToThree;
		
		public const string MusicCDIdentifier = "MCDI";//2.2, 2.3, 2.4
		public const string UniqueFileIdentifierFrameId = "UFID";//2.2, 2.3, 2.4
		public const string PrivateFrameId = "PRIV";//2.3, 2.4
		public const string AlbumFrameId = "TALB";//2.2, 2.3, 2.4
		public const string BeatsPerMinuteFrameId = "TBPM";//2.2, 2.3, 2.4
		public const string ComposerFrameId = "TCOM";//2.2, 2.3, 2.4
		public const string ContentTypeFrameId = "TCON";//2.2, 2.3, 2.4
		public const string CopyrightFrameId = "TCOP";//2.2, 2.3, 2.4
		public const string DateFrameId = "TDAT";//2.2, 2.3
		public const string PlaylistDelayFrameId = "TDLY";//2.2, 2.3, 2.4
		public const string EncodedByFrameId = "TENC";//2.2, 2.3, 2.4
		public const string TextWriterFrameId = "TEXT";//2.2, 2.3, 2.4
		public const string FileTypeFrameId = "TFLT";//2.2, 2.3, 2.4
		public const string TimeFrameId = "TIME";//2.2, 2.3
		public const string ContentGroupFrameId = "TIT1";//2.2, 2.3, 2.4
		public const string TitleFrameId = "TIT2";//2.2, 2.3, 2.4
		public const string SubtitleFrameId = "TIT3";//2.2, 2.3, 2.4
		public const string InitialKeyFrameId = "TKEY";//2.2, 2.3, 2.4
		public const string LanguageFrameId = "TLAN";//2.2, 2.3, 2.4
		public const string LengthFrameId = "TLEN";//2.2, 2.3, 2.4
		public const string MediaTypeFrameId = "TMED";//2.2, 2.3, 2.4
		public const string OriginalAlbumFrameId = "TOAL";//2.2, 2.3, 2.4
		public const string OriginalFilenameFrameId = "TOFN";//2.2, 2.3, 2.4
		public const string OriginalTextWriterFrameId = "TOLY";//2.2, 2.3, 2.4
		public const string OriginalArtistFrameId = "TOPE";//2.2, 2.3, 2.4
		public const string OriginalReleaseYearFrameId = "TORY";//2.2, 2.3
		public const string FileOwnerFrameId = "TOWN";//2.3, 2.4
		public const string LeadArtistFrameId = "TPE1";//2.2, 2.3, 2.4
		public const string BandFrameId = "TPE2";//2.2, 2.3, 2.4
		public const string ConductorFrameId = "TPE3";//2.2, 2.3, 2.4
		public const string RemixedByFrameId = "TPE4";//2.2, 2.3, 2.4
		public const string PartOfASetFrameId = "TPOS";//2.2, 2.3, 2.4
		public const string PublisherFrameId = "TPUB";//2.2, 2.3, 2.4
		public const string TrackNumberFrameId = "TRCK";//2.2, 2.3, 2.4
		public const string RecordingDateFrameId = "TRDA";//2.2, 2.3
		public const string InternetRadioStationNameFrameId = "TRSN";//2.3, 2.4
		public const string InternetRadioStationOwnerFrameId = "TRSO";//2.3, 2.4
		public const string SizeFrameId = "TSIZ";//2.2, 2.4
		public const string IsrcFrameId = "TSRC";//2.2, 2.3, 2.4
		public const string SoftwareHardwareSettingsFrameId = "TSSE";//2.2, 2.3, 2.4
		public const string YearFrameId = "TYER";//2.2, 2.3
		public const string MusicianCreditsListFrameId = "TMCL";//2.4
		public const string InvolvedPeopleListNewFrameId = "TIPL";//2.4
		public const string InvolvedPeopleListOldFrameId = "IPLS";//2.2, 2.3
		public const string RecordingTimeFrameId = "TDRC";//2.4
		public const string OriginalReleaseTimeFrameId = "TDOR";//2.4
		public const string EncodingTimeFrameId = "TDEN";//2.4
		public const string ReleaseTimeFrameId = "TDRL";//2.4
		public const string TaggingTimeFrameId = "TDTG";//2.4
		public const string MoodFrameId = "TMOO";//2.4
		public const string ProducedNoticeFrameId = "TPRO";//2.4
		public const string AlbumSortOrderFrameId = "TSOA";//2.4
		public const string PerformerSortOrderFrameId = "TSOP";//2.4
		public const string TitleSortOrderFrameId = "TSOT";//2.4
		public const string SetSubtitleFrameId = "TSST";//2.4
		public const string UserDefinedTextFrameId = "TXXX";//2.2, 2.3, 2.4
		public const string UserDefinedUrlFrameId = "WXXX";//2.2, 2.3, 2.4
		public const string CommercialInformationUrlFrameId = "WCOM";//2.2, 2.3, 2.4
		public const string CopyrightInformationUrlFrameId = "WCOP";//2.2, 2.3, 2.4
		public const string AudioFileUrlFrameId = "WOAF";//2.2, 2.3, 2.4
		public const string ArtistUrlFrameId = "WOAR";//2.2, 2.3, 2.4
		public const string AudioSourceUrlFrameId = "WOAS";//2.2, 2.3, 2.4
		public const string InternetRadioStationUrlFrameId = "WORS";//2.2, 2.3, 2.4
		public const string PaymentUrlFrameId = "WPAY";//2.2, 2.3, 2.4
		public const string PublisherUrlFrameId = "WPUB";//2.2, 2.3, 2.4
		public const string UnsynchronizedLyricsFrameId = "USLT";//2.2, 2.3, 2.4
		public const string CommentFrameId = "COMM";//2.2, 2.3, 2.4
		public const string TermsOfUseFrameId = "USER";//2.3, 2.4
		public const string OwnershipFrameId = "OWNE";//2.3, 2.4
		public const string CommercialFrameId = "COMR";//2.3, 2.4
		public const string ReverbFrameId = "RVRB";//2.2, 2.3, 2.4
		public const string AttachedPictureFrameId = "APIC";//2.2, 2.3, 2.4
		public const string EncapsulatedObjectFrameId = "GEOB";//2.2, 2.3, 2.4
		public const string PlayCounterFrameId = "PCNT";//2.2, 2.3, 2.4
		public const string PopularimenterFrameId = "POPM";//2.2, 2.3, 2.4
		public const string AudioEncryptionFrameId = "AENC";//2.2, 2.3, 2.4
		public const string LinkedInformationFrameId = "LINK";//2.2, 2.3, 2.4
		public const string PositionSynchronizationFrameId = "POSS";//2.3, 2.4
		public const string EncryptionMethodRegistrationFrameId = "ENCR";//2.3, 2.4
		public const string GroupIdentificationRegistrationFrameId = "GRID";//2.3, 2.4
		public const string SignatureFrameId = "SIGN";//2.4
		public const string SeekFrameId = "SEEK";//2.4
		public const string EventTimingCodesFrameId = "ETCO";//2.2, 2.3, 2.4
		public const string SynchronizedLyricsFrameId = "SYLT";//2.2, 2.3, 2.4
		public const string ChapterFrameId = "CHAP";//2.3, 2.4 addendum
		public const string TableOfContentsFrameId = "CTOC";//2.3, 2.4 addendum
		
		public const string RecommendedBufferSizeFrameId = "RBUF";//2.2, 2.3, 2.4 not implemented
		public const string EncryptedMetaDataFrameId = "CRM";//2.2 not implemented
		public const string AudioSeekPointIndexFrameId = "ASPI";//2.4 not implemented
		public const string MpegLocationLookupTableFrameId = "MLLT";//2.2, 2.3, 2.4 not implemented
		public const string SynchronizedTempoCodesFrameId = "SYTC";//2.2, 2.3, 2.4 not implemented
		public const string RelativeVolumeAdjustmentOldFrameId = "RVAD";//2.2, 2.3 not implemented
		public const string RelativeVolumeAdjustmentNewFrameId = "RVA2";//2.4 not implemented
		public const string EqualizationOldFrameId = "EQUA";//2.2, 2.3 not implemented
		public const string EqualizationNewFrameId = "EQU2";//2.4 not implemented
		
		static FrameFactory() {
            threeToFour = new Dictionary<string, string>(200);
            fourToThree = new Dictionary<string, string>(200);

            threeToFour["UFI"] = "UFID";
            threeToFour["TT1"] = "TIT1";
            threeToFour["TT2"] = "TIT2";
            threeToFour["TT3"] = "TIT3";
            threeToFour["TP1"] = "TPE1";
            threeToFour["TP2"] = "TPE2";
            threeToFour["TP3"] = "TPE3";
            threeToFour["TP4"] = "TPE4";
            threeToFour["TCM"] = "TCOM";
            threeToFour["TXT"] = "TEXT";
            threeToFour["TLA"] = "TLAN";
            threeToFour["TCO"] = "TCON";
            threeToFour["TAL"] = "TALB";
            threeToFour["TPA"] = "TPOS";
            threeToFour["TRK"] = "TRCK";
            threeToFour["TRC"] = "TSRC";
            threeToFour["TYE"] = "TYER";
            threeToFour["TDA"] = "TDAT";
            threeToFour["TIM"] = "TIME";
            threeToFour["TRD"] = "TRDA";
            threeToFour["TMT"] = "TMED";
            threeToFour["TFT"] = "TFLT";
            threeToFour["TBP"] = "TBPM";
            threeToFour["TCR"] = "TCOP";
            threeToFour["TPB"] = "TPUB";
            threeToFour["TEN"] = "TENC";
            threeToFour["TSS"] = "TSSE";
            threeToFour["TOF"] = "TOFN";
            threeToFour["TLE"] = "TLEN";
            threeToFour["TSI"] = "TSIZ";
            threeToFour["TDY"] = "TDLY";
            threeToFour["TKE"] = "TKEY";
            threeToFour["TOT"] = "TOAL";
            threeToFour["TOA"] = "TOPE";
            threeToFour["TOL"] = "TOLY";
            threeToFour["TOR"] = "TORY";
            threeToFour["TXX"] = "TXXX";
            threeToFour["WAF"] = "WOAF";
            threeToFour["WAR"] = "WOAR";
            threeToFour["WAS"] = "WOAS";
            threeToFour["WCM"] = "WCOM";
            threeToFour["WCP"] = "WCOP";
            threeToFour["WPB"] = "WPUB";
            threeToFour["WXX"] = "WXXX";
            threeToFour["IPL"] = "IPLS";
            threeToFour["MCI"] = "MCDI";
            threeToFour["ETC"] = "ETCO";
            threeToFour["MLL"] = "MLLT";
            threeToFour["STC"] = "SYTC";
            threeToFour["ULT"] = "USLT";
            threeToFour["SLT"] = "SYLT";
            threeToFour["COM"] = "COMM";
            threeToFour["RVA"] = "RVAD";
            threeToFour["EQU"] = "EQUA";
            threeToFour["REV"] = "RVRB";
            threeToFour["PIC"] = "APIC";
            threeToFour["GEO"] = "GEOB";
            threeToFour["CNT"] = "PCNT";
            threeToFour["POP"] = "POPM";
            threeToFour["BUF"] = "RBUF";
            threeToFour["LNK"] = "LINK";
			threeToFour["CRA"] = "AENC";

			fourToThree["UFID"] = "UFI";
			fourToThree["TIT1"] = "TT1";
			fourToThree["TIT2"] = "TT2";
			fourToThree["TIT3"] = "TT3";
			fourToThree["TPE1"] = "TP1";
			fourToThree["TPE2"] = "TP2";
			fourToThree["TPE3"] = "TP3";
			fourToThree["TPE4"] = "TP4";
			fourToThree["TCOM"] = "TCM";
			fourToThree["TEXT"] = "TXT";
			fourToThree["TLAN"] = "TLA";
			fourToThree["TCON"] = "TCO";
			fourToThree["TALB"] = "TAL";
			fourToThree["TPOS"] = "TPA";
			fourToThree["TRCK"] = "TRK";
			fourToThree["TSRC"] = "TRC";
			fourToThree["TYER"] = "TYE";
			fourToThree["TDAT"] = "TDA";
			fourToThree["TIME"] = "TIM";
			fourToThree["TRDA"] = "TRD";
			fourToThree["TMED"] = "TMT";
			fourToThree["TFLT"] = "TFT";
			fourToThree["TBPM"] = "TBP";
			fourToThree["TCOP"] = "TCR";
			fourToThree["TPUB"] = "TPB";
			fourToThree["TENC"] = "TEN";
			fourToThree["TSSE"] = "TSS";
			fourToThree["TOFN"] = "TOF";
			fourToThree["TLEN"] = "TLE";
			fourToThree["TSIZ"] = "TSI";
			fourToThree["TDLY"] = "TDY";
			fourToThree["TKEY"] = "TKE";
			fourToThree["TOAL"] = "TOT";
			fourToThree["TOPE"] = "TOA";
			fourToThree["TOLY"] = "TOL";
			fourToThree["TORY"] = "TOR";
			fourToThree["TXXX"] = "TXX";
			fourToThree["WOAF"] = "WAF";
			fourToThree["WOAR"] = "WAR";
			fourToThree["WOAS"] = "WAS";
			fourToThree["WCOM"] = "WCM";
			fourToThree["WCOP"] = "WCP";
			fourToThree["WPUB"] = "WPB";
			fourToThree["WXXX"] = "WXX";
			fourToThree["IPLS"] = "IPL";
			fourToThree["MCDI"] = "MCI";
			fourToThree["ETCO"] = "ETC";
			fourToThree["MLLT"] = "MLL";
			fourToThree["SYTC"] = "STC";
			fourToThree["USLT"] = "ULT";
			fourToThree["SYLT"] = "SLT";
			fourToThree["COMM"] = "COM";
			fourToThree["RVAD"] = "RVA";
			fourToThree["EQUA"] = "EQU";
			fourToThree["RVRB"] = "REV";
			fourToThree["APIC"] = "PIC";
			fourToThree["GEOB"] = "GEO";
			fourToThree["PCNT"] = "CNT";
			fourToThree["POPM"] = "POP";
			fourToThree["RBUF"] = "BUF";
			fourToThree["LINK"] = "LNK";
			threeToFour["AENC"] = "CRA";
		}
		
		public static Frame GetFrame(string frameId) {
			if (frameId == null) {
				throw new ArgumentNullException("frameId");
			}
			if (frameId.Length == 3) {
				frameId = ID3v2_2IdToID3v2_3Id(frameId);
			} else if (frameId.Length != 4) {
				string msg = String.Format("'{0}' is not a valid frame identifier.", frameId);
				throw new ID3InvalidFrameIdentifierException(msg);
			}
			
			FrameFlags preserveTagAltered = FrameFlags.PreserveTagAltered;
			FrameFlags preserveTagAndFileAltered = FrameFlags.PreserveTagAltered | FrameFlags.PreserveFileAltered;
			
			frameId = frameId.ToUpperInvariant();
			switch (frameId) {
				
				case LengthFrameId:
				case EncodedByFrameId:
					return new MultiStringTextFrame(frameId, preserveTagAltered);
					
				case AlbumFrameId:
				case ComposerFrameId:
				case ContentTypeFrameId:
				case CopyrightFrameId:
				case TextWriterFrameId:
				case FileTypeFrameId:
				case ContentGroupFrameId:
				case TitleFrameId:
				case SubtitleFrameId:
				case InitialKeyFrameId:
				case LanguageFrameId:
				case MediaTypeFrameId:
				case OriginalAlbumFrameId:
				case OriginalFilenameFrameId:
				case OriginalTextWriterFrameId:
				case OriginalArtistFrameId:
				case FileOwnerFrameId:
				case LeadArtistFrameId:
				case BandFrameId:
				case ConductorFrameId:
				case RemixedByFrameId:
				case PartOfASetFrameId:
				case PublisherFrameId:
				case TrackNumberFrameId:
				case RecordingDateFrameId:
				case InternetRadioStationNameFrameId:
				case InternetRadioStationOwnerFrameId:
				case IsrcFrameId:
				case SoftwareHardwareSettingsFrameId:
				case MusicianCreditsListFrameId:
				case InvolvedPeopleListNewFrameId:
				case InvolvedPeopleListOldFrameId:
				case MoodFrameId:
				case ProducedNoticeFrameId:
				case AlbumSortOrderFrameId:
				case PerformerSortOrderFrameId:
				case TitleSortOrderFrameId:
				case SetSubtitleFrameId:
					return new MultiStringTextFrame(frameId, preserveTagAndFileAltered);
				
				case SizeFrameId:
					return new NumericTextFrame(frameId, preserveTagAltered);
					
				case BeatsPerMinuteFrameId:
				case DateFrameId:
				case PlaylistDelayFrameId:
				case TimeFrameId:
				case OriginalReleaseYearFrameId:
				case YearFrameId:
					return new NumericTextFrame(frameId, preserveTagAndFileAltered);
				
				case RecordingTimeFrameId:
				case OriginalReleaseTimeFrameId:
				case EncodingTimeFrameId:
				case ReleaseTimeFrameId:
				case TaggingTimeFrameId:
					return new TimestampTextFrame(frameId, preserveTagAndFileAltered);
				
				case CommercialInformationUrlFrameId:
				case CopyrightInformationUrlFrameId:
				case AudioFileUrlFrameId:
				case ArtistUrlFrameId:
				case AudioSourceUrlFrameId:
				case InternetRadioStationUrlFrameId:
				case PaymentUrlFrameId:
				case PublisherUrlFrameId:
					return new TextFrame(frameId, preserveTagAndFileAltered);
				
				case UniqueFileIdentifierFrameId:
				case PrivateFrameId:
					return new DataFrame(frameId, preserveTagAndFileAltered);
				
				case MusicCDIdentifier:
					return new BinaryFrame(frameId, preserveTagAndFileAltered);
				
				case UserDefinedTextFrameId:
					return new UserDefinedTextFrame(frameId, preserveTagAndFileAltered, false);
					
				case UserDefinedUrlFrameId:
					return new UserDefinedTextFrame(frameId, preserveTagAndFileAltered, true);
					
				case UnsynchronizedLyricsFrameId:
				case CommentFrameId:
					return new CommentAndLyricsFrame(frameId, preserveTagAndFileAltered);
				
				case TermsOfUseFrameId:
					return new TermsOfUseFrame(frameId, preserveTagAndFileAltered);
				
				case OwnershipFrameId:
					return new OwnershipFrame(frameId, preserveTagAndFileAltered);
				
				case CommercialFrameId:
					return new CommercialFrame(frameId, preserveTagAndFileAltered);
				
				case ReverbFrameId:
					return new ReverbFrame(frameId, preserveTagAndFileAltered);
				
				case AttachedPictureFrameId:
					return new PictureFrame(frameId, preserveTagAndFileAltered);
				
				case EncapsulatedObjectFrameId:
					return new EncapsulatedObjectFrame(frameId, preserveTagAndFileAltered);
				
				case PlayCounterFrameId:
					return new PlayCounterFrame(frameId, preserveTagAndFileAltered);
				
				case PopularimenterFrameId:
					return new PopularimeterFrame(frameId, preserveTagAndFileAltered);
				
				case AudioEncryptionFrameId:
					return new AudioEncryptionFrame(frameId, preserveTagAltered);
				
				case LinkedInformationFrameId:
					return new LinkedInformationFrame(frameId, preserveTagAndFileAltered);
				
				case PositionSynchronizationFrameId:
					return new PositionSynchronizationFrame(frameId, preserveTagAltered);
				
				case EncryptionMethodRegistrationFrameId:
				case GroupIdentificationRegistrationFrameId:
					return new SymbolRegistrationFrame(frameId, preserveTagAndFileAltered);
				
				case SignatureFrameId:
					return new SignatureFrame(frameId, preserveTagAndFileAltered);
				
				case SeekFrameId:
					return new SeekFrame(frameId, preserveTagAltered);
				
				case EventTimingCodesFrameId:
					return new EventTimingFrame(frameId, preserveTagAltered);
				
				case SynchronizedLyricsFrameId:
					return new SynchronizedLyricsFrame(frameId, preserveTagAltered);
				
				case ChapterFrameId:
					return new ChapterFrame(frameId, preserveTagAndFileAltered);
				
				case TableOfContentsFrameId:
					return new TableOfContentsFrame(frameId, preserveTagAndFileAltered);
				
				case RecommendedBufferSizeFrameId:
				case EncryptedMetaDataFrameId:
					return new BinaryFrame(frameId, preserveTagAndFileAltered);
				
				case AudioSeekPointIndexFrameId:
				case MpegLocationLookupTableFrameId:
				case SynchronizedTempoCodesFrameId:
				case RelativeVolumeAdjustmentOldFrameId:
				case RelativeVolumeAdjustmentNewFrameId:
				case EqualizationOldFrameId:
				case EqualizationNewFrameId:
					return new BinaryFrame(frameId, preserveTagAltered);
				
				default:
					return new BinaryFrame(frameId, preserveTagAndFileAltered);
			}//end switch
		}
		
		public static string ID3v2_2IdToID3v2_3Id(string frameId) {
            string val;
			
			if (frameId == null) {
                throw new ArgumentNullException("frameId");
            }

            frameId = frameId.ToUpperInvariant();
			if (threeToFour.TryGetValue(frameId, out val)) {
				return val;
			} else {
				return frameId;
			}
        }
		
		public static string ID3v2_3IdToID3v2_2Id(string frameId) {
            string val;
			
			if (frameId == null) {
                throw new ArgumentNullException("frameId");
            }

			frameId = frameId.ToUpperInvariant();
			if (fourToThree.TryGetValue(frameId, out val)) {
				return val;
			} else {
				return frameId;
			}
        }
    }	
}