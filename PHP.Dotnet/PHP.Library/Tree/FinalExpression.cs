﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;

namespace PHP.Tree
{
    public enum ScalarAffinity
    {
        STRING,
        NULL,
        BOOL,
        LONG,
        DOUBLE,
        OBJECT,
        ARRAY,
    }

    public abstract class FinalExpression : Expression
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
            return GetTypeName ();
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

        public static implicit operator FinalExpression (string other)
        {
            return new StringExpression (other);
        }

        public static implicit operator FinalExpression (long other)
        {
            return new LongExpression (other);
        }

        public static implicit operator FinalExpression (double other)
        {
            return new DoubleExpression (other);
        }

        public static implicit operator FinalExpression (bool other)
        {
            return new BoolExpression (other);
        }

    }

    public sealed class StringExpression : FinalExpression
    {
        public readonly string Value;

        public StringExpression (string value)
        {
            Value = value;
        }

        public override string GetStringValue ()
        {
            return Value;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.STRING;
        }

        protected override string GetTypeName ()
        {
            return $"string: {Value}";
        }
    }

    public sealed class LongExpression : FinalExpression
    {
        public readonly long Value;

        public LongExpression (long value)
        {
            Value = value;
        }

        public override string GetStringValue ()
        {
            return Value.ToString (CultureInfo.InvariantCulture);
        }

        public override long GetLongValue ()
        {
            return Value;
        }

        public override double GetDoubleValue ()
        {
            return Value;
        }

        public override bool GetBoolValue ()
        {
            return Value != 0;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.LONG;
        }

        protected override string GetTypeName ()
        {
            return $"long: {GetStringValue ()}";
        }
    }

    public sealed class DoubleExpression : FinalExpression
    {
        public readonly double Value;

        public DoubleExpression (double value)
        {
            Value = value;
        }

        public override string GetStringValue ()
        {
            return Value.ToString (CultureInfo.InvariantCulture);
        }

        public override long GetLongValue ()
        {
            return (long)Math.Round (Value);
        }

        public override double GetDoubleValue ()
        {
            return Value;
        }

        public override bool GetBoolValue ()
        {
            return Value != 0;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.DOUBLE;
        }

        protected override string GetTypeName ()
        {
            return $"double: {GetStringValue ()}";
        }
    }

    public sealed class BoolExpression : FinalExpression
    {
        public readonly bool Value;

        public BoolExpression (bool value)
        {
            Value = value;
        }

        public override string GetStringValue ()
        {
            return Value ? "true" : "false";
        }

        public override long GetLongValue ()
        {
            return Value ? 1 : 0;
        }

        public override double GetDoubleValue ()
        {
            return Value ? 1 : 0;
        }

        public override bool GetBoolValue ()
        {
            return Value;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.BOOL;
        }

        protected override string GetTypeName ()
        {
            return $"bool: {GetStringValue ()}";
        }
    }

    public sealed class EmptyExpression : FinalExpression
    {
        public override string GetStringValue ()
        {
            return "";
        }

        public override long GetLongValue ()
        {
            return 0;
        }

        public override double GetDoubleValue ()
        {
            return 0;
        }

        public override bool GetBoolValue ()
        {
            return false;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.NULL;
        }

        protected override string GetTypeName ()
        {
            return "empty";
        }
    }

    public sealed class NullExpression : FinalExpression
    {
        protected override string GetTypeName ()
        {
            return "null";
        }

        public override string GetStringValue ()
        {
            return "";
        }

        public override long GetLongValue ()
        {
            return 0;
        }

        public override double GetDoubleValue ()
        {
            return 0;
        }

        public override bool GetBoolValue ()
        {
            return false;
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.NULL;
        }

    }

    public sealed class ObjectPointerExpression : FinalExpression
    {
        private readonly IObject _object;

        public ObjectPointerExpression (IObject obj)
        {
            _object = obj;
        }

        public IObject Object => _object;

        protected override string GetTypeName ()
        {
            return "object";
        }

        public override string GetStringValue ()
        {
            return Object.ToString ();
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.OBJECT;
        }

    }

    public sealed class ArrayPointerExpression : FinalExpression
    {
        private readonly IArray _array;

        public ArrayPointerExpression (IArray array)
        {
            _array = array;
        }

        public IArray Array => _array;

        protected override string GetTypeName ()
        {
            return "array";
        }

        public override string GetStringValue ()
        {
            return Array.ToString ();
        }

        public override ScalarAffinity GetScalarAffinity ()
        {
            return ScalarAffinity.ARRAY;
        }

    }

}
