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
            Exception ex;

            switch (code.Number)
            {
                case 17:
                    return new Noop(initialState);
                case 4:
                    var ms = code.ValueFor(ArgumentKind.P);
                    if (ms == null)
                    {
                        ex = new ArgumentException("Dwell commands require a P argument");
                        ex.Data[nameof(code)] = code;
                        throw ex;
                    }
                    return new Noop(initialState, TimeSpan.FromMilliseconds(ms.Value));
                default:
                    ex = new UnknownGcodeException($"No applicable operation for G{code.Number}");
                    ex.Data[nameof(code)] = code;
                    throw ex;
            }
        }
    }
}