using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gcodes
{
    public class Lexer
    {
        List<Pattern> patterns;
        Regex skips;
        string src;
        int pointer;
        int lineNumber;

        public Lexer(string src)
        {
            skips = new Regex(@"\G\s+|;[^\n]*", RegexOptions.Compiled);
            this.src = src;
            pointer = 0;
            lineNumber = 0;

            patterns = new List<Pattern>
            {
                new Pattern(@"\GG", TokenKind.G),
                new Pattern(@"\GN", TokenKind.N),
                new Pattern(@"\GM", TokenKind.M),
                new Pattern(@"\GX", TokenKind.X),
                new Pattern(@"\GY", TokenKind.Y),
                new Pattern(@"\GZ", TokenKind.Z),
                new Pattern(@"\GF", TokenKind.F),
                new Pattern(@"\GI", TokenKind.I),
                new Pattern(@"\GJ", TokenKind.J),

                new Pattern(@"\G[-+]?(\d+\.\d+|\.\d+|\d+\.?)", TokenKind.Number),
            };
        }

        public IEnumerable<Token> Tokenize()
        {
            while (pointer < src.Length)
            {
                SkipStuff();
                yield return NextToken();
            }
        }

        private void SkipStuff()
        {
            while (pointer < src.Length)
            {
                var match = skips.Match(src, pointer);

                if (match.Success)
                {
                    pointer += match.Length;
                    lineNumber += match.Value.Count(c => c == '\n');
                }
                else
                {
                    return;
                }
            }
        }

        private Token NextToken()
        {
            foreach (var pat in patterns)
            {
                if (pat.TryMatch(src, pointer, out Token tok))
                {
                    pointer = tok.Span.End;
                    if (tok.Value != null)
                    {
                        lineNumber += tok.Value.Count(c => c == '\n');
                    }
                    return tok;
                }
            }

            var column = CurrentColumn();
            throw new UnrecognisedCharacterException(lineNumber + 1, column + 1, src[pointer]);
        }

        private int CurrentColumn()
        {
            var lastNewline = src.LastIndexOf('\n', pointer, pointer);

            return lastNewline < 0 ? pointer : pointer - lastNewline;
        }
    }
}
