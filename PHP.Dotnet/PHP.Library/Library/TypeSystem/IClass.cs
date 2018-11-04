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

    }

    public interface IClassCollection
    {
        bool TryGetValue (NameOfClass name, out IClass res);
        bool Contains (NameOfClass name);
        IEnumerable<IClass> GetAll ();
    }

    public sealed class ClassCollection : IClassCollection
    {
        private ImmutableArray<IClass> _data = ImmutableArray<IClass>.Empty;

        public ClassCollection ()
        {
        }

        public bool TryGetValue (NameOfClass name, out IClass res)
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

        public bool Contains (NameOfClass name)
        {
            return TryGetValue (name, out var dummy);
        }

        public IEnumerable<IClass> GetAll ()
        {
            return _data;
        }

        public void Add (IClass value)
        {
            if (value == null) return;

            if (TryGetValue (value.Name, out var existing_value))
            {
                Log.Error ($"Cannot add function {value.Name}: already exists: {existing_value} vs {value}");
            }
            else
            {
                _data = _data.Add (value);
            }
        }

        public void Replace (IClass value)
        {
            if (value == null) return;

            if (TryGetValue (value.Name, out var existing_value))
            {
                _data = _data.Remove (existing_value);
            }

            _data = _data.Add (value);
        }

    }
}
