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
    public interface IFunction
    {
        NameOfFunction Name { get; }
        Result Execute (EvaluatedCallSignature call_signature, Tree.Scope scope);
        ScriptScope GetDeclarationScope ();
    }

    public interface IReadOnlyFunctionCollection
    {
        bool TryGetValue (NameOfFunction name, out IFunction res);
        bool Contains (NameOfFunction name);
        IEnumerable<IFunction> GetAll ();
    }

    public interface IFunctionCollection : IReadOnlyFunctionCollection
    {
        void Add (IFunction value);
    }

    public sealed class FunctionCollection : IFunctionCollection
    {
        private ImmutableArray<IFunction> _data = ImmutableArray<IFunction>.Empty;

        public FunctionCollection ()
        {
        }

        bool IReadOnlyFunctionCollection.TryGetValue (NameOfFunction name, out IFunction res)
        {
            foreach (IFunction value in _data)
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

        bool IReadOnlyFunctionCollection.Contains (NameOfFunction name)
        {
            return ((IReadOnlyFunctionCollection)this).TryGetValue (name, out var dummy);
        }

        IEnumerable<IFunction> IReadOnlyFunctionCollection.GetAll ()
        {
            return _data;
        }

        void IFunctionCollection.Add (IFunction value)
        {
            if (value == null) return;

            if (((IReadOnlyFunctionCollection)this).TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add function {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _data = _data.Add (value);
            }
        }

        public void Replace (IFunction value)
        {
            if (value == null) return;

            if (((IReadOnlyFunctionCollection)this).TryGetValue (value.Name, out var existing_value))
            {
                _data = _data.Remove (existing_value);
            }

            _data = _data.Add (value);
        }

    }
}
