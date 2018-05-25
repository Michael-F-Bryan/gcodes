using Gcodes.Ast;
using System;

namespace Gcodes.Tokens
{
    /// <summary>
    /// The various possible kinds of tokens.
    /// </summary>
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

    /// <summary>
    /// Extension methods for the <see cref="Token"/> enum.
    /// </summary>
    public static class TokenKindExt
    {
        /// <summary>
        /// Does this <see cref="TokenKind"/> have a meaningful string value?
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Try to convert the <see cref="TokenKind"/> into an 
        /// <see cref="ArgumentKind"/>, if applicable.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Raised if there is no corresponding <see cref="ArgumentKind"/> for
        /// this <see cref="TokenKind"/>.
        /// </exception>
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

                case TokenKind.H:
                    return ArgumentKind.H;
                case TokenKind.P:
                    return ArgumentKind.P;
                case TokenKind.S:
                    return ArgumentKind.S;
                case TokenKind.F:
                    return ArgumentKind.FeedRate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), "No equivalent argument kind");
            }
        }
    }
}
