using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Execution;
using PHP.Library;
using PHP.Library.TypeSystem;
using PHP.Standard;

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
        public string StackTrace => _formatStackTrace ();

        private string _formatStackTrace ()
        {
            List<string> l = new List<string> ();
            Scope s = this;
            while (s != null)
            {
                if (s is MethodScope method_scope)
                {
                    l.Add ($"   at {method_scope.Classes.First ().Name}::{method_scope.Method.Name} ({method_scope.Signature.Parameters.Select (p => p.EvaluatedValue).Join (", ")})");
                }
                else if (s is FunctionScope function_scope)
                {
                    l.Add ($"   at global::{function_scope.Function.Name} ({function_scope.Signature.Parameters.Select (p => p.EvaluatedValue).Join (", ")})");
                }
                s = s.Parent;
            }
            return l.Join ("\n");
        }

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

    public interface IFunctionLikeScope
    {
        ScriptScope GetDeclarationScopeOfFunction ();
        EvaluatedSignature Signature { get; }
    }

    public sealed class RootScope : Scope
    {
        private readonly Context _context;
        private readonly IFunctionCollection _functions;
        private readonly IVariableCollection _variables;
        private readonly IClassCollection _classes;
        private readonly IObjectCollection _objects;
        private readonly IArrayCollection _arrays;

        public RootScope (Context context)
        {
            _context = context;
            _functions = new FunctionCollection ();
            _variables = new VariableCollection ();
            _classes = new ClassCollection ();
            _objects = new ObjectCollection ();
            _arrays = new ArrayCollection ();

            StandardLibrary.Populate (_functions);
            StandardLibrary.Populate (_variables);
        }

        public Context Context => _context;
        public IFunctionCollection Functions => _functions;
        public IClassCollection Classes => _classes;
        public IObjectCollection Objects => _objects;
        public IArrayCollection Arrays => _arrays;
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

    public sealed class FunctionScope : Scope, IFunctionLikeScope
    {
        private readonly Scope _parentscope;
        private readonly IFunction _function;
        private readonly IVariableCollection _variables;
        private readonly EvaluatedSignature _signature;

        public FunctionScope (Scope parentscope, IFunction function, EvaluatedSignature signature)
        {
            _parentscope = parentscope;
            _function = function;
            _signature = signature;
            _variables = new MergedVariableCollection (
                collection_parent: _parentscope.Variables,
                collection_own: new VariableCollection ()
            );
        }

        public IFunction Function => _function;
        public EvaluatedSignature Signature => _signature;
        public override Scope Parent => _parentscope;
        public override RootScope Root => _parentscope.Root;
        public override IVariableCollection Variables => _variables;
        public override string ScopeName => $"function: {_function.Name}";
        public ScriptScope GetDeclarationScopeOfFunction () => _function.GetDeclarationScope ();
    }

    public sealed class MethodScope : Scope, IFunctionLikeScope
    {
        private readonly Scope _parentscope;
        private readonly IMethod _method;
        private readonly IObject _object;
        private readonly IVariableCollection _variables;
        private readonly IReadOnlyList<IClass> _classes;
        private readonly EvaluatedSignature _signature;

        public MethodScope (Scope parentscope, IMethod method, EvaluatedSignature signature, IObject obj, IReadOnlyList<IClass> classes)
        {
            _parentscope = parentscope;
            _method = method;
            _signature = signature;
            _object = obj;
            _classes = classes;
            _variables = new MergedVariableCollection (
                collection_parent: new MergedVariableCollection (
                    collection_parent: _parentscope.Root.Variables,
                    collection_own: _object?.Variables
                ),
                collection_own: new VariableCollection ()
            );
        }

        public IMethod Method => _method;
        public EvaluatedSignature Signature => _signature;
        public IObject Object => _object;
        public IReadOnlyList<IClass> Classes => _classes;
        public override Scope Parent => _parentscope;
        public override RootScope Root => _parentscope.Root;
        public override IVariableCollection Variables => _variables;
        public override string ScopeName => $"method: {_method.Name}, object: {_object}";
        public ScriptScope GetDeclarationScopeOfFunction () => _method.GetDeclarationScope ();
    }

}
