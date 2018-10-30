using System;
using System.Runtime.Serialization;

namespace PHP
{
    internal class InterpreterException : Exception
    {
        public InterpreterException (string message)
            : base (message)
        {
        }

    }
}
