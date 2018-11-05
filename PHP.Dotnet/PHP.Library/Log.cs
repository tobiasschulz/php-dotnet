using System;
using System.Collections.Generic;
using System.Text;

namespace PHP
{
    public static class Log
    {
        public static void Error (Exception ex)
        {
            _write ("ERROR", ex);
        }

        public static void Error (string message)
        {
            _write ("ERROR", message);
        }

        public static void Error (object message)
        {
            _write ("ERROR", message);
        }

        public static void Debug (string message)
        {
            _write ("D", message);
        }

        public static void Debug (object message)
        {
            _write ("D", message);
        }

        public static void Message (string message)
        {
            _write ("MESSAGE", message);
        }

        public static void Message (object message)
        {
            _write ("MESSAGE", message);
        }

        public static void _write (string level, object message)
        {
            string padding = level.Length == 1 ? "       " : level.Length == 2 ? "      " : level.Length == 3 ? "     " : level.Length == 4 ? "    " : level.Length == 5 ? "   " : level.Length == 6 ? "  " : level.Length == 7 ? " " : "";
            Console.WriteLine ($"[{level}]{padding} {message}");
        }


    }
}
