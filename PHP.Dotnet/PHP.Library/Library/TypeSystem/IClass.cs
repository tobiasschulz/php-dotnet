using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Library.TypeSystem
{
    public interface IClass
    {
        NameOfClass Name { get; }
        IVariableCollection Fields { get; }
        IMethodCollection Methods { get; }
    }

    public interface IReadOnlyClassCollection
    {
        bool TryGetValue (NameOfClass name, out IClass res);
        bool Contains (NameOfClass name);
        IEnumerable<IClass> GetAll ();
    }

    public interface IClassCollection : IReadOnlyClassCollection
    {
        void Add (IClass value);
    }

    public sealed class ClassCollection : IClassCollection
    {
        private ImmutableArray<IClass> _data = ImmutableArray<IClass>.Empty;

        public ClassCollection ()
        {
        }

        bool IReadOnlyClassCollection.TryGetValue (NameOfClass name, out IClass res)
        {
            foreach (IClass value in _data)
            {
                if (value.Name == name)
                {
                    res = value;
                    return true;
                }
            }
            res = null;
            return false;
        }

        bool IReadOnlyClassCollection.Contains (NameOfClass name)
        {
            return ((IReadOnlyClassCollection)this).TryGetValue (name, out var dummy);
        }

        IEnumerable<IClass> IReadOnlyClassCollection.GetAll ()
        {
            return _data;
        }

        void IClassCollection.Add (IClass value)
        {
            if (value == null) return;

            if (((IReadOnlyClassCollection)this).TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add function {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _data = _data.Add (value);
            }
        }
        
    }
}
