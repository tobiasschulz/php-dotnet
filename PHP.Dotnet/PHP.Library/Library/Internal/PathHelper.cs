using System;
using System.Collections.Generic;
using System.Text;
using PHP.Standard;

namespace PHP.Library.Internal
{
    public static class PathHelper
    {
        private static char [] _slash_array = new [] { '/' };

        internal static string NormalizePath (string path)
        {
            bool starts_with_slash = !string.IsNullOrEmpty (path) && path [0] == '/';
            string res = path.Replace ('\\', '/').Split (_slash_array, StringSplitOptions.RemoveEmptyEntries).Join ("/");
            if (starts_with_slash && res != null && !string.IsNullOrEmpty (res) && res [0] != '/')
            {
                res = '/' + res;
            }
            return res;
        }
    }
}
