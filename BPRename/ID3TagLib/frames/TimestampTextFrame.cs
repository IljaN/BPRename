using System;
using System.ComponentModel;
using System.Globalization;

namespace ID3TagLib {
    
	[Serializable]
    public class TimestampTextFrame: MultiStringTextFrame, IEquatable<TimestampTextFrame> {
		
		protected internal TimestampTextFrame(string frameID, FrameFlags flags) : base(frameID, flags) { }
		
		protected override void CheckText(string text) {
			ParseTimestampString(text);
		}
		
		public bool Equals(TimestampTextFrame other) {
			return base.Equals(other as MultiStringTextFrame);
		}
		
		public static DateTime ParseTimestampString(string value) {
			string formatString;
			
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			if (String.IsNullOrEmpty(value)) {
				return DateTime.MinValue;
			}
			formatString = GetFormatString(GetTimestampPrecision(value));
			
			try {
				return DateTime.ParseExact(value, formatString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
				
			} catch (FormatException e) {
				throw new ID3MalformedTimestampException(value, e);
			}
		}
		
		public static string GetTimestampString(DateTime d, TimestampPrecision precision) {
           if (d == DateTime.MinValue) {
                return String.Empty;
            }
            
            return d.ToString(GetFormatString(precision), DateTimeFormatInfo.InvariantInfo);
        }
		
		public static TimestampPrecision GetTimestampPrecision(string value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			
			switch (value.Length) {
				
				case 4:
					return TimestampPrecision.Year;
				
				case 7:
					return TimestampPrecision.Month;
					
				case 10:
					return TimestampPrecision.Day;
				
				case 13:
					return TimestampPrecision.Hours;
					
				case 16:
					return TimestampPrecision.Minutes;
				
				case 19:
					return TimestampPrecision.Seconds;
				
				default:
					throw new ID3MalformedTimestampException(value);
			}
		}
		
		private static string GetFormatString(TimestampPrecision precision) {
			switch (precision) {
			
				case TimestampPrecision.Year:
                    return "yyyy";
                
                case TimestampPrecision.Month:
                    return "yyyy-MM";
                
                case TimestampPrecision.Day:
                    return "yyyy-MM-dd";
                
                case TimestampPrecision.Hours:
                    return "yyyy-MM-ddTHH";
                
                case TimestampPrecision.Minutes:
                    return "yyyy-MM-ddTHH:mm";
                
                case TimestampPrecision.Seconds:
                    return "yyyy-MM-ddTHH:mm:ss";
                
				default:
					throw new InvalidEnumArgumentException("precision", (int)precision, typeof(TimestampPrecision));
            }
		}
	}
}