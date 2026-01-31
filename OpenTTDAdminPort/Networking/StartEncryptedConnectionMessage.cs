namespace OpenTTDAdminPort.Networking;

public record StartEncryptedConnectionMessage(
    AdminPortCrypto.PacketEncryptionHandler EncryptionHandler);