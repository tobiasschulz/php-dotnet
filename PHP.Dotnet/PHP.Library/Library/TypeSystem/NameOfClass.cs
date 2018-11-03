using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct NameOfClass : IEquatable<NameOfClass>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public NameOfClass (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (NameOfClass) && Equals ((NameOfClass)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (NameOfClass other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (NameOfClass name, NameOfClass other)
        {
            return name.Equals (other);
        }

        public static bool operator != (NameOfClass name, NameOfClass other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator NameOfClass (string other)
        {
            return new NameOfClass (other);
        }

    }

}
