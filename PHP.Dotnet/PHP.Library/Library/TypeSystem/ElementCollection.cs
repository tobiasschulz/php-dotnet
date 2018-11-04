using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.TypeSystem
{
    public interface IElement<TName> where TName : struct
    {
        TName Name { get; }
    }

    public interface IReadOnlyElementCollection<TName, TElement> where TName : struct where TElement : class, IElement<TName>
    {
        bool TryGetValue (TName name, out TElement res);
        bool Contains (TName name);
        IEnumerable<TElement> GetAll ();
    }

    public interface IElementCollection<TName, TElement> : IReadOnlyElementCollection<TName, TElement> where TName : struct where TElement : class, IElement<TName>
    {
        void Add (TElement value);
    }

    public class MergedElementCollection<TName, TElement> : IElementCollection<TName, TElement> where TName : struct where TElement : class, IElement<TName>
    {
        protected readonly IReadOnlyElementCollection<TName, TElement> _collection_parent;
        protected readonly IElementCollection<TName, TElement> _collection_own;

        public MergedElementCollection (IReadOnlyElementCollection<TName, TElement> collection_parent, IElementCollection<TName, TElement> collection_own)
        {
            _collection_parent = collection_parent;
            _collection_own = collection_own;
        }

        bool IReadOnlyElementCollection<TName, TElement>.TryGetValue (TName name, out TElement res)
        {
            return _collection_own.TryGetValue (name, out res) || _collection_parent.TryGetValue (name, out res);
        }
        
        bool IReadOnlyElementCollection<TName, TElement>.Contains (TName name)
        {
            return _collection_own.Contains (name) || _collection_parent.Contains (name);
        }

        IEnumerable<TElement> IReadOnlyElementCollection<TName, TElement>.GetAll ()
        {
            return _collection_own.GetAll ().Concat (_collection_parent.GetAll ());
        }

        void IElementCollection<TName, TElement>.Add (TElement value)
        {
            if (value == null) return;

            if (_collection_own.TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add method {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _collection_own.Add (value);
            }
        }

    }

    public class ElementCollection<TName, TElement> : IElementCollection<TName, TElement> where TName : struct where TElement : class, IElement<TName>
    {
        private ImmutableArray<TElement> _data = ImmutableArray<TElement>.Empty;

        public ElementCollection ()
        {
        }

        bool IReadOnlyElementCollection<TName, TElement>.TryGetValue (TName name, out TElement res)
        {
            foreach (TElement value in _data)
            {
                if (value.Name.Equals (name))
                {
                    res = value;
                    return true;
                }
            }
            res = null;
            return false;
        }

        bool IReadOnlyElementCollection<TName, TElement>.Contains (TName name)
        {
            return ((IReadOnlyElementCollection<TName, TElement>)this).TryGetValue (name, out var dummy);
        }

        IEnumerable<TElement> IReadOnlyElementCollection<TName, TElement>.GetAll ()
        {
            return _data;
        }

        void IElementCollection<TName, TElement>.Add (TElement value)
        {
            if (value == null) return;

            if (((IReadOnlyElementCollection<TName, TElement>)this).TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _data = _data.Add (value);
            }
        }

    }
}
