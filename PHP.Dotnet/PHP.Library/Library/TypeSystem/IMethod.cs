using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Library.TypeSystem
{
    public interface IMethod
    {
        MethodName Name { get; }
        Result Execute (EvaluatedCallSignature call_signature, Scope scope);
    }
    
    public interface IReadOnlyMethodCollection
    {
        bool TryGetValue (MethodName name, out IMethod res);
        bool Contains (MethodName name);
        IEnumerable<IMethod> GetAll ();
    }

    public interface IMethodCollection : IReadOnlyMethodCollection
    {
        void Add (IMethod value);
    }

    public sealed class MergedMethodCollection : IMethodCollection, IReadOnlyMethodCollection
    {
        private readonly IMethodCollection _collection_parent;
        private readonly IMethodCollection _collection_own;

        public MergedMethodCollection (IMethodCollection collection_readonly, IMethodCollection collection_editable)
        {
            _collection_parent = collection_readonly;
            _collection_own = collection_editable;
        }

        bool IReadOnlyMethodCollection.TryGetValue (MethodName name, out IMethod res)
        {
            return _collection_own.TryGetValue (name, out res) || _collection_parent.TryGetValue (name, out res);
        }
        
        bool IReadOnlyMethodCollection.Contains (MethodName name)
        {
            return _collection_own.Contains (name) || _collection_parent.Contains (name);
        }

        IEnumerable<IMethod> IReadOnlyMethodCollection.GetAll ()
        {
            return _collection_own.GetAll ().Concat (_collection_parent.GetAll ());
        }

        void IMethodCollection.Add (IMethod value)
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

    public sealed class MethodCollection : IMethodCollection, IReadOnlyMethodCollection
    {
        private ImmutableArray<IMethod> _data = ImmutableArray<IMethod>.Empty;

        public MethodCollection ()
        {
        }

        bool IReadOnlyMethodCollection.TryGetValue (MethodName name, out IMethod res)
        {
            foreach (IMethod value in _data)
            {
                if (value.Name == name)
                {
                    res = value;
                    return true;
                }
            }
            res = null;
            return false;
        }

        bool IReadOnlyMethodCollection.Contains (MethodName name)
        {
            return ((IReadOnlyMethodCollection)this).TryGetValue (name, out var dummy);
        }

        IEnumerable<IMethod> IReadOnlyMethodCollection.GetAll ()
        {
            return _data;
        }

        void IMethodCollection.Add (IMethod value)
        {
            if (value == null) return;

            if (((IReadOnlyMethodCollection)this).TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add method {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _data = _data.Add (value);
            }
        }

    }
}
