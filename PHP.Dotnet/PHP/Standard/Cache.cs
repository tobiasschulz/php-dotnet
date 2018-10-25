using System;
using System.Collections.Generic;
using System.Text;

namespace PHP.Standard
{
    public sealed class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue> ();

        public int MaxCount { get; set; } = 0;

        public TValue Get (TKey key, Func<TKey, TValue> _func)
        {
            if (_cache.TryGetValue (key, out TValue result))
            {
                return result;
            }
            else
            {
                int max_count = MaxCount;
                if (max_count != 0 && _cache.Count > max_count)
                {
                    _cache.Clear ();
                }

                result = _func (key);
                if (result != default) _cache [key] = result;
                return result;
            }
        }

        public TValue Get (TKey key, Func<TValue> _func)
        {
            if (_cache.TryGetValue (key, out TValue result))
            {
                return result;
            }
            else
            {
                int max_count = MaxCount;
                if (max_count != 0 && _cache.Count > max_count)
                {
                    _cache.Clear ();
                }

                result = _func ();
                if (result != default) _cache [key] = result;
                return result;
            }
        }

        public void Clear ()
        {
            _cache.Clear ();
        }
    }

    public sealed class ConcurrentCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue> ();
        private readonly object _cache_lock = new object ();

        public int MaxCount { get; set; } = 0;

        public TValue Get (TKey key, Func<TKey, TValue> _func)
        {
            lock (_cache_lock)
            {
                if (_cache.TryGetValue (key, out TValue result))
                {
                    return result;
                }
                else
                {
                    int max_count = MaxCount;
                    if (max_count != 0 && _cache.Count > max_count)
                    {
                        _cache.Clear ();
                    }

                    result = _func (key);
                    if (result != default) _cache [key] = result;
                    return result;
                }
            }
        }

        public TValue Get (TKey key, Func<TValue> _func)
        {
            lock (_cache_lock)
            {
                if (_cache.TryGetValue (key, out TValue result))
                {
                    return result;
                }
                else
                {
                    int max_count = MaxCount;
                    if (max_count != 0 && _cache.Count > max_count)
                    {
                        _cache.Clear ();
                    }

                    result = _func ();
                    if (result != default) _cache [key] = result;
                    return result;
                }
            }
        }

        public void Clear ()
        {
            lock (_cache_lock)
            {
                _cache.Clear ();
            }
        }
    }
}
