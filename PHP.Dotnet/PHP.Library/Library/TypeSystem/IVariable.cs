using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.TypeSystem
{
    public interface IVariable
    {
        VariableName Name { get; }
        FinalExpression Value { get; set; }
    }

    public sealed class Variable : IVariable
    {
        private readonly VariableName _name;
        private FinalExpression _value;

        public Variable (VariableName name)
        {
            _name = name;
            _value = new NullExpression ();
        }

        public VariableName Name
        {
            get => _name;
        }

        public FinalExpression Value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }
    }

    public interface IVariableCollection
    {
        bool TryGetValue (VariableName name, out IVariable res);
        void EnsureExists (VariableName name, out IVariable res);
        bool Contains (VariableName name);
        IEnumerable<IVariable> GetAll ();
        void Add (IVariable value, bool replace = true);
    }

    public sealed class MergedVariableCollection : IVariableCollection
    {
        private readonly IVariableCollection _collection_parent;
        private readonly IVariableCollection _collection_own;

        public MergedVariableCollection (IVariableCollection collection_readonly, IVariableCollection collection_editable)
        {
            _collection_parent = collection_readonly;
            _collection_own = collection_editable;
        }

        bool IVariableCollection.TryGetValue (VariableName name, out IVariable res)
        {
            return _collection_own.TryGetValue (name, out res) || _collection_parent.TryGetValue (name, out res);
        }

        void IVariableCollection.EnsureExists (VariableName name, out IVariable res)
        {
            if (_collection_parent.Contains (name))
            {
                _collection_parent.EnsureExists (name, out res);
            }
            else
            {
                _collection_own.EnsureExists (name, out res);
            }
        }
        bool IVariableCollection.Contains (VariableName name)
        {
            return _collection_own.Contains (name) || _collection_parent.Contains (name);
        }

        void IVariableCollection.Add (IVariable value, bool replace)
        {
            if (value == null) return;

            if (_collection_parent.Contains (value.Name))
            {
                _collection_parent.Add (value, replace: replace);
            }
            else
            {
                _collection_own.Add (value, replace: replace);
            }
        }

        IEnumerable<IVariable> IVariableCollection.GetAll ()
        {
            return _collection_own.GetAll ().Concat (_collection_parent.GetAll ());
        }
    }

    public sealed class VariableCollection : IVariableCollection
    {
        private ImmutableArray<IVariable> _data = ImmutableArray<IVariable>.Empty;

        public VariableCollection ()
        {
        }

        public bool TryGetValue (VariableName name, out IVariable res)
        {
            foreach (IVariable value in _data)
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

        public void EnsureExists (VariableName name, out IVariable res)
        {
            if (!TryGetValue (name, out res))
            {
                res = new Variable (name);
                Add (res);
            }
        }

        public bool Contains (VariableName name)
        {
            return TryGetValue (name, out var dummy);
        }

        public IEnumerable<IVariable> GetAll ()
        {
            return _data;
        }

        public void Add (IVariable value, bool replace = true)
        {
            if (value == null) return;

            if (TryGetValue (value.Name, out var existing_value))
            {
                if (replace)
                {
                    _data = _data.Remove (existing_value);
                    _data = _data.Add (value);
                }
                else
                {
                    Log.Error ($"Cannot add function {value.Name}: already exists: {existing_value} vs {value}");
                }
            }
            else
            {
                _data = _data.Add (value);
            }
        }
    }
}
