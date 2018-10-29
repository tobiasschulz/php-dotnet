using System;
using System.Collections.Generic;
using System.Text;
using PHP.Library.Functions;
using PHP.Tree;

namespace PHP.Library
{
    public static class StandardLibrary
    {
        public static void Populate (FunctionCollection functions)
        {
            functions.Add (new Echo ());
            functions.Add (new Define ());
            functions.Add (new Realpath ());
            functions.Add (new Dirname ());
        }
    }
}
