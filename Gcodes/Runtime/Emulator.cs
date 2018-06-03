using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gcodes.Ast;

namespace Gcodes.Runtime
{
    public class Emulator : Interpreter
    {
        private MachineState state;
        private double time;
        public OperationFactory Operations { get; set; } = new OperationFactory();

        public event EventHandler<StateChangeEventArgs> StateChanged;
        public event EventHandler<OperationExecutedEventArgs> OperationExecuted;

        public Emulator() : this(new MachineState()) { }
        public Emulator(MachineState InitialState)
        {
            State = InitialState;
            time = 0;
        }

        public MachineState State
        {
            get => state;
            set
            {
                state = value;
                OnStateChange();
            }
        }

        public double Time
        {
            get => time;
            set
            {
                time = value;
                OnStateChange();
            }
        }

        public double MinimumTimeStep { get; set; }

        public override void Visit(Gcode code)
        {
            var operation = Operations.GcodeOp(code, State);

            if (operation != null)
            {
                ExecuteOperation(operation);
            }

            OnOperationExecuted(operation, code);
        }

        private void OnOperationExecuted(IOperation operation, Code code)
        {
            OperationExecuted?.Invoke(this, new OperationExecutedEventArgs(operation, code));
        }

        private void ExecuteOperation(IOperation operation)
        {
            var timeStep = Math.Max(operation.Duration.TotalSeconds / 20.0, MinimumTimeStep);
            var start = Time;
            var end = start + operation.Duration.TotalSeconds;

            var numSteps = operation.Duration.TotalSeconds / timeStep;

            for (int step = 0; step < Math.Floor(numSteps); step++)
            {
                var deltaTime = step * timeStep;
                var newState = operation.NextState(TimeSpan.FromSeconds(deltaTime));
                UpdateState(start + deltaTime, newState);
            }

            if (Time != end)
            {
                UpdateState(end, operation.NextState(operation.Duration));
            }
        }

        private void UpdateState(double newTime, MachineState newState)
        {
            state = newState;
            time = newTime;
            OnStateChange();
        }

        private void OnStateChange()
        {
            StateChanged?.Invoke(this, new StateChangeEventArgs(State, Time));
        }


        public class StateChangeEventArgs : EventArgs
        {
            public StateChangeEventArgs(MachineState newState, double time)
            {
                NewState = newState;
                Time = time;
            }

            public MachineState NewState { get; }
            public double Time { get; }
        }

        public class OperationExecutedEventArgs : EventArgs
        {
            public OperationExecutedEventArgs(IOperation operation, Code code)
            {
                Operation = operation;
                Code = code;
            }

            public Code Code { get; }
            public IOperation Operation { get; }
        }
    }
}
