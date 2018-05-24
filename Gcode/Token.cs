using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcode
{
    public class Token
    {
        TokenKind kind;
        string value;

        public Token(Span span, TokenKind kind, string value)
        {
            Span = span;
            Kind = kind;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Span Span { get; }
        public TokenKind Kind { get; }
        public string Value { get; }
    }

    public enum TokenKind
    {
        G,
        M,
        X,
        Y,
        Integer,
        Float,
    }
}
