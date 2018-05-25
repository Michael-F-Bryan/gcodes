using Gcodes.Tokens;
using System;
using System.Collections.Generic;

namespace Gcodes.Ast
{
    public class LineNumber : IEquatable<LineNumber>
    {
        public Span Span { get; }
        public int Number { get; }

        public LineNumber(int number, Span span)
        {
            Span = span;
            Number = number;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LineNumber);
        }

        public bool Equals(LineNumber other)
        {
            return other != null &&
                   EqualityComparer<Span>.Default.Equals(Span, other.Span) &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            var hashCode = 1293783753;
            hashCode = hashCode * -1521134295 + EqualityComparer<Span>.Default.GetHashCode(Span);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(LineNumber number1, LineNumber number2)
        {
            return EqualityComparer<LineNumber>.Default.Equals(number1, number2);
        }

        public static bool operator !=(LineNumber number1, LineNumber number2)
        {
            return !(number1 == number2);
        }
    }
}
