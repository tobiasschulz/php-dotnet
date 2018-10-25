using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace PHP.Standard
{
    [JsonConverter (typeof (UnixTimestampConverter))]
    public struct UnixTimestamp : IComparable, IComparable<UnixTimestamp>, IEquatable<UnixTimestamp>
    {
        private long _timestamp;

        public static readonly UnixTimestamp Epoch = new UnixTimestamp ();
        public static readonly DateTime EpochDateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static UnixTimestamp Now => FromDateTime (DateTime.UtcNow);
        // public static readonly string FormatLocal_Default = "dd.MM.yyyy HH:mm";
        public static string FormatLocal_Current => "#yyyy-MM-dd HH:mm";

        [JsonIgnore]
        public long UnixTimeSeconds => _timestamp;
        [JsonIgnore]
        public Date Date => global::PHP.Standard.Date.FromDateTime (DateTime);
        [JsonIgnore]
        public Date LocalDate => global::PHP.Standard.Date.FromDateTime (DateTime.ToLocalTime ());
        [JsonIgnore]
        public TimeOfDay TimeOfDay => global::PHP.Standard.TimeOfDay.FromTimeSpan (DateTime.TimeOfDay);

        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                if (_timestamp == 0)
                {
                    return DateTime.MinValue;
                }
                if (_timestamp > 4102444800)
                {
                    return DateTime.MinValue;
                }
                try
                {
                    return EpochDateTime.AddSeconds (_timestamp);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return EpochDateTime;
                }
            }
        }

        public static UnixTimestamp FromUnixTimeSeconds (long seconds)
        {
            return new UnixTimestamp
            {
                _timestamp = seconds
            };
        }

        public static UnixTimestamp FromDateTime (DateTime? nullableDateTime)
        {
            return FromDateTime (dateTime: nullableDateTime ?? DateTime.MinValue);
        }

        public static UnixTimestamp FromDateTime (DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException ("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
            }
            long seconds;
            if (dateTime == DateTime.MinValue)
            {
                seconds = 0;
            }
            else
            {
                seconds = (long)(dateTime.ToUniversalTime () - EpochDateTime).TotalSeconds;
            }
            return new UnixTimestamp
            {
                _timestamp = seconds
            };
        }

        public static UnixTimestamp operator + (UnixTimestamp a, TimeSpan b)
        {
            return new UnixTimestamp { _timestamp = a._timestamp + (long)b.TotalSeconds };
        }

        public static UnixTimestamp operator - (UnixTimestamp a, TimeSpan b)
        {
            return new UnixTimestamp { _timestamp = a._timestamp - (long)b.TotalSeconds };
        }

        public static UnixTimestamp operator + (UnixTimestamp a, long b)
        {
            return new UnixTimestamp { _timestamp = a._timestamp + (long)b };
        }

        public static UnixTimestamp operator - (UnixTimestamp a, long b)
        {
            return new UnixTimestamp { _timestamp = a._timestamp - (long)b };
        }

        public static TimeSpan operator - (UnixTimestamp a, UnixTimestamp b)
        {
            return TimeSpan.FromSeconds ((a.IsNotNull () && b.IsNotNull ()) ? (a._timestamp - b._timestamp) : 0);
        }

        public static bool operator == (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp == t2._timestamp;
        }

        public static bool operator != (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp != t2._timestamp;
        }

        public static bool operator < (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp < t2._timestamp;
        }

        public static bool operator <= (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp <= t2._timestamp;
        }

        public static bool operator > (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp > t2._timestamp;
        }

        public static bool operator >= (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp >= t2._timestamp;
        }

        public override bool Equals (object obj)
        {
            if (obj is UnixTimestamp)
            {
                return _timestamp == ((UnixTimestamp)obj)._timestamp;
            }
            return false;
        }

        public bool Equals (UnixTimestamp other)
        {
            return _timestamp == other._timestamp;
        }

        public static bool Equals (UnixTimestamp t1, UnixTimestamp t2)
        {
            return t1._timestamp == t2._timestamp;
        }

        public override int GetHashCode ()
        {
            return (int)_timestamp ^ (int)(_timestamp >> 32);
        }

        public static int Compare (UnixTimestamp t1, UnixTimestamp t2)
        {
            if (t1._timestamp > t2._timestamp) return 1;
            if (t1._timestamp < t2._timestamp) return -1;
            return 0;
        }

        // Returns a value less than zero if this  object
        public int CompareTo (object value)
        {
            if (value == null) return 1;
            if (!(value is UnixTimestamp))
                return 0;
            long t = ((UnixTimestamp)value)._timestamp;
            if (_timestamp > t) return 1;
            if (_timestamp < t) return -1;
            return 0;
        }

        public int CompareTo (UnixTimestamp value)
        {
            long t = value._timestamp;
            if (_timestamp > t) return 1;
            if (_timestamp < t) return -1;
            return 0;
        }

        public bool IsNull ()
        {
            return _timestamp <= 0;
        }

        public bool IsNotNull ()
        {
            return _timestamp > 0;
        }

        public UnixTimestamp IfNull (UnixTimestamp other)
        {
            return IsNotNull () ? this : other;
        }


        public string FormatSortable (string default_value = "")
        {
            if (IsNull ())
            {
                return default_value;
            }
            else
            {
                return DateTime.ToString ("yyyy-MM-ddTHH:mm:ss.fff");
            }
        }

        public string FormatInternational (bool withMilliseconds = false, string default_value = "")
        {
            if (IsNull ())
            {
                return default_value;
            }
            else if (withMilliseconds)
            {
                return DateTime.ToString ("yyyy-MM-dd HH:mm:ss.fff");
            }
            else
            {
                return DateTime.ToString ("yyyy-MM-dd HH:mm:ss");
            }
        }

        public override string ToString ()
        {
            return UnixTimeSeconds.ToString ();
        }
    }
}
