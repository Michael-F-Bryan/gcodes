using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gcodes.Ast
{
    public class Gcode: Code, IEquatable<Gcode>
    {
        private List<Argument> args;

        public Gcode(int number, List<Argument> args, Span span, int? line = null): base(span, line)
        {
            Number = number;
            this.args = args;
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

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as Gcode);
        }

        public bool Equals(Gcode other)
        {
            return other != null &&
                   base.Equals(other) &&
                   args.SequenceEqual(other.args) &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            var hashCode = 1590044514;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Argument>>.Default.GetHashCode(args);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }

        public int Number { get; }
        public IReadOnlyList<Argument> Arguments { get => args; }

        public static bool operator ==(Gcode gcode1, Gcode gcode2)
        {
            return EqualityComparer<Gcode>.Default.Equals(gcode1, gcode2);
        }

        public static bool operator !=(Gcode gcode1, Gcode gcode2)
        {
            return !(gcode1 == gcode2);
        } 
        #endregion
    }
}
