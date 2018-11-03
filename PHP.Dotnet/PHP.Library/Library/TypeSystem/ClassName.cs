using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct ClassName : IEquatable<ClassName>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public ClassName (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (ClassName) && Equals ((ClassName)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (ClassName other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (ClassName name, ClassName other)
        {
            return name.Equals (other);
        }

        public static bool operator != (ClassName name, ClassName other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator ClassName (Devsense.PHP.Syntax.Name other)
        {
            return new ClassName (other.Value);
        }

        public static implicit operator ClassName (string other)
        {
            return new ClassName (other);
        }

        public static implicit operator ClassName (Devsense.PHP.Syntax.NameRef other)
        {
            return new ClassName (other.Name.Value);
        }

        public static implicit operator ClassName (Devsense.PHP.Syntax.QualifiedName other)
        {
            return new ClassName (other.Name.Value);
        }

        public static implicit operator ClassName (Devsense.PHP.Syntax.TranslatedQualifiedName other)
        {
            return new ClassName (other.Name.QualifiedName.Name.Value);
        }
    }

}
