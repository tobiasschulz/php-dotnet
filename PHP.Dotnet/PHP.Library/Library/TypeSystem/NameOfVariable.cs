using System;

namespace PHP.Library.TypeSystem
{
    public struct NameOfVariable : IEquatable<NameOfVariable>, IEquatable<string>
    {
        public string Value { get => _value; set => _value = value; }
        private string _value;

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
            _value = value ?? string.Empty;
        }

        public override bool Equals (object obj)
        {
            if (!(obj is NameOfVariable)) return false;
            return Equals ((NameOfVariable)obj);
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode ();
        }

        public override string ToString ()
        {
            return _value;
        }

        public bool Equals (NameOfVariable other)
        {
            return _value.Equals (other._value);
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
            return _value.Equals (other);
        }

        public static bool operator == (NameOfVariable name, string str)
        {
            return name.Equals (str);
        }

        public static bool operator != (NameOfVariable name, string str)
        {
            return !name.Equals (str);
        }

        public static implicit operator NameOfVariable (Devsense.PHP.Syntax.VariableName other)
        {
            return new NameOfVariable (other.Value);
        }

        public static implicit operator NameOfVariable (Devsense.PHP.Syntax.VariableNameRef other)
        {
            return new NameOfVariable (other.Name.Value);
        }

    }

}
