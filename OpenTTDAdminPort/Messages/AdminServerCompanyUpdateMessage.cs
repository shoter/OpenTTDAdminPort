namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerCompanyUpdateMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE;

        public byte CompanyId { get; internal set; }

        public string CompanyName { get; internal set; } = default!;

        public string ManagerName { get; internal set; } = default!;

        public byte Color { get; internal set; }

        public bool HasPassword { get; internal set; }

        public byte MonthsOfBankruptcy { get; internal set; }
    }
}
