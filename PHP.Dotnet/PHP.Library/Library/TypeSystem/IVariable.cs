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
    public interface IVariable
    {
        NameOfVariable Name { get; }
        FinalExpression Value { get; set; }
    }

    public sealed class Variable : IVariable
    {
        private readonly NameOfVariable _name;
        private FinalExpression _value;

        public Variable (NameOfVariable name)
        {
            _name = name;
            _value = new NullExpression ();
        }

        public NameOfVariable Name
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
        bool TryGetValue (NameOfVariable name, out IVariable res);
        void EnsureExists (NameOfVariable name, out IVariable res);
        bool Contains (NameOfVariable name);
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

        bool IVariableCollection.TryGetValue (NameOfVariable name, out IVariable res)
        {
            return _collection_own.TryGetValue (name, out res) || _collection_parent.TryGetValue (name, out res);
        }

        void IVariableCollection.EnsureExists (NameOfVariable name, out IVariable res)
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
        bool IVariableCollection.Contains (NameOfVariable name)
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

        public bool TryGetValue (NameOfVariable name, out IVariable res)
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

        public void EnsureExists (NameOfVariable name, out IVariable res)
        {
            if (!TryGetValue (name, out res))
            {
                res = new Variable (name);
                Add (res);
            }
        }

        public bool Contains (NameOfVariable name)
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
