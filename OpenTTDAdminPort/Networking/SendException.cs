using System;

namespace OpenTTDAdminPort.Networking
{
    [Serializable]
    public class SendException : AdminPortException
    {
        public SendException(string message)
            : base(message)
        {
        }

        public SendException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SendException()
            : base()
        {
        }
    }
}
