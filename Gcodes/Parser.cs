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

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            index = 0;
        }

        public bool Finished => index >= tokens.Count;

        public Parser(string src) : this(new Lexer(src).Tokenize().ToList()) { }

        public Mcode ParseMCode()
        {
            var start = index;
            var line = ParseLineNumber();

            var m = Chomp(TokenKind.M);
            if (m == null)
            {
                index = start;
                return null;
            }

            var numberTok = ParseInteger() ?? throw ParseError(TokenKind.Number); ;

            var number = int.Parse(numberTok.Value);
            var span = numberTok.Span.Merge(m.Span);
            span = line == null ? span : span.Merge(line.Span);

            return new Mcode(number, span, line?.Number);
        }

        internal Tcode ParseTCode()
        {
            var start = index;
            var line = ParseLineNumber();

            var t = Chomp(TokenKind.T);
            if (t == null)
            {
                index = start;
                return null;
            }

            var numberTok = ParseInteger() ?? throw ParseError(TokenKind.Number); ;

            var number = int.Parse(numberTok.Value);
            var span = numberTok.Span.Merge(t.Span);
            span = line == null ? span : span.Merge(line.Span);

            return new Tcode(number, span, line?.Number);
        }

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

            var line = ParseLineNumber();

            var g = Chomp(TokenKind.G);
            if (g == null)
            {
                index = start;
                return null;
            }

            var numberTok = ParseInteger() ?? throw ParseError(TokenKind.Number); ;
            var number = int.Parse(numberTok.Value);
            var args = ParseArguments();

            var span = args.Aggregate(g.Span.Merge(numberTok.Span), (acc, elem) => acc.Merge(elem.Span));

            if (line != null)
            {
                span = span.Merge(line.Span);
            }

            return new Gcode(number, args, span, line?.Number);
        }

        private List<Argument> ParseArguments()
        {
            var args = new List<Argument>();

            while (!Finished)
            {
                var arg = ParseArgument();
                if (arg != null)
                {
                    args.Add(arg);
                }
                else
                {
                    break;
                }
            }

            return args;
        }

        private Token ParseInteger()
        {
            var numberTok = Chomp(TokenKind.Number);

            if (numberTok == null)
            {
                return null;
            }

            if (numberTok.Value.Contains('.') || numberTok.Value.Contains('-'))
            {
                throw new ParseException("The number for a \"G\" code should be a positive integer", numberTok.Span);
            }

            return numberTok;
        }

        internal Argument ParseArgument()
        {
            var kindTok = Chomp(TokenKind.F, TokenKind.P,
                TokenKind.X, TokenKind.Y, TokenKind.Z,
                TokenKind.I, TokenKind.J, TokenKind.K,
                TokenKind.A, TokenKind.B, TokenKind.C);
            if (kindTok == null) return null;

            var valueTok = Chomp(TokenKind.Number) ?? throw ParseError(TokenKind.Number);

            var span = kindTok.Span.Merge(valueTok.Span);
            var value = double.Parse(valueTok.Value);
            var kind = kindTok.Kind.AsArgumentKind();

            return new Argument(kind, value, span);
        }

        public IEnumerable<Code> Parse()
        {
            while (!Finished)
            {
                yield return NextItem();
            }
        }

        private Code NextItem()
        {
            Code got = ParseGCode() ?? ParseMCode() ?? ParseTCode() ?? (Code)ParseOCode();

            if (got == null)
                throw ParseError(TokenKind.G, TokenKind.M, TokenKind.T, TokenKind.O);

            return got;
        }

        internal Ocode ParseOCode()
        {
            var start = index;
            var line = ParseLineNumber();

            var O = Chomp(TokenKind.O);
            if (O == null)
            {
                index = start;
                return null;
            }

            var numberTok = ParseInteger() ?? throw ParseError(TokenKind.Number); ;

            var span = O.Span.Merge(numberTok.Span);
            if (line != null)
            {
                span = span.Merge(line.Span);
            }

            return new Ocode(int.Parse(numberTok.Value), span, line?.Number);
        }

        /// <summary>
        /// Parses a line number (<c>"N50"</c>). If there are multiple 
        /// consecutive line numbers, this skips to the last line number and
        /// matches that.
        /// </summary>
        /// <returns></returns>
        internal LineNumber ParseLineNumber()
        {
            Token n = null;
            Token numberTok = null;

            do
            {
                n = Chomp(TokenKind.N);
                if (n == null) break;

                numberTok = ParseInteger() ?? throw ParseError(TokenKind.Number);
            } while (Peek()?.Kind == TokenKind.N);
                
            if (n == null || numberTok == null)
            {
                return null;
            }

            return new LineNumber(int.Parse(numberTok.Value), n.Span.Merge(numberTok.Span));
        }

        private Exception ParseError(params TokenKind[] expected)
        {
            var next = Peek();

            if (next != null)
            {
                return new UnexpectedTokenException(expected, next.Kind, next.Span);
            }
            else
            {
                return new UnexpectedEOFException(expected);
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
