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
            var src = EmbeddedFixture.ExtractFile("circle.txt");
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
        [InlineData(0, "ab\ncdef\nghij\n", 1, 'a')]
        [InlineData(4, "ab\r\ncdef\r\nghij\n", 2, 'c')]
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
        public void GetSpanInfo(int start, int end, string src, string value)
        {
            var map = new FileMap(src);
            var span = new Span(start, end);
            var shouldBe = new SpanInfo(span, map.LocationFor(start), map.LocationFor(end), value);
            
            var got = map.SpanInfoFor(span);

            Assert.Equal(shouldBe, got);
        }
    }
}
