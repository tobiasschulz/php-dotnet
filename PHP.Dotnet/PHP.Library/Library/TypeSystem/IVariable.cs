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
    public interface IVariable : IElement<NameOfVariable>
    {
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

    public interface IReadOnlyVariableCollection : IReadOnlyElementCollection<NameOfVariable, IVariable>
    {
    }

    public interface IVariableCollection : IReadOnlyVariableCollection, IElementCollection<NameOfVariable, IVariable>
    {
        void EnsureExists (NameOfVariable name, out IVariable res);
    }

    public sealed class MergedVariableCollection : MergedElementCollection<NameOfVariable, IVariable>, IVariableCollection, IReadOnlyVariableCollection
    {
        public MergedVariableCollection (IReadOnlyVariableCollection collection_parent, IVariableCollection collection_own)
            : base (collection_parent, collection_own)
        {
        }

        void IVariableCollection.EnsureExists (NameOfVariable name, out IVariable res)
        {
            if (!_collection_parent.TryGetValue (name, out res))
            {
                ((IVariableCollection)_collection_own).EnsureExists (name, out res);
            }
        }
    }

    public sealed class VariableCollection : ElementCollection<NameOfVariable, IVariable>, IVariableCollection, IReadOnlyVariableCollection
    {
        public void EnsureExists (NameOfVariable name, out IVariable res)
        {
            if (!((IVariableCollection)this).TryGetValue (name, out res))
            {
                res = new Variable (name);
                ((IVariableCollection)this).Add (res);
            }
        }

    }
}
