using System;
using System.Collections.Generic;
using System.Text;

namespace PHP
{
    public static class Log
    {
        public static void Error (Exception ex)
        {
            Console.WriteLine (ex.ToString ());
        }

        public static void Error (string message)
        {
            Console.WriteLine (message);
        }

        public static void Error (object message)
        {
            Console.WriteLine (message);
        }

        public static void Debug (string message)
        {
            Console.WriteLine (message);
        }

        public static void Debug (object message)
        {
            Console.WriteLine (message);
        }

        public static void Message (string message)
        {
            Console.WriteLine (message);
        }

        public static void Message (object message)
        {
            Console.WriteLine (message);
        }


    }
}
