using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    // ╳

    public abstract class Expression
    {
        private static ImmutableHashSet<string> FalseStrings = new []
        {
            "false",
            "False",
            "FALSE",
            "null",
            "Null",
            "NULL",
            "0",
            "0.0",
        }.ToImmutableHashSet ();

        public override string ToString ()
        {
            return $"[{this.GetType ().Name}: '{GetTypeName ()}']";
        }

        public virtual string GetStringValue ()
        {
            return "";
        }

        public virtual bool GetBoolValue ()
        {
            string s = GetStringValue ();
            return !string.IsNullOrEmpty (s) && !FalseStrings.Contains (s);
        }

        public virtual double GetDoubleValue ()
        {
            string s = GetStringValue ();
            return s.ToDouble ();
        }

        public virtual long GetLongValue ()
        {
            string s = GetStringValue ();
            return s.ToLong ();
        }

        public virtual ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.STRING;
        }

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
                        child.Print (new TreeParams (p4, diff: 4, is_not_last: !is_last_child_of_group, add_line: !is_last_child_of_group));
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
                        if (i < indent) sb [i] = '|';
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

    public sealed class DocExpression : Expression
    {
        protected override string GetTypeName ()
        {
            return "doc";
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
                ("vars", VarList),
            };
        }

        protected override string GetTypeName ()
        {
            return "global statement";
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
                ("right", Right),
            };
        }

        protected override string GetTypeName ()
        {
            return "assign";
        }
    }

    public sealed class InstanceOfExpression : Expression
    {
        public readonly Name? TypeName;
        public readonly Expression Value;

        public InstanceOfExpression (InstanceOfEx e)
        {
            TypeName = e.ClassNameRef.QualifiedName;
            Value = Expressions.Parse (e.Expression);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("value", Value),
            };
        }

        protected override string GetTypeName ()
        {
            return $"instance of: {TypeName}";
        }
    }

    public sealed class RequireFileExpression : Expression
    {
        public readonly InclusionType Mode;
        public readonly Expression FilePath;

        public RequireFileExpression (IncludingEx e)
        {
            Mode = (InclusionType)e.InclusionType;
            FilePath = Expressions.Parse (e.Target);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("file", FilePath),
            };
        }

        protected override string GetTypeName ()
        {
            return $"require: {Mode}";
        }

        public enum InclusionType
        {
            Include = 0,
            IncludeOnce = 1,
            Require = 2,
            RequireOnce = 3,
            Prepended = 4,
            Appended = 5
        }
    }

}
