using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PHP.Standard
{
    public static class StringExtensions
    {
        public static string Base64Encode (this string str)
        {
            return Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (str));
        }

        public static string Base64Decode (this string str)
        {
            byte [] buffer = Convert.FromBase64String (str);
            return System.Text.Encoding.UTF8.GetString (buffer, 0, buffer.Length);
        }

        public static string ToStringUTF8 (this byte [] byteArray)
        {
            return System.Text.Encoding.UTF8.GetString (byteArray, 0, byteArray.Length);
        }

        public static byte [] ToByteArray (this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes (str);
        }

        public static string Truncate (this string value, int maxLength)
        {
            if (!string.IsNullOrEmpty (value) && value.Length > maxLength)
            {
                return value.Substring (0, maxLength);
            }

            return value;
        }

        public static string FormatSortable (this DateTime date)
        {
            return date.ToString ("yyyy-MM-ddTHH:mm:ss.fff");
        }

        public static string UppercaseFirst (this string str)
        {
            if (string.IsNullOrEmpty (str))
            {
                return string.Empty;
            }
            char [] a = str.ToCharArray ();
            a [0] = char.ToUpper (a [0]);
            return new string (a);
        }

        public static string ToJson (this object obj, bool inline = false, bool ignoreNull = true, bool errorHandling = true, LogWithOptionalParameterList _ = default, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            string result = ConfigHelper.WriteConfig (stuff: obj, inline: inline, ignoreNull: ignoreNull, errorHandling: errorHandling, context: $"{filePath}:{lineNumber} {memberName}");
            if (inline)
            {
                result = result.TrimEnd ('\r', '\n');
            }
            return result;
        }

        public static T FromJson<T> (this string json, bool errorHandling = true, LogWithOptionalParameterList _ = default, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            where T : class, new()
        {
            return ConfigHelper.ReadConfig<T> (json, errorHandling: errorHandling, context: $"{filePath}:{lineNumber} {memberName}");
        }

        public static object FromJson (this string json, Type type, bool errorHandling = true, LogWithOptionalParameterList _ = default, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            return ConfigHelper.ReadConfig (json, type, errorHandling, context: $"{filePath}:{lineNumber} {memberName}");
        }

        public static string Between (this string source, string left, string right)
        {
            return Regex.Match (source, string.Format ("{0}(.*){1}", left, right)).Groups [1].Value;
        }

        public static bool ContainsAny (this string self, params string [] choices)
        {
            if (self == null)
                return false;
            foreach (string choice in choices)
            {
                if (!string.IsNullOrWhiteSpace (choice) && self.Contains (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAll (this string self, params string [] choices)
        {
            if (self == null)
                return false;
            foreach (string choice in choices)
            {
                if (!string.IsNullOrWhiteSpace (choice) && !self.Contains (choice))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool StartsWithAny (this string self, params string [] choices)
        {
            if (self == null)
                return false;
            foreach (string choice in choices)
            {
                if (!string.IsNullOrWhiteSpace (choice) && self.StartsWith (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EndsWithAny (this string self, params string [] choices)
        {
            if (self == null)
                return false;
            foreach (string choice in choices)
            {
                if (!string.IsNullOrWhiteSpace (choice) && self.EndsWith (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static string Multiply (this string source, int multiplier)
        {
            StringBuilder sb = new StringBuilder (multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append (source);
            }
            return sb.ToString ();
        }

        public static string ReplaceStart (this string source, string search, string replacement)
        {
            if (source.StartsWith (search))
            {
                return replacement + source.Substring (search.Length);
            }
            else
            {
                return source;
            }
        }

        public static string ReplaceEnd (this string source, string search, string replacement)
        {
            if (source.EndsWith (search))
            {
                return source.Substring (0, source.Length - search.Length) + replacement;
            }
            else
            {
                return source;
            }
        }

        private static readonly Regex _rgxNumericOnly = new Regex ("[^0-9]");

        public static string ToNumericOnly (this string input)
        {
            return _rgxNumericOnly.Replace (input, "");
        }


        private static HashSet<char> _allowedChars = new HashSet<char> (new []
            {
            '.', ':', ',', ';', ' ', '/', '\\', '_', '-', '+', '*', '(', ')', '{', '}', '[', ']', '#', '?', '\'', '"', '!', '^', '=', '%', '@', '$',
        });

        public static string ToUnicodeLettersAndAscii (string s)
        {
            return new string (s.ToCharArray ().Where (c => char.IsLetterOrDigit (c) || _allowedChars.Contains (c)).ToArray ());
        }

        public static bool EqualsWithIgnoreCase (this string left, string right)
        {
            return left.Equals (right, StringComparison.InvariantCultureIgnoreCase);
        }
        

    }
}
