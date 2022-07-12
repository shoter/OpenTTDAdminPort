namespace OpenTTDAdminPort.Game
{
    public enum AdminUpdateType
    {
        /// <summary>
        /// Updates about the date of the game.
        /// </summary>
        ADMIN_UPDATE_DATE,

        /// <summary>
        /// Updates about the information of clients.
        /// </summary>
        ADMIN_UPDATE_CLIENT_INFO,

        /// <summary>
        /// Updates about the generic information of companies.
        /// </summary>
        ADMIN_UPDATE_COMPANY_INFO,

        /// <summary>
        /// Updates about the economy of companies.
        /// </summary>
        ADMIN_UPDATE_COMPANY_ECONOMY,

        /// <summary>
        /// Updates about the statistics of companies.
        /// </summary>
        ADMIN_UPDATE_COMPANY_STATS,

        /// <summary>
        /// The admin would like to have chat messages.
        /// </summary>
        ADMIN_UPDATE_CHAT,

        /// <summary>
        /// The admin would like to have console messages.
        /// </summary>
        ADMIN_UPDATE_CONSOLE,

        /// <summary>
        /// The admin would like a list of all DoCommand names.
        /// </summary>
        ADMIN_UPDATE_CMD_NAMES,

        /// <summary>
        /// The admin would like to have DoCommand information.
        /// </summary>
        ADMIN_UPDATE_CMD_LOGGING,

        /// <summary>
        /// The admin would like to have gamescript messages.
        /// </summary>
        ADMIN_UPDATE_GAMESCRIPT,

        /// <summary>
        /// Must ALWAYS be on the end of this list!! (period)
        /// </summary>
        ADMIN_UPDATE_END,
    }
}
