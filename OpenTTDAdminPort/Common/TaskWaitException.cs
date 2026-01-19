using System;
using System.Runtime.Serialization;

namespace OpenTTDAdminPort.Common
{
    internal class TaskWaitException : Exception
    {
        public TaskWaitException()
        {
        }

        public TaskWaitException(string message)
            : base(message)
        {
        }

        public TaskWaitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}