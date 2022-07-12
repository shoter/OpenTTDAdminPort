namespace OpenTTDAdminPort
{
    public class AdminServerInfo
    {
        // TODO: add `required` when new C# version arrives. This will get rid of `default!`
        public string ServerName { get; internal init; } = default!;

        public string RevisionName { get; internal init; } = default!;

        public bool IsDedicated { get; internal set; }

        public string MapName { get; internal init; } = default!;
    }
}
