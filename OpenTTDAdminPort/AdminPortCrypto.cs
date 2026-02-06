using System;
using System.Linq;
using System.Text;
using OpenTTDAdminPort.Networking;
using static Monocypher.Monocypher;

namespace OpenTTDAdminPort;

public static class AdminPortCrypto
{
    /// <summary>
    /// The number of bytes the public and secret keys are in X25519.
    /// </summary>
    public const int X25519_KEY_SIZE = 32;

    /// <summary>
    /// The number of bytes the nonces are in X25519.
    /// </summary>
    public const int X25519_NONCE_SIZE = 24;

    /// <summary>
    /// The number of bytes the message authentication codes are in X25519.
    /// </summary>
    public const int X25519_MAC_SIZE = 16;

    /// <summary>
    /// The number of bytes the (random) payload of the authentication message has.
    /// </summary>
    public const int X25519_KEY_EXCHANGE_MESSAGE_SIZE = 8;

    /// <summary>
    /// Derived encryption keys from X25519 key exchange.
    /// First 32 bytes: Client-to-Server key
    /// Second 32 bytes: Server-to-Client key
    /// </summary>
    public record DerivedKeys(
        byte[] ClientToServerKey,
        byte[] ServerToClientKey);

    public static (byte[] SecretKey, byte[] PublicKey) GenerateKeyPair()
    {
        // Generate private key (32 bytes)
        byte[] privateKey = new byte[X25519_KEY_SIZE];
        byte[] publicKey = new byte[X25519_KEY_SIZE];
        Random.Shared.NextBytes(privateKey);
        crypto_x25519_public_key(publicKey, privateKey);
        return (privateKey, publicKey);
    }

    // public static byte[] GenerateNonce()
    // {
    //     var random = new SecureRandom();
    //     var nonce = new byte[X25519_NONCE_SIZE];
    //     random.NextBytes(nonce);
    //     return nonce;
    // }
    //
    // public static byte[] GenerateAuthPayload()
    // {
    //     var random = new SecureRandom();
    //     var payload = new byte[X25519_KEY_EXCHANGE_MESSAGE_SIZE];
    //     random.NextBytes(payload);
    //     return payload;
    // }

    /// <summary>
    /// Performs X25519 key exchange and derives encryption keys using BLAKE2b.
    /// Matches OpenTTD's X25519DerivedKeys::Exchange implementation.
    /// </summary>
    /// <param name="peerPublicKey">Public key from the other party (32 bytes)</param>
    /// <param name="ourSecretKey">Our secret key (32 bytes)</param>
    /// <param name="ourPublicKey">Our public key (32 bytes)</param>
    /// <param name="extraPayload">Extra payload (password for PAKE, empty for others)</param>
    /// <returns>Derived keys or null if key exchange failed</returns>
    public static DerivedKeys? PerformKeyExchange(
        byte[] peerPublicKey,
        byte[] ourSecretKey,
        byte[] ourPublicKey,
        string extraPayload = "")
    {
        byte[] shared_secret = new byte[X25519_KEY_SIZE];
        crypto_x25519(shared_secret, ourSecretKey, peerPublicKey);

        if(shared_secret.All(x => x == 0))
        {
            throw new Exception(
                "A shared secret of all zeros means that the peer tried to force the shared secret to a known constant.");
        }

        crypto_blake2b_ctx ctx = default;
        byte[] keys = new byte[X25519_KEY_SIZE * 2];
        crypto_blake2b_init(ref ctx, keys.Length);
        crypto_blake2b_update(ref ctx, shared_secret);
        crypto_blake2b_update(ref ctx, peerPublicKey);
        crypto_blake2b_update(ref ctx, ourPublicKey);

        crypto_blake2b_update(ref ctx, Encoding.ASCII.GetBytes(extraPayload));
        crypto_blake2b_final(ref ctx, keys);

        byte[] clientToServerKey = new byte[X25519_KEY_SIZE];
        byte[] serverToClientKey = new byte[X25519_KEY_SIZE];

        Array.Copy(keys, clientToServerKey, X25519_KEY_SIZE);
        Array.Copy(keys, X25519_KEY_SIZE, serverToClientKey, 0, X25519_KEY_SIZE);

        return new DerivedKeys(clientToServerKey, serverToClientKey);
    }

