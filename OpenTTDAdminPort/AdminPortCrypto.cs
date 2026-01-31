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
            plaintext = new byte[0];
            return false;
        }
    }

    /// <summary>
    /// Handles ongoing encryption/decryption of packets after authentication.
    /// Implements Monocypher's incremental AEAD with key ratcheting.
    /// </summary>
    public class PacketEncryptionHandler
    {
        private readonly byte[] sendNonce;
        private readonly byte[] receiveNonce;
        private byte[] sendKey;
        private byte[] receiveKey;

        public PacketEncryptionHandler(
            byte[] encryptionNonce,
            byte[] clientToServerKey,
            byte[] serverToClientKey)
        {
            sendKey = HChaCha20(clientToServerKey, encryptionNonce[0..16]);
            receiveKey = HChaCha20(serverToClientKey, encryptionNonce[0..16]);
            sendNonce = encryptionNonce[16..24];
            receiveNonce = encryptionNonce[16..24];
        }

        public byte[] EncryptPacket(byte[] plaintext)
        {
            byte[] authKey = new byte[64];
            ChaCha20Keystream(authKey, sendKey, sendNonce, counter: 0);

            byte[] ciphertext = new byte[plaintext.Length];
            ChaCha20Xor(ciphertext, plaintext, sendKey, sendNonce, counter: 1);

            byte[] mac = Poly1305ComputeMac(authKey[0..32], ciphertext);

            sendKey = authKey[32..64];

            return BuildPacket(mac, ciphertext);
        }

        public byte[] DecryptPacket(byte[] mac, byte[] encrypted)
        {
            byte[] authKey = new byte[64];
            ChaCha20Keystream(authKey, receiveKey, receiveNonce, counter: 0);

            byte[] expectedMac = Poly1305ComputeMac(authKey[0..32], encrypted);

            if (!ConstantTimeEquals(mac, expectedMac))
            {
                throw new System.Security.Cryptography.CryptographicException("MAC verification failed");
            }

            byte[] plaintext = new byte[encrypted.Length];
            ChaCha20Xor(plaintext, encrypted, receiveKey, receiveNonce, counter: 1);

            receiveKey = authKey[32..64];

            return plaintext;
        }

        private static byte[] HChaCha20(byte[] key, byte[] nonce)
        {
            uint[] state = new uint[16];

            state[0] = 0x61707865;
            state[1] = 0x3320646e;
            state[2] = 0x79622d32;
            state[3] = 0x6b206574;

            state[4] = LoadLittleEndian32(key, 0);
            state[5] = LoadLittleEndian32(key, 4);
            state[6] = LoadLittleEndian32(key, 8);
            state[7] = LoadLittleEndian32(key, 12);
            state[8] = LoadLittleEndian32(key, 16);
            state[9] = LoadLittleEndian32(key, 20);
            state[10] = LoadLittleEndian32(key, 24);
            state[11] = LoadLittleEndian32(key, 28);

            state[12] = LoadLittleEndian32(nonce, 0);
            state[13] = LoadLittleEndian32(nonce, 4);
            state[14] = LoadLittleEndian32(nonce, 8);
            state[15] = LoadLittleEndian32(nonce, 12);

            for (int i = 0; i < 10; i++)
            {
                QuarterRound(ref state[0], ref state[4], ref state[8], ref state[12]);
                QuarterRound(ref state[1], ref state[5], ref state[9], ref state[13]);
                QuarterRound(ref state[2], ref state[6], ref state[10], ref state[14]);
                QuarterRound(ref state[3], ref state[7], ref state[11], ref state[15]);
                QuarterRound(ref state[0], ref state[5], ref state[10], ref state[15]);
                QuarterRound(ref state[1], ref state[6], ref state[11], ref state[12]);
                QuarterRound(ref state[2], ref state[7], ref state[8], ref state[13]);
                QuarterRound(ref state[3], ref state[4], ref state[9], ref state[14]);
            }

            byte[] result = new byte[32];
            StoreLittleEndian32(result, 0, state[0]);
            StoreLittleEndian32(result, 4, state[1]);
            StoreLittleEndian32(result, 8, state[2]);
            StoreLittleEndian32(result, 12, state[3]);
            StoreLittleEndian32(result, 16, state[12]);
            StoreLittleEndian32(result, 20, state[13]);
            StoreLittleEndian32(result, 24, state[14]);
            StoreLittleEndian32(result, 28, state[15]);

            return result;
        }

        private static void QuarterRound(
            ref uint a, ref uint b,
            ref uint c, ref uint d)
        {
            a += b;
            d ^= a;
            d = RotateLeft(d, 16);
            c += d;
            b ^= c;
            b = RotateLeft(b, 12);
            a += b;
            d ^= a;
            d = RotateLeft(d, 8);
            c += d;
            b ^= c;
            b = RotateLeft(b, 7);
        }

        private static uint RotateLeft(uint value, int offset)
        {
            return (value << offset) | (value >> (32 - offset));
        }

        private static uint LoadLittleEndian32(byte[] data, int offset)
        {
            return (uint)data[offset]
                | ((uint)data[offset + 1] << 8)
                | ((uint)data[offset + 2] << 16)
                | ((uint)data[offset + 3] << 24);
        }

        private static void StoreLittleEndian32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)value;
            data[offset + 1] = (byte)(value >> 8);
            data[offset + 2] = (byte)(value >> 16);
            data[offset + 3] = (byte)(value >> 24);
        }

        private static void ChaCha20Keystream(byte[] output, byte[] key, byte[] nonce, ulong counter)
        {
            ChaCha20Djb(output, null, output.Length, key, nonce, counter);
        }

        private static void ChaCha20Xor(byte[] output, byte[] input, byte[] key, byte[] nonce, ulong counter)
        {
            ChaCha20Djb(output, input, input.Length, key, nonce, counter);
        }

        private static void ChaCha20Djb(byte[] output, byte[]? input, int length, byte[] key, byte[] nonce, ulong ctr)
        {
            uint[] state = new uint[16];
            state[0] = 0x61707865;
            state[1] = 0x3320646e;
            state[2] = 0x79622d32;
            state[3] = 0x6b206574;
            state[4] = LoadLittleEndian32(key, 0);
            state[5] = LoadLittleEndian32(key, 4);
            state[6] = LoadLittleEndian32(key, 8);
            state[7] = LoadLittleEndian32(key, 12);
            state[8] = LoadLittleEndian32(key, 16);
            state[9] = LoadLittleEndian32(key, 20);
            state[10] = LoadLittleEndian32(key, 24);
            state[11] = LoadLittleEndian32(key, 28);
            state[12] = (uint)ctr;
            state[13] = (uint)(ctr >> 32);
            state[14] = LoadLittleEndian32(nonce, 0);
            state[15] = LoadLittleEndian32(nonce, 4);

            int offset = 0;
            while (length > 0)
            {
                uint[] working = new uint[16];
                Array.Copy(state, working, 16);

                for (int i = 0; i < 10; i++)
                {
                    QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
                    QuarterRound(ref working[1], ref working[5], ref working[9], ref working[13]);
                    QuarterRound(ref working[2], ref working[6], ref working[10], ref working[14]);
                    QuarterRound(ref working[3], ref working[7], ref working[11], ref working[15]);
                    QuarterRound(ref working[0], ref working[5], ref working[10], ref working[15]);
                    QuarterRound(ref working[1], ref working[6], ref working[11], ref working[12]);
                    QuarterRound(ref working[2], ref working[7], ref working[8], ref working[13]);
                    QuarterRound(ref working[3], ref working[4], ref working[9], ref working[14]);
                }

                for (int j = 0; j < 16; j++)
                {
                    working[j] += state[j];
                }

                byte[] keystream = new byte[64];
                for (int j = 0; j < 16; j++)
                {
                    StoreLittleEndian32(keystream, j * 4, working[j]);
                }

                int blockLen = Math.Min(64, length);
                for (int j = 0; j < blockLen; j++)
                {
                    byte p = (input != null) ? input[offset + j] : (byte)0;
                    output[offset + j] = (byte)(p ^ keystream[j]);
                }

                offset += blockLen;
                length -= blockLen;

                state[12]++;
                if (state[12] == 0)
                {
                    state[13]++;
                }
            }
        }

        private static byte[] Poly1305ComputeMac(byte[] authKey, byte[] ciphertext)
        {
            int paddedLen = ciphertext.Length + ((16 - (ciphertext.Length % 16)) % 16);
            byte[] message = new byte[paddedLen + 16];

            Array.Copy(ciphertext, 0, message, 0, ciphertext.Length);

            int sizeOffset = paddedLen;
            ulong textSize = (ulong)ciphertext.Length;
            for (int i = 0; i < 8; i++)
            {
                message[sizeOffset + 8 + i] = (byte)((textSize >> (i * 8)) & 0xFF);
            }

            return Sodium.OneTimeAuth.Sign(message, authKey);
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            uint diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }

        private static byte[] BuildPacket(byte[] mac, byte[] encrypted)
        {
            int totalSize = 2 + mac.Length + encrypted.Length;
            byte[] packet = new byte[totalSize];

            packet[0] = (byte)(totalSize & 0xFF);
            packet[1] = (byte)((totalSize >> 8) & 0xFF);
            Array.Copy(mac, 0, packet, 2, mac.Length);
            Array.Copy(encrypted, 0, packet, 2 + mac.Length, encrypted.Length);

            return packet;
        }
    }
}