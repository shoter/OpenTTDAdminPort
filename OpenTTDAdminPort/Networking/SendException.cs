using System;

namespace OpenTTDAdminPort.Networking
{
    public class SendException : AdminPortException
    {
        public SendException(string message) : base(message)
        {
        }

        public SendException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
