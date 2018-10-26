using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    public enum TokenType2
    {
        BLOCK = 0,
        COMMENT = 1,
        OUTSIDE = 2,
        IDENTIFIER = 3,
        STRING = 4,
        VARIABLE = 5,
        FLOW_CONTROL = 6,
        OPERATOR = 7,
        GROUP = 8,
    }

    public abstract class BaseToken2
    {
        internal abstract void DumpLog (int indent);

        internal void _log (int indent, string value)
        {
            Log.Debug ($"{new string (' ', indent * 4) }- {value}");
        }
    }

    public sealed class RegularToken2 : BaseToken2
    {
        public readonly TokenType2 Type;
        public readonly string Buffer;
        public IReadOnlyList<BaseToken2> Children => _children;

        private readonly List<BaseToken2> _children = new List<BaseToken2> ();

        public RegularToken2 (TokenType2 type, string buffer, IEnumerable<BaseToken2> children = null)
        {
            Type = type;
            Buffer = buffer;

            if (children != null)
            {
                foreach (var c in children)
                {
                    Add (c);
                }
            }
        }

        public void Add (BaseToken2 child)
        {
            _children.Add (child);
        }
        
        public override string ToString ()
        {
            return $"[Token2: {Type} '{Buffer}']";
        }

        internal override void DumpLog (int indent)
        {
                _log (indent, $"{Type} '{Buffer}'");

            foreach (var e in Children)
            {
                e.DumpLog (indent + 1);
            }
        }

    }

    public sealed class FunctionToken2 : BaseToken2
    {
        public readonly BaseToken2 FunctionRef;
        public readonly BaseToken2 Caller;
        public readonly BaseToken2 Arguments;

        public FunctionToken2 (BaseToken2 function_ref, BaseToken2 caller, BaseToken2 arguments)
        {
            FunctionRef = function_ref;
            Caller = caller;
            Arguments = arguments;
        }

        public override string ToString ()
        {
            return $"[FunctionToken2: {FunctionRef}]";
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"Function Call:");
            _log (indent + 1, $"FunctionRef:");
            FunctionRef?.DumpLog (indent + 2);
            _log (indent + 1, $"Caller:");
            Caller?.DumpLog (indent + 2);
            _log (indent + 1, $"Arguments:");
            Arguments.DumpLog (indent + 2);
        }

    }

    public sealed class ClassAccessToken2 : BaseToken2
    {
        public readonly string Kind;
        public readonly BaseToken2 Caller;
        public readonly BaseToken2 Property;

        public ClassAccessToken2 (string kind, BaseToken2 caller, BaseToken2 property)
        {
            Kind = kind;
            Caller = caller;
            Property = property;
        }

        public override string ToString ()
        {
            return $"[Access: {Kind}]";
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"Access: ({Kind})");
            _log (indent + 1, $"Caller:");
            Caller.DumpLog (indent + 2);
            _log (indent + 1, $"Property:");
            Property.DumpLog (indent + 2);
        }

    }

    public sealed class FlowControlToken2 : BaseToken2
    {
        public readonly string Name;
        public readonly BaseToken2 Condition;
        public readonly BaseToken2 Block;

        public FlowControlToken2 (string name, BaseToken2 condition, BaseToken2 block)
        {
            Name = name;
            Condition = condition;
            Block = block;
        }

        public override string ToString ()
        {
            return $"[FlowControlToken2: {Name}]";
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"Flow: {Name}:");
            _log (indent + 1, $"Condition:");
            Condition?.DumpLog (indent + 2);
            _log (indent + 1, $"Block:");
            Block.DumpLog (indent + 2);
        }

    }


}
