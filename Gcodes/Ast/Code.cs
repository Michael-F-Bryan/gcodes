using Gcodes.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    /// <summary>
    /// The base class all AST nodes in a gcode file inherit from.
    /// </summary>
    public abstract class Code : IEquatable<Code>
    {
        protected Code(Span span, int? line = null)
        {
            Line = line;
            Span = span;
        }

        /// <summary>
        /// The item's line number, if one was supplied.
        /// </summary>
        public int? Line { get; }
        /// <summary>
        /// The item's location within its source text.
        /// </summary>
        public Span Span { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept(GcodeVisitor visitor);

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
