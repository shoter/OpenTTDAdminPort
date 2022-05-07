namespace OpenTTDAdminPort.Events
{
    public record PlayerDisconnectedEvent(uint ClientId, string PlayerName);
}
