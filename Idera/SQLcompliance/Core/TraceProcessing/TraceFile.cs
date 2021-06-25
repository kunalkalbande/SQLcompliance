using System;
using System.IO;
using Xceed.Compression;
using Idera.SQLcompliance.Core.SevenZipComperssion;
using Idera.SQLcompliance.Core.XCeedCompression;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
    /// <summary>
    /// Summary description for TraceFile.
    /// </summary>
    internal class TraceFile
    {


        //-----------------------------------------------------------------------
        // CalculateChecksum - Padded with random seed 
        //-----------------------------------------------------------------------
        static public int
           CalculateChecksum(
              string fileName
           )
        {
            return (int)SevenZipCompressionUtility.CalculateCrc32(fileName);
        }

        //-----------------------------------------------------------------------
        // UncompressToDisk
        //-----------------------------------------------------------------------
        static public void
           UncompressToDisk(
              string sourceFileName,
              string decompressedFileName
           )
        {

            var fileExtension = Path.GetExtension(sourceFileName);
            if (fileExtension == ".trz" || fileExtension == ".biz")
                XCeedCompressionUtility.UncompressToDisk(sourceFileName, decompressedFileName);
            else if (fileExtension == ".trc" || fileExtension == ".xel" || fileExtension == ".sqlaudit" || fileExtension == ".bin")
            {
                // Do nothing for uncompressed files
            }
            else
                SevenZipCompressionUtility.UncompressToDisk(sourceFileName, decompressedFileName);
        }

        //-----------------------------------------------------------------------
        // CompressToDisk
        //-----------------------------------------------------------------------
        static public string
           CompressToDisk(
              string sourceFileName
           )
        {
            string compressedFilename;

            try
            {
                string extension = Path.GetExtension(sourceFileName);
                extension = extension.Remove(extension.Length - 1, 1) + "7z";

                compressedFilename = Path.ChangeExtension(sourceFileName, extension);
                SevenZipCompressionUtility.CompressToDisk(sourceFileName, compressedFilename);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, e, true);
                throw e;
            }
            return compressedFilename;
        }


        //-----------------------------------------------------------------------
        // CompressToByteArray
        //-----------------------------------------------------------------------
        public static byte[]
           CompressToByteArray(
           string sourceFileName
           )
        {
            return CompressToByteArray(sourceFileName, CompressionLevel.Lowest);
        }

        //-----------------------------------------------------------------------
        // CompressToByteArray
        //-----------------------------------------------------------------------
        public static byte[]
           CompressToByteArray(
              string sourceFileName,
              CompressionLevel level
           )
        {
            if (!File.Exists(sourceFileName))
                return null;

            byte[] output = null;

            try
            {
                using (FileStream sourceFile = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[sourceFile.Length];

                    if ((sourceFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output = QuickCompression.Compress(buffer, CompressionMethod.Deflated, level);
                    }

                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Error compressing " + sourceFileName + ".",
                                         e,
                                         true);
            }
            return output;
        }

        //-----------------------------------------------------------------------
        // CompressToMemoryStream
        //-----------------------------------------------------------------------
        /// <summary>
        /// Compress the source file into a memory stream object
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        internal static MemoryStream
           CompressToMemoryStream(
           string sourceFileName
           )
        {
            if (!File.Exists(sourceFileName))
                return null;

            return SevenZipCompressionUtility.CompressToMemoryStream(sourceFileName);
        }

        internal static MemoryStream
           ConvertToMemoryStream(
           string sourceFileName
           )
        {
            if (!File.Exists(sourceFileName))
                return null;

            FileStream fs = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            fs.Close();
            fs.Dispose();
            return ms;
        }

        //-----------------------------------------------------------------------
        // PersistToDisk
        //-----------------------------------------------------------------------
        /// <summary>
        /// Write the input file name to disk using combined directory and fileName.
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        /// </summary>
        internal static string
           PersistToDisk(
              string directory,
              string fileName,
              Stream fileStream
           )
        {
            FileStream localFile;
            string localFileName = Path.Combine(directory, fileName);

            try
            {
                if (fileStream.CanRead)
                {
                    using (localFile = new FileStream(localFileName, FileMode.CreateNew))
                    {

                        //                  using( FileStream reader = new FileStream( fileStream, FileAccess.Read ) )
                        {
                            int bytesRead;
                            byte[] buffer = new byte[CoreConstants.StreamBufferSize];

                            while ((bytesRead = fileStream.Read(buffer, 0, CoreConstants.StreamBufferSize)) > 0)
                            {
                                localFile.Write(buffer, 0, bytesRead);
                            }

                            // Make sure all of the buffers are flushed to OS
                            NativeMethods.FlushFileBuffers(localFile.SafeFileHandle.DangerousGetHandle());

                            localFile.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("PersistToDiskStream::Unable to write data to trace file.", e, ErrorLog.Severity.Error, true);
                string message = String.Format(CoreConstants.Exception_Format_ErrorWritingStreamToDisk, localFileName);
                //ErrorLog.Instance.Write( ErrorLog.Level.Debug, message, e, true );
                throw new CoreException(message, e);
            }

            return localFileName;
        }

        //-----------------------------------------------------------------------
        // PersistToDisk
        //-----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static string
           PersistToDisk(
           string directory,
           string fileName,
           byte[] fileContent)
        {
            return PersistToDisk(directory, fileName, fileContent, fileContent.Length);
        }
        //-----------------------------------------------------------------------
        // PersistToDisk
        //-----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static string
           PersistToDisk(
           string directory,
           string fileName,
           byte[] fileContent,
           int length
           )
        {
            FileStream localFile;
            string localFileName = Path.Combine(directory, fileName);

            try
            {
                using (localFile = new FileStream(localFileName, FileMode.CreateNew))
                {
                    localFile.Write(fileContent, 0, length);

                    // Make sure all of the buffers are flushed to OS
                    NativeMethods.FlushFileBuffers(localFile.SafeFileHandle.DangerousGetHandle());
                    localFile.Close();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("PersistToDiskArray::Unable to write data to trace file.", e, ErrorLog.Severity.Error, true);
                string message = String.Format(CoreConstants.Exception_Format_ErrorWritingStreamToDisk, localFileName);
                throw new CoreException(message, e);
            }
            return localFileName;
        }
    }
}
