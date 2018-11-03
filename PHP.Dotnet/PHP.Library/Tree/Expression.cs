using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Library.TypeSystem;
using PHP.Standard;

namespace PHP.Tree
{
    // ╳

    public abstract class Expression
    {
        public override string ToString ()
        {
            string typename = GetTypeName ();
            if (typename.Contains ('\n')) typename = typename.Replace ("\n", "\\n");
            return $"[{this.GetType ().Name}: '{typename}']";
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
            if (value.Contains ('\n')) value = value.Replace ("\n", "\\n");
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

        public GlobalStmtExpression (ImmutableArray<Expression> varlist)
        {
            VarList = varlist;
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

        public AssignExpression (Expression left, Expression right)
        {
            Left = left;
            Right = right;
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
        public readonly NameOfClass Name;
        public readonly Expression Value;

        public InstanceOfExpression (NameOfClass name, Expression value)
        {
            Name = name;
            Value = value;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("value", Value),
            };
        }

        protected override string GetTypeName ()
        {
            return $"instance of: {Name}";
        }
    }

    public sealed class RequireFileExpression : Expression
    {
        public readonly InclusionType Mode;
        public readonly Expression FilePath;

        public RequireFileExpression (InclusionType mode, Expression file_path)
        {
            Mode = mode;
            FilePath = file_path;
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
