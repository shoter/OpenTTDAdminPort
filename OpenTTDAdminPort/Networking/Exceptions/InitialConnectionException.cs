using System;
using System.Runtime.Serialization;

namespace OpenTTDAdminPort.Networking.Exceptions
{
    [Serializable]
    public class InitialConnectionException : Exception
    {
        public InitialConnectionException()
        {
        }

        public InitialConnectionException(string? message)
            : base(message)
        {
        }

        public InitialConnectionException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected InitialConnectionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
