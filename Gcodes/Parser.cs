using Gcodes.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int index;

        public Parser(IEnumerable<Token> tokens)
        {
            this.tokens = tokens.ToList() ?? throw new ArgumentNullException(nameof(tokens));
            index = 0;
        }

        public bool Finished => index >= tokens.Count;

        public Parser(string src) : this(new Lexer(src).Tokenize()) { }

        /// <summary>
        /// Parse a single gcode.
        /// </summary>
        /// <remarks>
        /// This roughly matches the following grammar:
        /// <code>
        /// gcode := [line_number] "G" INTEGER argument*
        /// line_number := "N" INTEGER
        /// argument := "X" FLOAT
        ///           | "
        /// </code>
        /// </remarks>
        /// <returns></returns>
        public Gcode NextGcode()
        {
            var start = index;

            var g = Chomp(TokenKind.G);
            var number = Chomp(TokenKind.Integer);

            if (g == null || number == null)
            {
                index = start;
                return null;
            }
            throw new NotImplementedException();
        }

        internal Argument ParseArgument()
        {
            throw new NotImplementedException();
        }

        internal LineNumber ParseLineNumber()
        {
            var n = Chomp(TokenKind.N);
            if (n == null) { return null; }

            var number = Chomp(TokenKind.Integer);
            if (number == null)
            {
                ThrowParseError(TokenKind.Integer);
            }

            return new LineNumber(int.Parse(number.Value), n.Span.Merge(number.Span));
        }

        private void ThrowParseError(params TokenKind[] expected)
        {
            var next = Peek();

            if (next != null)
            {
                throw new UnexpectedTokenException(expected, next.Kind, next.Span);
            }
            else
            {
                throw new UnexpectedEOFException(expected);
            }
        }

        private Token Chomp(TokenKind kind)
        {
            var tok = Peek();
            if (tok?.Kind == kind)
            {
                index += 1;
                return tok;
            }
            else
            {
                return null;
            }
        }

        private Token Peek()
        {
            if (index < tokens.Count)
            {
                return tokens[index];
            }
            else
            {
                return null;
            }
        }
    }
}
