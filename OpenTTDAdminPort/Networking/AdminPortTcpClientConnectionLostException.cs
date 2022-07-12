using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    [Serializable]
    public class AdminPortTcpClientConnectionLostException : AdminPortException
    {
        public AdminPortTcpClientConnectionLostException(string message)
            : base(message)
        {
        }

        public AdminPortTcpClientConnectionLostException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AdminPortTcpClientConnectionLostException()
            : base()
        {
        }
    }
}
