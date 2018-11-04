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
        Result Execute (EvaluatedCallSignature call_signature, Scope scope);
    }
    
    public interface IReadOnlyMethodCollection : IReadOnlyElementCollection<NameOfMethod, IMethod>
    {
    }

    public interface IMethodCollection : IReadOnlyMethodCollection, IElementCollection<NameOfMethod, IMethod>
    {
    }

    public sealed class MergedMethodCollection : MergedElementCollection<NameOfMethod, IMethod>, IMethodCollection, IReadOnlyMethodCollection
    {
        public MergedMethodCollection (IReadOnlyElementCollection<NameOfMethod, IMethod> collection_parent, IElementCollection<NameOfMethod, IMethod> collection_own)
            : base (collection_parent, collection_own)
        {
        }
    }

    public sealed class MethodCollection : ElementCollection<NameOfMethod, IMethod>, IMethodCollection, IReadOnlyMethodCollection
    {
    }
}
