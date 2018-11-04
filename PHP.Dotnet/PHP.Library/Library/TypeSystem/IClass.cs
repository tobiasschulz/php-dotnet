using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Library.TypeSystem
{
    public interface IClass : IElement<NameOfClass>
    {
        IReadOnlyList<NameOfClass> Parents { get; }
        IReadOnlyList<NameOfClass> Interfaces { get; }
        IFieldCollection Fields { get; }
        IMethodCollection Methods { get; }
    }

    public interface IReadOnlyClassCollection : IReadOnlyElementCollection<NameOfClass, IClass>
    {
    }

    public interface IClassCollection : IReadOnlyClassCollection, IElementCollection<NameOfClass, IClass>
    {
    }

    public sealed class ClassCollection : ElementCollection<NameOfClass, IClass>, IClassCollection, IReadOnlyClassCollection
    {
    }
}
