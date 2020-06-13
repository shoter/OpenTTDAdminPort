﻿namespace OpenTTDAdminPort.Messaging
{
    public class AdminJoinMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_JOIN;

        public string Password { get; }

        public string AdminName { get; }

        public string AdminVersion { get; }

        public AdminJoinMessage(string password, string adminName, string adminVersion)
        {
            this.Password = password;
            this.AdminName = adminName;
            this.AdminVersion = adminVersion;
        }
    }
}
