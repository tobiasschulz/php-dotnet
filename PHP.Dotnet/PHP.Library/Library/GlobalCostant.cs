using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Tree;

namespace PHP.Library
{
    public abstract class GlobalConstant : IVariable
    {
        private readonly VariableName _name;

        protected GlobalConstant (VariableName name)
        {
            _name = name;
        }

        public VariableName Name => _name;
        public FinalExpression Value => _getValue ();

        internal abstract FinalExpression _getValue ();

        VariableName IVariable.Name => _name;
        FinalExpression IVariable.Value
        {
            get => _getValue ();
#pragma warning disable RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
            set { }
#pragma warning restore RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
        }
    }

}
