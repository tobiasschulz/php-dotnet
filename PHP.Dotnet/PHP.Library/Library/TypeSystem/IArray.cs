﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.TypeSystem
{
    public static class ArrayExtensions
    {
        public static ArrayId LastestArrayId = 0;
    }

    public enum ArrayId
    {
    }

    public interface IArray : IElement<ArrayId>
    {
        bool TryGetValue (ArrayKey key, out ArrayItem res);
        bool Contains (ArrayKey key);
        IEnumerable<ArrayItem> GetAll ();
        void Set (ArrayItem item);
        FinalExpression this [ArrayKey key] { get; set; }
        FinalExpression AsExpression { get; }
        ArrayItem Shift ();
        void Clear ();
    }

    public interface IReadOnlyArrayCollection : IReadOnlyElementCollection<ArrayId, IArray>
    {
    }

    public interface IArrayCollection : IReadOnlyArrayCollection, IElementCollection<ArrayId, IArray>
    {
    }

    public sealed class ArrayCollection : ElementCollection<ArrayId, IArray>, IArrayCollection, IReadOnlyArrayCollection
    {
    }

    public sealed class ArrayItem
    {
        public readonly ArrayKey Key;
        public readonly FinalExpression Value;

        public ArrayItem (ArrayKey key, FinalExpression value)
        {
            Key = key;
            Value = value;
        }
    }

    public sealed class ArrayStructure : IArray
    {
        private readonly ArrayId _id;
        private readonly HashSet<ArrayKey> _keys = new HashSet<ArrayKey> ();
        private readonly List<ArrayItem> _data = new List<ArrayItem> ();
        private long _highest_integer_key = -1;

        public ArrayStructure ()
        {
            _id = ++ArrayExtensions.LastestArrayId;
        }

        ArrayId IElement<ArrayId>.Name => _id;

        public bool TryGetValue (ArrayKey key, out ArrayItem res)
        {
            if (_keys.Contains (key))
            {
                foreach (ArrayItem value in _data)
                {
                    if (value.Key.Equals (key))
                    {
                        Log.Debug ($"get array value: arr = {_id}, key = {key}, value = {value}");
                        res = value;
                        return true;
                    }
                }
            }
            res = null;
            return false;
        }

        public bool Contains (ArrayKey key)
        {
            return _keys.Contains (key);
        }

        public IEnumerable<ArrayItem> GetAll ()
        {
            return _data;
        }

        public void Set (ArrayItem item)
        {
            if (item == null) return;

            ArrayKey key = item.Key;

            if (key.Value.Length == 0)
            {
                key = new ArrayKey ((_highest_integer_key + 1).ToString ());
                item = new ArrayItem (key, item.Value);
            }

            if (TryGetValue (key, out ArrayItem item_old))
            {
                _data.Remove (item_old);
            }

            _data.Add (item);
            _keys.Add (key);

            Log.Debug ($"set array value: arr = {_id}, key = {item.Key}, value = {item.Value}");

            if (key.IsDigitsOnly ())
            {
                long k = key.Value.ToLong ();
                _highest_integer_key = k > _highest_integer_key ? k : _highest_integer_key;
            }
        }

        public ArrayItem Shift ()
        {
            if (_data.Count == 0)
            {
                return null;
            }

            ArrayItem first = _data [0];
            var old_remaining = _data.Skip (1).ToArray ();
            Clear ();
            foreach (var old_item in old_remaining)
            {
                ArrayKey key = old_item.Key;
                FinalExpression value = old_item.Value;
                if (key.IsDigitsOnly ())
                {
                    key = new ArrayKey ((_highest_integer_key + 1).ToString ());
                }

                _data.Add (new ArrayItem (key, value));
                _keys.Add (key);
            }
            return first;
        }

        public void Clear ()
        {
            _data.Clear ();
            _keys.Clear ();
            _highest_integer_key = -1;
        }

        public FinalExpression this [ArrayKey key]
        {
            get
            {
                return TryGetValue (key, out var item) ? item.Value : new NullExpression ();
            }
            set
            {
                Set (new ArrayItem (key, value));
            }
        }

        public FinalExpression AsExpression
        {
            get => new ArrayPointerExpression (this);
        }

    }
}
