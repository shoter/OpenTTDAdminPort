using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Networking
{
    public record SendMessage(IAdminMessage Message);
}
