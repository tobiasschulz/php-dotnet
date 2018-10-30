﻿using System;
using System.Threading.Tasks;

namespace PHP.Standard
{
    public static class Events
    {
        public static void Empty ()
        {
        }

        public static void Empty<T> (T value)
        {
        }

        public static void Empty<T1, T2> (T1 value1, T2 value2)
        {
        }

        public static void Empty<T1, T2, T3> (T1 value1, T2 value2, T3 value3)
        {
        }

        public static void Empty<T1, T2, T3, T4> (T1 value1, T2 value2, T3 value3, T4 value4)
        {
        }

        public static void Empty<T1, T2, T3, T4, T5> (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
        }

        public static Task EmptyTask ()
        {
            return Tasks.Completed;
        }

        public static Task EmptyTask<T> (T value)
        {
            return Tasks.Completed;
        }

        public static Task EmptyTask<T1, T2> (T1 value1, T2 value2)
        {
            return Tasks.Completed;
        }

        public static Task EmptyTask<T1, T2, T3> (T1 value1, T2 value2, T3 value3)
        {
            return Tasks.Completed;
        }

        public static Task EmptyTask<T1, T2, T3, T4> (T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return Tasks.Completed;
        }

        public static Task EmptyTask<T1, T2, T3, T4, T5> (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            return Tasks.Completed;
        }

        public static T Identity<T> (T value)
        {
            return value;
        }

        public static T0 Default<T0> ()
        {
            return default;
        }

        public static T0 Default<T1, T0> (T1 value1)
        {
            return default;
        }
    }
}
