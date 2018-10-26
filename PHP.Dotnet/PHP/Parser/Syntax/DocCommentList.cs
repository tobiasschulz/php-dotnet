using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Helper class containing list of DOC comments during tokenization.
    /// Provides searching for DOC comment above given position.
    /// </summary>
    internal class DocCommentList
    {
        private struct DocInfo
        {
            /// <summary>
            /// DOC comment instance.
            /// </summary>
            public PHPDocBlock PhpDoc;

            /// <summary>
            /// DOC comment span including following whitespace.
            /// </summary>
            public Span2 Extent;

            /// <summary>
            /// Determines whether this block is above given element.
            /// </summary>
            public bool IsAbove(LangElement element) => element != null && Extent.End <= element.Span.Start;

            /// <summary>
            /// Determines whether this block is below given element or element is <c>null</c>.
            /// </summary>
            public bool IsBelowOrNull(LangElement element) => element == null || Extent.End > element.Span.End;
        }

        #region Fields & Properties

        /// <summary>
        /// Ordered list of DOC comments. Can be <c>null</c>.
        /// </summary>
        private List<DocInfo> _doclist = new List<DocInfo>();

        /// <summary>
        /// Extent of included DOC comments span.
        /// </summary>
        public Span2 Extent { get { return _span; } }
        private Span2 _span = Span2.Invalid;

        #endregion

        /// <summary>
        /// Inserts DOC block into the list.
        /// </summary>
        public void AppendBlock(PHPDocBlock/*!*/phpDoc, int endPosition)
        {
            Debug.Assert(phpDoc != null);
            Debug.Assert(endPosition >= phpDoc.Span.End);
            Debug.Assert(_doclist == null || _doclist.Count == 0 || _doclist.Last().Extent.Start < phpDoc.Span.Start, "Blocks have to be appended in order.");

            var docinfo = new DocInfo()
            {
                PhpDoc = phpDoc,
                Extent = Span2.FromBounds(phpDoc.Span.Start, endPosition)
            };

            var list = _doclist;
            if (list == null)
            {
                _doclist = list = new List<DocInfo>(4);
            }

            list.Add(docinfo);

            _span = Span2.FromBounds(list[0].Extent.Start, list.Last().Extent.End);
        }

        /// <summary>
        /// Finds DOC comment above given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseBlock(int position, out PHPDocBlock phpdoc)
        {
            var index = this.FindIndex(position - 1);
            return TryReleaseIndexedBlock(index, out phpdoc);
        }

        /// <summary>
        /// Finds DOC comment inside given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseBlock(Span2 position, out PHPDocBlock phpdoc)
        {
            var index = FindFirstIn(_doclist, position);
            return TryReleaseIndexedBlock(index, out phpdoc);
        }

        /// <summary>
        /// Finds DOC comment above given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseIndexedBlock(int index, out PHPDocBlock phpdoc)
        {
            if (index >= 0 && index < _doclist.Count)
            {
                var list = _doclist;
                phpdoc = list[index].PhpDoc;
                list.RemoveAt(index);
                this.UpdateSpan();

                //
                return true;
            }

            //
            phpdoc = null;
            return false;
        }

        /// <summary>
        /// Finds DOC comment at given position and annotates statement with it.
        /// </summary>
        public void Annotate(LangElement element)
        {
            PHPDocBlock phpdoc;
            if (TryReleaseBlock(element.Span.Start, out phpdoc))
            {
                element.SetPHPDoc(phpdoc);
            }
        }

        /// <summary>
        /// Finds DOC comment at given position and annotates statement with it.
        /// </summary>
        public void AnnotateMember(LangElement element)
        {
            PHPDocBlock phpdoc;
            if (LastDocBlock != null && LastDocBlock.Span.End > element.Span.Start && 
                TryReleaseBlock(Span2.Combine(element.Span, LastDocBlock.Span), out phpdoc))
            {
                element.SetPHPDoc(phpdoc);
            }
        }

        /// <summary>
        /// Merges DOC comments into the list of statements.
        /// </summary>
        /// <param name="extent">Span of code block containing <paramref name="stmts"/>.</param>
        /// <param name="stmts">List of statements to be merged with overlapping DOC comments.</param>
        /// <param name="factory">Factory used to create PHPDoc statements.</param>
        public void Merge(Text.Span2 extent, IList<LangElement>/*!*/stmts, INodesFactory<LangElement, Span2> factory)
        {
            Debug.Assert(extent.IsValid);
            Debug.Assert(stmts != null);

            if (_span.OverlapsWith(extent))
            {
                var list = _doclist;
                int insertAt = 0;
                int count = 0;

                DocInfo doc;
                int indexFrom = FindFirstIn(list, extent);

                for (var index = indexFrom; index < list.Count && (doc = list[index]).Extent.OverlapsWith(extent); index++)
                {
                    // skip stmts fully above {doc}
                    while (insertAt < stmts.Count && doc.IsBelowOrNull(stmts[insertAt]))
                    {
                        insertAt++;
                    }

                    // insert {doc} into {stmts}
                    if (insertAt == stmts.Count || doc.IsAbove(stmts[insertAt]))
                    {
                        stmts.Insert(insertAt, factory.PHPDoc(doc.PhpDoc.Span, doc.PhpDoc));
                        insertAt++;
                    }

                    //
                    count++;
                }

                //
                if (count != 0)
                {
                    list.RemoveRange(indexFrom, count);
                    this.UpdateSpan();
                }
            }
        }

        private void UpdateSpan()
        {
            var list = _doclist;
            _span = (list == null || list.Count == 0)
                    ? Span2.Invalid
                    : Span2.FromBounds(list[0].Extent.Start, list.Last().Extent.End);
        }

        /// <summary>
        /// Finds index of DOC comment at given position.
        /// </summary>
        private int FindIndex(int position)
        {
            if (_span.Contains(position))
            {
                Debug.Assert(_doclist != null);
                int index = FindIndex(_doclist, position);
                if (index >= 0 && _doclist[index].Extent.Contains(position))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Binary search.
        /// </summary>
        private static int FindIndex(List<DocInfo>/*!*/list, int position)
        {
            int a = 0, b = list.Count - 1;
            while (a <= b)
            {
                int x = (a + b) >> 1;
                var doc = list[x];
                if (doc.Extent.Contains(position))
                    return x;

                if (position > doc.Extent.Start)
                    a = x + 1;
                else
                    b = x - 1;
            }

            return -1;
        }

        /// <summary>
        /// Gets lowest index of DOC comment that intersects given span. Returns count of items if nothing was found.
        /// </summary>
        private static int FindFirstIn(List<DocInfo>/*!*/list, Span2 span)
        {
            Debug.Assert(span.IsValid);
            Debug.Assert(list != null);

            int result = list.Count;
            int a = 0, b = result - 1;
            while (a <= b)
            {
                // modified binary search to find lowest index of element within {span}

                int x = (a + b) >> 1;
                var doc = list[x];
                if (doc.Extent.Start >= span.End)
                {
                    b = x - 1;
                }
                else if (doc.Extent.End <= span.Start)
                {
                    a = x + 1;
                }
                else
                {
                    result = x;
                    b = x - 1;  // and try lower
                }
            }

            //
            return result;
        }

        public PHPDocBlock LastDocBlock => _doclist.Count > 0? _doclist.Last().PhpDoc: null;
    }
}
