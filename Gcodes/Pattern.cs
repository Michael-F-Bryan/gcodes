using System;
using System.Text.RegularExpressions;

namespace Gcodes
{
    public class Pattern
    {
        Regex regex;
        TokenKind kind;

        public Pattern(string pattern, TokenKind kind)
        {
            if (!pattern.StartsWith(@"\G"))
                throw new ArgumentException("Pattern must match the start of input", nameof(pattern));

            regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.kind = kind;
        }

        public bool TryMatch(string src, int startIndex, out Token tok)
        {
            var match = regex.Match(src, startIndex);

            if (match.Success)
            {
                var span = new Span(startIndex, startIndex + match.Length);

                if (kind.HasValue())
                {
                    tok = new Token(span, kind, match.Value);
                }
                else
                {
                    tok = new Token(span, kind);
                }
                return true;
            }
            else
            {
                tok = null;
                return false;
            }
        }
    }
}