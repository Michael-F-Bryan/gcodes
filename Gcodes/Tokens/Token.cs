using System;
using System.Collections.Generic;
using System.Text;

namespace Gcodes.Tokens
{
    /// <summary>
    /// The smallest atomic unit in a gcode file, containing a token kind,
    /// the original string, and its location in the source text (as a
    /// <see cref="Span"/>).
    /// </summary>
    public class Token : IEquatable<Token>
    {
        /// <summary>
        /// Create a new <see cref="Token"/> which doesn't have a useful
        /// string value (e.g. punctuation).
        /// </summary>
        /// <param name="span"></param>
        /// <param name="kind"></param>
        public Token(Span span, TokenKind kind) : this(span, kind, null) { }
        /// <summary>
        /// Create a new <see cref="Token"/> out of its constituent parts.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="kind"></param>
        /// <param name="value"></param>
        public Token(Span span, TokenKind kind, string value)
        {
            Span = span;
            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Where the <see cref="Token"/> lies in its source text.
        /// </summary>
        public Span Span { get; }
        /// <summary>
        /// Which kind of token is this?
        /// </summary>
        public TokenKind Kind { get; }
        /// <summary>
        /// The token's original string text. May be <c>null</c> if the token
        /// kind doesn't care about its source text.
        /// </summary>
        public string Value { get; }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as Token);
        }

        public bool Equals(Token other)
        {
            return other != null &&
                   EqualityComparer<Span>.Default.Equals(Span, other.Span) &&
                   Kind == other.Kind &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1030702410;
            hashCode = hashCode * -1521134295 + EqualityComparer<Span>.Default.GetHashCode(Span);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Kind.ToString());

            if (Kind.HasValue())
            {
                sb.AppendFormat("({0})", Value);
            }

            sb.AppendFormat(" @ {0}", Span);

            return sb.ToString();
        }

        public static bool operator ==(Token token1, Token token2)
        {
            return EqualityComparer<Token>.Default.Equals(token1, token2);
        }

        public static bool operator !=(Token token1, Token token2)
        {
            return !(token1 == token2);
        } 
        #endregion
    }
}
