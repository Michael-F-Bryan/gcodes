using Gcodes.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gcodes.Test.Runtime
{
    public class EmulatorTest
    {
        [Fact]
        public void CanInstantiate()
        {
            var emulator = new Emulator();
        }

        [Theory(Skip = "Not all operations are implemented")]
        [InlineData("circle.gcode")]
        [InlineData("simple_mill.gcode")]
        [InlineData("371373P.gcode")]
        public void EmulateAValidGcodeProgram(string filename)
        {
            var src = EmbeddedFixture.ExtractFile(filename);
            var emulator = new Emulator();

            emulator.Run(src);
        }
    }
}
