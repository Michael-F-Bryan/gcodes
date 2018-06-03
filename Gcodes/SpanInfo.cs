using Gcodes.Tokens;
using System;
using System.Collections.Generic;

namespace Gcodes
{
    public class SpanInfo : IEquatable<SpanInfo>
    {
        public SpanInfo(Span span, Location start, Location end, string value)
        {
            Span = span;
            Start = start;
            End = end;
            Value = value;
        }

        public Span Span { get; }
        public Location Start { get; }
        public Location End { get; }
        public string Value { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SpanInfo);
        }

        public bool Equals(SpanInfo other)
        {
            return other != null &&
                   EqualityComparer<Span>.Default.Equals(Span, other.Span) &&
                   EqualityComparer<Location>.Default.Equals(Start, other.Start) &&
                   EqualityComparer<Location>.Default.Equals(End, other.End) &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = 1648364364;
            hashCode = hashCode * -1521134295 + EqualityComparer<Span>.Default.GetHashCode(Span);
            hashCode = hashCode * -1521134295 + EqualityComparer<Location>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<Location>.Default.GetHashCode(End);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public static bool operator ==(SpanInfo info1, SpanInfo info2)
        {
            return EqualityComparer<SpanInfo>.Default.Equals(info1, info2);
        }

        public static bool operator !=(SpanInfo info1, SpanInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
