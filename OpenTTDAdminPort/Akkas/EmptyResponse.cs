namespace OpenTTDAdminPort.Akkas
{
    /// <summary>
    /// Generic response message
    /// </summary>
    public class EmptyResponse
    {
        public static EmptyResponse Instance { get; } = new EmptyResponse();

        private EmptyResponse() { }

    }
}
