using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    public abstract class Code : IEquatable<Code>
    {
        protected Code(Span span, int? line = null)
        {
            Line = line;
            Span = span;
        }

        public int? Line { get; }
        public Span Span { get; }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as Code);
        }

        public bool Equals(Code other)
        {
            return other != null &&
                   EqualityComparer<int?>.Default.Equals(Line, other.Line);
        }

        public override int GetHashCode()
        {
            return -2039293089 + EqualityComparer<int?>.Default.GetHashCode(Line);
        }

        public static bool operator ==(Code code1, Code code2)
        {
            return EqualityComparer<Code>.Default.Equals(code1, code2);
        }

        public static bool operator !=(Code code1, Code code2)
        {
            return !(code1 == code2);
        } 
        #endregion
    }
}
