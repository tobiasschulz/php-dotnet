using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct NameOfMethod : IEquatable<NameOfMethod>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public NameOfMethod (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        // magic
        public static readonly NameOfMethod Construct = new NameOfMethod ("__construct");
        public static readonly NameOfMethod Destruct = new NameOfMethod ("__destruct");
        public static readonly NameOfMethod Clone = new NameOfMethod ("__clone");
        public static readonly NameOfMethod Tostring = new NameOfMethod ("__tostring");
        public static readonly NameOfMethod Sleep = new NameOfMethod ("__sleep");
        public static readonly NameOfMethod Wakeup = new NameOfMethod ("__wakeup");
        public static readonly NameOfMethod Get = new NameOfMethod ("__get");
        public static readonly NameOfMethod Set = new NameOfMethod ("__set");
        public static readonly NameOfMethod Call = new NameOfMethod ("__call");
        public static readonly NameOfMethod Invoke = new NameOfMethod ("__invoke");
        public static readonly NameOfMethod CallStatic = new NameOfMethod ("__callStatic");
        public static readonly NameOfMethod Unset = new NameOfMethod ("__unset");
        public static readonly NameOfMethod Isset = new NameOfMethod ("__isset");

        public bool IsCloneName => this.Equals (Clone);
        public bool IsConstructName => this.Equals (Construct);
        public bool IsDestructName => this.Equals (Destruct);
        public bool IsCallName => this.Equals (Call);
        public bool IsCallStaticName => this.Equals (CallStatic);
        public bool IsToStringName => this.Equals (Tostring);

        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (NameOfMethod) && Equals ((NameOfMethod)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (NameOfMethod other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (NameOfMethod name, NameOfMethod other)
        {
            return name.Equals (other);
        }

        public static bool operator != (NameOfMethod name, NameOfMethod other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator NameOfMethod (string other)
        {
            return new NameOfMethod (other);
        }
    }

}
