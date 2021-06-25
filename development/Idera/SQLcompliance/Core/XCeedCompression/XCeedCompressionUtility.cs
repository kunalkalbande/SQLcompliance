using System;
using System.IO;
using Xceed.Compression;
using Xceed.Compression.Formats;

namespace Idera.SQLcompliance.Core.XCeedCompression
{
    internal static class XCeedCompressionUtility
    {

        static XCeedCompressionUtility()
        {
            Xceed.Compression.Formats.Licenser.LicenseKey = CoreConstants.Xceed_LicenseKey_Compression;
        }

        /// <summary>
        /// Uncompresses to disk using XCeed Compression.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="decompressedFileName">Name of the decompressed file.</param>
        static internal void UncompressToDisk(string sourceFileName, string decompressedFileName)
        {
            using (Stream sourceFile = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
            {
                using (Stream destinationFile = new FileStream(decompressedFileName, FileMode.Create))
                {
                    byte[] buffer = new byte[CoreConstants.StreamBufferSize];
                    int read;

                    using (XceedCompressedStream standard = new XceedCompressedStream(sourceFile, CompressionMethod.Deflated, CompressionLevel.Lowest))
                    {
                        while ((read = standard.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destinationFile.Write(buffer, 0, read);
                        }

                        standard.Close();
                    }
                }
                sourceFile.Close();
            }
        }

    }
}
