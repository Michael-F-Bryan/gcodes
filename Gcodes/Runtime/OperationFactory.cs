using Gcodes.Ast;
using System;

namespace Gcodes.Runtime
{
    /// <summary>
    /// A factory class for instantiating and configuring an <see cref="IOperation"/>
    /// which corresponds to the provided <see cref="Gcode"/>.
    /// </summary>
    public class OperationFactory
    {
        public virtual IOperation GcodeOp(Gcode code, MachineState initialState)
        {
            switch (code.Number)
            {
                default:
                    var ex = new UnknownGcodeException($"No applicable operation for G{code.Number}");
                    ex.Data[nameof(code)] = code;
                    throw ex;
            }
        }
    }
}