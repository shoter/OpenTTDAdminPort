namespace OpenTTDAdminPort.Networking
{
    public record AdminPortTcpClientConnect(string Ip, int Port) : IAdminPortTcpClientMessage;
}
