using System;

namespace Gcodes.Tokens
{
    public struct Span
    {
        /// <summary>
        ///  The empty span.
        /// </summary>
        public static Span Empty = new Span(0, 0);

        public int Start { get; }
        public int End { get; }

        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start}:{End}";
        }

        public Span Merge(Span other)
        {
            return new Span(Math.Min(Start, other.Start), Math.Max(End, other.End));
        }
    }
}
