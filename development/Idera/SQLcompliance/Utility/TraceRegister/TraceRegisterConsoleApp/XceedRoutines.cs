using System;
using System.IO;
using Xceed.Compression;
using Xceed.Compression.Formats;

namespace Idera.SQLcompliance.Utility.TraceRegisterConsoleApp
{
   /// <summary>
   /// Summary description for XceedRoutines.
   /// </summary>
   internal class XceedRoutines
   {
      private const int   ChecksumSeed            = 3662;
      private const int   StreamBufferSize        = 32768;
      public XceedRoutines()
      {
      }
		
      //-----------------------------------------------------------------------
      // CalculateChecksum - Padded with random seed 
      //-----------------------------------------------------------------------
      static public int
         CalculateChecksum(
         string                fileName
         )
      {
         
         int crc = ChecksumSeed;
         
         try
         {
            using(Stream sourceFile = new FileStream( fileName, FileMode.Open, FileAccess.Read ))
            {
              
               byte[] buffer = new byte[ StreamBufferSize ];
               int read = 0;
              
               while( ( read = sourceFile.Read( buffer, 0, buffer.Length ) ) > 0 )
               {
                  crc = ChecksumStream.CalculateCrc32( buffer,0,read,crc );
               }
               sourceFile.Close() ;
            }
         }
         catch (Exception e )
         {
            throw new Exception( String.Format( "Error calculating checksum: {0}", e.Message ) );
         }
         
         return crc;
      }
	
      //-----------------------------------------------------------------------
      // UncompressToDisk
      //-----------------------------------------------------------------------
      static public void
         UncompressToDisk( 
            string               sourceFileName,
            string               decompressedFileName
         )
      {
         using(Stream sourceFile = new FileStream( sourceFileName, FileMode.Open, FileAccess.Read ) )
         {
            using ( Stream destinationFile = new FileStream( decompressedFileName, FileMode.Create ) )
            {
               byte[] buffer = new byte[ StreamBufferSize ];
               int read = 0;

               using(XceedCompressedStream standard = new XceedCompressedStream( sourceFile ))
               {
                  while( ( read = standard.Read( buffer, 0, buffer.Length ) ) > 0 )
                  {
                     destinationFile.Write( buffer, 0, read );
                  }
				      
                  standard.Close();
               }
            }
            sourceFile.Close();
         }
      }

      //-----------------------------------------------------------------------
      // CompressToDisk
      //-----------------------------------------------------------------------
      static public void
         CompressToDisk(
            string            sourceFileName,
            string            compressedFileName
         )
      {
         
         try
         {
            using(Stream sourceFile = new FileStream( sourceFileName, FileMode.Open, FileAccess.Read ),
                     destinationFile = new FileStream( compressedFileName, FileMode.Create ))
            {

               byte [] buffer = new byte[ StreamBufferSize ];
               int read = 0;

               using(XceedCompressedStream standard = new XceedCompressedStream( destinationFile ))
               {
                  while( ( read = sourceFile.Read( buffer, 0, buffer.Length ) ) > 0 )
                  {
                     standard.Write( buffer, 0, read );
                  }
                  standard.Close();
               }
               sourceFile.Close();
            }
         }
         catch( Exception e )
         {
            throw new Exception( String.Format( "Error compressing file: {0}", e.Message ) );
         }
      }


      //-----------------------------------------------------------------------
      // CompressToByteArray
      //-----------------------------------------------------------------------
      public static byte[]  
         CompressToByteArray(
         string            sourceFileName
         )
      {
         return CompressToByteArray( sourceFileName, CompressionLevel.Lowest );
      }

      //-----------------------------------------------------------------------
      // CompressToByteArray
      //-----------------------------------------------------------------------
      public static byte[]  
         CompressToByteArray(
         string            sourceFileName,
         CompressionLevel  level
         )
      {
         if( !File.Exists( sourceFileName ) )
            return null;

         byte [] output = null;

         try
         {
            using(FileStream sourceFile = new FileStream( sourceFileName, FileMode.Open, FileAccess.Read ))
            {
               byte [] buffer = new byte [sourceFile.Length];
               int bytesread = 0;

               if( (bytesread = sourceFile.Read( buffer, 0, buffer.Length)) > 0 ) 
               {
                  output = QuickCompression.Compress( buffer,CompressionMethod.Deflated, level );
               }
				 
            }
         }
         catch( Exception e )
         {
            throw e;
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
         string            sourceFileName
         )
      {
         if( !File.Exists( sourceFileName ) )
            return null;

         using(FileStream sourceFile = new FileStream( sourceFileName, FileMode.Open, FileAccess.Read ))
         {
            byte [] buffer = new byte [StreamBufferSize];
            int     bytesRead = 0;

            MemoryStream destinationStream = new MemoryStream();
            using( XceedCompressedStream standard = new XceedCompressedStream( destinationStream,
                      CompressionMethod.Deflated,
                      CompressionLevel.Lowest) )
            {  
               while( (bytesRead = sourceFile.Read( buffer, 0, StreamBufferSize)) > 0 ) 
               {
                  standard.Write( buffer, 0, bytesRead );
               }
            }
            destinationStream.Close();
            return destinationStream;
         }
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
         PersistToDisk (
         string directory,
         string fileName,
         Stream fileStream
         )
      {
         FileStream localFile = null;
         string     localFileName = Path.Combine( directory, fileName );

         try
         {
            if( fileStream.CanRead )
            {
               using( localFile = new FileStream( localFileName, FileMode.CreateNew ) )
               {

                  //                  using( FileStream reader = new FileStream( fileStream, FileAccess.Read ) )
               {
                  int bytesRead = 0;
                  byte [] buffer = new byte [StreamBufferSize];

                  while( ( bytesRead = fileStream.Read( buffer, 0, StreamBufferSize )) > 0 )
                  {
                     localFile.Write( buffer, 0, bytesRead );
                  }

                  // Make sure all of the buffers are flushed to OS
                  NativeMethods.FlushFileBuffers(localFile.Handle);
				      
                  localFile.Close();
               }
               }
            }
         }
         catch( Exception e )
         {
            throw e;
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
         PersistToDisk (
         string directory,
         string fileName,
         byte [] fileContent
         )
      {
         FileStream localFile = null;
         string     localFileName = Path.Combine( directory, fileName );

         try
         {
            using( localFile = new FileStream( localFileName, FileMode.CreateNew ) )
            {
               localFile.Write( fileContent, 0, fileContent.Length );
               
               // Make sure all of the buffers are flushed to OS
               NativeMethods.FlushFileBuffers(localFile.Handle);
               
               localFile.Close();
            }
         }
         catch( Exception e )
         {
            throw e;
         }

         return localFileName;
      }
   }
}
