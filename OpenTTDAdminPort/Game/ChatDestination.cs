namespace OpenTTDAdminPort.Game
{
    public enum ChatDestination
    {
        /// <summary>
        /// Send message/notice to all clients (All)
        /// </summary>
        DESTTYPE_BROADCAST,
        /// <summary>
        ///  Send message/notice to everyone playing the same company (Team)
        /// </summary>
        DESTTYPE_TEAM,
        /// <summary>
        ///  Send message/not   ice to only a certain client (Private)
        /// </summary>
        DESTTYPE_CLIENT,
    }
}
