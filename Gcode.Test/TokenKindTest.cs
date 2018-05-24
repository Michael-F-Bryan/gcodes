﻿using Gcodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcode.Test
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