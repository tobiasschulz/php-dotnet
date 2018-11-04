using System;
using PHP.Tree;
using System.IO;
using PHP.Library.TypeSystem;

namespace PHP.Library.Constants
{
    public class DIRECTORY_SEPARATOR : GlobalConstant
    {
        public DIRECTORY_SEPARATOR ()
            : base (new NameOfVariable ("DIRECTORY_SEPARATOR"))
        {
        }

        protected override FinalExpression _getValue ()
        {
            return new StringExpression (Directory.Exists ("/bin") ? "/" : "\\");
        }
    }
}
