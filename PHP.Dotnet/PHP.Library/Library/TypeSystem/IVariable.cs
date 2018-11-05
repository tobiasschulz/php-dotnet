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
        IVariable EnsureExists (NameOfVariable name);
        new FinalExpression this [NameOfVariable key] { get; set; }
    }

    public sealed class MergedVariableCollection : MergedElementCollection<NameOfVariable, IVariable>, IVariableCollection, IReadOnlyVariableCollection
    {
        public MergedVariableCollection (IReadOnlyVariableCollection collection_parent, IVariableCollection collection_own)
            : base (collection_parent, collection_own)
        {
        }

        IVariable IVariableCollection.EnsureExists (NameOfVariable name)
        {
            if (!_collection_parent.TryGetValue (name, out var res))
            {
                res = ((IVariableCollection)_collection_own).EnsureExists (name);
            }
            return res;
        }

        FinalExpression IVariableCollection.this [NameOfVariable key]
        {
            get => ((IVariableCollection)this).EnsureExists (key).Value;
            set => ((IVariableCollection)this).EnsureExists (key).Value = value;
        }
    }

    public sealed class VariableCollection : ElementCollection<NameOfVariable, IVariable>, IVariableCollection, IReadOnlyVariableCollection
    {
        public IVariable EnsureExists (NameOfVariable name)
        {
            if (!((IVariableCollection)this).TryGetValue (name, out var res))
            {
                res = new Variable (name);
                ((IVariableCollection)this).Add (res);
            }
            return res;
        }

        FinalExpression IVariableCollection.this [NameOfVariable key]
        {
            get => ((IVariableCollection)this).EnsureExists (key).Value;
            set => ((IVariableCollection)this).EnsureExists (key).Value = value;
        }
    }
}
