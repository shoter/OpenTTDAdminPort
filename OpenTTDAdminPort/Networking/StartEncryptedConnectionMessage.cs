namespace OpenTTDAdminPort.Networking;

public record StartEncryptedConnectionMessage(
    AdminPortCrypto.PacketEncryptionHandler SenderEncryptionHandler,
    AdminPortCrypto.PacketEncryptionHandler ReceiverEncryptionHandler);