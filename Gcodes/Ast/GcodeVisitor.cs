using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    /// <summary>
    /// A base class which can be used to inspect gcodes at runtime using
    /// the visitor pattern.
    /// </summary>
    public class GcodeVisitor
    {
        public virtual void Visit(Gcode code) { }
        public virtual void Visit(Mcode code) { }
        public virtual void Visit(Tcode tcode) { }
        public virtual void Visit(Ocode code) { }
    }
}
