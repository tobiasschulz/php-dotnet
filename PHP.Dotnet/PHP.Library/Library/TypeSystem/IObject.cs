using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Library.TypeSystem
{
    public static class ObjectExtensions
    {
        public static ObjectId LastestObjectId = 0;
    }

    public enum ObjectId
    {
    }

    public interface IObject : IElement<ObjectId>
    {
        IReadOnlyList<IClass> Classes { get; }
        IEnumerable<NameOfClass> ClassNames { get; }
        IVariableCollection Variables { get; }
        IReadOnlyMethodCollection Methods { get; }
    }

    public interface IReadOnlyObjectCollection : IReadOnlyElementCollection<ObjectId, IObject>
    {
    }

    public interface IObjectCollection : IReadOnlyObjectCollection, IElementCollection<ObjectId, IObject>
    {
    }

    public sealed class ObjectCollection : ElementCollection<ObjectId, IObject>, IObjectCollection, IReadOnlyObjectCollection
    {
    }
}
