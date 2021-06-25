using System;
using System.Security.Cryptography;
using System.Text;

// for DES implementation

namespace SQLcomplianceCwfAddin.Helpers
{
    internal static class EncryptionHelper
    {
        private const string ConstKey = "Idera-CWF";

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param account="plaintext">The plaintext.</param>
        /// <returns></returns>
        internal static string QuickEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return (plainText);
            }
            string encrypted;
            try
            {
                using (var algorithm = TripleDESCryptoServiceProvider.Create())
                {
                    try
                    {
                        algorithm.Mode = CipherMode.ECB;
                        algorithm.Key = GenerateKey(algorithm);
                        using (var transform = algorithm.CreateEncryptor())
                        {
                            var buffer = UnicodeEncoding.Unicode.GetBytes(plainText);
                            buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                            encrypted = Convert.ToBase64String(buffer);
                        }
                    }
                    finally
                    {
                        algorithm.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (encrypted);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param account="encryptedText">The encrypted text.</param>
        /// <returns></returns>
        internal static string QuickDecrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return (encryptedText);
            }
            string plaintext;
            using (var algorithm = TripleDESCryptoServiceProvider.Create())
            {
                try
                {
                    algorithm.Mode = CipherMode.ECB;
                    var key = GenerateKey(algorithm);
                    algorithm.Key = key;

                    // Decrypt encrypted byte buffer and return ASCII string
                    using (ICryptoTransform transform = algorithm.CreateDecryptor())
                    {
                        // Base64 decode and decrypt the encrypted string
                        var buffer = Convert.FromBase64String(encryptedText);
                        buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                        plaintext = UnicodeEncoding.Unicode.GetString(buffer);

                    }
                }
                finally
                {
                    algorithm.Clear();
                }
            }
            return (plaintext);
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param account="algorithm">The algorithm.</param>
        /// <returns></returns>
        /// <remarks>
        /// This version of Generate key always returns the password encoded to 192 bits.
        /// Other versions rolling around inside Idera are probably going to return 128 bits 
        /// if the key will fit in 128 bits (16 bytes for the division impaired).
        /// This version matches the helper in java.
        /// </remarks>
        private static byte[] GenerateKey(SymmetricAlgorithm algorithm)
        {
            var sTemp = ConstKey.PadRight(24, ' ');
            // convert the secret key to byte array
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

    }

}
