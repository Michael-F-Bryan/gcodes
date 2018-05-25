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
        List<Regex> skips;
        string src;
        int pointer;
        int lineNumber;

        public bool Finished => pointer >= src.Length;

        public event EventHandler<CommentEventArgs> CommentDetected;

        public Lexer(string src)
        {
            skips = new List<Regex>
            {
                new Regex(@"\G\s+", RegexOptions.Compiled),
                new Regex(@"\G;([^\n\r]*)", RegexOptions.Compiled),
                new Regex(@"\G\(([^)\n\r]*)\)", RegexOptions.Compiled),
            };
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
            while (!Finished)
            {
                SkipStuff();
                if (Finished) break;
                yield return NextToken();
            }
        }

        private void SkipStuff()
        {
            int currentPass;

            do
            {
                currentPass = pointer;

                foreach (var skip in skips)
                {
                    var match = skip.Match(src, pointer);

                    if (match.Success)
                    {
                        pointer += match.Length;
                        lineNumber += match.Value.Count(c => c == '\n');
                        OnCommentDetected(match);
                    }
                }
            } while (pointer < src.Length && pointer != currentPass);
        }

        private void OnCommentDetected(Match match)
        {
            for (int i = 1; i < match.Groups.Count; i++)
            {
                var group = match.Groups[i];
                if (group.Success)
                {
                    CommentDetected?.Invoke(this, new CommentEventArgs(group.Value));
                    break;
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
            var lastNewline = src.LastIndexOf('\n', pointer);
            return lastNewline < 0 ? pointer : pointer - lastNewline;
        }
    }

    public class CommentEventArgs : EventArgs
    {
        public CommentEventArgs(string comment)
        {
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

        public string Comment { get; }
    }
}
