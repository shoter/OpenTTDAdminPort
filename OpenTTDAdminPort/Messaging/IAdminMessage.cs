namespace OpenTTDAdminPort.Messaging
{
    public interface IAdminMessage
    {
        AdminMessageType MessageType { get; }
    }
}
