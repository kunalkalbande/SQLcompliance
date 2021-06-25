using System;
using Idera.SQLcompliance.Core.Collector ;

namespace Idera.SQLcompliance.Core.Scripting
{
	/// <summary>
	/// Summary description for ScriptGenerator.
	/// </summary>
	public class ScriptGenerator
	{
		public ScriptGenerator()
		{
		}

      public static string GenerateGlobals(SQLcomplianceConfiguration config)
      {
         return String.Format("-host {0} -port {1}", config.Server, config.ServerPort) ;
      }

      public static string GenerateGroom(string globals, string server, int days, bool skipIntegrity)
      {
         return String.Format("{0} {1} groom {2} {3} {4}",
            CoreConstants.CLI_Name,
            globals,
            server,
            days,
            skipIntegrity ? "-nointegrity" : "") ;
      }

      public static string GenerateArchive(string globals, ArchiveSettings settings)
      {
         string archivePeriod = "" ;

         switch(settings.ArchivePeriod)
         {
            case 12:
               archivePeriod = "month" ;
               break ;
            case 3:
               archivePeriod = "quarter" ;
               break ;
            case 1:
               archivePeriod = "year";
               break; 
         }
         return String.Format("{0} {1} archive {2} {3} {4} {5} {6} {7}",
            CoreConstants.CLI_Name,
            globals,
            settings.TargetInstance.Length == 0 ? "-all" : settings.TargetInstance,
            settings.ArchiveDays,
            settings.IntegrityCheckAction == IntegrityCheckAction.SkipCheck ? "-nointegrity" : "",
            "-prefix " + settings.Prefix,
            "-partition " + archivePeriod,
            "-timezone \"" + settings.TimeZoneName + "\"") ;
      }
	}
}
