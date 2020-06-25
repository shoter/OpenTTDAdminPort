namespace OpenTTDAdminPort
{
    public enum AdminConnectionState
    {
        Idle,
        Connecting,
        Connected,
        Disconnecting,
        Errored,
        ErroredCritical, // it will get back into Idle state
    }
}
