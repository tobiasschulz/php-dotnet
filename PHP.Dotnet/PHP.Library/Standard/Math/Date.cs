using System;
using Newtonsoft.Json;

namespace PHP.Standard
{
    [JsonConverter (typeof (DateConverter))]
    public struct Date : IComparable, IComparable<Date>, IEquatable<Date>
    {
        int _year;
        int _month;
        int _day;

        public int Year => _year;
        public int Month => _month;
        public int Day => _day;

        public static readonly Date Default = new Date ();
        public static Date Today => FromDateTime (DateTime.UtcNow);

        public Date (int year, int month, int day)
        {
            _year = year;
            _month = month;
            _day = day;
        }

        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                try
                {
                    return new DateTime (_year, _month, _day, 0, 0, 0, DateTimeKind.Utc);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return UnixTimestamp.EpochDateTime;
                }
            }
        }

        [JsonIgnore]
        public UnixTimestamp UnixTimestamp => UnixTimestamp.FromDateTime (DateTime);

        public static Date FromDateTime (DateTime dateTime)
        {
            return new Date
            {
                _year = dateTime.Year,
                _month = dateTime.Month,
                _day = dateTime.Day,
            };
        }

        public static Date FromUnixTimestamp (UnixTimestamp timestamp)
        {
            if (timestamp.IsNull ())
            {
                return Default;
            }
            var dateTime = timestamp.DateTime;
            return new Date
            {
                _year = dateTime.Year,
                _month = dateTime.Month,
                _day = dateTime.Day,
            };
        }

        public static UnixTimestamp operator + (Date a, TimeOfDay b)
        {
            return a.UnixTimestamp + (long)b.TotalSeconds;
        }

        public static UnixTimestamp operator - (Date a, TimeOfDay b)
        {
            return a.UnixTimestamp - (long)b.TotalSeconds;
        }

        public static bool operator == (Date t1, Date t2)
        {
            return t1._year == t2._year && t1._month == t2._month && t1._day == t2._day;
        }

        public static bool operator != (Date t1, Date t2)
        {
            return t1._year != t2._year || t1._month != t2._month || t1._day != t2._day;
        }

        public static bool operator < (Date t1, Date t2)
        {
            return t1.combinedFields < t2.combinedFields;
        }

        public static bool operator <= (Date t1, Date t2)
        {
            return t1.combinedFields <= t2.combinedFields;
        }

        public static bool operator > (Date t1, Date t2)
        {
            return t1.combinedFields > t2.combinedFields;
        }

        public static bool operator >= (Date t1, Date t2)
        {
            return t1.combinedFields >= t2.combinedFields;
        }

        public override bool Equals (object obj)
        {
            if (obj is Date)
            {
                var other = ((Date)obj);
                return _year == other._year && _month == other._month && _day == other._day;
            }
            return false;
        }

        public bool Equals (Date other)
        {
            return _year == other._year && _month == other._month && _day == other._day;
        }

        public static bool Equals (Date t1, Date t2)
        {
            return t1._year == t2._year && t1._month == t2._month && t1._day == t2._day;
        }

        private long combinedFields => _year * 10000 + _month * 100 + _day;

        public override int GetHashCode ()
        {
            return (int)combinedFields;
        }

        public static int Compare (Date t1, Date t2)
        {
            var c1 = t1.combinedFields;
            var c2 = t2.combinedFields;
            if (c1 > c2) return 1;
            if (c1 < c2) return -1;
            return 0;
        }

        // Returns a value less than zero if this  object
        public int CompareTo (object value)
        {
            if (value == null) return 1;
            if (!(value is Date))
                return 0;
            long t = ((Date)value).combinedFields;
            var c = combinedFields;
            if (c > t) return 1;
            if (c < t) return -1;
            return 0;
        }

        public int CompareTo (Date value)
        {
            long t = value.combinedFields;
            var c = combinedFields;
            if (c > t) return 1;
            if (c < t) return -1;
            return 0;
        }

        public bool IsNull ()
        {
            return _year < 1970 || _month <= 0 || _day <= 0;
        }

        public bool IsNotNull ()
        {
            return !IsNull ();
        }

        public Date IfNull (Date other)
        {
            return IsNotNull () ? this : other;
        }

        public override string ToString ()
        {
            return $"{_year:D4}-{_month:D2}-{_day:D2}";
        }

    }
}

