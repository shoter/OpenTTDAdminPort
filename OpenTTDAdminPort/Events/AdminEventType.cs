namespace OpenTTDAdminPort.Events
{
    public enum AdminEventType
    {
        ChatMessageReceived = 1,
        ConsoleMessage = 2,
        AdminRcon = 3,
        Pong = 4,
        ClientInfo = 5,
        ClientUpdate = 6,

        // Not connected with in-game packets
        ServerRestarted = 1000

    }
}
