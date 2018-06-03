using System;
using System.Linq;

namespace Gcodes.Tokens
{
    /// <summary>
    /// A location in the source text as a pair of byte indices.
    /// </summary>
    public struct Span
    {
        /// <summary>
        ///  The empty span.
        /// </summary>
        public static Span Empty = new Span(0, 0);

        /// <summary>
        /// The index a segment of text starts at.
        /// </summary>
        public int Start { get; }
        /// <summary>
        /// The index <b>one after</b> the end of the selected text.
        /// </summary>
        public int End { get; }
        public int Length { get => End - Start; }

        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Calculate the line number this <see cref="Span"/> starts at.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public int LineNumber(string src)
        {
            var start = Start;
            return src.Where((_, i) => i < start).Where(c => c == '\n').Count() + 1;
        }

        /// <summary>
        /// Determine which column this span starts in.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public int ColumnNumber(string src)
        {
            var lastNewline = src.LastIndexOf('\n', Start);
            return lastNewline < 0 ? Start : Start - lastNewline;
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }

        /// <summary>
        /// Retrieve the <see cref="Span"/> encompassing two spans.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Span Merge(Span other)
        {
            return new Span(Math.Min(Start, other.Start), Math.Max(End, other.End));
        }
    }
}
