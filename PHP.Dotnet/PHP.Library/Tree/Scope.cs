using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Library;
using PHP.Library.TypeSystem;

namespace PHP.Tree
{
    public abstract class Scope
    {
        protected Scope ()
        {
        }

        public abstract Scope Parent { get; }
        public abstract RootScope Root { get; }
        public abstract IVariableCollection Variables { get; }
        public abstract string ScopeName { get; }

        public override string ToString ()
        {
            return $"[Scope: {ScopeName}]";
        }

        public void FindNearestScope<TScope> (Action<TScope> a)
        {
            Scope s = this;
            while (s != null)
            {
                if (s is TScope desired_scope)
                {
                    a (desired_scope);
                    break;
                }
                s = s.Parent;
            }
        }
    }

    public sealed class RootScope : Scope
    {
        private readonly Context _context;
        private readonly IFunctionCollection _functions;
        private readonly IVariableCollection _variables;
        private readonly IClassCollection _classes;
        private readonly IObjectCollection _objects;

        public RootScope (Context context)
        {
            _context = context;
            _functions = new FunctionCollection ();
            _variables = new VariableCollection ();
            _classes = new ClassCollection ();
            _objects = new ObjectCollection ();

            StandardLibrary.Populate (_functions);
            StandardLibrary.Populate (_variables);
        }

        public Context Context => _context;
        public IFunctionCollection Functions => _functions;
        public IClassCollection Classes => _classes;
        public IObjectCollection Objects => _objects;
        public override Scope Parent => null;
        public override RootScope Root => this;
        public override IVariableCollection Variables => _variables;
        public override string ScopeName => "root";
    }

    public sealed class ScriptScope : Scope
    {
        private readonly Scope _parentscope;
        private readonly IScript _script;
        private readonly IVariableCollection _variables;

        public ScriptScope (Scope parentscope, IScript script)
        {
            _parentscope = parentscope;
            _script = script;
            _variables = new MergedVariableCollection (
                collection_parent: _parentscope.Variables,
                collection_own: new VariableCollection ()
            );
        }

        public IScript Script => _script;
        public override Scope Parent => _parentscope;
        public override RootScope Root => _parentscope.Root;
        public override IVariableCollection Variables => _variables;
        public override string ScopeName => "script";
    }

    public sealed class FunctionScope : Scope
    {
        private readonly Scope _parentscope;
        private readonly IFunction _function;
        private readonly IVariableCollection _variables;

        public FunctionScope (Scope parentscope, IFunction function)
        {
            _parentscope = parentscope;
            _function = function;
            _variables = new MergedVariableCollection (
                collection_parent: _parentscope.Variables,
                collection_own: new VariableCollection ()
            );
        }

        public IFunction Function => _function;
        public override Scope Parent => _parentscope;
        public override RootScope Root => _parentscope.Root;
        public override IVariableCollection Variables => _variables;
        public override string ScopeName => $"function: {_function.Name}";
    }

}
