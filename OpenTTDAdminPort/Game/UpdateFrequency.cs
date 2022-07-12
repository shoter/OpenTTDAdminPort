namespace OpenTTDAdminPort.Game
{
    public enum UpdateFrequency
    {
        /// <summary>
        /// The admin can poll this.
        /// </summary>
        ADMIN_FREQUENCY_POLL = 0x01,

        /// <summary>
        /// The admin gets information about this on a daily basis.
        /// </summary>
        ADMIN_FREQUENCY_DAILY = 0x02,

        /// <summary>
        /// The admin gets information about this on a weekly basis.
        /// </summary>
        ADMIN_FREQUENCY_WEEKLY = 0x04,

        /// <summary>
        /// The admin gets information about this on a monthly basis.
        /// </summary>
        ADMIN_FREQUENCY_MONTHLY = 0x08,

        /// <summary>
        /// The admin gets information about this on a quarterly basis.
        /// </summary>
        ADMIN_FREQUENCY_QUARTERLY = 0x10,

        /// <summary>
        /// The admin gets information about this on a yearly basis.
        /// </summary>
        ADMIN_FREQUENCY_ANUALLY = 0x20,

        /// <summary>
        /// The admin gets information about this when it changes.
        /// </summary>
        ADMIN_FREQUENCY_AUTOMATIC = 0x40,
    }
}
