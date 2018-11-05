using System;

namespace PHP.Library.TypeSystem
{
    public readonly struct NameOfVariable : IEquatable<NameOfVariable>, IEquatable<string>
    {
        private readonly string _value;

        public string Value => _value ?? string.Empty;

        public static readonly NameOfVariable ThisVariableName = new NameOfVariable ("this");

        public const string EnvName = "_ENV";
        public const string ServerName = "_SERVER";
        public const string GlobalsName = "GLOBALS";
        public const string RequestName = "_REQUEST";
        public const string GetName = "_GET";
        public const string PostName = "_POST";
        public const string CookieName = "_COOKIE";
        public const string HttpRawPostDataName = "HTTP_RAW_POST_DATA";
        public const string FilesName = "_FILES";
        public const string SessionName = "_SESSION";

        public bool IsThisVariableName => this == ThisVariableName;

        public bool IsAutoGlobal => IsAutoGlobalVariableName (this.Value);

        public static bool IsAutoGlobalVariableName (string name)
        {
            switch (name)
            {
                case GlobalsName:
                case ServerName:
                case EnvName:
                case CookieName:
                case HttpRawPostDataName:
                case FilesName:
                case RequestName:
                case GetName:
                case PostName:
                case SessionName:
                    return true;

                default:
                    return false;
            }
        }

        public NameOfVariable (string value)
        {
            _value = !string.IsNullOrEmpty (value) ? value : null;
        }

        public bool IsEmpty => this.Value.Length == 0;

        public override bool Equals (object obj)
        {
            if (!(obj is NameOfVariable)) return false;
            return Equals ((NameOfVariable)obj);
        }

        public override int GetHashCode ()
        {
            return Value.GetHashCode ();
        }

        public override string ToString ()
        {
            return Value;
        }

        public bool Equals (NameOfVariable other)
        {
            return Value.Equals (other.Value);
        }

        public static bool operator == (NameOfVariable name, NameOfVariable other)
        {
            return name.Equals (other);
        }

        public static bool operator != (NameOfVariable name, NameOfVariable other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return Value.Equals (other);
        }

        public static bool operator == (NameOfVariable name, string str)
        {
            return name.Equals (str);
        }

        public static bool operator != (NameOfVariable name, string str)
        {
            return !name.Equals (str);
        }

        public static implicit operator NameOfVariable (string other)
        {
            return new NameOfVariable (other);
        }

    }

}
