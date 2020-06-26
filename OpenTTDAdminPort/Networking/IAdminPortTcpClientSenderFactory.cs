using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal interface IAdminPortTcpClientSenderFactory
    {
        IAdminPortTcpClientSender Create(ITcpClient tcpClient);
    }
}
