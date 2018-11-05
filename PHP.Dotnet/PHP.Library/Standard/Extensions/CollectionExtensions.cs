using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PHP.Standard
{
    public static class StandardCollectionExtensions
    {
        public static T Get<T> (this IReadOnlyList<T> list, int index, T default_value)
        {
            return index >= 0 && index < list.Count ? list [index] : default_value;
        }

        public static void Set<T> (this IList<T> list, int index, T value)
        {
            if (index >= 0 && index < list.Count)
            {
                list [index] = value;
            }
        }

        public static V Get<K, V> (this IReadOnlyDictionary<K, V> dict, K key, V default_value)
        {
            return dict.TryGetValue (key, out V res) ? res : default_value;
        }

        public static Dictionary<K, V> ToDictionarySafe<T, K, V> (this IEnumerable<T> enumerable, Func<T, K> key, Func<T, V> value, K defaultKey = default, V default_value = default)
        {
            var result = new Dictionary<K, V> ();
            foreach (var item in enumerable)
            {
                result [key (item)] = value (item);
            }
            return result;
        }

        public static IEnumerable<T> TakeLast<T> (this IEnumerable<T> source, int N)
        {
            var enumerable = source as T [] ?? source.ToArray ();
            return enumerable.Skip (System.Math.Max (0, enumerable.Count () - N));
        }

        public static string Join<T> (this IEnumerable<T> enumerable, string delimiter)
        {
            return string.Join (delimiter, enumerable.Select (e => e.ToString ()).ToArray ());
        }

        public static string Join<T> (this IEnumerable<T> enumerable, char delimiter)
        {
            return string.Join (delimiter.ToString (), enumerable.Select (e => e.ToString ()).ToArray ());
        }

        public static T [] Extend<T> (this T [] firstArray, params T [] secondArray) where T : class
        {
            if (secondArray == null)
            {
                throw new ArgumentNullException ("secondArray");
            }
            if (firstArray == null)
            {
                return secondArray;
            }
            return firstArray.Concat (secondArray).ToArray (); // although Concat is not recommended for performance reasons
        }

        [EditorBrowsable (EditorBrowsableState.Never)]
        public static T [] Extend<T> (this T firstItem, params T [] secondArray) where T : class
        {
            if (secondArray == null)
            {
                throw new ArgumentNullException ("secondArray");
            }
            if (firstItem == null)
            {
                return secondArray;
            }
            return new T [] { firstItem }.Concat (secondArray).ToArray (); // although Concat is not recommended for performance reasons
        }

        public static byte [] ToByteArray (this Stream stream)
        {
            if (stream is MemoryStream)
            {
                MemoryStream memStream = stream as MemoryStream;
                return memStream.ToArray ();
            }
            else
            {
                stream.Position = 0;
                using (var memStream = new MemoryStream ())
                {
                    stream.CopyTo (memStream);
                    return memStream.ToArray ();
                }
            }
        }

        public static IEnumerable<List<T>> Partition<T> (this IList<T> source, int size)
        {
            for (int i = 0; i < source.Count; i++)
            {
                yield return new List<T> (source.Skip (size * i).Take (size));
            }
        }

        public static bool IsAny<T> (this T self, params T [] choices)
        {
            if (self == null)
                return false;
            foreach (T choice in choices)
            {
                if (self.Equals (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAny<T> (this T? self, params T [] choices) where T : struct
        {
            if (self == null)
                return false;
            foreach (T choice in choices)
            {
                if (self.Equals (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNone<T> (this T self, params T [] choices)
        {
            return !IsAny<T> (self, choices);
        }

        public static bool IsNone<T> (this T? self, params T [] choices) where T : struct
        {
            return !IsAny<T> (self, choices);
        }

        public static bool IsAny<T> (this T self, IReadOnlyList<T> choices)
        {
            if (self == null)
                return false;
            foreach (T choice in choices)
            {
                if (self.Equals (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAny<T> (this T? self, IReadOnlyList<T> choices) where T : struct
        {
            if (self == null)
                return false;
            foreach (T choice in choices)
            {
                if (self.Equals (choice))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNone<T> (this T self, IReadOnlyList<T> choices)
        {
            return !IsAny<T> (self, choices);
        }

        public static bool IsNone<T> (this T? self, IReadOnlyList<T> choices) where T : struct
        {
            return !IsAny<T> (self, choices);
        }

        public static bool AddRange<T> (this HashSet<T> self, IEnumerable<T> items)
        {
            bool allAdded = true;
            foreach (T item in items)
            {
                allAdded &= self.Add (item);
            }
            return allAdded;
        }

        public static bool None<TSource> (this IEnumerable<TSource> source)
        {
            return !source.Any ();
        }

        public static bool None<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any (predicate);
        }

        /// <summary>
        /// Converts list to <see cref="ImmutableArray{T}"/> safely. If the list is <c>null</c>, empty array is returned.
        /// </summary>
        public static ImmutableArray<T> AsImmutableSafe<T> (this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return ImmutableArray<T>.Empty;
            }
            else
            {
                return list.ToImmutableArray ();
            }
        }
    }

}
