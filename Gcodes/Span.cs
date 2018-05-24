using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes
{
    public struct Span
    {
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
