using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Networking
{
    internal record ReceiveMessage(IAdminMessage Message);
}
