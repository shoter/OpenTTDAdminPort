namespace OpenTTDAdminPort.Messages
{
    public interface IAdminMessage
    {
        AdminMessageType MessageType { get; }
    }
}
