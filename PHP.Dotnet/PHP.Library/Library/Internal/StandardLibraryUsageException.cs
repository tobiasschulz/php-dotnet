using System;
using System.Collections.Generic;
using System.Text;

namespace PHP.Library.Internal
{
    public abstract class StandardLibraryUsageException : InternalException
    {
        protected StandardLibraryUsageException (string message)
            : base (message)
        {
        }
    }

    public sealed class WrongParameterCountException : StandardLibraryUsageException
    {
        public WrongParameterCountException (Function function, int expected, int actual)
            : base ($"Wrong parameter count for function {function}: expected {expected}, actual {actual}")
        {
        }
    }

}
