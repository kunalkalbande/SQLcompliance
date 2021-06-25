using System;
using System.IO;

namespace Idera.SQLcompliance.Core.Event
{
    using System.Globalization;

    /// <summary>
	/// Summary description for TraceFileNameAttributes.
	/// </summary>
	public struct TraceFileNameAttributes
	{
      private const string Separators                                   = "_.";
      private const string TraceFileNameFormat                          = "{0}_{1}_{2}_{3}_";
      private const string TraceFileNameFormatWithSequence              = "{0}_{1}_{2}_{3}_{4}_";
      private const string FileNameFormatWithSequenceXE                 = "XE{0}_{1}_{2}_{3}_{4}_";
      private const string FileNameFormatForAuditLogsWithSequence       = "AL{0}_{1}_{2}_{3}_{4}_";

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
            if (name.StartsWith("AL") || name.StartsWith("XE"))
            {
                name = name.Substring(2);
            }
            string [] parts = name.Split( Separators.ToCharArray() );
            attrs.InstanceAlias = parts[0];  
            attrs.AuditLevel = int.Parse(parts[1]);
            attrs.AuditCategory = int.Parse(parts[2]);
            attrs.Version = int.Parse(parts[3]);

            attrs.AuditSequence = int.Parse( parts[4]);
            // Fixed reading using Parse Exact considering Fraction is variable
            try
            {
                 attrs.CreateTime = DateTime.Parse(String.Format( "{0}-{1}-{2} {3}:{4}:{5}.{6}",
                   parts[5].Substring( 0, 4 ), // year
                   parts[5].Substring( 4, 2 ), // month
                   parts[5].Substring( 6, 2 ), // day
                   parts[5].Substring( 8, 2 ), // hour
                   parts[5].Substring( 10, 2 ), // minute
                   parts[5].Substring( 12, 2 ), // second
                   parts[5].Substring( 14, 3 ) // fraction
                 ));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                    String.Format(CoreConstants.Exception_InvalidTraceDateFormat, info.Name),
                    e,
                    true);

                // Try using Parse Exact
                attrs.CreateTime = DateTime.ParseExact(
                parts[5].Substring(0, CoreConstants.DateTimeParseFormat.Length),
                CoreConstants.DateTimeParseFormat,
                CultureInfo.InstalledUICulture);
            }
            if (parts.Length < 8 || parts[6].StartsWith("."))
                attrs.Sequence = 0;
            else if (parts.Length == 10)
                attrs.Sequence = int.Parse(parts[7]);
            else
                attrs.Sequence = int.Parse(parts[6]);
               
            // Calculate Trace Type (unique numeric identifier for event processing state tables)
            attrs.TraceType = attrs.AuditLevel + (20 * attrs.AuditCategory) + (1000 * attrs.AuditSequence);
         }
         catch( Exception e)
         {
             // unknown trace file name format
             // we are ignoring SQL server default trace file name error
             if (!attrs.FullName.Contains("_1_998_"))
                ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                         String.Format( CoreConstants.Exception_InvalidTraceFileNameFormat, info.Name ),
                                         e,
                                         true);
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

      public static string
       ComposeTraceFileNameXE(
       string server,
       int auditLevel,
       int auditCategory,
       int version,
       int sequence
       )
      {
          return String.Format(FileNameFormatWithSequenceXE,
                                server,
                                auditLevel,
                                auditCategory,
                                version,
                                sequence);
      }

      public static string
       ComposeSessionNameXE(
       string server,
       int auditLevel,
       int auditCategory,
       int version,
       int sequence
       )
      {
          return String.Format(FileNameFormatWithSequenceXE,
                                server,
                                auditLevel,
                                auditCategory,
                                version,
                                sequence);
      }


      //5.5 Audit Logs
      public static string
       ComposeAuditLogFileName(
       string server,
       int auditLevel,
       int auditCategory,
       int version,
       int sequence
       )
      {
          return String.Format(FileNameFormatForAuditLogsWithSequence,
                                server,
                                auditLevel,
                                auditCategory,
                                version,
                                sequence);
      }
	}
}
