using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcodes.Test
{
    public class FileMapTest
    {
        private readonly FileMap map;
        string src;

        public FileMapTest()
        {
            src = EmbeddedFixture.ExtractFile("circle.txt");
            map = new FileMap(src);
        }

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
        public void YouCanLookUpLocationDetails(int index, int line, int column, char letter)
        {
            // for sanity, we double check the letter under the cursor
            var gotLetter = src[index];
            Assert.Equal(letter, gotLetter);

            var shouldBe = new Location(index, line, column);

            var got = map.LocationFor(index);

            Assert.Equal(shouldBe, got);
        }
    }
}
