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
    public interface IMethod : IElement<NameOfMethod>
    {
        Result Execute (IObject obj, IReadOnlyList<IClass> classes, EvaluatedSignature call_signature, Scope scope);
        ScriptScope GetDeclarationScope ();
        DeclarationSignature DeclarationSignature { get; }
    }
    
    public interface IReadOnlyMethodCollection : IReadOnlyElementCollection<NameOfMethod, IMethod>
    {
    }

    public interface IMethodCollection : IReadOnlyMethodCollection, IElementCollection<NameOfMethod, IMethod>
    {
    }

    public sealed class MergedReadOnlyMethodCollection : MergedReadOnlyElementCollection<NameOfMethod, IMethod>, IReadOnlyMethodCollection
    {
        public MergedReadOnlyMethodCollection (IReadOnlyElementCollection<NameOfMethod, IMethod> collection_parent, IReadOnlyElementCollection<NameOfMethod, IMethod> collection_own)
            : base (collection_parent, collection_own)
        {
        }
    }

    public sealed class MethodCollection : ElementCollection<NameOfMethod, IMethod>, IMethodCollection, IReadOnlyMethodCollection
    {
        public static readonly IReadOnlyMethodCollection Empty = new MethodCollection ();

        internal static IReadOnlyMethodCollection FromClasses (IReadOnlyList<IClass> classes)
        {
            IReadOnlyMethodCollection res = MethodCollection.Empty;

            foreach (var t in classes)
            {
                res = new MergedReadOnlyMethodCollection (t.Methods, res);
            }

            return res;
        }
    }
}
