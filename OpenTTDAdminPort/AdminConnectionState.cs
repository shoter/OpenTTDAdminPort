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
        /// This state will happen when we have ongoing connection. During this state client will try to send quit message.
        /// Status will turn into <see cref="ErroredOut"/> after a while.
        /// </summary>
        Errored,
        /// <summary>
        /// This state indicates there is no connection and no more operations can be done on the client.
        /// </summary>
        ErroredOut,
    }
}
