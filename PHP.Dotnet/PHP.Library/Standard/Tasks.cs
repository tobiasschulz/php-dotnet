using System;
using System.Threading.Tasks;

namespace PHP.Standard
{
    public static class Tasks
    {
        public static readonly Task Completed = Task.Delay (0);

        public static void Forget (this Task task)
        {
            task.ContinueWith ((t) =>
            {
                if (t.Exception != null)
                {
                    Log.Error ($"Error in Fire-and-forget Task: {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void Forget<T> (this Task<T> task)
        {
            task.ContinueWith ((t) =>
            {
                if (t.Exception != null)
                {
                    Log.Error ($"Error in Fire-and-forget Task: {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void Finally (this Task task, Action onFinally)
        {
            task.ContinueWith ((t) =>
            {
                if (t.Exception != null)
                {
                    Log.Error ($"Error in Finally() Task: {t.Exception}");
                }
                onFinally ();
            });
        }

        public static void Finally<T> (this Task<T> task, Action onFinally)
        {
            task.ContinueWith ((t) =>
            {
                if (t.Exception != null)
                {
                    Log.Error ($"Error in Finally() Task: {t.Exception}");
                }
                onFinally ();
            });
        }
    }
}
