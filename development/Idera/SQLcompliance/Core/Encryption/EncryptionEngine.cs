using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Idera.SQLcompliance.Core.Encryption
{

	/// <summary>
	/// Encapsulates logic for encrypting buffers of data using various algorithms
	/// </summary>
	internal class EncryptionEngine
	{
	   #region enum
	   
		public enum EncryptionType
		{
			None,
			Rijndael,
			RC2,
			DES,
			TripleDES
		}		
		
		#endregion

		#region Instance variables

		/// <summary>
		/// The encryption algorithm
		/// </summary>
		private SymmetricAlgorithm algorithm;

		/// <summary>
		/// The cryptographic transform
		/// </summary>
		private ICryptoTransform cryptoTransform;	
		
		#endregion

		#region Constructors

		/// <summary>
		/// Constructor to configure the encryption algorithm given the encryption type and key
		/// </summary>
		/// <param name="encryptionType"></param>
		/// <param name="key"></param>
		internal
		   EncryptionEngine
		   (
		      EncryptionType   encryptionType,
		      string                  key,
		      bool                    encrypt
		   )
		{
			// The encryption algorithm
			algorithm = SymmetricAlgorithm.Create(encryptionType.ToString());			

			// Configure the Key and IV properties of the encryption 
			// algorithm based on the values provided.
			KeySizes[] keySizes = algorithm.LegalKeySizes;
			KeySizes[] blockSizes = algorithm.LegalBlockSizes;			
			byte[] salt = Encoding.ASCII.GetBytes(key);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, salt);
			pdb.HashName = "SHA256"; // need this resolution for highest encryption
			algorithm.Key = pdb.GetBytes(keySizes[0].MaxSize / 8);
			byte[] ivBytes = pdb.GetBytes(blockSizes[0].MinSize / 8);
			algorithm.IV = ivBytes;

			// Create the CryptoTransform object	
			if (encrypt) {
				cryptoTransform = algorithm.CreateEncryptor();				
			} else {
				cryptoTransform = algorithm.CreateDecryptor();			
			}			
						
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Encrypt a buffer of data
		/// </summary>
		/// <param name="buffer">The buffer of unencrypted data</param>
		/// <param name="bufferLength">Length of data in buffer to encrypt</param>
		/// <param name="encryptedLength">Length of encrypted data in buffer</param>
		internal void Encrypt(byte[] buffer, int offset, int length, out int encryptedLength) {	
		
			byte[] outputBuffer = cryptoTransform.TransformFinalBlock(buffer, offset, length);
			outputBuffer.CopyTo(buffer, offset);
			encryptedLength = outputBuffer.Length;
			outputBuffer = null;

		}

		/// <summary>
		/// Decrypt a buffer of data
		/// </summary>
		/// <param name="buffer">The buffer of encrypted data</param>
		/// <param name="bufferLength">Length of data in buffer to decrypt</param>
		/// <param name="encryptedLength">Length of unencrypted data in buffer</param>
		internal void Decrypt(byte[] buffer, int offset, int length, out int decryptedLength) {						

			byte[] outputBuffer = cryptoTransform.TransformFinalBlock(buffer, offset, length);
			outputBuffer.CopyTo(buffer, offset);
			decryptedLength = outputBuffer.Length;
			outputBuffer = null;
			
		}

		#endregion	

		#region Static methods

		public static string
		   QuickEncrypt(
		      string               plaintext)
      {
			return EncryptionHelper.QuickEncryptInternal(plaintext);
		}
		
		public static string
		   QuickDecrypt(string encryptedText)
		{
			return EncryptionHelper.QuickDecryptInternal(encryptedText);
		}

		#endregion		

	}

}
