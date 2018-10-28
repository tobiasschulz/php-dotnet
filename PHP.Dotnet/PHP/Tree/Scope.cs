using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace PHP.Tree
{
    public abstract class Scope
    {
        protected Scope ()
        {
        }

        public abstract GlobalScope GetGlobalScope ();
    }

    public sealed class GlobalScope : Scope
    {
        public readonly FunctionCollection GlobalFunctions;

        public GlobalScope ()
        {
        }

        public override GlobalScope GetGlobalScope ()
        {
            return this;
        }
    }

    public sealed class ScriptScope : Scope
    {
        public readonly GlobalScope GlobalScope;

        public ScriptScope (Scope value)
        {
            GlobalScope = value.GetGlobalScope ();
        }

        public override GlobalScope GetGlobalScope ()
        {
            return GlobalScope;
        }
    }

}
