using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes
{
    /// <summary>
    /// A map for finding details about a particular location in text.
    /// </summary>
    public class FileMap
    {
        private SortedDictionary<int, Location> locations = new SortedDictionary<int, Location>();
        private string src;
        private readonly bool usesCrLf;

        public string LineEnding { get => usesCrLf ? "\r\n" : "\n"; }

        public FileMap(string src)
        {
            this.src = src ?? throw new ArgumentNullException(nameof(src));
            usesCrLf = src.Contains("\r\n");
        }

        public Location LocationFor(int byteIndex)
        {
            locations.TryGetValue(byteIndex, out Location location);

            if (location == null)
            {
                location = locations[byteIndex] = CalculateLocation(byteIndex);
            }

            return location;
        }

        private Location CalculateLocation(int byteIndex)
        {
            var line = LineNumber(byteIndex);
            var column = ColumnNumber(byteIndex);

            return new Location(byteIndex, line, column);
        }

        internal int ColumnNumber(int byteIndex)
        {
            var lastNewline = src.LastIndexOf(LineEnding, byteIndex);

            if (lastNewline < 0)
            {
                return byteIndex + 1;
            }
            else
            {
                var startOfLine = lastNewline + LineEnding.Length;
                int column = byteIndex - startOfLine;
                // one-based index
                column += 1;
                return column;
            }
        }

        internal int LineNumber(int byteIndex)
        {
            var closestLocation = locations.Values.Where(loc => loc.ByteIndex < byteIndex).LastOrDefault();

            var currentIndex = closestLocation?.ByteIndex ?? 0;
            var currentLine = closestLocation?.Line ?? 0;

            do
            {
                currentLine += 1;

                var nextNewLine = src.IndexOf(LineEnding, currentIndex);
                if (nextNewLine < 0 || nextNewLine > byteIndex)
                {
                    break;
                }

                currentIndex = nextNewLine + 1;
            } while (currentIndex < byteIndex);

            return currentLine;
        }
    }
}
