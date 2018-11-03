using System;
using System.Diagnostics;

namespace PHP.Tree
{
    /*
    public struct Name : IEquatable<Name>, IEquatable<string>
    {
        private readonly string _value;
        private readonly int _hashcode;

        public string Value => _value;

        public Name (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        public static readonly Name [] EmptyNames = new Name [0];
        public static readonly Name EmptyBaseName = new Name ("");
        public static readonly Name SelfClassName = new Name ("self");
        public static readonly Name StaticClassName = new Name ("static");
        public static readonly Name ParentClassName = new Name ("parent");
        public static readonly Name AutoloadName = new Name ("__autoload");
        public static readonly Name ClrCtorName = new Name (".ctor");
        public static readonly Name ClrInvokeName = new Name ("Invoke"); // delegate Invoke method
        public static readonly Name AppStaticName = new Name ("AppStatic");
        public static readonly Name AppStaticAttributeName = new Name ("AppStaticAttribute");
        public static readonly Name ExportName = new Name ("Export");
        public static readonly Name ExportAttributeName = new Name ("ExportAttribute");
        public static readonly Name DllImportAttributeName = new Name ("DllImportAttribute");
        public static readonly Name DllImportName = new Name ("DllImport");
        public static readonly Name OutAttributeName = new Name ("OutAttribute");
        public static readonly Name OutName = new Name ("Out");
        public static readonly Name DeclareHelperName = new Name ("<Declare>");
        public static readonly Name LambdaFunctionName = new Name ("<Lambda>");
        public static readonly Name ClosureFunctionName = new Name ("{closure}");
        public static readonly Name AnonymousClassName = new Name ("class@anonymous");

        public bool IsParentClassName => this.Equals (ParentClassName);
        public bool IsSelfClassName => this.Equals (SelfClassName);
        public bool IsStaticClassName => this.Equals (StaticClassName);
        public bool IsReservedClassName => IsParentClassName || IsSelfClassName || IsStaticClassName;

        /// <summary>
        /// <c>true</c> if the name was generated for the 
        /// <see cref="Devsense.PHP.Syntax.Ast.AnonymousTypeDecl"/>, 
        /// <c>false</c> otherwise.
        /// </summary>
        public bool IsGenerated => _value.StartsWith (AnonymousClassName.Value);


        public override bool Equals (object obj)
        {
            return obj != null && obj.GetType () == typeof (Name) && Equals ((Name)obj);
        }

        public override int GetHashCode ()
        {
            return this._hashcode;
        }

        public override string ToString ()
        {
            return this._value;
        }

        public bool Equals (Name other)
        {
            return this.GetHashCode () == other.GetHashCode () && Equals (other.Value);
        }

        public static bool operator == (Name name, Name other)
        {
            return name.Equals (other);
        }

        public static bool operator != (Name name, Name other)
        {
            return !name.Equals (other);
        }

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator Name (string other)
        {
            return new Name (other);
        }

    }

    public static class NameHelper
    {

        /// <summary>
        /// Separator of class name and its static field in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        public const string ClassMemberSeparator = "::";

        /// <summary>
        /// Splits the <paramref name="value"/> into class name and member name if it is double-colon separated.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <param name="className">Will contain the class name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise <c>null</c>.</param>
        /// <param name="memberName">Will contain the member name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise it contains original <paramref name="value"/>.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax (string value, out string className, out string memberName)
        {

            int separator;
            if ((separator = value.IndexOf (':')) >= 0 &&    // value.Contains( ':' )
                (separator = System.Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf (value, ClassMemberSeparator, separator, value.Length - separator, System.Globalization.CompareOptions.Ordinal)) > 0) // value.Contains( "::" )
            {
                className = value.Remove (separator);
                memberName = value.Substring (separator + ClassMemberSeparator.Length);
                return true;
            }
            else
            {
                className = null;
                memberName = value;
                return false;
            }
        }

        /// <summary>
        /// Determines if given <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax (string value)
        {
            return value != null && value.Contains (ClassMemberSeparator);
        }

    }
    */


}
