﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;
using PHP.Execution;

namespace PHP.Tree
{
    public interface IFunctionDeclaration
    {
        Name Name { get; }
        Result Execute ();
    }

    public sealed class FunctionCollection
    {
        private ImmutableArray<IFunctionDeclaration> _data = ImmutableArray<IFunctionDeclaration>.Empty;

        public FunctionCollection ()
        {

        }

        public bool TryGetValue (Name name, out IFunctionDeclaration res)
        {
            foreach (IFunctionDeclaration value in _data)
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

        public void Add (IFunctionDeclaration value)
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

    }
}
