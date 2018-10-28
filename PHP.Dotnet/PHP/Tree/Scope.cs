using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Library;

namespace PHP.Tree
{
    public abstract class Scope
    {
        protected Scope ()
        {
        }

        public abstract RootScope Root { get; }
        public abstract string GetScopeName ();

        public override string ToString ()
        {
            return $"[Scope: '{GetScopeName ()}']";
        }
    }

    public sealed class RootScope : Scope
    {
        private readonly Context _context;
        private readonly FunctionCollection _globalfunctions;

        public RootScope (Context context)
        {
            _context = context;
            _globalfunctions = new FunctionCollection ();

            StandardLibrary.Populate (_globalfunctions);
        }

        public override RootScope Root
        {
            get => this;
        }

        public Context Context
        {
            get => _context;
        }

        public FunctionCollection GlobalFunctions
        {
            get => _globalfunctions;
        }

        public override string GetScopeName ()
        {
            return "root";
        }
    }

    public sealed class ScriptScope : Scope
    {
        private readonly Scope _parentscope;

        public ScriptScope (Scope value)
        {
            _parentscope = value;
        }

        public override RootScope Root
        {
            get => _parentscope.Root;
        }

        public override string GetScopeName ()
        {
            return "script";
        }
    }

    public sealed class FunctionScope : Scope
    {
        private readonly Scope _parentscope;
        private readonly IFunctionDeclaration _function;

        public FunctionScope (Scope value, IFunctionDeclaration function)
        {
            _parentscope = value;
            _function = function;
        }

        public override RootScope Root
        {
            get => _parentscope.Root;
        }

        public override string GetScopeName ()
        {
            return $"function: {_function.Name}";
        }
    }

}
