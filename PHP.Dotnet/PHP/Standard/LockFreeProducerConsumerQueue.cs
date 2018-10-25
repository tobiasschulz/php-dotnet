using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PHP.Standard
{
    public sealed class LockFreeProducerConsumerQueue
    {
        private Action _queue;

        public void Enqueue (Action message)
        {
            while (true)
            {
                var expected_old_queue = _queue;
                var new_queue = expected_old_queue + message;
                var actual_old_queue = Interlocked.CompareExchange (ref _queue, new_queue, expected_old_queue);

                if (expected_old_queue == actual_old_queue)
                    break;
            }
        }

        public void Process ()
        {
            var current_queue = Interlocked.Exchange (ref _queue, null);
            current_queue?.Invoke ();
        }
    }
}
