using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    public class LineNumber
    {
        public Span Span { get; }
        public int Number { get; }

        public LineNumber(int number, Span span)
        {
            Span = span;
            Number = number;
        }
    }
}
