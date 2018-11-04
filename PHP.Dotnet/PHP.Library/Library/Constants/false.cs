using System;
using PHP.Tree;
using System.IO;
using PHP.Library.TypeSystem;

namespace PHP.Library.Constants
{
    public class @false : ManagedGlobalConstant
    {
        public @false ()
            : base (new NameOfVariable ("false"))
        {
        }

        protected override FinalExpression _getValue ()
        {
            return new BoolExpression (false);
        }
    }
}
