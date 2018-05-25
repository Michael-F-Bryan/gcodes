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
        A,
        B,
        C,
        H,
        P,
        K,
        O,
        T,
        S,
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

                case TokenKind.A:
                    return ArgumentKind.A;
                case TokenKind.B:
                    return ArgumentKind.B;
                case TokenKind.C:
                    return ArgumentKind.C;

                case TokenKind.I:
                    return ArgumentKind.I;
                case TokenKind.J:
                    return ArgumentKind.J;
                case TokenKind.K:
                    return ArgumentKind.K;

                case TokenKind.P:
                    return ArgumentKind.P;
                case TokenKind.F:
                    return ArgumentKind.FeedRate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), "No equivalent argument kind");
            }
        }
    }
}
