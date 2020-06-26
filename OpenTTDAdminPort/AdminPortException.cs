using System;
using System.Runtime.Serialization;

namespace OpenTTDAdminPort
{
    public class AdminPortException : Exception
    {
        public AdminPortException(string message) : base(message)
        {
        }

        public AdminPortException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
