using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;
using Sprache;
using System.Linq;
using System.Linq.Expressions;

namespace PHP
{
    public abstract class PhpExpression
    {
        protected PhpExpression ()
        {
        }

        public static T From<T> (T [] arr, Func<T, T, T> func)
        {
            if (arr.Length == 1)
            {
                return arr [0];
            }
            T res = arr [0];
            for (int i = 1; i < arr.Length; i++)
            {
                res = func (res, arr [i]);
            }
            return res;
        }
    }

    public sealed class PhpAssignment : PhpExpression
    {
        public readonly PhpExpression Left;
        public readonly PhpExpression Right;

        public PhpAssignment (PhpExpression left, PhpExpression right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString ()
        {
            return $"[Assignment: {Left} = {Right} ]";
        }
    }

    public sealed class PhpCall : PhpExpression
    {
        public readonly PhpExpression Name;
        public readonly PhpExpression [] Arguments;

        public PhpCall (PhpExpression name, PhpExpression [] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString ()
        {
            return $"[Call: {Name} ( {Arguments.Join (", ")} ) ]";
        }
    }

    public sealed class PhpMath : PhpExpression
    {
        public readonly string Operator;
        public readonly PhpExpression Left;
        public readonly PhpExpression Right;

        public PhpMath (string op, PhpExpression left, PhpExpression right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override string ToString ()
        {
            return $"[Math: {Left} {Operator} {Left} ]";
        }
    }

    public sealed class PhpObjectAccess : PhpExpression
    {
        public readonly PhpExpression Left;
        public readonly PhpExpression Right;

        public PhpObjectAccess (PhpExpression left, PhpExpression right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString ()
        {
            return $"[ObjectAccess: {Left} -> {Left} ]";
        }
    }

    public sealed class PhpStaticAccess : PhpExpression
    {
        public readonly PhpExpression Left;
        public readonly PhpExpression Right;

        public PhpStaticAccess (PhpExpression left, PhpExpression right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString ()
        {
            return $"[StaticAccess: {Left} :: {Left} ]";
        }
    }

    public sealed class PhpStatementList : PhpExpression
    {
        public readonly PhpExpression [] Statements;

        public PhpStatementList (PhpExpression [] statements)
        {
            Statements = statements;
        }

        public override string ToString ()
        {
            return $"[Statements: {Statements.Join (" ; ")} ]";
        }
    }

    public sealed class PhpBlock : PhpExpression
    {
        public readonly PhpExpression Content;

        public PhpBlock (PhpExpression content)
        {
            Content = content;
        }

        public override string ToString ()
        {
            return $"[Block: {Content} ]";
        }
    }

    public abstract class PhpLiteral : PhpExpression
    {
        public readonly string Value;

        protected PhpLiteral (string value)
        {
            Value = value;
        }

        public override string ToString ()
        {
            return $"[Literal: {Value} ]";
        }
    }

    public sealed class PhpIdentifier : PhpLiteral
    {
        public PhpIdentifier (string value)
            : base (value)
        {
        }
    }

    public sealed class PhpString : PhpLiteral
    {
        public PhpString (string value)
            : base (value)
        {
        }

        public override string ToString ()
        {
            return $"[String: {Value} ]";
        }
    }

    public sealed class PhpVariable : PhpLiteral
    {
        public PhpVariable (string value)
            : base (value)
        {
        }

        public override string ToString ()
        {
            return $"[Variable: {Value} ]";
        }
    }

    public abstract class PhpNumber : PhpLiteral
    {
        protected PhpNumber (string value)
            : base (value)
        {
        }
    }

    public sealed class PhpDouble : PhpNumber
    {
        public PhpDouble (string value)
            : base (value)
        {
        }

        public override string ToString ()
        {
            return $"[Double: {Value} ]";
        }
    }

    public sealed class PhpLong : PhpNumber
    {
        public PhpLong (string value)
            : base (value)
        {
        }

