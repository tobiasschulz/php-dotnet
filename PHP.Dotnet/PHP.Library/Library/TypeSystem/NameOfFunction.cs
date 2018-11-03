using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct NameOfFunction : IEquatable<NameOfFunction>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public NameOfFunction (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }


        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (NameOfFunction) && Equals ((NameOfFunction)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (NameOfFunction other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (NameOfFunction name, NameOfFunction other)
        {
            return name.Equals (other);
        }

        public static bool operator != (NameOfFunction name, NameOfFunction other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator NameOfFunction (Devsense.PHP.Syntax.Name other)
        {
            return new NameOfFunction (other.Value);
        }

        public static implicit operator NameOfFunction (string other)
        {
            return new NameOfFunction (other);
        }

        public static implicit operator NameOfFunction (Devsense.PHP.Syntax.NameRef other)
        {
            return new NameOfFunction (other.Name.Value);
        }

        public static implicit operator NameOfFunction (Devsense.PHP.Syntax.QualifiedName other)
        {
            return new NameOfFunction (other.Name.Value);
        }

        public static implicit operator NameOfFunction (Devsense.PHP.Syntax.TranslatedQualifiedName other)
        {
            return new NameOfFunction (other.Name.QualifiedName.Name.Value);
        }
    }

}
