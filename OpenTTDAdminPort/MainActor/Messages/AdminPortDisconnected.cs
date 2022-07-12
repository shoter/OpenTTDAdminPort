namespace OpenTTDAdminPort.MainActor.Messages
{
    public static class AdminPortDisconnected
    {
        public static AdminPortDisconnect Instance { get; } = new AdminPortDisconnect();
    }
}
