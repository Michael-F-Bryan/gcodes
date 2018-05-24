using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcode.Test
{
    public class LexerTest
    {
        [Fact]
        public void CanInstantiate()
        {
            var lexer = new Lexer("asd");
        }

        [Fact]
        public void DetectInvalidCharacters()
        {
            var shouldBe = new UnrecognisedCharacterException(0, '$');
            var lexer = new Lexer("$Foo");

            try
            {
                lexer.Tokenize().ToList();
            }
            catch (UnrecognisedCharacterException got)
            {
                Assert.Equal(shouldBe.Index, got.Index);
                Assert.Equal(shouldBe.Character, got.Character);
                return;
            }

            throw new Exception("No exception was thrown");
        }

        [Theory]
        [InlineData("12", TokenKind.Integer)]
        [InlineData("1.23", TokenKind.Float)]
        [InlineData("G", TokenKind.G)]
        [InlineData("M", TokenKind.M)]
        [InlineData("X", TokenKind.X)]
        [InlineData("Y", TokenKind.Y)]
        [InlineData("Z", TokenKind.Z)]
        [InlineData("F", TokenKind.F)]
        public void RecogniseStandardTokens(string src, TokenKind kind)
        {
            var lexer = new Lexer(src);
            var tok = lexer.Tokenize().First();

            Assert.Equal(kind, tok.Kind);
        }

        [Fact]
        public void SkipComments()
        {
            var lexer = new Lexer("; this is a comment\nG13");

            var tok = lexer.Tokenize().First();
            Assert.Equal(TokenKind.G, tok.Kind);
        }

        [Fact]
        public void SkipWhitespace()
        {
            var lexer = new Lexer(" G");

            var tok = lexer.Tokenize().First();
            Assert.Equal(TokenKind.G, tok.Kind);
        }
    }
}
