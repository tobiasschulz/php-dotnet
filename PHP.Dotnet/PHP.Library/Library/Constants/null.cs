using System;
using PHP.Tree;
using System.IO;
using PHP.Library.TypeSystem;

namespace PHP.Library.Constants
{
    public class @null : ManagedGlobalConstant
    {
        public @null ()
            : base (new NameOfVariable ("null"))
        {
        }

        protected override FinalExpression _getValue ()
        {
            return new NullExpression ();
        }
    }
}
