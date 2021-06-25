using System;
using System.IO;

namespace Idera.SQLcompliance.Utility.TraceRegister
{

   /// <summary>
   /// Summary description for TraceFileNameAttributes.
   /// </summary>
   public struct TraceFileNameAttributes
   {
      private const string Separators                         = "_.";
      private const string TraceFileNameFormat                = "{0}_{1}_{2}_{3}_";
      private const string TraceFileNameFormatWithSequence    = "{0}_{1}_{2}_{3}_{4}_";

      public string   FullName;
      public string   Path;
      public string   InstanceAlias;
      public int      Version;
      public DateTime CreateTime;
      public int      AuditLevel;
      public int      AuditCategory;
      public int      AuditSequence;
      public int      Sequence;
      
      public int      TraceType; // calculated field

      public string TrcZippedExtension;
      public string BinZippedExtension;
      public string FileName;


        /// <summary>
        /// Parse the traces file name and returns all the attributes
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TraceFileNameAttributes 
         GetNameAttributes(
         FileInfo info
         )
      {
         TraceFileNameAttributes attrs = new TraceFileNameAttributes();

         try
         {
            attrs.Path = info.DirectoryName;
            attrs.FullName = info.FullName;
            string name = info.Name;
            string [] parts = name.Split( Separators.ToCharArray() );
            attrs.InstanceAlias = parts[0].StartsWith("XE") ? parts[0].Remove(0, 2) : parts[0];
            attrs.AuditLevel = int.Parse(parts[1]);
            attrs.AuditCategory = int.Parse(parts[2]);
            attrs.Version = int.Parse(parts[3]);

            attrs.AuditSequence = int.Parse( parts[4]);
            parts[5] = parts[5].Length == 15 ? parts[5] += "00" : parts[5];
            attrs.CreateTime = DateTime.Parse(String.Format( "{0}-{1}-{2} {3}:{4}:{5}.{6}",
               parts[5].Substring( 0, 4 ), // year
               parts[5].Substring( 4, 2 ), // month
               parts[5].Substring( 6, 2 ), // day
               parts[5].Substring( 8, 2 ), // hour
               parts[5].Substring( 10, 2 ), // minute
               parts[5].Substring( 12, 2 ), // second
               parts[5].Substring( 14, 3 ) // fraction
               ));
            if( parts.Length < 8 || parts[6].StartsWith( "." ) )
               attrs.Sequence = 0;
            else
               attrs.Sequence = int.Parse(parts[6]);
               
            // Calculate Trace Type (unique numeric identifier for event processing state tables)
            attrs.TraceType = attrs.AuditLevel + (20 * attrs.AuditCategory) + (1000 * attrs.AuditSequence);
            attrs.TrcZippedExtension = parts[parts.Length - 1];
            attrs.BinZippedExtension = attrs.TrcZippedExtension.Equals("tr7z") ? "bi7z" : "biz";
            attrs.FileName = name.Split(Separators.ToCharArray()[1])[0];
         }
         catch
         {
            throw new Exception( "Invalid trace file name format." );
         }

         return attrs;
      }

      /// <summary>
      /// Creates a trace file name with the specified trace attributes
      /// </summary>
      /// <param name="server"></param>
      /// <param name="auditLevel"></param>
      /// <param name="auditCategory"></param>
      /// <param name="version"></param>
      /// <returns></returns>
      public static string
         ComposeTraceFileName (
         string instanceAlias,
         int    auditLevel,
         int    auditCategory,
         int    version
         )
      {
         return String.Format( TraceFileNameFormatWithSequence,
            instanceAlias,
            auditLevel,
            auditCategory,
            version,
            0 );
      }

      /// <summary>
      /// Creates a trace file name with the specified trace attributes
      /// </summary>
      /// <param name="server"></param>
      /// <param name="auditLevel"></param>
      /// <param name="auditCategory"></param>
      /// <param name="version"></param>
      /// <returns></returns>
      public static string
         ComposeTraceFileName (
         string server,
         int    auditLevel,
         int    auditCategory,
         int    version,
         int    sequence
         )
      {
         return String.Format( TraceFileNameFormatWithSequence,
            server,
            auditLevel,
            auditCategory,
            version,
            sequence );
      }
   }
}
