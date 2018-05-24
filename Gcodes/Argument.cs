namespace Gcodes
{
    public class Argument
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
    }

    public enum ArgumentKind
    {
        X,
        Y,
        Z,
        FeedRate,
    }
}
