using Gcodes.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Gcodes.Ast
{
    public class Gcode
    {
        private List<Argument> args;

        public Gcode(int number, List<Argument> args, Span span)
        {
            Number = number;
            this.args = args;
            Span = span;
        }

        public double? ValueFor(ArgumentKind kind)
        {
            var found = args.Where(arg => arg.Kind == kind);

            if (found.Any())
            {
                return found.First().Value;
            }
            else
            {
                return null;
            }
        }

        public int Number { get; }
        public Span Span { get; }
        public IReadOnlyList<Argument> Arguments { get => args; }
    }
}
