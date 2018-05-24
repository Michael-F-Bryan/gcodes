using Gcodes.Ast;
using System;

namespace Gcodes.Tokens
{
    public enum TokenKind
    {
        G,
        M,
        X,
        Y,
        Number,
        Z,
        F,
        N,
        I,
        J,
    }

    public static class TokenKindExt
    {
        public static bool HasValue(this TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Number:
                    return true;
                default:
                    return false;
            }
        }

        public static ArgumentKind AsArgumentKind(this TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.X:
                    return ArgumentKind.X;
                case TokenKind.Y:
                    return ArgumentKind.Y;
                case TokenKind.Z:
                    return ArgumentKind.Z;
                case TokenKind.F:
                    return ArgumentKind.FeedRate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), "No equivalent argument kind");
            }
        }
    }
}
