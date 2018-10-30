using System;
using PHP.Tree;
using System.IO;

namespace PHP.Library.Library.Constants
{
    public class DIRECTORY_SEPARATOR : GlobalConstant
    {
        public DIRECTORY_SEPARATOR ()
            : base (new VariableName ("DIRECTORY_SEPARATOR"))
        {
        }

        internal override FinalExpression _getValue ()
        {
            return new StringExpression (Directory.Exists ("/bin") ? "/" : "\\");
        }
    }
}
