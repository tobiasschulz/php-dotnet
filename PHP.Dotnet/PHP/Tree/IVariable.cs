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
        private readonly IVariableCollection _collection_readonly;
        private readonly IVariableCollection _collection_editable;

        public MergedVariableCollection (IVariableCollection collection_readonly, IVariableCollection collection_editable)
        {
            _collection_readonly = collection_readonly;
            _collection_editable = collection_editable;
        }

        bool IVariableCollection.TryGetValue (VariableName name, out IVariable res)
        {
            return _collection_editable.TryGetValue (name, out res) || _collection_readonly.TryGetValue (name, out res);
        }

        void IVariableCollection.EnsureExists (VariableName name, out IVariable res)
        {
            if (_collection_readonly.Contains (name))
            {
                _collection_readonly.EnsureExists (name, out res);
            }
            else
            {
                _collection_editable.EnsureExists (name, out res);
            }
        }
        bool IVariableCollection.Contains (VariableName name)
        {
            return _collection_editable.Contains (name) || _collection_readonly.Contains (name);
        }

        void IVariableCollection.Add (IVariable value, bool replace)
        {
            if (value == null) return;

            if (_collection_readonly.Contains (value.Name))
            {
                _collection_readonly.Add (value, replace: replace);
            }
            else
            {
                _collection_editable.Add (value, replace: replace);
            }
        }

        IEnumerable<IVariable> IVariableCollection.GetAll ()
        {
            return _collection_editable.GetAll ().Concat (_collection_readonly.GetAll ());
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
                }
                else
                {
                    Log.Error ($"Cannot add function {value.Name}: already exists: {existing_value} vs {value}");
                    return;
                }
            }
            else
            {
                _data = _data.Add (value);
            }
        }
    }
}
