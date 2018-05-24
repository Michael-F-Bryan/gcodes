using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcodes.Test
{
    public class ParserTest
    {
        [Fact]
        public void BoringGcode()
        {
            var src = "G01";
            var parser = new Parser(src);

            var got = parser.NextGcode();

            Assert.Equal(1, got.Number);
            Assert.Equal(new Span(0, 3), got.Span);
        }

        [Fact]
        public void ParseLineNumber()
        {
            var src = "n10";
            var parser = new Parser(src);

            var got = parser.ParseLineNumber();

            Assert.Equal(10, got.Number);
            Assert.Equal(new Span(0, 3), got.Span);
            Assert.True(parser.Finished);
        }

        [Theory]
        [InlineData("N50.0")]
        [InlineData("N-23")]
        public void LineNumbersAreIntegers(string src)
        {
            var parser = new Parser(src);
            Assert.Throws<ParseException>(() => parser.ParseLineNumber());
        }

        [Theory]
        [InlineData("X50", ArgumentKind.X, 50.0)]
        [InlineData("F-30.5", ArgumentKind.FeedRate, -30.5)]
        public void ParseAnArgument(string src, ArgumentKind kind, double value)
        {
            var parser = new Parser(src);
            var shouldBe = new Argument(kind, value, new Span(0, src.Length));

            var got = parser.ParseArgument();

            Assert.Equal(shouldBe, got);
        }
    }
}
