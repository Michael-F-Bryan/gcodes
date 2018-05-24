using Gcodes.Ast;
using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Gcode ParseGCode()
        {
            var start = index;

            var g = Chomp(TokenKind.G);
            if (g == null) return null;

            var numberTok = Chomp(TokenKind.Number);

            if (numberTok == null)
            {
                index = start;
                return null;
            }
            
            if (numberTok.Value.Contains('.') || numberTok.Value.Contains('-'))
            {
                throw new ParseException("The number for a \"G\" code should be a positive integer", numberTok.Span);
            }

            var number = int.Parse(numberTok.Value);

            var args = new List<Argument>();

            while (!Finished)
            {
                var arg = ParseArgument();
                if (arg != null)
                {
                    args.Add(arg);
                } else
                {
                    break;
                }
            }

            var span = args.Aggregate(g.Span.Merge(numberTok.Span), (acc, elem) => acc.Merge(elem.Span));

            return new Gcode(number, args, span);
        }

        internal Argument ParseArgument()
        {
            var kindTok = Chomp(TokenKind.F, TokenKind.X, TokenKind.Y, TokenKind.Z);
            if (kindTok == null) return null;

            var valueTok = Chomp(TokenKind.Number);
            if (valueTok == null)
                ThrowParseError(TokenKind.Number);

            var span = kindTok.Span.Merge(valueTok.Span);
            var value = double.Parse(valueTok.Value);
            var kind = kindTok.Kind.AsArgumentKind();

            return new Argument(kind, value, span);
        }

        internal LineNumber ParseLineNumber()
        {
            var n = Chomp(TokenKind.N);
            if (n == null) { return null; }

            var number = Chomp(TokenKind.Number);
            if (number == null)
            {
                ThrowParseError(TokenKind.Number);
            }

            if (number.Value.Contains('.') || number.Value.Contains("-"))
            {
                throw new ParseException("Line numbers must be positive integers");
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

        private Token Chomp(params TokenKind[] kind)
        {
            var tok = Peek();
            if (tok != null && kind.Contains(tok.Kind))
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
