using System;
using System.Collections.Generic;
using System.Text;

namespace PHP.Library.Internal
{
    public abstract class InternalException : Exception
    {
        protected InternalException (string message)
            : base (message)
        {
        }
    }

}
