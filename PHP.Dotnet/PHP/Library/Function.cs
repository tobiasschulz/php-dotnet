using System;
using System.Collections.Generic;
using System.Text;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library
{
    public abstract class Function : IFunctionDeclaration
    {
        private readonly Name _name;

        protected Function (Name name)
        {
            _name = name;
        }

        public Name Name => _name;

        protected abstract Result _execute (CallSignature call_signature, FunctionScope function_scope);

        Name IFunctionDeclaration.Name => Name;

        Result IFunctionDeclaration.Execute (CallSignature call_signature, Scope scope)
        {
            FunctionScope function_scope = new FunctionScope (scope, this);
            return _execute (call_signature, function_scope);
        }
    }
}
