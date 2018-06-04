using System;
using Gcodes.Tokens;
using Xunit;

namespace Gcodes.Test
{
    public class FileMapTest
    {

        [Fact]
        public void CanInstantiate()
        {
            var src = "Hello World";

            var fm = new FileMap(src);
        }

        [Theory]
        [InlineData(5, 1, 6, 'i')]
        // end of the first line
        [InlineData(65, 1, 66, '/')]
        [InlineData(70, 3, 1, 'G')]
        [InlineData(75, 3, 6, '2')]
        public void YouCanLookUpLocationDetails(int index, int line, int column, char sanityCheck)
        {
            var src = EmbeddedFixture.ExtractFile("circle.gcode");
            Assert.Equal(sanityCheck, src[index]);
            var map = new FileMap(src);
            var shouldBe = new Location(index, line, column);

            var got = map.LocationFor(index);

            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData(3, "\nabcdefghijk", 3, 'c')]
        [InlineData(3, "abcdefghijk", 4, 'd')]
        [InlineData(8, "abc\r\ndefghijk", 4, 'g')]
        public void CheckColumnIndices(int index, string src, int shouldBe, char sanityCheck)
        {
            Assert.Equal(sanityCheck, src[index]);
            var map = new FileMap(src);

            var got = map.ColumnNumber(index);

            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData(10, "ab\ncdef\nghij\n", 3, 'i')]
        [InlineData(10 + 2, "ab\r\ncdef\r\nghij\n", 3, 'i')]
        [InlineData(2, "ab\ncdef\nghij\n", 1, '\n')]
        [InlineData(8, "ab\r\ncdef\r\nghij\n", 2, '\r')]
        [InlineData(9, "ab\r\ncdef\r\nghij\n", 2, '\n')]
        [InlineData(0, "ab\ncdef\nghij\n", 1, 'a')]
        [InlineData(4, "ab\r\ncdef\r\nghij\n", 2, 'c')]
        [InlineData(39, "  N1   (* ALPHA V2.0 P80 U 5 FF MV)\r\n  N2   G78 M13", 2, 'N')]
        [InlineData(46, "  N1   (* ALPHA V2.0 P80 U 5 FF MV)\r\n  N2   G78 M13", 2, '8')]
        public void CheckLineNumbers(int index, string src, int shouldBe, char sanityCheck)
        {
            Assert.Equal(sanityCheck, src[index]);
            var map = new FileMap(src);

            var got = map.LineNumber(index);

            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData(1, 5, "ab\r\ncdef\r\nghij\n", "b\r\nc")]
        [InlineData(0, 2, "ab\r\ncdef\r\nghij\n", "ab")]
        [InlineData(39, 47, "  N1   (* ALPHA V2.0 P80 U 5 FF MV)\r\n  N2   G78 M13", "N2   G78")]
        public void GetSpanInfo(int start, int end, string src, string value)
        {
            var map = new FileMap(src);
            var span = new Span(start, end);
            var shouldBe = new SpanInfo(span, map.LocationFor(start), map.LocationFor(end), value);

            var got = map.SpanInfoFor(span);

            Assert.Equal(shouldBe, got);
        }

        [Theory]
        [InlineData(10, "ab\ncdef\nghij\n", 3)]
        [InlineData(10 + 2, "ab\r\ncdef\r\nghij\n", 3)]
        [InlineData(2, "ab\ncdef\nghij\n", 1)]
        [InlineData(0, "ab\ncdef\nghij\n", 1)]
        [InlineData(4, "ab\r\ncdef\r\nghij\n", 2)]
        [InlineData(39, "  N1   (* ALPHA V2.0 P80 U 5 FF MV)\r\n  N2   G78 M13", 2)]
        [InlineData(46, "  N1   (* ALPHA V2.0 P80 U 5 FF MV)\r\n  N2   G78 M13", 2)]
        public void WarmTheCacheAndGetLocations(int index, string src, int shouldBe)
        {
            var map = new FileMap(src);
            WarmFileMapCache(map, src.Length);

            var got = map.LineNumber(index);

            Assert.Equal(shouldBe, got);
        }

        private void WarmFileMapCache(FileMap map, int length)
        {
            var rng = new Random(1234);
            var currentIndex = 0;

            while (currentIndex < length)
            {
                map.LocationFor(currentIndex);
                currentIndex += rng.Next(1, 20);
            }
        }
    }
}
