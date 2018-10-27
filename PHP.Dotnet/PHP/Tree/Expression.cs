using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    // ╳

    public abstract class Expression
    {
        public void Print (TreeParams p = default)
        {
            _printSelf (p);
            _printChildren (p, _getChildren ());
        }

        protected void _printLine (TreeParams p, string value)
        {
            Log.Debug ($"[  ] {p.indent_str} {value}");
        }

        protected virtual void _printSelf (TreeParams p)
        {
            _printLine (p, $"{_getBranchSymbol (is_last: !p.is_not_last_child)}── {GetTypeName ()}");
        }

        protected abstract string GetTypeName ();

        protected virtual TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [0];
        }

        protected void _printChildren (TreeParams p, TreeChildGroup [] groups)
        {
            int count_groups = groups.Count (cg => cg.IsDisplayed ());
            int i = 0;
            foreach (var child_group in groups)
            {
                if (!child_group.IsDisplayed ()) continue;

                bool is_last_group = i + 1 == count_groups;

                TreeParams p4 = new TreeParams (p, diff: 4, is_not_last: !is_last_group, add_line: !is_last_group);

                if (!string.IsNullOrEmpty (child_group.Name))
                {
                    _printLine (p4, $"{_getBranchSymbol (is_last: is_last_group)}── {child_group.Name}:");

                    int count_children = child_group.Children.Length;
                    int k = 0;
                    foreach (var child in child_group.Children)
                    {
                        bool is_last_child_of_group = k + 1 == count_children;
                        bool is_last_child_ever = is_last_group && is_last_child_of_group;
                        child.Print (new TreeParams (p4, diff: 4, is_not_last: !is_last_child_of_group, add_line: !is_last_child_ever));
                        k++;
                    }
                }
                else
                {
                    int count_children = child_group.Children.Length;
                    int k = 0;
                    foreach (var child in child_group.Children)
                    {
                        bool is_last_child_of_group = k + 1 == count_children;
                        bool is_last_child_ever = is_last_group && is_last_child_of_group;
                        child.Print (new TreeParams (p4, is_not_last: !is_last_child_of_group, add_line: !is_last_child_ever));
                        k++;
                    }
                }

                i++;
            }
        }

        private string _getBranchSymbol (bool is_last) => is_last ? "└" : "├";

        public readonly struct TreeParams
        {
            public readonly int indent;
            public readonly bool is_not_last_child;
            public readonly ImmutableArray<int> lines;
            public readonly string indent_str;

            public TreeParams (TreeParams p, int diff = 0, bool is_not_last = false, bool add_line = false)
            {
                this.indent = p.indent + diff;
                this.is_not_last_child = is_not_last;
                this.lines = p.lines;

                if (add_line)
                {
                    if (lines.IsDefaultOrEmpty)
                    {
                        lines = ImmutableArray.Create (indent + 1);
                    }
                    else
                    {
                        lines = lines.Add (indent + 1);
                    }
                }

                StringBuilder sb = new StringBuilder (p.indent_str);
                while (sb.Length < indent) sb.Append (' ');
                if (!lines.IsDefaultOrEmpty)
                {
                    foreach (int i in lines)
                    {
                     if (i < indent)   sb [i] = '|';
                    }
                }
                indent_str = sb.ToString ();
            }
        }

        public class TreeChildGroup
        {
            public readonly string Name;
            public readonly ImmutableArray<Expression> Children = ImmutableArray<Expression>.Empty;

            public TreeChildGroup (string name, ImmutableArray<Expression> children)
            {
                Name = name;
                Children = children;
            }

            internal bool IsDisplayed ()
            {
                return !string.IsNullOrEmpty (Name) || Children.Length != 0;
            }

            public static implicit operator TreeChildGroup ((string name, Expression child) other)
            {
                return new TreeChildGroup (other.name, other.child != null ? ImmutableArray.Create (other.child) : ImmutableArray<Expression>.Empty);
            }

            public static implicit operator TreeChildGroup ((string name, IEnumerable<Expression> children) other)
            {
                return new TreeChildGroup (other.name, other.children != null ? other.children.ToImmutableArray () : ImmutableArray<Expression>.Empty);
            }
        }
    }

    public sealed class EmptyExpression : Expression
    {

        protected override string GetTypeName ()
        {
            return "empty";
        }
    }

    public sealed class GlobalCodeExpression : Expression
    {
        public readonly ImmutableArray<Expression> Body;

        public GlobalCodeExpression (GlobalCode e)
        {
            Body = e.Statements.Select (c => Expressions.Parse (c)).ToImmutableArray ();
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("body", Body)
            };
        }

        protected override string GetTypeName ()
        {
            return "root";
        }
    }

    public sealed class GlobalStmtExpression : Expression
    {
        public readonly ImmutableArray<Expression> VarList;

        public GlobalStmtExpression (GlobalStmt e)
        {
            VarList = e.VarList.Select (c => Expressions.Parse (c)).ToImmutableArray ();
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup []{
                ("vars", VarList)
            };
        }

        protected override string GetTypeName ()
        {
            return "global statement";
        }
    }

    public sealed class ConditionalBlockExpression : Expression
    {
        public readonly ImmutableArray<BaseIfExpression> Ifs = ImmutableArray<BaseIfExpression>.Empty;
        public readonly ElseExpression Else;

        public ConditionalBlockExpression (IfStmt e)
        {
            foreach (var c in e.Conditions)
            {
                if (c.Condition == null)
                {
                    if (Else == null)
                    {
                        Else = new ElseExpression (c);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException ($"Multiple else expressions ???");
                    }
                }
                else
                {
                    if (Ifs.Length == 0)
                    {
                        Ifs = Ifs.Add (new IfExpression (c));
                    }
                    else
                    {
                        Ifs = Ifs.Add (new ElseIfExpression (c));
                    }
                }
            }
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Ifs),
                ("", Else)
            };
        }

        protected override string GetTypeName ()
        {
            return "condition";
        }
    }

    public abstract class BaseIfExpression : Expression
    {
        public readonly Expression Condition;
        public readonly Expression Body;

        public BaseIfExpression (ConditionalStmt e)
        {
            Condition = Expressions.Parse (e.Condition);
            Body = Expressions.Parse (e.Statement);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("condition", Condition),
                ("body", Body)
            };
        }
    }

    public sealed class IfExpression : BaseIfExpression
    {
        public IfExpression (ConditionalStmt e)
            : base (e)
        {
        }

        protected override string GetTypeName ()
        {
            return "if";
        }
    }

    public sealed class ElseIfExpression : BaseIfExpression
    {
        public ElseIfExpression (ConditionalStmt e)
            : base (e)
        {
        }

        protected override string GetTypeName ()
        {
            return "else if";
        }
    }

    public sealed class ElseExpression : Expression
    {
        public readonly Expression Body;

        public ElseExpression (ConditionalStmt e)
        {
            Body = Expressions.Parse (e.Statement);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("body", Body)
            };
        }

        protected override string GetTypeName ()
        {
            return "else";
        }
    }

    public sealed class BlockExpression : Expression
    {
        public readonly ImmutableArray<Expression> Body;

        public BlockExpression (BlockStmt e)
        {
            Body = e.Statements.Select (c => Expressions.Parse (c)).ToImmutableArray ();
        }

        protected override string GetTypeName ()
        {
            return "block";
        }
    }

    public sealed class AssignExpression : Expression
    {
        public readonly Expression Left;
        public readonly Expression Right;

        public AssignExpression (ValueAssignEx e)
        {
            Left = Expressions.Parse (e.LValue);
            Right = Expressions.Parse (e.RValue);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("left", Left),
                ("right", Right)
            };
        }

        protected override string GetTypeName ()
        {
            return "assign";
        }
    }

    public sealed class FunctionCallExpression : Expression
    {
        public readonly FunctionSignature Signature;

        public FunctionCallExpression (DirectFcnCall e)
        {
            Log.Debug ($"function call name: {e.FullName.Name},  {e.FullName.OriginalName},  {e.FullName.FallbackName}");
            Signature = new FunctionSignature (e.CallSignature);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", Signature.Parameters)
            };
        }

        protected override string GetTypeName ()
        {
            return "function call";
        }
    }

    public sealed class StaticMethodCallExpression : Expression
    {
        public readonly FunctionName MethodName;
        public readonly FunctionSignature Signature;

        public StaticMethodCallExpression (DirectStMtdCall e)
        {
            MethodName = e.MethodName;
            Signature = new FunctionSignature (e.CallSignature);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", Signature.Parameters)
            };
        }

        protected override string GetTypeName ()
        {
            return $"function call: {MethodName}";
        }
    }
    

    public sealed class CatchExpression : Expression
    {
        public readonly Expression Body;
        public readonly Variable Variable;
        public readonly TypeRef TargetType;

        public CatchExpression (CatchItem e)
        {
            Body = Expressions.Parse (e.Body);
            Variable = new Variable (e.Variable);
            TargetType = e.TargetType;
        }

        protected override string GetTypeName ()
        {
            return "catch";
        }
    }

    public sealed class TryExpression : Expression
    {
        public readonly Expression Body;
        public readonly Expression Finally;
        public readonly ImmutableArray<CatchExpression> Catches;

        public TryExpression (TryStmt e)
        {
            Body = Expressions.Parse (e.Body);
            Finally = Expressions.Parse (e.FinallyItem?.Body);
            Catches = e.Catches.Select (c => new CatchExpression (c)).ToImmutableArray ();
        }

        protected override string GetTypeName ()
        {
            return "try";
        }
    }
}
