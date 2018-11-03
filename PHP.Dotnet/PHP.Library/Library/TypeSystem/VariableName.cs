using System;

namespace PHP.Library.TypeSystem
{
    public struct VariableName : IEquatable<VariableName>, IEquatable<string>
    {
        public string Value { get => _value; set => _value = value; }
        private string _value;

        public static readonly VariableName ThisVariableName = new VariableName ("this");

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

        public VariableName (string value)
        {
            _value = value ?? string.Empty;
        }

        public override bool Equals (object obj)
        {
            if (!(obj is VariableName)) return false;
            return Equals ((VariableName)obj);
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode ();
        }

        public override string ToString ()
        {
            return _value;
        }

        public bool Equals (VariableName other)
        {
            return _value.Equals (other._value);
        }

        public static bool operator == (VariableName name, VariableName other)
        {
            return name.Equals (other);
        }

        public static bool operator != (VariableName name, VariableName other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return _value.Equals (other);
        }

        public static bool operator == (VariableName name, string str)
        {
            return name.Equals (str);
        }

        public static bool operator != (VariableName name, string str)
        {
            return !name.Equals (str);
        }

        public static implicit operator VariableName (Devsense.PHP.Syntax.VariableName other)
        {
            return new VariableName (other.Value);
        }

        public static implicit operator VariableName (Devsense.PHP.Syntax.VariableNameRef other)
        {
            return new VariableName (other.Name.Value);
        }

    }

}
