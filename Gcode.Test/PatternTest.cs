using Gcodes.Tokens;
using System;
using Xunit;

namespace Gcodes.Test
{
    public class PatternTest
    {
        [Fact]
        public void CanInstantiate()
        {
            var pat = new Pattern(@"\GG", TokenKind.G);
        }

        [Fact]
        public void MustAlwaysMatchInputStart()
        {
            Assert.Throws<ArgumentException>(() => new Pattern(@"\d+", TokenKind.Number));
        }

        [Fact]
        public void MatchASimpleInteger()
        {
            var src = "123";
            var pat = new Pattern(@"\G\d+", TokenKind.Number);

            Assert.True(pat.TryMatch(src, 0, out Token tok));

            Assert.Equal(new Span(0, src.Length), tok.Span);
            Assert.Equal(TokenKind.Number, tok.Kind);
            Assert.Equal("123", tok.Value);
        }

        [Fact]
        public void NotMatched()
        {
            var src = "123";
            var pat = new Pattern(@"\G[a-z]+", TokenKind.Number);

            Assert.False(pat.TryMatch(src, 0, out Token tok));
            Assert.Null(tok);
        }

        [Fact]
        public void PatternIgnoreCase()
        {
            var pat = new Pattern(@"\Gasd", TokenKind.Number);


            Assert.True(pat.TryMatch("asd", 0, out Token tok));
            Assert.True(pat.TryMatch("ASD", 0, out tok));
            Assert.True(pat.TryMatch("AsD", 0, out tok));
        }
    }
}