        public override string ToString ()
        {
            return $"[Long: {Value} ]";
        }
    }

    public sealed class PhpNull : PhpLiteral
    {
        public PhpNull ()
            : base (null)
        {
        }

        public override string ToString ()
        {
            return $"[Null]";
        }
    }

    public sealed class PhpBool : PhpLiteral
    {
        public PhpBool (bool value)
            : base (value ? "1" : "0")
        {
        }

        public override string ToString ()
        {
            return $"[Bool: {Value} ]";
        }
    }

    public class CodeParser
    {
        public static Lazy<CodeParser> Instance = new Lazy<CodeParser> ();

        private static Parser<string> Operator (string op)
        {
            return Parse.String (op).Token ().Return (op);
        }

        private static readonly Parser<string> Add = Operator ("+");
        private static readonly Parser<string> Subtract = Operator ("-");
        private static readonly Parser<string> Multiply = Operator ("*");
        private static readonly Parser<string> Divide = Operator ("/");
        private static readonly Parser<string> Modulo = Operator ("%");
        private static readonly Parser<string> Power = Operator ("^");
        private static readonly Parser<string> ArrowObject = Operator ("->");
        private static readonly Parser<string> StaticDoubleColon = Operator ("::");
        private static readonly Parser<string> ArrowArray = Operator ("=>");

        private static List<char> EscapeChars = new List<char> { '\"', '\\', 'b', 'f', 'n', 'r', 't' };

        private static Parser<U> EnumerateInput<T, U> (T [] input, Func<T, Parser<U>> parser)
        {
            if (input == null || input.Length == 0) throw new ArgumentNullException ("input");
            if (parser == null) throw new ArgumentNullException ("parser");

            return i =>
            {
                foreach (var inp in input)
                {
                    var res = parser (inp) (i);
                    if (res.WasSuccessful) return res;
                }

                return Result.Failure<U> (null, null, null);
            };
        }

        private static Parser<string> NumberPartInt =
                from minus in Parse.String ("-").Text ().Optional ()
                from digits in Parse.Digit.XAtLeastOnce ().Text ()
                select (minus.IsDefined ? minus.Get () : "") + digits;

        private static Parser<string> NumberPartFrac =
                from dot in Parse.String (".").Text ()
                from digits in Parse.Digit.XAtLeastOnce ().Text ()
                select dot + digits;

        private static Parser<PhpExpression> Number =
                from integer in NumberPartInt
                from frac in NumberPartFrac.Optional ()
                select frac.IsDefined ? (PhpNumber)new PhpDouble (integer + frac.Get ()) : (PhpNumber)new PhpLong (integer);

        private static Parser<char> ControlChar =
                from first in Parse.Char ('\\')
                from next in EnumerateInput (EscapeChars.ToArray (), c => Parse.Char (c))
                select ((next == 't') ? '\t' :
                        (next == 'r') ? '\r' :
                        (next == 'n') ? '\n' :
                        (next == 'f') ? '\f' :
                        (next == 'b') ? '\b' :
                        next);

        private static Parser<char> StringChar = Parse.AnyChar.Except (Parse.Char ('"').Or (Parse.Char ('\\'))).Or (ControlChar);

        private static Parser<char> VariableChar = Parse.LetterOrDigit.Or (Parse.Char ('_'));

        private static Parser<PhpExpression> Variable =
                from first in Parse.Char ('$')
                from chars in VariableChar.Many ().Text ()
                select new PhpVariable (chars);

        private static Parser<PhpExpression> Identifier =
                from chars in VariableChar.Many ().Text ()
                select new PhpIdentifier (chars);

        private static Parser<PhpExpression> String =
                from first in Parse.Char ('"')
                from chars in StringChar.Many ().Text ()
                from last in Parse.Char ('"')
                select new PhpString (chars);

        private static Parser<PhpExpression> Null =
                from str in Parse.IgnoreCase ("null")
                select new PhpNull ();

