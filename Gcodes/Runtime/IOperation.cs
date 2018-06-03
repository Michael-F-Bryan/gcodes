using System;

namespace Gcodes.Runtime
{
    /// <summary>
    /// An operation which will give you the machine's state at any arbitrary  
    /// time between when it starts and ends.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Get the machine's state at some arbitrary time.
        /// </summary>
        /// <param name="deltaTime">
        /// The amount of time since this operation started.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if you try to get the state after the operation ended (i.e.
        /// <paramref name="deltaTime"/> is greater than <see cref="Duration"/>).
        /// </exception>
        MachineState NextState(TimeSpan deltaTime);

        /// <summary>
        /// How long the operation should take to complete.
        /// </summary>
        TimeSpan Duration { get; }
    }
}