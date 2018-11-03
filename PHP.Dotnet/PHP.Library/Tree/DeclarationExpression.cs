using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    public abstract class DeclarationExpression : Expression
    {

    }

    public sealed class FunctionDeclarationExpression : DeclarationExpression
    {
        public readonly Name Name;
        public readonly DeclarationSignature DeclarationSignature;
        public readonly Expression Body;

        public FunctionDeclarationExpression (FunctionDecl e)
        {
            Name = e.Name;
            DeclarationSignature = new DeclarationSignature (e.Signature);
            Body = Expressions.Parse (e.Body);
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
        public readonly Name Name;
        public readonly ImmutableArray<Expression> Members;

        public ClassDeclarationExpression (NamedTypeDecl e)
        {
            Name = e.Name;
            Members = e.Members.SelectMany (member =>
            {
                MemberAttributes attributes = new MemberAttributes (member.Modifiers);
                switch (member)
                {
                    case FieldDeclList l:
                        return l.Fields.Select (f => new ClassFieldDeclarationExpression (attributes, f));

                    case MethodDecl l:
                        return new [] { new ClassMethodDeclarationExpression (attributes, l) };

                    default:
                        Log.Error ($"Unknown member: {member}");
                        return Enumerable.Empty<Expression> ();
                }
            }).ToImmutableArray ();
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
        public readonly VariableName Name;
        public readonly Expression Initializer;
        public readonly MemberAttributes Attributes;

        public ClassFieldDeclarationExpression (MemberAttributes attributes, FieldDecl e)
        {
            Name = e.Name;
            Initializer = Expressions.Parse (e.Initializer);
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

        public readonly Name Name;
        public readonly DeclarationSignature DeclarationSignature;
        public readonly Expression Body;
        public readonly MemberAttributes Attributes;

        public ClassMethodDeclarationExpression (MemberAttributes attributes, MethodDecl e)
        {
            Name = e.Name;
            DeclarationSignature = new DeclarationSignature (e.Signature);
            Body = Expressions.Parse (e.Body);
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

        public MemberAttributes (Devsense.PHP.Syntax.PhpMemberAttributes modifiers)
        {
            if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Public) != 0)
                Publicity = Publicity.Public;
            else if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Protected) != 0)
                Publicity = Publicity.Protected;
            else if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Private) != 0)
                Publicity = Publicity.Private;
            else
                Publicity = Publicity.Public;

            IsStatic = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Static) != 0);
            IsAbstract = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Abstract) != 0);
            IsConstructor = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Constructor) != 0);
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

        public BreakExpression (JumpStmt e)
        {
            CountOfLoops = Expressions.Parse (e.Expression);
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

        public ContinueExpression (JumpStmt e)
        {
            CountOfLoops = Expressions.Parse (e.Expression);
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

        public ReturnExpression (JumpStmt e)
        {
            ReturnValue = Expressions.Parse (e.Expression);
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
