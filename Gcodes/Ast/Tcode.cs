using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gcodes.Tokens;

namespace Gcodes.Ast
{
    public class Tcode : Code
    {
        public Tcode(int number, Span span, int? line = null) : base(span, line)
        {
            Number = number;
        }

        public int Number { get; }

        public override void Accept(IGcodeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
