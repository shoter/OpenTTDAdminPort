using System;

namespace OpenTTDAdminPort.Networking
{
    [Serializable]
    public class ReceiveLoopException : AdminPortException
    {
        public ReceiveLoopException(string message)
            : base(message)
        {
        }

        public ReceiveLoopException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ReceiveLoopException()
            : base()
        {
        }
    }
}
