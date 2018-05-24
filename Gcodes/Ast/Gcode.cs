using System.Collections.Generic;

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

        public int Number { get; }
        public Span Span { get; }
    }
}
