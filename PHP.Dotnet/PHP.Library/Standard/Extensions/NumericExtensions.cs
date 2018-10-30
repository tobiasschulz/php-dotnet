using System;
using System.Globalization;
using System.Text;

namespace PHP.Standard
{
    public static class NumericExtensions
    {
        public static double Clamp (this double self, double min, double max)
        {
            return Math.Min (max, Math.Max (self, min));
        }

        public static int Clamp (this int self, int min, int max)
        {
            return Math.Min (max, Math.Max (self, min));
        }

        public static long Clamp (this long self, long min, long max)
        {
            return Math.Min (max, Math.Max (self, min));
        }

        public static int ToInteger (this string str, int default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            if (int.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static long ToLong (this string str, long default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            if (long.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out long result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static uint ToIntegerUnsigned (this string str, uint default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            if (uint.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out uint result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static ulong ToLongUnsigned (this string str, ulong default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            if (ulong.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static float ToFloat (this string str, float default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            str = str.Replace (",", ".");
            if (float.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static double ToDouble (this string str, double default_value = 0)
        {
            if (string.IsNullOrWhiteSpace (str))
            {
                return default_value;
            }
            str = str.Replace (",", ".");
            if (double.TryParse (str, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            else
            {
                return default_value;
            }
        }

        public static string IfEmpty (this string str, string alternative)
        {
            return string.IsNullOrEmpty (str) ? alternative : str;
        }

        public static string NullIfEmpty (this string str)
        {
            return string.IsNullOrEmpty (str) ? null : str;
        }

        public static string NullIfZero<T> (this T id)
        {
            return (id == null || string.IsNullOrWhiteSpace (id.ToString ()) || id.ToString () == "0") ? null : id.ToString ();
        }

        public static string NullIfZero (this string str)
        {
            return (str == null || string.IsNullOrWhiteSpace (str) || str == "0") ? null : str;
        }

        public static T? NullIfDefault<T> (this T? o) where T : struct
        {
            if (!o.HasValue)
            {
                return null;
            }
            else if (o.Value.Equals (default (T)))
            {
                return null;
            }
            else
            {
                return o.Value;
            }
        }

        public static T? IfDefault<T> (this T? o, T? alternative) where T : struct
        {
            if (!o.HasValue)
            {
                return alternative;
            }
            else if (o.Value.Equals (default (T)))
            {
                return alternative;
            }
            else
            {
                return o.Value;
            }
        }

        public static bool Is (this string str, string other)
        {
            return str.NullIfZero () == other.NullIfZero ();
        }

        public static bool IsNull (this long? num)
        {
            return num == null || num == 0;
        }

        public static bool IsNull (this long num)
        {
            return num == 0;
        }
        

        // byte mask = (byte)(1 << bitInByteIndex);
        // bool isSet = (bytes [byteIndex] & mask) != 0;
        // set to 1
        // bytes [byteIndex] |= mask;
        // Set to zero
        // bytes [byteIndex] &= ~mask;
        // Toggle
        // bytes [byteIndex] ^= mask;

        public static byte [] ToBinaryArray (this string s)
        {
            var length = s.Length;
            var bytes = new byte [(int)Math.Ceiling ((double)length / 8.0)];
            int byteIndex, bitInByteIndex;
            byte mask;
            for (int bitIndex = 0; bitIndex < length; bitIndex++)
            {
                byteIndex = bitIndex / 8;
                bitInByteIndex = bitIndex % 8;
                mask = (byte)(1 << bitInByteIndex);
                if (s [bitIndex] == '1')
                {
                    bytes [byteIndex] |= mask;
                }
                else
                {
                    bytes [byteIndex] &= (byte)~mask;
                }
            }
            return bytes;
        }

        public static byte [] ToBinaryArray (this bool [] bits)
        {
            var length = bits.Length;
            var bytes = new byte [(int)Math.Ceiling ((double)length / 8.0)];
            int byteIndex, bitInByteIndex;
            byte mask;
            for (int bitIndex = 0; bitIndex < length; bitIndex++)
            {
                byteIndex = bitIndex / 8;
                bitInByteIndex = bitIndex % 8;
                mask = (byte)(1 << bitInByteIndex);
                if (bits [bitIndex] == true)
                {
                    bytes [byteIndex] |= mask;
                }
                else
                {
                    bytes [byteIndex] &= (byte)~mask;
                }
            }
            return bytes;
        }

        public static string ToBinaryString (this byte [] bytes)
        {
            var sb = new StringBuilder ();
            var length = bytes.Length;
            byte mask;
            for (int byteIndex = 0; byteIndex < length; byteIndex++)
            {
                for (int bitInByteIndex = 0; bitInByteIndex < 8; bitInByteIndex++)
                {
                    mask = (byte)(1 << bitInByteIndex);
                    bool isSet = (bytes [byteIndex] & mask) != 0;
                    sb.Append (isSet ? '1' : '0');
                }
            }
            return sb.ToString ();
        }

        public static string ToBinaryString (this bool [] bits)
        {
            var sb = new StringBuilder ();
            var length = bits.Length;
            for (int bitIndex = 0; bitIndex < length; bitIndex++)
            {
                sb.Append (bits [bitIndex] ? '1' : '0');
            }
            return sb.ToString ();
        }

        public static bool [] ToBitArray (this byte [] bytes)
        {
            var length = bytes.Length;
            var bits = new bool [length * 8];
            byte mask;
            for (int byteIndex = 0; byteIndex < length; byteIndex++)
            {
                for (int bitInByteIndex = 0; bitInByteIndex < 8; bitInByteIndex++)
                {
                    mask = (byte)(1 << bitInByteIndex);
                    bool isSet = (bytes [byteIndex] & mask) != 0;
                    int bitIndex = byteIndex * 8 + bitInByteIndex;
                    bits [bitIndex] = isSet;
                }
            }
            return bits;
        }

        public static bool [] ToBitArray (this string s)
        {
            var length = s.Length;
            var bits = new bool [length];
            for (int bitIndex = 0; bitIndex < length; bitIndex++)
            {
                bits [bitIndex] = s [bitIndex] == '1';
            }
            return bits;
        }
    }
}
