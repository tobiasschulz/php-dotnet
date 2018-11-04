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
        IReadOnlyFieldCollection Fields { get; }
        IReadOnlyMethodCollection Methods { get; }
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

    public static class ClassCollectionExtensions
    {
        public static IReadOnlyList<IClass> GetWithParentClasses (this IReadOnlyClassCollection that, IClass type)
        {
            Dictionary<NameOfClass, IClass> classes = new Dictionary<NameOfClass, IClass>
            {
                [type.Name] = type,
            };
            List<NameOfClass> next = type.Parents.Concat (type.Interfaces).ToList ();
            do
            {
                IReadOnlyList<NameOfClass> current = next.ToImmutableArray ();
                next.Clear ();
                foreach (var parent_name in current)
                {
                    if (!classes.ContainsKey (parent_name) && that.TryGetValue (parent_name, out IClass parent))
                    {
                        classes [parent_name] = parent;
                        next.AddRange (parent.Parents);
                        next.AddRange (parent.Interfaces);
                    }
                }
            } while (next.Count != 0);
            return classes.Values.ToImmutableArray ();
        }

    }
}
