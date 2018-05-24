using Gcodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes
{
    public enum TokenKind
    {
        G,
        M,
        X,
        Y,
        Integer,
        Float,
        Z,
        F,
        N,
    }

    public static class TokenKindExt
    {
        public static bool HasValue(this TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Integer:
                case TokenKind.Float:
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
