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

            var got = parser.ParseGCode();

            Assert.Equal(1, got.Number);
            Assert.Equal(new Span(0, 3), got.Span);
        }

        [Fact]
        public void MoreInterestingGcode()
        {
            var src = "G555 X-23.4 F1200 Y-0.0 Z+3.1415";
            var parser = new Parser(src);

            var got = parser.ParseGCode();

            Assert.Equal(555, got.Number);
            Assert.Equal(new Span(0, src.Length), got.Span);
            Assert.Equal(4, got.Arguments.Count);

            Assert.Equal(-23.4, got.ValueFor(ArgumentKind.X));
            Assert.Equal(1200, got.ValueFor(ArgumentKind.FeedRate));
            Assert.Equal(0, got.ValueFor(ArgumentKind.Y));
            Assert.Equal(3.1415, got.ValueFor(ArgumentKind.Z));
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
