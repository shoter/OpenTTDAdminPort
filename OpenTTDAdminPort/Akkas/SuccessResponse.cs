namespace OpenTTDAdminPort.Akkas
{
    public class SuccessResponse
    {
        public static SuccessResponse Instance { get; } = new SuccessResponse();

        private SuccessResponse()
        {
        }
    }
}
