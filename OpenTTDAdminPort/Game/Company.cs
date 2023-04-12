namespace OpenTTDAdminPort.Game;
    public record Company(
        byte Id,
        string Name,
        string ManagerName,
        byte Color,
        bool HasPassword,
        OttdDate CreationDate,
        bool IsAi,
        byte MonthsOfBankruptcy
    );