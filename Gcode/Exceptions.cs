using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcode
{

    [Serializable]
    public class GcodeException : Exception
    {
        public GcodeException() { }
        public GcodeException(string message) : base(message) { }
        public GcodeException(string message, Exception inner) : base(message, inner) { }
        protected GcodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class UnrecognisedCharacterException : GcodeException
    {
        public int Index { get; }
        public char Character { get; }

        public UnrecognisedCharacterException(int index, char character) : this($"Unrecognised character \"{character}\" at index {index}")
        {
            Index = index;
            Character = character;
        }
        public UnrecognisedCharacterException() { }
        public UnrecognisedCharacterException(string message) : base(message) { }
        public UnrecognisedCharacterException(string message, Exception inner) : base(message, inner) { }
        protected UnrecognisedCharacterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
