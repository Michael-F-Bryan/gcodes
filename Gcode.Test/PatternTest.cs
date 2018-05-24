using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcode.Test
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
            Assert.Throws<ArgumentException>(() => new Pattern(@"\d+", TokenKind.Integer));
        }

        [Fact]
        public void MatchASimpleInteger()
        {
            var src = "123";
            var pat = new Pattern(@"\G\d+", TokenKind.Integer);

            Assert.True(pat.TryMatch(src, 0, out Token tok));

            Assert.Equal(new Span(0, src.Length), tok.Span);
            Assert.Equal(TokenKind.Integer, tok.Kind);
            Assert.Equal("123", tok.Value);
        }

        [Fact]
        public void NotMatched()
        {
            var src = "123";
            var pat = new Pattern(@"\G[a-z]+", TokenKind.Integer);

            Assert.False(pat.TryMatch(src, 0, out Token tok));
            Assert.Null(tok);
        }
    }
}
