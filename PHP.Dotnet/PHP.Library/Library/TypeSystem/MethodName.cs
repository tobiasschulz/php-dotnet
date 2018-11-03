using System;
using System.Diagnostics;

namespace PHP.Library.TypeSystem
{
    public struct MethodName : IEquatable<MethodName>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public MethodName (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        // magic
        public static readonly MethodName Construct = new MethodName ("__construct");
        public static readonly MethodName Destruct = new MethodName ("__destruct");
        public static readonly MethodName Clone = new MethodName ("__clone");
        public static readonly MethodName Tostring = new MethodName ("__tostring");
        public static readonly MethodName Sleep = new MethodName ("__sleep");
        public static readonly MethodName Wakeup = new MethodName ("__wakeup");
        public static readonly MethodName Get = new MethodName ("__get");
        public static readonly MethodName Set = new MethodName ("__set");
        public static readonly MethodName Call = new MethodName ("__call");
        public static readonly MethodName Invoke = new MethodName ("__invoke");
        public static readonly MethodName CallStatic = new MethodName ("__callStatic");
        public static readonly MethodName Unset = new MethodName ("__unset");
        public static readonly MethodName Isset = new MethodName ("__isset");

        public bool IsCloneName => this.Equals (Clone);
        public bool IsConstructName => this.Equals (Construct);
        public bool IsDestructName => this.Equals (Destruct);
        public bool IsCallName => this.Equals (Call);
        public bool IsCallStaticName => this.Equals (CallStatic);
        public bool IsToStringName => this.Equals (Tostring);

        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (MethodName) && Equals ((MethodName)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (MethodName other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (MethodName name, MethodName other)
        {
            return name.Equals (other);
        }

        public static bool operator != (MethodName name, MethodName other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator MethodName (Devsense.PHP.Syntax.Name other)
        {
            return new MethodName (other.Value);
        }

        public static implicit operator MethodName (string other)
        {
            return new MethodName (other);
        }

        public static implicit operator MethodName (Devsense.PHP.Syntax.NameRef other)
        {
            return new MethodName (other.Name.Value);
        }

        public static implicit operator MethodName (Devsense.PHP.Syntax.QualifiedName other)
        {
            return new MethodName (other.Name.Value);
        }

        public static implicit operator MethodName (Devsense.PHP.Syntax.TranslatedQualifiedName other)
        {
            return new MethodName (other.Name.QualifiedName.Name.Value);
        }
    }

}
