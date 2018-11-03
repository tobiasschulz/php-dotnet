using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;

namespace PHP.Tree
{
    public abstract class DeclarationExpression : Expression
    {

    }

    public sealed class FunctionDeclarationExpression : DeclarationExpression
    {
        public readonly NameOfFunction Name;
        public readonly DeclarationSignature DeclarationSignature;
        public readonly Expression Body;

        public FunctionDeclarationExpression (NameOfFunction name, DeclarationSignature declaration_signature, Expression body)
        {
            Name = name;
            DeclarationSignature = declaration_signature;
            Body = body;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", DeclarationSignature.Parameters),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"function declaration: {Name}";
        }
    }

    public sealed class ClassDeclarationExpression : DeclarationExpression
    {
        public readonly NameOfClass Name;
        public readonly ImmutableArray<Expression> Members;

        public ClassDeclarationExpression (NameOfClass name, ImmutableArray<Expression> members)
        {
            Name = name;
            Members = members;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("members", Members),
            };
        }

        protected override string GetTypeName ()
        {
            return $"class declaration: {Name}";
        }
    }

    public sealed class ClassFieldDeclarationExpression : DeclarationExpression
    {
        public readonly NameOfVariable Name;
        public readonly Expression Initializer;
        public readonly MemberAttributes Attributes;

        public ClassFieldDeclarationExpression (NameOfVariable name, Expression initializer, MemberAttributes attributes)
        {
            Name = name;
            Initializer = initializer;
            Attributes = attributes;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("initializer", Initializer),
            };
        }

        protected override string GetTypeName ()
        {
            return $"class field: {Name} ({Attributes})";
        }
    }

    public sealed class ClassMethodDeclarationExpression : DeclarationExpression
    {
        public readonly NameOfMethod Name;
        public readonly DeclarationSignature DeclarationSignature;
        public readonly Expression Body;
        public readonly MemberAttributes Attributes;

        public ClassMethodDeclarationExpression (NameOfMethod name, DeclarationSignature declaration_signature, Expression body, MemberAttributes attributes)
        {
            Name = name;
            DeclarationSignature = declaration_signature;
            Body = body;
            Attributes = attributes;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", DeclarationSignature.Parameters),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"method declaration: {Name} ({Attributes})";
        }
    }

    public readonly struct MemberAttributes
    {
        public readonly Publicity Publicity;
        public readonly bool IsStatic;
        public readonly bool IsAbstract;
        public readonly bool IsConstructor;

        public MemberAttributes (Publicity publicity, bool is_static, bool is_abstract, bool is_constructor) : this ()
        {
            Publicity = publicity;
            IsStatic = is_static;
            IsAbstract = is_abstract;
            IsConstructor = is_constructor;
        }

        public override string ToString ()
        {
            string res = Publicity.ToString ().ToLower ();
            if (IsStatic) res += " static";
            if (IsAbstract) res += " abstract";
            if (IsConstructor) res += " constructor";
            return res;
        }
    }

    public enum Publicity
    {
        Public,
        Protected,
        Private,
    }

    public sealed class BreakExpression : DeclarationExpression
    {
        public readonly Expression CountOfLoops;

        public BreakExpression (Expression count_of_loop)
        {
            CountOfLoops = count_of_loop;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("count of loops", CountOfLoops),
            };
        }

        protected override string GetTypeName ()
        {
            return $"break";
        }
    }

    public sealed class ContinueExpression : DeclarationExpression
    {
        public readonly Expression CountOfLoops;

        public ContinueExpression (Expression count_of_loop)
        {
            CountOfLoops = count_of_loop;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("count of loops", CountOfLoops),
            };
        }

        protected override string GetTypeName ()
        {
            return $"continue";
        }
    }

    public sealed class ReturnExpression : DeclarationExpression
    {
        public readonly Expression ReturnValue;

        public ReturnExpression (Expression return_value)
        {
            ReturnValue = return_value;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("return value", ReturnValue),
            };
        }

        protected override string GetTypeName ()
        {
            return $"return";
        }
    }

}
