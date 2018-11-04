using System;
using PHP.Tree;
using System.IO;
using PHP.Library.TypeSystem;

namespace PHP.Library.Constants
{
    public class @true : ManagedGlobalConstant
    {
        public @true ()
            : base (new NameOfVariable ("true"))
        {
        }

        protected override FinalExpression _getValue ()
        {
            return new BoolExpression (true);
        }
    }
}
