namespace OpenTTDAdminPort
{
    public enum AdminConnectionState
    {
        Idle,
        /// <summary>
        /// Establishing connection.
        /// Status will turn into <see cref="Connected"/>
        /// </summary>
        Connecting,
        /// <summary>
        /// Connection established
        /// </summary>
        Connected,
    }
}
