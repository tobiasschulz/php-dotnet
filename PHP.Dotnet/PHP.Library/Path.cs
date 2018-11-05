using System;
using System.Diagnostics;
using System.IO;
using PHP.Library.Internal;

namespace PHP
{
    public readonly struct NormalizedPath : IEquatable<NormalizedPath>
    {
        public static readonly NormalizedPath DEFAULT_ROOT = new NormalizedPath ("/");
        public static readonly NormalizedPath DEFAULT_DOT = new NormalizedPath (".");
        public static readonly NormalizedPath DEFAULT_EVAL_PHP = new NormalizedPath ("eval.php");

        private readonly string _original;
        private readonly string _normalized;
        private readonly int _hashcode;

        public string Original => _original ?? string.Empty;
        public string Normalized => _normalized ?? string.Empty;

        public NormalizedPath (string value)
        {
            Debug.Assert (value != null);
            this._original = value.Replace (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
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
