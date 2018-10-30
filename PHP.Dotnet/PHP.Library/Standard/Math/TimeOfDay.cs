using System;
using Newtonsoft.Json;

namespace PHP.Standard
{
    public struct TimeOfDay : IComparable, IComparable<TimeOfDay>, IEquatable<TimeOfDay>
    {
        int _hour;
        int _min;
        int _sec;

        public int Hours => _hour;
        public int Minutes => _min;
        public int Seconds => _sec;

        public static readonly TimeOfDay Default = new TimeOfDay ();
        public static TimeOfDay Now => FromTimeSpan (DateTime.UtcNow.TimeOfDay);

        public TimeOfDay (int hour, int min, int sec)
        {
            _hour = hour;
            _min = min;
            _sec = sec;
        }

        private TimeOfDay (TimeSpan timeSpan)
        {
            _hour = timeSpan.Hours + timeSpan.Days * 24;
            _min = timeSpan.Minutes;
            _sec = timeSpan.Seconds;
        }

        [JsonIgnore]
        public TimeSpan TimeSpan => new TimeSpan (_hour, _min, _sec);
        [JsonIgnore]
        public long TotalSeconds => _hour * 3600 + _min * 60 + _sec;
        [JsonIgnore]
        public UnixTimestamp UnixTimestamp => UnixTimestamp.FromUnixTimeSeconds (TotalSeconds);

        public static TimeOfDay ParseLocal (string str, TimeZoneInfo timezone)
        {
            int offsetHours = 0;
            int offsetMinutes = 0;
            if (timezone != null)
            {
                var offset = timezone.GetUtcOffset (DateTime.Now);
                offsetHours = offset.Hours;
                offsetMinutes = offset.Minutes;
            }

            string [] a = (str ?? string.Empty).Split (':');
            switch (a.Length)
            {
                case 1:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, 0, 0);
                case 2:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, a [1].ToInteger () - offsetMinutes, 0);
                case 3:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, a [1].ToInteger () - offsetMinutes, a [2].ToInteger ());
                default:
                    return TimeOfDay.Default;
            }
        }

        public static TimeOfDay ParseUtc (string str)
        {
            int offsetHours = 0;
            int offsetMinutes = 0;

            string [] a = (str ?? string.Empty).Split (':');
            switch (a.Length)
            {
                case 1:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, 0, 0);
                case 2:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, a [1].ToInteger () - offsetMinutes, 0);
                case 3:
                    return new TimeOfDay (a [0].ToInteger () - offsetHours, a [1].ToInteger () - offsetMinutes, a [2].ToInteger ());
                default:
                    return TimeOfDay.Default;
            }
        }

        public static TimeOfDay FromTimeSpan (TimeSpan timeSpan)
        {
            return new TimeOfDay (timeSpan);
        }

        public static TimeOfDay FromUnixTimestamp (UnixTimestamp timestamp)
        {
            if (timestamp.IsNull ())
            {
                return Default;
            }
            var timeSpan = timestamp.DateTime.TimeOfDay;
            return new TimeOfDay (timeSpan);
        }

        public static TimeOfDay operator + (TimeOfDay a, TimeSpan b)
        {
            var timeSpan = a.TimeSpan + b;
            return new TimeOfDay (timeSpan);
        }

        public static TimeOfDay operator - (TimeOfDay a, TimeSpan b)
        {
            var timeSpan = a.TimeSpan - b;
            return new TimeOfDay (timeSpan);
        }

        public static TimeSpan operator + (TimeSpan a, TimeOfDay b)
        {
            var timeSpan = a + b.TimeSpan;
            return timeSpan;
        }

        public static TimeSpan operator - (TimeSpan a, TimeOfDay b)
        {
            var timeSpan = a - b.TimeSpan;
            return timeSpan;
        }

        public static TimeSpan operator - (TimeOfDay a, TimeOfDay b)
        {
            var timeSpan = a.TimeSpan - b.TimeSpan;
            return timeSpan;
        }

        public static bool operator == (TimeOfDay t1, TimeOfDay t2)
        {
            return t1._hour == t2._hour && t1._min == t2._min && t1._sec == t2._sec;
        }

        public static bool operator != (TimeOfDay t1, TimeOfDay t2)
        {
            return t1._hour != t2._hour || t1._min != t2._min || t1._sec != t2._sec;
        }

        public static bool operator < (TimeOfDay t1, TimeOfDay t2)
        {
            return t1.TotalSeconds < t2.TotalSeconds;
        }

        public static bool operator <= (TimeOfDay t1, TimeOfDay t2)
        {
            return t1.TotalSeconds <= t2.TotalSeconds;
        }

        public static bool operator > (TimeOfDay t1, TimeOfDay t2)
        {
            return t1.TotalSeconds > t2.TotalSeconds;
        }

        public static bool operator >= (TimeOfDay t1, TimeOfDay t2)
        {
            return t1.TotalSeconds >= t2.TotalSeconds;
        }

        public override bool Equals (object obj)
        {
            if (obj is TimeOfDay)
            {
                var other = ((TimeOfDay)obj);
                return _hour == other._hour && _min == other._min && _sec == other._sec;
            }
            return false;
        }

        public bool Equals (TimeOfDay other)
        {
            return _hour == other._hour && _min == other._min && _sec == other._sec;
        }

        public static bool Equals (TimeOfDay t1, TimeOfDay t2)
        {
            return t1._hour == t2._hour && t1._min == t2._min && t1._sec == t2._sec;
        }

        public override int GetHashCode ()
        {
            return (int)TotalSeconds;
        }

        public static int Compare (TimeOfDay t1, TimeOfDay t2)
        {
            var c1 = t1.TotalSeconds;
            var c2 = t2.TotalSeconds;
            if (c1 > c2) return 1;
            if (c1 < c2) return -1;
            return 0;
        }

        // Returns a value less than zero if this  object
        public int CompareTo (object value)
        {
            if (value == null) return 1;
            if (!(value is TimeOfDay))
                return 0;
            long t = ((TimeOfDay)value).TotalSeconds;
            var c = TotalSeconds;
            if (c > t) return 1;
            if (c < t) return -1;
            return 0;
        }

        public int CompareTo (TimeOfDay value)
        {
            long t = value.TotalSeconds;
            var c = TotalSeconds;
            if (c > t) return 1;
            if (c < t) return -1;
            return 0;
        }

        public bool IsNull ()
        {
            return TotalSeconds <= 0;
        }

        public bool IsNotNull ()
        {
            return !IsNull ();
        }

        public TimeOfDay IfNull (TimeOfDay other)
        {
            return IsNotNull () ? this : other;
        }

        public override string ToString ()
        {
            return $"{_hour:D2}:{_min:D2}:{_sec:D2}";
        }
    }
}
