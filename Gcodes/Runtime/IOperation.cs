using System;

namespace Gcodes.Runtime
{
    public interface IOperation
    {
        /// <summary>
        /// Get the appropriate state <paramref name="deltaT"/> seconds since
        /// the operation was started.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        MachineState NextState(TimeSpan deltaTime);

        /// <summary>
        /// How long the operation should take to complete.
        /// </summary>
        TimeSpan Duration { get; }
    }
}