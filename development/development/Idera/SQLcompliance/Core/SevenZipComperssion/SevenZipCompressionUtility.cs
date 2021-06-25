using SevenZip;
using System;
using System.IO;

namespace Idera.SQLcompliance.Core.SevenZipComperssion
{
    internal static class SevenZipCompressionUtility
    {
        /// <summary>
        /// Compresses to disk Using Seven Zip.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="compressedFilename">The compressed filename.</param>
        internal static void CompressToDisk(string sourceFileName, string compressedFilename)
        {
            Int32 dictionary = 1 << 23;
            Int32 posStateBits = 2;
            Int32 litContextBits = 3;
            Int32 litPosBits = 0;
            Int32 algorithm = 2;
            Int32 numFastBytes = 32;

            string mf = "bt4";
            bool eos = true;
            bool stdInMode = false;

            CoderPropID[] propIDs =  {
               CoderPropID.DictionarySize,
               CoderPropID.PosStateBits,
               CoderPropID.LitContextBits,
               CoderPropID.LitPosBits,
               CoderPropID.Algorithm,
               CoderPropID.NumFastBytes,
               CoderPropID.MatchFinder,
               CoderPropID.EndMarker
           };

            object[] properties = {
               (Int32)(dictionary),
               (Int32)(posStateBits),
               (Int32)(litContextBits),
               (Int32)(litPosBits),
               (Int32)(algorithm),
               (Int32)(numFastBytes),
               mf,
               eos
           };

            using (FileStream inStream = new FileStream(sourceFileName, FileMode.Open))
            {
                using (FileStream outStream = new FileStream(compressedFilename, FileMode.Create))
                {
                    SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
                    encoder.SetCoderProperties(propIDs, properties);
                    encoder.WriteCoderProperties(outStream);
                    Int64 fileSize;
                    if (eos || stdInMode)
                        fileSize = -1;
                    else
                        fileSize = inStream.Length;
                    for (int i = 0; i < 8; i++)
                        outStream.WriteByte((Byte)(fileSize >> (8 * i)));
                    encoder.Code(inStream, outStream, -1, -1, null);
                }
                inStream.Close();
            }

        }

        /// <summary>
        /// Uncompresses to disk Using Seven Zip.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="decompressedFileName">Name of the decompressed file.</param>
        internal static void UncompressToDisk(string sourceFileName, string decompressedFileName)
        {
            try
            {

                using (FileStream input = new FileStream(sourceFileName, FileMode.Open))
                {
                    using (FileStream output = new FileStream(decompressedFileName, FileMode.Create))
                    {
                        SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();

                        byte[] properties = new byte[5];
                        if (input.Read(properties, 0, 5) != 5)
                            throw (new Exception("input .lzma is too short"));
                        decoder.SetDecoderProperties(properties);

                        long outSize = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            int v = input.ReadByte();
                            if (v < 0)
                                throw (new Exception("Can't Read 1"));
                            outSize |= ((long)(byte)v) << (8 * i);
                        }
                        long compressedSize = input.Length - input.Position;

                        decoder.Code(input, output, compressedSize, outSize, null);
                    }
                    input.Close();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Compresses to memory stream Using Seven Zip.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <returns></returns>
        internal static MemoryStream CompressToMemoryStream(string sourceFileName)
        {
            if (!File.Exists(sourceFileName))
                return null;
            MemoryStream ms = new MemoryStream();
            using (FileStream sourceFileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[sourceFileStream.Length];
                sourceFileStream.Read(bytes, 0, (int)sourceFileStream.Length);
                ms.Write(bytes, 0, (int)sourceFileStream.Length);
                sourceFileStream.Close();
            }
            return Compress(ms);
        }

        /// <summary>
        /// Calculates the CRC32.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        internal static uint CalculateCrc32(string fileName)
        {
            uint crc = CoreConstants.ChecksumSeed;
            using (Stream sourceFile = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[CoreConstants.StreamBufferSize];
                int read;
                while ((read = sourceFile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    crc = CalculateCrc32(buffer, 0, read, crc);
                }
                sourceFile.Close();
            }
            return crc;
        }

        private static uint CalculateCrc32(byte[] buffer, int offset, int count, uint previousCrc)
        {
            if (count == 0)
            {
                return previousCrc;
            }
            previousCrc = (uint)((int)previousCrc ^ -1);
            offset--;
            while (--count >= 0)
            {
                previousCrc = (Crc32.crc_32_tab[(previousCrc ^ buffer[++offset]) & 0xFF] ^ (previousCrc >> 8));
            }
            return (uint)((int)previousCrc ^ -1);
        }

        private static MemoryStream Compress(MemoryStream inStream)
        {
            inStream.Position = 0;

            CoderPropID[] propIDs =
            {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm
            };

            object[] properties =
            {
                (1 << 16),
                2,
                3,
                0,
                2
            };

            var outStream = new MemoryStream();
            var encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            for (var i = 0; i < 8; i++)
                outStream.WriteByte((byte)(inStream.Length >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            outStream.Flush();
            outStream.Position = 0;

            return outStream;
        }
    }
}
