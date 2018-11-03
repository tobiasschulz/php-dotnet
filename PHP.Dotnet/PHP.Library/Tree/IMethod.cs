using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Tree
{
    public interface IMethod
    {
        Name Name { get; }
        Result Execute (EvaluatedCallSignature call_signature, Scope scope);
    }

    public sealed class MethodCollection
    {
        private ImmutableArray<IMethod> _data = ImmutableArray<IMethod>.Empty;

        public MethodCollection ()
        {
        }

        public bool TryGetValue (Name name, out IMethod res)
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

        public bool Contains (Name name)
        {
            return TryGetValue (name, out var dummy);
        }

        internal ImmutableArray<IMethod> GetAll ()
        {
            return _data;
        }

        public void Add (IMethod value)
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

        public void Replace (IMethod value)
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
