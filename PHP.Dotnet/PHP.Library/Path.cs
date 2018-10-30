using System;
using System.Diagnostics;
using PHP.Helper;

namespace PHP
{
    public struct NormalizedPath : IEquatable<NormalizedPath>
    {
        private readonly string _original;
        private readonly string _normalized;
        private readonly int _hashcode;

        public string Original => _original ?? string.Empty;
        public string Normalized => _normalized ?? string.Empty;

        public NormalizedPath (string value)
        {
            Debug.Assert (value != null);
            this._original = value;
            this._normalized = PathHelper.NormalizePath (value);
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (this._normalized);
        }

        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (NormalizedPath) && Equals ((NormalizedPath)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._original;
        }

        public bool Equals (NormalizedPath other)
        {
            return this.GetHashCode () == other.GetHashCode () && string.Equals (_normalized, other._normalized, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator == (NormalizedPath name, NormalizedPath other)
        {
            return name.Equals (other);
        }

        public static bool operator != (NormalizedPath name, NormalizedPath other)
        {
            return !name.Equals (other);
        }

        public static implicit operator NormalizedPath (string other)
        {
            return new NormalizedPath (other);
        }

    }

}
