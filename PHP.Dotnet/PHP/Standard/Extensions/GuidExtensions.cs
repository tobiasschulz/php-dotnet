using System;

namespace PHP.Standard
{
    public static class GuidExtensions
    {
        public static string ToStringUpper (this Guid guid)
        {
            return guid.ToString ("D").ToUpper ();
        }
    }
}
