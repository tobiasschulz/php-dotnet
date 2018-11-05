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
    public abstract class ManagedFunction : IFunction
    {
        private readonly NameOfFunction _name;

        protected ManagedFunction (NameOfFunction name)
        {
            _name = name;
        }

        public NameOfFunction Name => _name;

        protected abstract Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope);

        NameOfFunction IFunction.Name => Name;

        Result IFunction.Execute (EvaluatedCallSignature call_signature, Scope scope)
        {
            FunctionScope function_scope = new FunctionScope (scope, this);
            try
            {
                return _execute (call_signature.Parameters, function_scope);
            }
            catch (StandardLibraryUsageException ex)
            {
                Log.Error (ex.Message);
                return Result.NULL;
            }
        }

        ScriptScope IFunction.GetDeclarationScope ()
        {
            return null;
        }
    }

}
