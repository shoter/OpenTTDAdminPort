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
        ClientJoin = 7,
        ClientDisconnect = 8,

        CompanyNew = 10,
        CompanyUpdate = 11,
        CompanyRemoval = 12,

        // Not connected with in-game packets
        ServerConnectionLost = 1000,
        ServerConnected = 1001,
    }
}
