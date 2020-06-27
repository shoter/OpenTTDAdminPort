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
        /// <summary>
        /// Server is closing connection - sending quit message.
        /// Status will turn into <see cref="Idle"/> after a while
        /// </summary>
        Disconnecting,
        /// <summary>
        /// This happens when there was an error with current connection.
        /// Client will try to gracely send quit message to the server and after a while it will try to connect again.
        /// </summary>
        Errored,
        /// <summary>
        /// This state indicates there is no connection and no more operations can be done on the client.
        /// </summary>
        ErroredOut,
    }
}
