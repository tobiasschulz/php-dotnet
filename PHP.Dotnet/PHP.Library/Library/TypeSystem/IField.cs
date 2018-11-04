using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.TypeSystem
{
    public interface IField : IElement<NameOfVariable>
    {
        Expression Initializer { get; }
        MemberAttributes Attributes { get; }
    }

    public sealed class Field : IField
    {
        public readonly NameOfVariable Name;
        public readonly Expression Initializer;
        public readonly MemberAttributes Attributes;

        public Field (NameOfVariable name, Expression initializer, MemberAttributes attributes)
        {
            Name = name;
            Initializer = initializer;
            Attributes = attributes;
        }

        NameOfVariable IElement<NameOfVariable>.Name => Name;
        Expression IField.Initializer => Initializer;
        MemberAttributes IField.Attributes => Attributes;
    }

    public interface IReadOnlyFieldCollection : IReadOnlyElementCollection<NameOfVariable, IField>
    {
    }

    public interface IFieldCollection : IReadOnlyFieldCollection, IElementCollection<NameOfVariable, IField>
    {
    }

    public sealed class MergedFieldCollection : MergedElementCollection<NameOfVariable, IField>, IFieldCollection, IReadOnlyFieldCollection
    {
        public MergedFieldCollection (IReadOnlyFieldCollection collection_parent, IFieldCollection collection_own)
            : base (collection_parent, collection_own)
        {
        }
    }

    public sealed class FieldCollection : ElementCollection<NameOfVariable, IField>, IFieldCollection, IReadOnlyFieldCollection
    {
    }
}
