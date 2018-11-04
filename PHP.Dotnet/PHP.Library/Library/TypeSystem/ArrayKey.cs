using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct ArrayKey : IEquatable<ArrayKey>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value ?? string.Empty;

        public ArrayKey (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        public override bool Equals (object obj)
        {
            return obj != null && obj is ArrayKey && Equals ((ArrayKey)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (ArrayKey other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (ArrayKey name, ArrayKey other)
        {
            return name.Equals (other);
        }

        public static bool operator != (ArrayKey name, ArrayKey other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator ArrayKey (string other)
        {
            return new ArrayKey (other);
        }

        public bool IsDigitsOnly ()
        {
            foreach (char c in Value)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }

}
