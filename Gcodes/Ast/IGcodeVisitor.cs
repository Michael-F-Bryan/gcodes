using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Ast
{
    public interface IGcodeVisitor
    {
        void VisitGcode(Gcode code);
        void VisitMcode(Mcode code);
        void VisitProgramNumber(Ocode code);
    }
}
