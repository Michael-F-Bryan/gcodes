using Gcodes.Ast;
using Gcodes.Tokens;
using System;
using Xunit;

namespace Gcodes.Test
{
    public class TokenKindTest
    {
        [Theory]
        [InlineData(TokenKind.X, ArgumentKind.X)]
        [InlineData(TokenKind.Y, ArgumentKind.Y)]
        [InlineData(TokenKind.Z, ArgumentKind.Z)]
        [InlineData(TokenKind.F, ArgumentKind.FeedRate)]
        public void ConvertTokenKindsToArgumentKinds(TokenKind src, ArgumentKind shouldBe)
        {
            var got = src.AsArgumentKind();
            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData(TokenKind.Number)]
        [InlineData(TokenKind.G)]
        [InlineData(TokenKind.M)]
        [InlineData(TokenKind.N)]
        public void CantConvertInvalidTokenKinds(TokenKind src)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => src.AsArgumentKind());
        }
    }
}
