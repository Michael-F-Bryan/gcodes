using Gcodes.Ast;
using Gcodes.Tokens;
using System.Collections.Generic;
using System.Linq;
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

            var shouldBe = new Gcode(1, new List<Argument>(), new Span(0, src.Length));
            Assert.Equal(shouldBe, got);
        }

        [Fact]
        public void BoringMcode()
        {
            var src = "N10 M5";
            var parser = new Parser(src);

            var got = parser.ParseMCode();

            var shouldBe = new Mcode(5, new Span(0, src.Length), 10);
            Assert.Equal(shouldBe, got);
        }

        [Fact]
        public void ProgramNumber()
        {
            var src = "N10 O500";
            var parser = new Parser(src);

            var got = parser.ParseOCode();

            var shouldBe = new Ocode(500, new Span(0, src.Length), 10);
            Assert.Equal(shouldBe, got);
        }

        [Fact]
        public void TCode()
        {
            var src = "N10 T30";
            var parser = new Parser(src);

            var got = parser.ParseTCode();

            var shouldBe = new Tcode(30, new Span(0, src.Length), 10);
            Assert.Equal(shouldBe, got);
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

        [Theory]
        [InlineData('a')]
        [InlineData('b')]
        [InlineData('C')]
        [InlineData('f')]
        [InlineData('P')]
        [InlineData('X')]
        [InlineData('Y')]
        [InlineData('Z')]
        [InlineData('I')]
        [InlineData('J')]
        [InlineData('K')]
        public void CharacterIsArgumentKind(char c)
        {
            var parser = new Parser($"{c}50.0");

            var got = parser.ParseArgument();

            Assert.NotNull(got);
        }

        [Theory]
        [InlineData("circle.txt")]
        [InlineData("simple_mill.txt")]
        [InlineData("371373P.gcode")]
        public void ParseRealGcodes(string filename)
        {
            var src = EmbeddedFixture.ExtractFile(filename);
            var parser = new Parser(src);

            var got = parser.Parse().ToList();

            Assert.NotEmpty(got);
        }
    }
}
