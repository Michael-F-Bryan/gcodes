using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcodes.Runtime
{
    public class Noop : IOperation, IEquatable<Noop>
    {
        private MachineState state;

        public Noop(MachineState state) : this(state, TimeSpan.FromSeconds(0)) { }
        public Noop(MachineState state, double duration) : this(state, TimeSpan.FromSeconds(duration)) { }
        public Noop(MachineState state, TimeSpan duration)
        {
            this.state = state;
            Duration = duration;
        }

        public TimeSpan Duration { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Noop);
        }

        public bool Equals(Noop other)
        {
            return other != null &&
                   EqualityComparer<MachineState>.Default.Equals(state, other.state) &&
                   Duration.Equals(other.Duration);
        }

        public override int GetHashCode()
        {
            var hashCode = -465428333;
            hashCode = hashCode * -1521134295 + EqualityComparer<MachineState>.Default.GetHashCode(state);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(Duration);
            return hashCode;
        }

        public MachineState NextState(TimeSpan deltaTime)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Noop noop1, Noop noop2)
        {
            return EqualityComparer<Noop>.Default.Equals(noop1, noop2);
        }

        public static bool operator !=(Noop noop1, Noop noop2)
        {
            return !(noop1 == noop2);
        }
    }
}
