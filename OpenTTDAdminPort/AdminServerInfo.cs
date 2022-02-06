namespace OpenTTDAdminPort
{
    public class AdminServerInfo
    {
        public string ServerName { get; internal init; } = default!;

        public string RevisionName { get; internal init; } = default!;

        public bool IsDedicated { get; internal set; }

        public string MapName { get; internal init; } = default !;

    }
}
