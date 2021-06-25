using System;
using System.Text;
using System.Security.Cryptography;

namespace Idera.SQLcompliance.Core.Encryption
{
	/// <summary>
	/// Summary description for EncryptionHelper.
	/// </summary>
	internal class EncryptionHelper
	{

		#region Private static variables

		private const string constKey = "SQLsafe";

		#endregion

		#region Private constructor

		// Private constructor, so that this class is not instantiated
		private EncryptionHelper() {}

		#endregion

		#region Quick encrypt/decrypt/hash

		/// <summary>
		/// Quick encrypt a string of plaintext data.  (Internal)
		/// </summary>
		/// <param name="plaintext">The plaintext string</param>
		/// <returns>The encrypted string</returns>
		/// <remarks>Uses TripleDES encryption, with hardcoded private key.</remarks>
		internal static string QuickEncryptInternal(string plaintext) {

			if (plaintext == null) {
				return null;
			}

			TripleDESCryptoServiceProvider des = ConfigureTripleDESProvider();
			
			// Encrypt the string and base64 encode the encrypted string
			// for platform independence
			byte[] buffer = UnicodeEncoding.Unicode.GetBytes(plaintext);
			string encrypted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));

			// Cleanup
			des = null;

			return encrypted;

		}

		internal static string QuickDecryptInternal(string encryptedText) {
			return QuickDecryptInternal(encryptedText, true);
		}

		/// <summary>
		/// Quick decrypt a string of encrypted data (encrypted with the hardcoded private key from QuickEncrypt)
		/// </summary>
		/// <param name="encryptedtext">The encrypted string</param>
		/// <returns>The decrypted string</returns>
		internal static string QuickDecryptInternal(string encryptedtext, bool unicode) {

			if (encryptedtext == null) {
				return null;
			}

			TripleDESCryptoServiceProvider des = ConfigureTripleDESProvider();

			// Base64 decode and decrypt the encrypted string
			byte[] buffer = Convert.FromBase64String(encryptedtext);

			//decrypt DES 3 encrypted byte buffer and return ASCII string
			string plaintext = null;
			if (unicode) {
				plaintext = UnicodeEncoding.Unicode.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
			} else {
				plaintext = ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
			}

			// Cleanup
			des = null;

			return plaintext;

		}
				
		#endregion

		#region Private methods

		/// <summary>
		/// Setup the TripleDES provider used by QuickEncrypt/QuickDecrypt
		/// </summary>
		/// <returns></returns>
		private static TripleDESCryptoServiceProvider ConfigureTripleDESProvider() {

			// Generate an MD5 hash from the private key 
         MD5 hashmd5 = new MD5();
         byte[] pwdhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(constKey));
         hashmd5 = null;

			// Use DES3 encryption
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

			// The key is the secret password hash
			des.Key = pwdhash;
			des.Mode = CipherMode.ECB; // could be CBC, CFB

			return des;

		}

		#endregion


      #region MD5 open source implementation
         // $Id: MD5.cs 18 2006-02-19 21:55:46Z jay $

         /* ***** BEGIN LICENSE BLOCK *****
          * Version: MPL 1.1
          *
          * The contents of this file are subject to the Mozilla Public License Version
          * 1.1 (the "License"); you may not use this file except in compliance with
          * the License. You may obtain a copy of the License at
          * http://www.mozilla.org/MPL/
          *
          * Software distributed under the License is distributed on an "AS IS" basis,
          * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
          * for the specific language governing rights and limitations under the
          * License.
          *
          * The Original Code is Classless.Hasher - C#/.NET Hash and Checksum Algorithm Library.
          *
          * The Initial Developer of the Original Code is Classless.net.
          * Portions created by the Initial Developer are Copyright (C) 2004 the Initial
          * Developer. All Rights Reserved.
          *
          * Contributor(s):
          *		Jason Simeone (jay@classless.net)
          * 
          * ***** END LICENSE BLOCK ***** */

	         /// <summary>Computes the MD5 hash for the input data using the managed library.</summary>
	         private class MD5 : BlockHashAlgorithm {
		         internal uint[] accumulator;


		         /// <summary>Initializes a new instance of the MD5 class.</summary>
		         public MD5() : base(64) {
			         lock (this) {
				         HashSizeValue = 128;
				         accumulator = new uint[4];
				         Initialize();
			         }
		         }


		         /// <summary>Initializes the algorithm.</summary>
		         override public void Initialize() {
			         lock (this) {
				         accumulator[0] = 0x67452301;
				         accumulator[1] = 0xEFCDAB89;
				         accumulator[2] = 0x98BADCFE;
				         accumulator[3] = 0x10325476;
				         base.Initialize();
			         }
		         }


		         /// <summary>Process a block of data.</summary>
		         /// <param name="inputBuffer">The block of data to process.</param>
		         /// <param name="inputOffset">Where to start in the block.</param>
		         override protected void ProcessBlock(byte[] inputBuffer, int inputOffset) {
			         lock (this) {
				         uint[] workBuffer;
				         uint a = accumulator[0];
				         uint b = accumulator[1];
				         uint c = accumulator[2];
				         uint d = accumulator[3];

				         workBuffer = ByteToUInt(inputBuffer, inputOffset, BlockSize);

				         #region Round 1
				         a = FF(a, b, c, d, workBuffer[ 0],  7, 0xD76AA478);
				         d = FF(d, a, b, c, workBuffer[ 1], 12, 0xE8C7B756);
				         c = FF(c, d, a, b, workBuffer[ 2], 17, 0x242070DB);
				         b = FF(b, c, d, a, workBuffer[ 3], 22, 0xC1BDCEEE);
				         a = FF(a, b, c, d, workBuffer[ 4],  7, 0xF57C0FAF);
				         d = FF(d, a, b, c, workBuffer[ 5], 12, 0x4787C62A);
				         c = FF(c, d, a, b, workBuffer[ 6], 17, 0xA8304613);
				         b = FF(b, c, d, a, workBuffer[ 7], 22, 0xFD469501);
				         a = FF(a, b, c, d, workBuffer[ 8],  7, 0x698098D8);
				         d = FF(d, a, b, c, workBuffer[ 9], 12, 0x8B44F7AF);
				         c = FF(c, d, a, b, workBuffer[10], 17, 0xFFFF5BB1);
				         b = FF(b, c, d, a, workBuffer[11], 22, 0x895CD7BE);
				         a = FF(a, b, c, d, workBuffer[12],  7, 0x6B901122);
				         d = FF(d, a, b, c, workBuffer[13], 12, 0xFD987193);
				         c = FF(c, d, a, b, workBuffer[14], 17, 0xA679438E);
				         b = FF(b, c, d, a, workBuffer[15], 22, 0x49B40821);
				         #endregion

				         #region Round 2
				         a = GG(a, b, c, d, workBuffer[ 1],  5, 0xF61E2562);
				         d = GG(d, a, b, c, workBuffer[ 6],  9, 0xC040B340);
				         c = GG(c, d, a, b, workBuffer[11], 14, 0x265E5A51);
				         b = GG(b, c, d, a, workBuffer[ 0], 20, 0xE9B6C7AA);
				         a = GG(a, b, c, d, workBuffer[ 5],  5, 0xD62F105D);
				         d = GG(d, a, b, c, workBuffer[10],  9, 0x02441453);
				         c = GG(c, d, a, b, workBuffer[15], 14, 0xD8A1E681);
				         b = GG(b, c, d, a, workBuffer[ 4], 20, 0xE7D3FBC8);
				         a = GG(a, b, c, d, workBuffer[ 9],  5, 0x21E1CDE6);
				         d = GG(d, a, b, c, workBuffer[14],  9, 0xC33707D6);
				         c = GG(c, d, a, b, workBuffer[ 3], 14, 0xF4D50D87);
				         b = GG(b, c, d, a, workBuffer[ 8], 20, 0x455A14ED);
				         a = GG(a, b, c, d, workBuffer[13],  5, 0xA9E3E905);
				         d = GG(d, a, b, c, workBuffer[ 2],  9, 0xFCEFA3F8);
				         c = GG(c, d, a, b, workBuffer[ 7], 14, 0x676F02D9);
				         b = GG(b, c, d, a, workBuffer[12], 20, 0x8D2A4C8A);
				         #endregion

				         #region Round 3
				         a = HH(a, b, c, d, workBuffer[ 5],  4, 0xFFFA3942);
				         d = HH(d, a, b, c, workBuffer[ 8], 11, 0x8771F681);
				         c = HH(c, d, a, b, workBuffer[11], 16, 0x6D9D6122);
				         b = HH(b, c, d, a, workBuffer[14], 23, 0xFDE5380C);
				         a = HH(a, b, c, d, workBuffer[ 1],  4, 0xA4BEEA44);
				         d = HH(d, a, b, c, workBuffer[ 4], 11, 0x4BDECFA9);
				         c = HH(c, d, a, b, workBuffer[ 7], 16, 0xF6BB4B60);
				         b = HH(b, c, d, a, workBuffer[10], 23, 0xBEBFBC70);
				         a = HH(a, b, c, d, workBuffer[13],  4, 0x289B7EC6);
				         d = HH(d, a, b, c, workBuffer[ 0], 11, 0xEAA127FA);
				         c = HH(c, d, a, b, workBuffer[ 3], 16, 0xD4EF3085);
				         b = HH(b, c, d, a, workBuffer[ 6], 23, 0x04881D05);
				         a = HH(a, b, c, d, workBuffer[ 9],  4, 0xD9D4D039);
				         d = HH(d, a, b, c, workBuffer[12], 11, 0xE6DB99E5);
				         c = HH(c, d, a, b, workBuffer[15], 16, 0x1FA27CF8);
				         b = HH(b, c, d, a, workBuffer[ 2], 23, 0xC4AC5665);
				         #endregion

				         #region Round 4
				         a = II(a, b, c, d, workBuffer[ 0],  6, 0xF4292244);
				         d = II(d, a, b, c, workBuffer[ 7], 10, 0x432AFF97);
				         c = II(c, d, a, b, workBuffer[14], 15, 0xAB9423A7);
				         b = II(b, c, d, a, workBuffer[ 5], 21, 0xFC93A039);
				         a = II(a, b, c, d, workBuffer[12],  6, 0x655B59C3);
				         d = II(d, a, b, c, workBuffer[ 3], 10, 0x8F0CCC92);
				         c = II(c, d, a, b, workBuffer[10], 15, 0xFFEFF47D);
				         b = II(b, c, d, a, workBuffer[ 1], 21, 0x85845DD1);
				         a = II(a, b, c, d, workBuffer[ 8],  6, 0x6FA87E4F);
				         d = II(d, a, b, c, workBuffer[15], 10, 0xFE2CE6E0);
				         c = II(c, d, a, b, workBuffer[ 6], 15, 0xA3014314);
				         b = II(b, c, d, a, workBuffer[13], 21, 0x4E0811A1);
				         a = II(a, b, c, d, workBuffer[ 4],  6, 0xF7537E82);
				         d = II(d, a, b, c, workBuffer[11], 10, 0xBD3AF235);
				         c = II(c, d, a, b, workBuffer[ 2], 15, 0x2AD7D2BB);
				         b = II(b, c, d, a, workBuffer[ 9], 21, 0xEB86D391);
				         #endregion

				         accumulator[0] += a;
				         accumulator[1] += b;
				         accumulator[2] += c;
				         accumulator[3] += d;
			         }
		         }


		         /// <summary>Process the last block of data.</summary>
		         /// <param name="inputBuffer">The block of data to process.</param>
		         /// <param name="inputOffset">Where to start in the block.</param>
		         /// <param name="inputCount">How many blocks have been processed so far.</param>
		         override protected byte[] ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
			         lock (this) {
				         byte[] temp;
				         int paddingSize;
				         ulong size;

				         // Figure out how much padding is needed between the last byte and the size.
				         paddingSize = (int)((inputCount + Count) % BlockSize);
				         paddingSize = (BlockSize - 8) - paddingSize;
				         if (paddingSize < 1) { paddingSize += BlockSize; }

				         // Create the final, padded block(s).
				         temp = new byte[inputCount + paddingSize + 8];
				         Array.Copy(inputBuffer, inputOffset, temp, 0, inputCount);
				         temp[inputCount] = 0x80;
				         size = ((ulong)Count + (ulong)inputCount) * 8;
				         Array.Copy(ULongToByte(size), 0, temp, (inputCount + paddingSize), 8);

				         // Push the final block(s) into the calculation.
				         ProcessBlock(temp, 0);
				         if (temp.Length == (BlockSize * 2)) {
					         ProcessBlock(temp, BlockSize);
				         }

				         return UIntToByte(accumulator);
			         }
		         }
               
		         private uint FF(uint a, uint b, uint c, uint d, uint k, int s, uint t) {
			         a += k + t + (d ^ (b & (c ^ d)));
			         a = RotateLeft(a, s);
			         return a + b;
		         }

		         private uint GG(uint a, uint b, uint c, uint d, uint k, int s, uint t) {
			         a += k + t + (c ^ (d & (b ^ c)));
			         a = RotateLeft(a, s);
			         return a + b;
		         }

		         private uint HH(uint a, uint b, uint c, uint d, uint k, int s, uint t) {
			         a += k + t + (b ^c ^ d);
			         a = RotateLeft(a, s);
			         return a + b;
		         }

		         private uint II(uint a, uint b, uint c, uint d, uint k, int s, uint t) {
			         a += k + t + (c  ^ (b | ~d));
			         a = RotateLeft(a, s);
			         return a + b;
		         }
	         }

            abstract private class BlockHashAlgorithm : System.Security.Cryptography.HashAlgorithm
            {
               private int blockSize;
               private byte[] buffer;
               private int bufferCount;
               private long count;


               /// <summary>The size in bytes of an individual block.</summary>
               public int BlockSize
               {
                  get { return blockSize; }
               }

               /// <summary>The number of bytes currently in the buffer waiting to be processed.</summary>
               public int BufferCount
               {
                  get { return bufferCount; }
               }

               /// <summary>The number of bytes that have been processed.</summary>
               /// <remarks>This number does NOT include the bytes that are waiting in the buffer.</remarks>
               public long Count
               {
                  get { return count; }
               }


               /// <summary>Initializes a new instance of the BlockHashAlgorithm class.</summary>
               /// <param name="blockSize">The size in bytes of an individual block.</param>
               protected BlockHashAlgorithm(int blockSize)
                  : base()
               {
                  this.blockSize = blockSize;
               }


               /// <summary>Initializes the algorithm.</summary>
               /// <remarks>If this function is overriden in a derived class, the new function should call back to
               /// this function or you could risk garbage being carried over from one calculation to the next.</remarks>
               override public void Initialize()
               {
                  lock (this)
                  {
                     count = 0;
                     bufferCount = 0;
                     State = 0;
                     buffer = new byte[BlockSize];
                  }
               }


               /// <summary>Performs the hash algorithm on the data provided.</summary>
               /// <param name="array">The array containing the data.</param>
               /// <param name="ibStart">The position in the array to begin reading from.</param>
               /// <param name="cbSize">How many bytes in the array to read.</param>
               override protected void HashCore(byte[] array, int ibStart, int cbSize)
               {
                  lock (this)
                  {
                     int i;

                     // Use what may already be in the buffer.
                     if (BufferCount > 0)
                     {
                        if (cbSize < (BlockSize - BufferCount))
                        {
                           // Still don't have enough for a full block, just store it.
                           Array.Copy(array, ibStart, buffer, BufferCount, cbSize);
                           bufferCount += cbSize;
                           return;
                        }
                        else
                        {
                           // Fill out the buffer to make a full block, and then process it.
                           i = BlockSize - BufferCount;
                           Array.Copy(array, ibStart, buffer, BufferCount, i);
                           ProcessBlock(buffer, 0);
                           count += (long)BlockSize;
                           bufferCount = 0;
                           ibStart += i;
                           cbSize -= i;
                        }
                     }

                     // For as long as we have full blocks, process them.
                     for (i = 0; i < (cbSize - (cbSize % BlockSize)); i += BlockSize)
                     {
                        ProcessBlock(array, ibStart + i);
                        count += (long)BlockSize;
                     }

                     // If we still have some bytes left, store them for later.
                     int bytesLeft = cbSize % BlockSize;
                     if (bytesLeft != 0)
                     {
                        Array.Copy(array, ((cbSize - bytesLeft) + ibStart), buffer, 0, bytesLeft);
                        bufferCount = bytesLeft;
                     }
                  }
               }


               /// <summary>Performs any final activities required by the hash algorithm.</summary>
               /// <returns>The final hash value.</returns>
               override protected byte[] HashFinal()
               {
                  lock (this)
                  {
                     return ProcessFinalBlock(buffer, 0, bufferCount);
                  }
               }


               /// <summary>Process a block of data.</summary>
               /// <param name="inputBuffer">The block of data to process.</param>
               /// <param name="inputOffset">Where to start in the block.</param>
               abstract protected void ProcessBlock(byte[] inputBuffer, int inputOffset);


               /// <summary>Process the last block of data.</summary>
               /// <param name="inputBuffer">The block of data to process.</param>
               /// <param name="inputOffset">Where to start in the block.</param>
               /// <param name="inputCount">How many bytes need to be processed.</param>
               abstract protected byte[] ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
            }

            static private ushort RotateLeft(ushort x, int shift)
            {
               return (ushort)((x << shift) | (x >> (16 - shift)));
            }

            static private uint RotateLeft(uint x, int shift)
            {
               return (x << shift) | (x >> (32 - shift));
            }

            static private ulong RotateLeft(ulong x, int shift)
            {
               return (x << shift) | (x >> (64 - shift));
            }

            public enum EndianType
            {
               /// <summary>The Least Significant Byte is first.</summary>
               LittleEndian,
               /// <summary>The Most Significant Byte is first.</summary>
               BigEndian
            };

            static private byte[] UIntToByte(uint data) { return UIntToByte(new uint[1] { data }, 0, 1, EndianType.LittleEndian); }
            static private byte[] UIntToByte(uint data, EndianType endian) { return UIntToByte(new uint[1] { data }, 0, 1, endian); }
            static private byte[] UIntToByte(uint[] array) { return UIntToByte(array, 0, array.Length, EndianType.LittleEndian); }
            static private byte[] UIntToByte(uint[] array, EndianType endian) { return UIntToByte(array, 0, array.Length, endian); }
            static private byte[] UIntToByte(uint[] array, int offset, int length) { return UIntToByte(array, offset, length, EndianType.LittleEndian); }
            static private byte[] UIntToByte(uint[] array, int offset, int length, EndianType endian)
            {
               if ((length + offset) > array.Length)
               {
                  throw new Exception("The length and offset provided extend past the end of the array.");
               }

               byte[] temp = new byte[length * 4];

               for (int i = offset; i < (offset + length); i++)
               {
                  for (int j = 0; j < 4; j++)
                  {
                     if (endian == EndianType.LittleEndian)
                     {
                        temp[(i - offset) * 4 + j] = (byte)(array[i] >> (j * 8));
                     }
                     else
                     {
                        temp[(i - offset) * 4 + (3 - j)] = (byte)(array[i] >> (j * 8));
                     }
                  }
               }

               return temp;
            }

            static private byte[] ULongToByte(ulong data) { return ULongToByte(new ulong[1] { data }, 0, 1, EndianType.LittleEndian); }
            static private byte[] ULongToByte(ulong data, EndianType endian) { return ULongToByte(new ulong[1] { data }, 0, 1, endian); }
            static private byte[] ULongToByte(ulong[] array) { return ULongToByte(array, 0, array.Length, EndianType.LittleEndian); }
            static private byte[] ULongToByte(ulong[] array, EndianType endian) { return ULongToByte(array, 0, array.Length, endian); }
            static private byte[] ULongToByte(ulong[] array, int offset, int length) { return ULongToByte(array, offset, length, EndianType.LittleEndian); }
            static private byte[] ULongToByte(ulong[] array, int offset, int length, EndianType endian)
            {
               if ((length + offset) > array.Length)
               {
                  throw new Exception("The length and offset provided extend past the end of the array.");
               }

               byte[] temp = new byte[length * 8];

               for (int i = offset; i < (offset + length); i++)
               {
                  for (int j = 0; j < 8; j++)
                  {
                     if (endian == EndianType.LittleEndian)
                     {
                        temp[(i - offset) * 8 + j] = (byte)(array[i] >> (j * 8));
                     }
                     else
                     {
                        temp[(i - offset) * 8 + (7 - j)] = (byte)(array[i] >> (j * 8));
                     }
                  }
               }

               return temp;
            }

            static private uint[] ByteToUInt(byte[] array) { return ByteToUInt(array, 0, array.Length, EndianType.LittleEndian); }
            static private uint[] ByteToUInt(byte[] array, EndianType endian) { return ByteToUInt(array, 0, array.Length, endian); }
            static private uint[] ByteToUInt(byte[] array, int offset, int length) { return ByteToUInt(array, offset, length, EndianType.LittleEndian); }
            static private uint[] ByteToUInt(byte[] array, int offset, int length, EndianType endian)
            {
			      if ((length + offset) > array.Length) {
				      throw new Exception("The length and offset provided extend past the end of the array.");
			      }
			      if ((length % 4) != 0) {
				      throw new ArgumentException("The number of bytes to convert must be a multiple of 4.", "length");
			      }

			      uint[] temp = new uint[length / 4];

			      for (int i = 0, j = offset; i < temp.Length; i++) {
				      if (endian == EndianType.LittleEndian) {
					      temp[i] = (((uint)array[j++] & 0xFF)      ) |
							        (((uint)array[j++] & 0xFF) <<  8) |
							        (((uint)array[j++] & 0xFF) << 16) |
							        (((uint)array[j++] & 0xFF) << 24);
				      } else {
					      temp[i] = (((uint)array[j++] & 0xFF) << 24) |
							        (((uint)array[j++] & 0xFF) << 16) |
							        (((uint)array[j++] & 0xFF) <<  8) |
							        (((uint)array[j++] & 0xFF)      );
				      }
			      }

			      return temp;
		      }      
      #endregion

   }
}
