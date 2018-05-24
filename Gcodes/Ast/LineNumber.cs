using Gcodes.Tokens;

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
