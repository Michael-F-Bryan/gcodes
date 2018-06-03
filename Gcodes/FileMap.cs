using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcodes
{
    /// <summary>
    /// A map for finding details about a particular location in text.
    /// </summary>
    public class FileMap
    {
        private SortedDictionary<int, Location> locations = new SortedDictionary<int, Location>();
        private Dictionary<Span, SpanInfo> spans = new Dictionary<Span, SpanInfo>();
        private string src;
        private readonly bool usesCrLf;

        private string LineEnding { get => usesCrLf ? "\r\n" : "\n"; }

        public FileMap(string src)
        {
            this.src = src ?? throw new ArgumentNullException(nameof(src));
            usesCrLf = src.Contains("\r\n");
        }

        /// <summary>
        /// Get information associated with the provided span.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public SpanInfo SpanInfoFor(Span span)
        {
            if (!spans.TryGetValue(span, out SpanInfo info))
            {
                info = spans[span] = CalculateSpanInfo(span);
            }

            return info;
        }

        private SpanInfo CalculateSpanInfo(Span span)
        {
            var start = LocationFor(span.Start);
            var end = LocationFor(span.End);
            var value = src.Substring(span.Start, span.Length);

            return new SpanInfo(span, start, end, value);
        }

        public Location LocationFor(int byteIndex)
        {
            if (!locations.TryGetValue(byteIndex, out Location location))
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
