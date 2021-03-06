﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class echo : ManagedFunction
    {
        public echo ()
            : base ("echo")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            StringBuilder sb = new StringBuilder ();

            foreach (EvaluatedParameter p in parameters)
            {
                string s = p.EvaluatedValue.GetStringValue ();
                sb.Append (s);
                function_scope.Root.Context.Console.Out.Write (s);
            }

            return new Result (new StringExpression (sb.ToString ()));
        }
    }
}
