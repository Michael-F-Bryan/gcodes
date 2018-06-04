using Gcodes.Ast;
using Gcodes.Tokens;
using System;
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
            Assert.True(parser.GetFinished());
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
        [InlineData('P')]
        [InlineData('X')]
        [InlineData('Y')]
        [InlineData('Z')]
        [InlineData('I')]
        [InlineData('J')]
        [InlineData('K')]
        [InlineData('S')]
        [InlineData('H')]
        public void CharacterIsRecognisedAsArgumentKind(char c)
        {
            var parser = new Parser($"{c}50.0");

            var got = parser.ParseArgument();

            Assert.NotNull(got);
            // use reflection to check there's a corresponding argument kind
            var variants = Enum.GetNames(typeof(ArgumentKind));
            var variantNameShouldBe = c.ToString().ToUpper();
            Assert.Contains(variantNameShouldBe, variants);
        }

        [Fact]
        public void FeedRateIsArgumentKind()
        {
            var parser = new Parser("F1200");

            var got = parser.ParseArgument();

            Assert.NotNull(got);
            Assert.Equal(ArgumentKind.FeedRate, got.Kind);
        }

        [Fact]
        public void AllArgumentKindsCanBeConvertedFromTokens()
        {
            var variants = Enum.GetNames(typeof(ArgumentKind))
                .Where(v => v != "FeedRate"); // feed rate doesn't follow the naming convention

            foreach (var variant in variants)
            {
                var tokenKind = (TokenKind) Enum.Parse(typeof(TokenKind), variant);
                var asArgumentKind = tokenKind.AsArgumentKind();

                var convertedTo = Enum.GetName(typeof(ArgumentKind), asArgumentKind);
                Assert.Equal(variant, convertedTo);
            }

            Assert.Equal(ArgumentKind.FeedRate, TokenKind.F.AsArgumentKind());
        }

        [Theory]
        [InlineData("circle.gcode")]
        [InlineData("simple_mill.gcode")]
        [InlineData("371373P.gcode")]
        public void ParseRealGcodes(string filename)
        {
            var src = EmbeddedFixture.ExtractFile(filename);
            var parser = new Parser(src);

            var got = parser.Parse().ToList();

            Assert.NotEmpty(got);
        }

        [Fact]
        public void IgnoreDuplicateLineNumbers()
        {
            var src = "N1 N2 N50 G1";
            var parser = new Parser(src);

            var got = parser.ParseLineNumber();

            Assert.NotNull(got);
            Assert.Equal(50, got.Number);
        }

        [Fact]
        public void ParserIsDoneWhenOnlyLineNumbersAreLeft()
        {
            var src = "N1 N2 N3 N4";
            var parser = new Parser(src);

            Assert.True(parser.GetFinished());
        }

        [Fact]
        public void BaumerCodeHas64Items()
        {
            var src = EmbeddedFixture.ExtractFile("371373P.gcode");
            var parser = new Parser(src);

            var numCodes = parser.Parse().Count();

            // manually counted the number of g/m codes in the baumer file :(
            Assert.Equal(64, numCodes);
        }
    }
}
