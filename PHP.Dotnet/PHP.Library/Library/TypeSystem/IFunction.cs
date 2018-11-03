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
        Name Name { get; }
        Result Execute (EvaluatedCallSignature call_signature, Tree.Scope scope);
    }

    public sealed class FunctionCollection
    {
        private ImmutableArray<IFunction> _data = ImmutableArray<IFunction>.Empty;

        public FunctionCollection ()
        {
        }

        public bool TryGetValue (Name name, out IFunction res)
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

        public bool Contains (Name name)
        {
            return TryGetValue (name, out var dummy);
        }

        internal ImmutableArray<IFunction> GetAll ()
        {
            return _data;
        }

        public void Add (IFunction value)
        {
            if (value == null) return;

            if (TryGetValue (value.Name, out var existing_value))
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

            if (TryGetValue (value.Name, out var existing_value))
            {
                _data = _data.Remove (existing_value);
            }

            _data = _data.Add (value);
        }

    }
}
