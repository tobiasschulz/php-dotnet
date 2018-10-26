// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;

namespace Devsense.PHP.Text
{
    /// <summary>
    /// Represents text span.
    /// </summary>
    public struct Span2 : IEquatable<Span2>
    {
        #region Fields

        private int _start;
        private int _length;

        #endregion

        #region Properties

        public int Start
        {
            get
            {
                return _start;
            }
        }

        public int End
        {
            get
            {
                return _start + _length;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _length == 0;
            }
        }

        /// <summary>
        /// Gets value determining whether this span represents a valid span.
        /// </summary>
        public bool IsValid { get { return _length >= 0; } }

        /// <summary>
        /// Gets representation of an invalid span.
        /// </summary>
        public static Span2 Invalid { get { return new Span2() { _start = 0, _length = -1 }; } }

        public int StartOrInvalid => IsValid ? Start : -1;

        #endregion

        #region Construction

        public Span2(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            _start = start;
            _length = length;
        }

        public static Span2 FromBounds(int start, int end)
        {
            return new Span2(start, end - start);
        }

        #endregion

        #region Methods

        public static Span2 Combine(Span2 left, Span2 right)
        {
            return Span2.FromBounds(left.Start, right.End);
        }

        public bool Contains(int position)
        {
            return position >= this.Start && position < this.End;
        }

        public bool Contains(Span2 span)
        {
            return span._start >= this.Start && span.End <= this.End;
        }

        public bool OverlapsWith(Span2 span)
        {
            return Math.Max(this.Start, span.Start) < Math.Min(this.End, span.End);
        }

        public Span2? Overlap(Span2 span)
        {
            int start = Math.Max(this.Start, span.Start);
            int end = Math.Min(this.End, span.End);
            if (start < end)
                return new Span2?(Span2.FromBounds(start, end));

            return null;
        }

        public bool IntersectsWith(Span2 span)
        {
            return span.Start <= this.End && span.End >= this.Start;
        }

        public Span2? Intersection(Span2 span)
        {
            int start = Math.Max(this.Start, span.Start);
            int end = Math.Min(this.End, span.End);
            if (start <= end)
                return new Span2?(Span2.FromBounds(start, end));

            return null;
        }

        public static bool operator ==(Span2 left, Span2 right)
        {
            return left._start == right._start && left._length == right._length;
        }

        public static bool operator !=(Span2 left, Span2 right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets portion of document defined by this <see cref="Span2"/>.
        /// </summary>
        public string GetText(string document)
        {
            return document.Substring(_start, _length);
        }

        #endregion

        #region Object Members

        public override int GetHashCode()
        {
            return _start.GetHashCode() ^ _length.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Span2)
                return Equals((Span2)obj);

            return false;
        }

        public override string ToString()
        {
            return string.Format("[{0}..{1})", _start, _start + _length);
        }

        #endregion

        #region IEquatable<Span> Members

        public bool Equals(Span2 other)
        {
            return other._start == this._start && other._length == this._length;
        }

        #endregion
    }
}
