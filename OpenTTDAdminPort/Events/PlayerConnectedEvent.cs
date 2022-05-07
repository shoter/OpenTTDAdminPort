namespace OpenTTDAdminPort.Events
{
    public record PlayerConnectedEvent(uint ClientId, string PlayerName);
}