    /// <summary>
    /// Encrypts the authentication challenge message.
    /// Used during the AUTH_RESPONSE packet.
    /// </summary>
    /// <param name="message">8-byte random message to encrypt</param>
    /// <param name="key">Derived encryption key (32 bytes)</param>
    /// <param name="nonce">Nonce from server (24 bytes)</param>
    /// <param name="additionalData">Our public key as additional authenticated data (32 bytes)</param>
    public static (byte[] Mac, byte[] Ciphertext) EncryptAuthChallenge(
        byte[] message,
        byte[] key,
        byte[] nonce,
        byte[] additionalData)
    {
        var mac = new byte[X25519_MAC_SIZE];
        var ciphertext = new byte[message.Length];

        crypto_aead_lock(
            ciphertext,
            mac,
            key,
            nonce,
            additionalData,
            message);

        return (mac, ciphertext);
    }

    // /// <summary>
    // /// Decrypts and validates the authentication challenge message.
    // /// Used by server to validate AUTH_RESPONSE.
    // /// </summary>
    // /// <param name="ciphertext">Encrypted message</param>
    // /// <param name="mac">16-byte MAC</param>
    // /// <param name="key">Derived encryption key (32 bytes)</param>
    // /// <param name="nonce">Nonce (24 bytes)</param>
    // /// <param name="additionalData">Peer's public key as additional authenticated data (32 bytes)</param>
    // /// <param name="plaintext">Output: decrypted message</param>
    // /// <returns>True if decryption and MAC validation succeeded</returns>
    // public static bool DecryptAuthChallenge(
    //     byte[] ciphertext,
    //     byte[] mac,
    //     byte[] key,
    //     byte[] nonce,
    //     byte[] additionalData,
    //     out byte[] plaintext)
    // {
    //     try
    //     {
    //         // Combine ciphertext and MAC (libsodium expects MAC at the end)
    //         var combined = new byte[ciphertext.Length + mac.Length];
    //         Array.Copy(
    //             ciphertext,
    //             0,
    //             combined,
    //             0,
    //             ciphertext.Length);
    //         Array.Copy(
    //             mac,
    //             0,
    //             combined,
    //             ciphertext.Length,
    //             mac.Length);
    //     }
    //     catch
    //     {
    //         plaintext = [];
    //         return false;
    //     }
    // }

    /// <summary>
    /// Handles ongoing encryption/decryption of packets after authentication.
    /// Maintains state for XChaCha20-Poly1305 AEAD encryption.
    /// </summary>
    public class PacketEncryptionHandler
    {
        private crypto_aead_ctx context;

        /// <summary>
        /// Handles ongoing encryption/decryption of packets after authentication.
        /// Maintains state for XChaCha20-Poly1305 AEAD encryption.
        /// </summary>
        public PacketEncryptionHandler(byte[] encryptionNonce,
                                       byte[] key)
        {
            crypto_aead_init_x(ref context, key, encryptionNonce);
        }

        internal Packet EncryptPacket(Packet packet)
        {
            var buffer = packet.Buffer;
            byte[] mac = new byte[X25519_MAC_SIZE];
            Span<byte> cipherText = buffer.AsSpan(2, packet.Size - 2);

            crypto_aead_write(
                ref context,
                cipherText,
                mac,
                null,
                cipherText
                );

            Packet encryptedPacket = new();
            encryptedPacket.SendBytes(mac);
            encryptedPacket.SendBytes(cipherText);
            encryptedPacket.PrepareToSend();
            return encryptedPacket;
        }

        public byte[] DecryptPacket(Span<byte> mac, Span<byte> encrypted)
        {
            byte[] plainText = new byte[encrypted.Length];

            crypto_aead_read(
                ref context,
                plainText,
                mac,
                null,
                encrypted);

            return plainText;
        }
    }
}