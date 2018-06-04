using Gcodes.Ast;
using System;
using System.Collections.Generic;

namespace Gcodes.Runtime
{
    /// <summary>
    /// A factory class for instantiating and configuring an <see cref="IOperation"/>
    /// which corresponds to the provided <see cref="Gcode"/>.
    /// </summary>
    public class OperationFactory
    {
        private HashSet<int> ignoredGcodes = new HashSet<int>();

        /// <summary>
        /// Get the <see cref="IOperation"/> corresponding to a particular
        /// <see cref="Gcode"/>. 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="initialState"></param>
        /// <returns></returns>
        public virtual IOperation GcodeOp(Gcode code, MachineState initialState)
        {
            Exception ex;

            if (ignoredGcodes.Contains(code.Number))
            {
                return new Noop(initialState);
            }

            switch (code.Number)
            {
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

        /// <summary>
        /// Add one or more gcode instructions to the ignore list.
        /// </summary>
        /// <param name="numbers"></param>
        public void IgnoreGcode(params int[] numbers)
        {
            foreach (var number in numbers)
            {
                ignoredGcodes.Add(number);
            }
        }
    }
}