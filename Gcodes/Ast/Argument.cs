using Gcodes.Tokens;
using System;
using System.Collections.Generic;

namespace Gcodes.Ast
{
    /// <summary>
    /// A single argument, containing both an <see cref="ArgumentKind"/> and
    /// its corresponding value.
    /// </summary>
    public class Argument : IEquatable<Argument>
    {
        public Argument(ArgumentKind kind, double value, Span span)
        {
            Span = span;
            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// The argument's location in its source text.
        /// </summary>
        public Span Span { get; }
        /// <summary>
        /// Which kind of argument is this?
        /// </summary>
        public ArgumentKind Kind { get; }
        /// <summary>
        /// The argument's value (e.g. feed rate for a <see cref="ArgumentKind.FeedRate"/>).
        /// </summary>
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

    /// <summary>
    /// The various types of gcode arguments.
    /// </summary>
    public enum ArgumentKind
    {
        X,
        Y,
        Z,
        FeedRate,
        K,
        J,
        I,
        C,
        B,
        A,
        P,
        S,
        H,
    }
}
