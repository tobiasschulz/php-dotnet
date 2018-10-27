using System;

namespace PHP.Tree
{
    public struct Name : IEquatable<Name>, IEquatable<string>
    {
        public string Value
        {
            get { return _value; }
        }
        private readonly string _value;
        private readonly int _hashcode;

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

        /// <summary>
        /// Contains special (or &quot;magic&quot;) method names.
        /// </summary>
        public static class SpecialMethodNames
        {
            /// <summary>Constructor.</summary>
            public static readonly Name Construct = new Name ("__construct");

            /// <summary>Destructor.</summary>
            public static readonly Name Destruct = new Name ("__destruct");

            /// <summary>Invoked when cloning instances.</summary>
            public static readonly Name Clone = new Name ("__clone");

            /// <summary>Invoked when casting to string.</summary>
            public static readonly Name Tostring = new Name ("__tostring");

            /// <summary>Invoked when serializing instances.</summary>
            public static readonly Name Sleep = new Name ("__sleep");

            /// <summary>Invoked when deserializing instanced.</summary>
            public static readonly Name Wakeup = new Name ("__wakeup");

            /// <summary>Invoked when an unknown field is read.</summary>
            public static readonly Name Get = new Name ("__get");

            /// <summary>Invoked when an unknown field is written.</summary>
            public static readonly Name Set = new Name ("__set");

            /// <summary>Invoked when an unknown method is called.</summary>
            public static readonly Name Call = new Name ("__call");

            /// <summary>Invoked when an object is called like a function.</summary>
            public static readonly Name Invoke = new Name ("__invoke");

            /// <summary>Invoked when an unknown method is called statically.</summary>
            public static readonly Name CallStatic = new Name ("__callStatic");

            /// <summary>Invoked when an unknown field is unset.</summary>
            public static readonly Name Unset = new Name ("__unset");

            /// <summary>Invoked when an unknown field is tested for being set.</summary>
            public static readonly Name Isset = new Name ("__isset");
        };

        public bool IsCloneName
        {
            get { return this.Equals (SpecialMethodNames.Clone); }
        }

        public bool IsConstructName
        {
            get { return this.Equals (SpecialMethodNames.Construct); }
        }

        public bool IsDestructName
        {
            get { return this.Equals (SpecialMethodNames.Destruct); }
        }

        public bool IsCallName
        {
            get { return this.Equals (SpecialMethodNames.Call); }
        }

        public bool IsCallStaticName => this.Equals (SpecialMethodNames.CallStatic);

        public bool IsToStringName => this.Equals (SpecialMethodNames.Tostring); }

        public bool IsParentClassName => this.Equals (Name.ParentClassName); }

        public bool IsSelfClassName => this.Equals (Name.SelfClassName); }

        public bool IsStaticClassName => this.Equals (Name.StaticClassName); }

        public bool IsReservedClassName
        {
            get { return IsParentClassName || IsSelfClassName || IsStaticClassName; }
        }

        /// <summary>
        /// <c>true</c> if the name was generated for the 
        /// <see cref="Devsense.PHP.Syntax.Ast.AnonymousTypeDecl"/>, 
        /// <c>false</c> otherwise.
        /// </summary>
        public bool IsGenerated
        {
            get { return _value.StartsWith (AnonymousClassName.Value); }
        }

        #endregion

        /// <summary>
        /// Creates a name. 
        /// </summary>
        /// <param name="value">The name shouldn't be <B>null</B>.</param>
        public Name (string value)
        {
            Debug.Assert (value != null);
            this._value = value;
            this._hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode (value);
        }

        #region Utils

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

        #endregion

        #region Basic Overrides

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

        #endregion

        #region IEquatable<Name> Members

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

        #endregion

        #region IEquatable<string> Members

        public bool Equals (string other)
        {
            return string.Equals (_value, other, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }


}
