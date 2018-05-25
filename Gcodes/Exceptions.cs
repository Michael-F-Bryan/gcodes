using Gcodes.Tokens;
using System;

namespace Gcodes
{

    [Serializable]
    public class GcodeException : Exception
    {
        public GcodeException() { }
        public GcodeException(string message) : base(message) { }
        public GcodeException(string message, Exception inner) : base(message, inner) { }
        protected GcodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class UnrecognisedCharacterException : GcodeException
    {
        public int Line { get; }
        public int Column { get; }
        public char Character { get; }

        public UnrecognisedCharacterException(int line, int column, char character)
            : this($"Unrecognised character \"{character}\" at line {line} column {column}")
        {
            Line = line;
            Column = column;
            Character = character;
        }
        public UnrecognisedCharacterException() { }
        public UnrecognisedCharacterException(string message) : base(message) { }
        public UnrecognisedCharacterException(string message, Exception inner) : base(message, inner) { }
        protected UnrecognisedCharacterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class ParseException : GcodeException
    {
        public Span Span { get; } = Span.Empty;
        public ParseException() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Span span) : base(message)
        {
            Span = span;
        }
        public ParseException(string message, Exception inner) : base(message, inner) { }
        protected ParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class UnexpectedEOFException : ParseException
    {
        public TokenKind[] Expected { get; }

        public UnexpectedEOFException(TokenKind[] expected)
            : this($"Expected one of [{string.Join(", ", expected)}] but reached the end of input")
        {
            Expected = expected;
        }
        public UnexpectedEOFException() { }
        public UnexpectedEOFException(string message) : base(message) { }
        public UnexpectedEOFException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedEOFException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UnexpectedTokenException : ParseException
    {
        public TokenKind[] Expected { get; }
        public TokenKind Found { get; }

        public UnexpectedTokenException(TokenKind[] expected, TokenKind found, Span span)
            : base($"Expected one of [{string.Join(", ", expected)}] but found {found}", span)
        {
            Expected = expected;
            Found = found;
        }

        public UnexpectedTokenException() { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
