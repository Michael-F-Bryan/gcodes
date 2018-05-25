using Gcodes.Tokens;
using System;
using System.Collections.Generic;

namespace Gcodes.Ast
{
    public class Mcode : Code, IEquatable<Mcode>
    {
        public Mcode(int number, Span span, int? line = null): base(span, line)
        {
            Number = number;
        }

        public int Number { get; }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as Mcode);
        }

        public bool Equals(Mcode other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            var hashCode = -2028225194;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Mcode mcode1, Mcode mcode2)
        {
            return EqualityComparer<Mcode>.Default.Equals(mcode1, mcode2);
        }

        public static bool operator !=(Mcode mcode1, Mcode mcode2)
        {
            return !(mcode1 == mcode2);
        } 
        #endregion
    }
}