using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ImageEditor.Exceptions
{
    /// <summary>
    /// Исключения ядра искажение / фильтра
    /// </summary>
    [Serializable()]
    class InvalidKernelException : System.Exception
    {
        public InvalidKernelException() : base() { }
        public InvalidKernelException(string message) : base(message) { }
        public InvalidKernelException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected InvalidKernelException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
