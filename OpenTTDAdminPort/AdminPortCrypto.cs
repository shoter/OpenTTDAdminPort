using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Sodium;

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
    public class DerivedKeys
    {
        public byte[] ClientToServerKey { get; }

        public byte[] ServerToClientKey { get; }

        public DerivedKeys(byte[] clientToServer, byte[] serverToClient)
        {
            ClientToServerKey = clientToServer;
            ServerToClientKey = serverToClient;
        }
    }

    public static (byte[] SecretKey, byte[] PublicKey) GenerateKeyPair()
    {
        var random = new SecureRandom();

        // Generate private key (32 bytes)
        byte[] privateKey = new byte[X25519_KEY_SIZE];
        random.NextBytes(privateKey);

        // Create X25519 private key params
        var privParams = new X25519PrivateKeyParameters(privateKey, 0);

        // Derive public key
        var pubParams = privParams.GeneratePublicKey();

        byte[] publicKey = pubParams.GetEncoded();

        return (privateKey, publicKey);
    }

    public static byte[] GenerateNonce()
    {
        var random = new SecureRandom();
        var nonce = new byte[X25519_NONCE_SIZE];
        random.NextBytes(nonce);
        return nonce;
    }

    public static byte[] GenerateAuthPayload()
    {
        var random = new SecureRandom();
        var payload = new byte[X25519_KEY_EXCHANGE_MESSAGE_SIZE];
        random.NextBytes(payload);
        return payload;
    }

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
        // Perform X25519 key exchange
        var ourPrivateParams = new X25519PrivateKeyParameters(ourSecretKey, 0);
        var peerPublicParams = new X25519PublicKeyParameters(peerPublicKey, 0);

        var agreement = new byte[32];
        ourPrivateParams.GenerateSecret(peerPublicParams, agreement, 0);

        // Check for all-zero shared secret (security check)
        if (agreement.All(b => b == 0))
        {
            // Peer tried to force shared secret to known constant
            return null;
        }

        // Derive keys using BLAKE2b hash
        // Hash: shared_secret + server_public + client_public + extra_payload
        var blake2b = new Blake2bDigest(512); // 512 bits = 64 bytes output

        // Update with shared secret
        blake2b.BlockUpdate(agreement, 0, agreement.Length);
        blake2b.BlockUpdate(peerPublicKey, 0, peerPublicKey.Length);
        blake2b.BlockUpdate(ourPublicKey, 0, ourPublicKey.Length);

        // Update with extra payload (password for PAKE)
        if (!string.IsNullOrEmpty(extraPayload))
        {
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(extraPayload);
            blake2b.BlockUpdate(payloadBytes, 0, payloadBytes.Length);
        }

        // Finalize hash to get 64 bytes (two 32-byte keys)
        var derivedKeys = new byte[64];
        blake2b.DoFinal(derivedKeys, 0);

        // Split into two keys
        var clientToServerKey = new byte[32];
        var serverToClientKey = new byte[32];
        Array.Copy(
            derivedKeys,
            0,
            clientToServerKey,
            0,
            32);
        Array.Copy(
            derivedKeys,
            32,
            serverToClientKey,
            0,
            32);

        // Clear sensitive data
        Array.Clear(derivedKeys, 0, derivedKeys.Length);
        Array.Clear(agreement, 0, agreement.Length);

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
    /// <param name="mac">Output: 16-byte MAC</param>
    /// <param name="ciphertext">Output: encrypted message</param>
    public static (byte[] Mac, byte[] Ciphertext) EncryptAuthChallenge(
        byte[] message,
        byte[] key,
        byte[] nonce,
        byte[] additionalData)
    {
        // Use libsodium's XChaCha20-Poly1305 which supports 24-byte nonces
        var ciphertextWithMac = SecretAeadXChaCha20Poly1305.Encrypt(
            message,
            nonce,
            key,
            additionalData);

        // Extract MAC (last 16 bytes)
        var mac = new byte[X25519_MAC_SIZE];
        Array.Copy(
            ciphertextWithMac,
            ciphertextWithMac.Length - X25519_MAC_SIZE,
            mac,
            0,
            X25519_MAC_SIZE);

        // Ciphertext is everything except MAC
        var ciphertext = new byte[ciphertextWithMac.Length - X25519_MAC_SIZE];
        Array.Copy(
            ciphertextWithMac,
            0,
            ciphertext,
            0,
            ciphertext.Length);

        return (mac, ciphertext);
    }

    /// <summary>
    /// Decrypts and validates the authentication challenge message.
    /// Used by server to validate AUTH_RESPONSE.
    /// </summary>
    /// <param name="ciphertext">Encrypted message</param>
    /// <param name="mac">16-byte MAC</param>
    /// <param name="key">Derived encryption key (32 bytes)</param>
    /// <param name="nonce">Nonce (24 bytes)</param>
    /// <param name="additionalData">Peer's public key as additional authenticated data (32 bytes)</param>
    /// <param name="plaintext">Output: decrypted message</param>
    /// <returns>True if decryption and MAC validation succeeded</returns>
    public static bool DecryptAuthChallenge(
        byte[] ciphertext,
        byte[] mac,
        byte[] key,
        byte[] nonce,
        byte[] additionalData,
        out byte[] plaintext)
    {
        try
        {
            // Combine ciphertext and MAC (libsodium expects MAC at the end)
            var combined = new byte[ciphertext.Length + mac.Length];
            Array.Copy(
                ciphertext,
                0,
                combined,
                0,
                ciphertext.Length);
            Array.Copy(
                mac,
                0,
                combined,
                ciphertext.Length,
                mac.Length);

            plaintext = SecretAeadXChaCha20Poly1305.Decrypt(
                combined,
                nonce,
                key,
                additionalData);
            return true;
        }
        catch
        {
            plaintext = [];
            return false;
        }
    }

    /// <summary>
    /// Handles ongoing encryption/decryption of packets after authentication.
    /// Maintains state for XChaCha20-Poly1305 AEAD encryption.
    /// </summary>
    public class PacketEncryptionHandler
    {
        private readonly byte[] key;
        private readonly byte[] baseNonce;
        private ulong messageCounter;

        /// <summary>
        /// Creates an encryption handler for a specific direction.
        /// </summary>
        /// <param name="key">32-byte encryption key</param>
        /// <param name="nonce">24-byte base nonce</param>
        public PacketEncryptionHandler(byte[] key, byte[] nonce)
        {
            if (key.Length != 32)
            {
                throw new ArgumentException("Key must be 32 bytes", nameof(key));
            }

            if (nonce.Length != 24)
            {
                throw new ArgumentException("Nonce must be 24 bytes", nameof(nonce));
            }

            this.key = (byte[]) key.Clone();
            this.baseNonce = (byte[]) nonce.Clone();
            this.messageCounter = 0;
        }

        /// <summary>
        /// Encrypts a packet payload and generates MAC.
        /// </summary>
        /// <param name="plaintext">Packet data to encrypt</param>
        /// <param name="mac">Output: 16-byte MAC</param>
        /// <param name="ciphertext">Output: encrypted data</param>
        public void Encrypt(
            byte[] plaintext, out byte[] mac,
            out byte[] ciphertext)
        {
            var nonce = GetNextNonce();

            var output = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);

            // Extract MAC (last 16 bytes)
            mac = new byte[X25519_MAC_SIZE];
            Array.Copy(
                output,
                output.Length - X25519_MAC_SIZE,
                mac,
                0,
                X25519_MAC_SIZE);

            // Ciphertext is everything except MAC
            ciphertext = new byte[output.Length - X25519_MAC_SIZE];
            Array.Copy(
                output,
                0,
                ciphertext,
                0,
                ciphertext.Length);
        }

        /// <summary>
        /// Decrypts a packet payload and validates MAC.
        /// </summary>
        /// <param name="ciphertext">Encrypted packet data</param>
        /// <param name="mac">16-byte MAC</param>
        /// <param name="plaintext">Output: decrypted data</param>
        /// <returns>True if decryption and MAC validation succeeded</returns>
        public bool Decrypt(
            byte[] ciphertext, byte[] mac,
            out byte[] plaintext)
        {
            try
            {
                var nonce = GetNextNonce();

                // Combine ciphertext and MAC (libsodium expects MAC at the end)
                var combined = new byte[ciphertext.Length + mac.Length];
                Array.Copy(
                    ciphertext,
                    0,
                    combined,
                    0,
                    ciphertext.Length);
                Array.Copy(
                    mac,
                    0,
                    combined,
                    ciphertext.Length,
                    mac.Length);

                plaintext = SecretAeadXChaCha20Poly1305.Decrypt(combined, nonce, key);
                return true;
            }
            catch
            {
                plaintext = [];
                return false;
            }
        }

        /// <summary>
        /// Generates the next nonce by incrementing the message counter.
        /// XChaCha20-Poly1305 uses a 24-byte nonce with counter.
        /// </summary>
        private byte[] GetNextNonce()
        {
            var nonce = (byte[]) baseNonce.Clone();

            // Increment counter in the nonce (little-endian)
            ulong counter = messageCounter++;
            for (int i = 0; i < 8 && i < nonce.Length; i++)
            {
                nonce[i] = (byte) ((counter >> (i * 8)) & 0xFF);
            }

            return nonce;
        }

        /// <summary>
        /// Clears sensitive key material from memory.
        /// </summary>
        public void Dispose()
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(baseNonce, 0, baseNonce.Length);
        }
    }
}