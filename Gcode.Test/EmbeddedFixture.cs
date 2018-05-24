using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Test
{
    class EmbeddedFixture
    {
        public static string ExtractFile(string filename)
        {
            var asm = typeof(EmbeddedFixture).Assembly;
            filename = asm.GetName().Name + "." + filename;

            var availableFiles = asm.GetManifestResourceNames();

            using (var stream = asm.GetManifestResourceStream(filename))
            {
                if (stream == null)
                {
                    throw new ArgumentException("Invalid Fixture", nameof(filename));
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