        private static Parser<PhpExpression> Bool =
                from str in Parse.IgnoreCase ("true").Text ().Or (Parse.IgnoreCase ("false").Text ())
                select new PhpBool (str.ToLower () == "true" ? true : false);

        private static Parser<PhpExpression> Literal =
            from spaces1 in Parse.WhiteSpace.Optional ()
            from lit in (
                String
                .XOr (Variable)
                .XOr (Identifier)
                .XOr (Number)
                .XOr (Bool)
                .XOr (Null)
            )
            from spaces2 in Parse.WhiteSpace.Optional ()
            select lit;

        private static Parser<PhpExpression> FunctionCall =
                from name in MakeExpression (nameof (FunctionCall))
                from lparen in Parse.Char ('(')
                from arguments in Parse.Ref (() => Expression).DelimitedBy (Parse.Char (',').Token ())
                from rparen in Parse.Char (')')
                select new PhpCall (name, arguments.ToArray ());

        private static Parser<PhpExpression> AssignmentXXX =
                from left in MakeExpression (except: nameof (Assignment))
                from assignment_sign in Parse.Char ('=')
                from right in MakeExpression (except: nameof (Assignment))
                select new PhpAssignment (left, right);

        private static Parser<PhpExpression> Assignment =
                from sides in MakeExpression (nameof (Assignment))
                        .DelimitedBy (Parse.Char ('=').Token ())
                select PhpExpression.From (sides.ToArray (), (l, r) => new PhpAssignment (l, r));

        private static Parser<PhpExpression> StatementList =
                from statements in Parse.Ref (() => Expression)
                        .DelimitedBy (Parse.Char (';').Token ())
                select new PhpStatementList (statements.ToArray ());

        private static Parser<PhpExpression> Block =
                from first in Parse.Char ('{').Token ()
                from members in StatementList.Optional ()
                from last in Parse.Char ('}').Token ()
                select new PhpBlock (members.IsDefined ? members.Get () : null);


        private static Parser<PhpExpression> MakeExpression (params string [] except)
        {
            var subs = new []
            {
                (nameof(FunctionCall),   Parse.Ref (() => FunctionCall)),
                (nameof(Assignment),     Parse.Ref (() => Assignment)),
                (nameof(Literal),        Parse.Ref (() => Literal)),
            };
            Parser<PhpExpression> res = null;
            foreach (var (name, p) in subs)
            {
                if (except != null && except.Contains (name)) continue;

                if (res == null) res = p;
                else res = res.XOr (p);
            }
            return res;
        }


        private static Parser<PhpExpression> Expression = MakeExpression (except: null);

        /*
        ; private static Parser<PhpExpression> XXX = Literal
               .XOr (
                   Literal
               )
               .XOr (
                   FunctionCall
               )
               .XOr (
                   Block
               )
               .XOr (
                   Parse.ChainOperator (
                       ArrowObject,
                       Parse.Ref (() => Expression),
                       (o, l, r) => new PhpObjectAccess (l, r)
                   )
               )
               .XOr (
                   Parse.ChainOperator (
                       StaticDoubleColon,
                       Parse.Ref (() => Expression),
                       (o, l, r) => new PhpStaticAccess (l, r)
                   )
               )
               .XOr (
                   Parse.ChainOperator (
                       Add.Or (Subtract),
                       Parse.ChainOperator (
                           Multiply.Or (Divide).Or (Modulo),
                           Parse.ChainOperator (
                               Power,
                               Parse.Ref (() => Expression),
                               (o, l, r) => new PhpMath (o, l, r)
                           ),
                           (o, l, r) => new PhpMath (o, l, r)
                       ),
                       (o, l, r) => new PhpMath (o, l, r)
                   )
               );
               */

        private readonly Parser<PhpExpression> Script =
                Parse.Ref (() => Expression);

        public PhpExpression ParseScript (string value)
        {
            return Script.Parse (value.Trim ());
        }
    }
}
