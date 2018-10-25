using System;
using System.Collections.Generic;
using System.Text;
using PHP.Standard;

namespace PHP.Helper
{
    public static class PathHelper
    {
        private static char [] _slash_array = new [] { '/' };

        internal static string NormalizePath (string path)
        {
            return path.Replace ('\\', '/').Split (_slash_array, StringSplitOptions.RemoveEmptyEntries).Join ("/");
        }
    }
}
