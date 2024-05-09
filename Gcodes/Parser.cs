using Gcodes.Ast;
using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcodes
{
    /// <summary>
    /// A parser for converting a stream of tokens into their corresponding
    /// Gcodes and Mcodes.
    /// </summary>
    public class Parser
    {
        private readonly List<Token> tokens;
        private int index;

        /// <summary>
        /// Create a new Parser using the provided list of tokens.
        /// </summary>
        /// <param name="tokens"></param>
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            index = 0;
        }
        /// <summary>
        /// Create a new <see cref="Parser"/>, automatically invoking the 
        /// <see cref="Lexer"/> to tokenize the input.
        /// </summary>
        /// <param name="src"></param>
        public Parser(string src) : this(new Lexer(src).Tokenize().ToList()) { }

        /// <summary>
        /// Is this parser finished?
        /// </summary>
        /// <returns></returns>
        public bool GetFinished()
        {
            // being finished can mean one of two things.
            // either we've run out of input
            if (index >= tokens.Count)
                return true;

            // or *everything* to the end of the file will be ignored (e.g. line numbers)
            // a line number is just a "N" followed by a number, so we just need to make
            // sure all odd tokens are "N" and the even ones are numbers
            var tokensLeft = tokens.Count - index;
            if (tokensLeft % 2 != 0) return false;

            for (int i = index; i < tokens.Count; i += 2)
            {
                if (tokens[i].Kind != TokenKind.N)
                    return false;
            }
            for (int j = index + 1; j < tokens.Count; j += 2)
            {
                if (tokens[j].Kind != TokenKind.Number)
                    return false;
            }

            return true;
        }
        public IEnumerable<Code> Parse()
        {
            while (!GetFinished())
            {
                yield return NextItem();
            }
        }

        internal Mcode ParseMCode()
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
        internal Gcode ParseGCode()
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

        internal Argument ParseArgument()
        {
            var kindTok = Chomp(TokenKind.F, TokenKind.P, TokenKind.S, TokenKind.H,
                TokenKind.X, TokenKind.Y, TokenKind.Z,
                TokenKind.I, TokenKind.J, TokenKind.K,
                TokenKind.A, TokenKind.B, TokenKind.C);
            if (kindTok == null) return null;

            var valueTok = Chomp(TokenKind.Number) ?? throw ParseError(TokenKind.Number);

            var span = kindTok.Span.Merge(valueTok.Span);
            var value = double.Parse(valueTok.Value, CultureInfo.InvariantCulture);
            var kind = kindTok.Kind.AsArgumentKind();

            return new Argument(kind, value, span);
        }

        private Code NextItem()
        {
            Code got = ParseGCode() ?? ParseMCode() ?? ParseTCode() ?? (Code)ParseOCode();

            if (got == null)
                throw ParseError(TokenKind.G, TokenKind.M, TokenKind.T, TokenKind.O);

            return got;
        }

        private List<Argument> ParseArguments()
        {
            var args = new List<Argument>();

            while (!GetFinished())
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
