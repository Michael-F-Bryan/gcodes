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
            filename = asm.GetName().Name + ".Fixtures." + filename;

            var availableFiles = asm.GetManifestResourceNames();
            if (!availableFiles.Contains(filename))
            {
                throw new ArgumentException($"Resource \"{filename}\" not found in {string.Join(", ", availableFiles)}");
            }

            using (var stream = asm.GetManifestResourceStream(filename))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
