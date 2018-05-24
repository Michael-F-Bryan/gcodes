using System;
using System.Collections.Generic;

namespace Gcodes
{
    public class Argument : IEquatable<Argument>
    {
        public Argument(ArgumentKind kind, double value, Span span)
        {
            Span = span;
            Kind = kind;
            Value = value;
        }

        public Span Span { get; }
        public ArgumentKind Kind { get; }
        public double Value { get; }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as Argument);
        }

        public bool Equals(Argument other)
        {
            return other != null &&
                   EqualityComparer<Span>.Default.Equals(Span, other.Span) &&
                   Kind == other.Kind &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1030702410;
            hashCode = hashCode * -1521134295 + EqualityComparer<Span>.Default.GetHashCode(Span);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Argument argument1, Argument argument2)
        {
            return EqualityComparer<Argument>.Default.Equals(argument1, argument2);
        }

        public static bool operator !=(Argument argument1, Argument argument2)
        {
            return !(argument1 == argument2);
        } 
        #endregion
    }

    public enum ArgumentKind
    {
        X,
        Y,
        Z,
        FeedRate,
    }
}
