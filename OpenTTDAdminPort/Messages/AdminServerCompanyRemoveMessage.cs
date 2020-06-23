using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerCompanyRemoveMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE;

        public byte CompanyId { get; }

        public AdminCompanyRemoveReason RemoveReason { get; }

        public AdminServerCompanyRemoveMessage(byte companyId, byte removeReason)
        {
            this.CompanyId = companyId;
            this.RemoveReason = (AdminCompanyRemoveReason)removeReason;
        }
    }
}
