﻿using System;
using System.Collections.Generic;

namespace PHP.Standard
{
    public static partial class MoreEnumerable
    {
        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MinBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy (selector, null);
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/>
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MinBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException (nameof (source));
            if (selector == null) throw new ArgumentNullException (nameof (selector));
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator ())
            {
                if (!sourceIterator.MoveNext ())
                {
                    return default (TSource);
                }
                var min = sourceIterator.Current;
                var minKey = selector (min);
                while (sourceIterator.MoveNext ())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector (candidate);
                    if (comparer.Compare (candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MaxBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy (selector, null);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/>
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MaxBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException (nameof (source));
            if (selector == null) throw new ArgumentNullException (nameof (selector));
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator ())
            {
                if (!sourceIterator.MoveNext ())
                {
                    return default (TSource);
                }
                var max = sourceIterator.Current;
                var maxKey = selector (max);
                while (sourceIterator.MoveNext ())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector (candidate);
                    if (comparer.Compare (candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy (keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException (nameof (source));
            if (keySelector == null) throw new ArgumentNullException (nameof (keySelector));

            return _ (); IEnumerable<TSource> _ ()
            {
                var knownKeys = new HashSet<TKey> (comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add (keySelector (element)))
                        yield return element;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="HashSet{T}"/> of the source items using the default equality
        /// comparer for the type.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in source sequence.</typeparam>
        /// <param name="source">Source sequence</param>
        /// <returns>A hash set of the items in the sequence, using the default equality comparer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        /// <remarks>
        /// This evaluates the input sequence completely.
        /// </remarks>

        public static HashSet<TSource> ToHashSet<TSource> (this IEnumerable<TSource> source)
        {
            return source.ToHashSet (null);
        }

        /// <summary>
        /// Returns a <see cref="HashSet{T}"/> of the source items using the specified equality
        /// comparer for the type.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in source sequence.</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="comparer">Equality comparer to use; a value of null will cause the type's default equality comparer to be used</param>
        /// <returns>A hash set of the items in the sequence, using the default equality comparer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        /// <remarks>
        /// This evaluates the input sequence completely.
        /// </remarks>

        public static HashSet<TSource> ToHashSet<TSource> (this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException (nameof (source));
            return new HashSet<TSource> (source, comparer);
        }
    }
}
