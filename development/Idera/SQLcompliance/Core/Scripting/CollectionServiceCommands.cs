using System ;
using System.Collections ;
using System.Net ;
using System.Security.Principal ;
using System.Text ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.TimeZoneHelper ;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Core.Scripting
{
	/// <summary>
	/// Summary description for CollectionServiceCommands.
	/// </summary>
	public class CollectionServiceCommands : ICommandProvider
	{
      private Hashtable _commands ;

		public CollectionServiceCommands()
		{
         _commands = new Hashtable() ;
         RegisterCommands() ;
      }

      #region ICommandProvider Members

      public CMCommand[] GetAvailableCommands()
      {
         CMCommand[] retVal = new CMCommand[_commands.Count] ;
         ArrayList tmpList = new ArrayList(_commands.Values) ;
         for(int i = 0 ; i < _commands.Count ; i++)
            retVal[i] = (CMCommand)tmpList[i] ;

         return retVal ;
      }

      #endregion

      private void RegisterCommands()
      {
         CMCommand myCommand ;

         myCommand = new CMCommand("archive", new ExecuteCommandImpl(ArchiveProc)) ;
         myCommand.IsAdminRequired = true ;
         myCommand.Description = "Archive audited events" ;
         myCommand.Usage = "SQLcmCmd [global-options] archive {instance | -all} [days] [-nointegrity] [-prefix prefix] [-partition {month | quarter | year}] [-timezone timezone]" ;
         myCommand.DetailedDescription = "The archive operation allows you to archive events for a single instance or all instances." ;
         myCommand.AddArgument(new CMCmdArgument("instance", "specify the target instance to archive events for")) ;
         myCommand.AddArgument(new CMCmdArgument("-all", "archive all registered instances")) ;
         myCommand.AddArgument(new CMCmdArgument("days", "archive events older than the specified number of days")) ;
         myCommand.AddArgument(new CMCmdArgument("-nointegrity", "optional. skip the integrity check that is normally performed prior to archive")) ;
         myCommand.AddArgument(new CMCmdArgument("prefix", "specify a prefix for the archive's database name")) ;
         myCommand.AddArgument(new CMCmdArgument("-partition", "specify how to partition the archive data (month, quarter, year)")) ;
         myCommand.AddArgument(new CMCmdArgument("timezone", "specify a timezone to use when interpreting the event start times")) ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("groom", new ExecuteCommandImpl(GroomProc)) ;
         myCommand.IsAdminRequired = true ;
         myCommand.Description = "Groom audited events" ;
         myCommand.Usage = "SQLcmCmd [global options] groom {instance | -all} days [-nointegrity]" ;
         myCommand.DetailedDescription = "The groom operation allows you to groom events for a single instance or all instances." ;
         myCommand.AddArgument(new CMCmdArgument("instance", "specify the target instance to groom events for")) ;
         myCommand.AddArgument(new CMCmdArgument("-all", "groom all registered servers")) ;
         myCommand.AddArgument(new CMCmdArgument("days", "groom events older than the specified number of days")) ;
         myCommand.AddArgument(new CMCmdArgument("-nointegrity", "optional. do not perform an integrity check prior to grooming")) ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("checkintegrity", new ExecuteCommandImpl(CheckIntegrityProc)) ;
         myCommand.IsAdminRequired = true ;
         myCommand.Description = "Check integrity of audited events" ;
         myCommand.Usage = "SQLcmCmd [global options] checkintegrity instance [-fixintegrity]" ;
         myCommand.DetailedDescription = "The checkintegrity operation allows you to check and/or fix the integrity of events for a single instance." ;
         myCommand.AddArgument(new CMCmdArgument("instance", "specify the target instance to check event integrity for")) ;
         myCommand.AddArgument(new CMCmdArgument("-fixintegrity", "optional. fix any integrity errors found")) ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("registerinstance", new ExecuteCommandImpl(RegisterInstance));
         myCommand.IsAdminRequired = true;
         myCommand.Description = "Register a new audited SQL Server instance";
         myCommand.Usage = "SQLcmCmd [global options] registerinstance server [-config \"config file\", -deleteexisting]";
         myCommand.DetailedDescription = "The registerinstance operation allows you to register a new audited SQL Server instance.";
         myCommand.AddArgument(new CMCmdArgument("instance", "specify the name of the new audited SQL Server instance"));
         myCommand.AddArgument(new CMCmdArgument("-config", "optional. path to the XML config file"));
         myCommand.AddArgument(new CMCmdArgument("-deleteexisting", "optional. delete an existing event database if it exists in the Repository and create a new one"));
         myCommand.Visible = true;
         _commands[myCommand.Command] = myCommand;

         myCommand = new CMCommand("auditdatabase", new ExecuteCommandImpl(AuditDatabase));
         myCommand.IsAdminRequired = true;
         myCommand.Description = "Audit a new SQL Server database";
         myCommand.Usage = "SQLcmCmd [global options] auditdatabase instance databasename [-regulation {PCI | HIPAA | PCI,HIPAA} | -config \"config file\"]";
         myCommand.DetailedDescription = "The auditdatabase operation allows you to audit a new SQL Server database.";
         myCommand.AddArgument(new CMCmdArgument("instance", "specify the target instance that contains the new audited database"));
         myCommand.AddArgument(new CMCmdArgument("databasename", "specify the name of the new audited SQL Server database"));
         myCommand.AddArgument(new CMCmdArgument("-regulation", "optional. specify the name of the regulation guideline to be applied to this database.  Use a comma delimited list (no spaces) to apply more than one regulation."));
         myCommand.AddArgument(new CMCmdArgument("-config", "optional. path to the XML config file"));
         myCommand.Visible = true;
         _commands[myCommand.Command] = myCommand; 
         
         myCommand = new CMCommand("timezones", new ExecuteCommandImpl(TimeZonesProc));
         myCommand.IsAdminRequired = false ;
         myCommand.Description = "List timezone information" ;
         myCommand.Usage = "SQLcmCmd timezones" ;
         myCommand.DetailedDescription = "The timezones operation allows you list the timezone information for a collection service host.  The timezone name is used as an argument to the archive operation." ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("serversettings", new ExecuteCommandImpl(ServerSettingsProc)) ;
         myCommand.IsAdminRequired = false ;
         myCommand.Description = "List collection server settings" ;
         myCommand.Usage = "SQLcmCmd serversettings" ;
         myCommand.DetailedDescription = "The serversettings operation displays information about the server configuration." ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("updateindex", new ExecuteCommandImpl(UpdateIndexProc)) ;
         myCommand.IsAdminRequired = true ;
         myCommand.Description = "Update the indexes on the target database" ;
         myCommand.Usage = "SQLcmCmd updateindex {databaseName | -all}" ;
         myCommand.DetailedDescription = "The updateindex operation applies the most recent indexes to the specified database.  " +
            "This operation can be take some time depending on the size of the target database.  This process will also " +
            "impact the performance of inserts into the target database.  It is recommneded that this operation be performed " +
            "during non-peak hours for the corresponding audited server." ;
         myCommand.AddArgument(new CMCmdArgument("databaseName", "specify the target database to upgrade indexes for")) ;
         myCommand.AddArgument(new CMCmdArgument("-all", "upgrade the indexes for all database that need it")) ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("collect", new ExecuteCommandImpl(CollectNowProc));
         myCommand.IsAdminRequired = true;
         myCommand.Description = "Collect trace data from agent";
         myCommand.Usage = "SQLcmCmd [global-options] collect server";
         myCommand.DetailedDescription = "The collection operation allows you to force the agent to transmit any accumulated trace data to the collection server.";
         myCommand.AddArgument(new CMCmdArgument("server", "specify the target server to collect data from"));
         myCommand.Visible = true;
         _commands[myCommand.Command] = myCommand;
      }

      public static RemoteCollector GetCollectionService(GlobalArguments globals)
      {
         try
         {
            if(globals.Host.Length == 0)
               globals.Host = Dns.GetHostName() ;
            if(globals.Port == -1)
               globals.Port = CoreConstants.CollectionServerTcpPort ;

             RemoteCollector srv = CoreRemoteObjectsProvider.RemoteCollector(globals.Host, globals.Port);
            srv.PingCollectionServer() ;

            return srv ;
         }
         catch(Exception)
         {
            return null ;
         }
      }

      public static AgentManager GetAgentManager(GlobalArguments globals)
      {
         try
         {
            if (globals.Host.Length == 0)
               globals.Host = Dns.GetHostName();
            if (globals.Port == -1)
               globals.Port = CoreConstants.CollectionServerTcpPort;

            AgentManager mgr = CoreRemoteObjectsProvider.AgentManager(globals.Host, globals.Port);
            mgr.Ping();

            return mgr;
         }
         catch (Exception)
         {
            return null;
         }
      }

      private CMCommandResult ArchiveProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         ArchiveSettings settings = new ArchiveSettings() ;
         CMCommandResult retVal = new CMCommandResult() ;
         int i = 1 ;

         if(args.Length < 2)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         // Server Name first
         if(String.Compare(args[i], "-all", true) == 0)
            settings.TargetInstance = "" ;
         else
            settings.TargetInstance = args[i].ToUpper() ;
         i++ ;

         // Days
         if(i < args.Length && !args[i].StartsWith("-"))
         {
            try
            {
               settings.ArchiveDays = Int32.Parse(args[i]) ;
               if(settings.ArchiveDays <= 0)
               {
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
                  retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "days")) ;
                  return retVal ;
               }
               i++ ;
            }
            catch(FormatException)
            {
               retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
               retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "days")) ;
               return retVal ;
            }
         }

         // Skip Integrity
         if(i < args.Length && String.Compare(args[i], "-nointegrity", true) == 0)
         {
            settings.IntegrityCheckAction = IntegrityCheckAction.SkipCheck ;
            i++ ;
         }else
            settings.IntegrityCheckAction = IntegrityCheckAction.PerformCheck ;

         // Prefix
         if(i + 1 < args.Length && String.Compare(args[i], "-prefix", true) == 0)
         {
            i++ ; // gobble -prefix
            settings.Prefix = args[i++] ;
         }

         // Partition
         if(i + 1 < args.Length && String.Compare(args[i], "-partition", true) == 0)
         {
            switch(args[++i].ToLower())
            {
               case "month":
                  settings.ArchivePeriod = 12 ;
                  break ;
               case "quarter":
                  settings.ArchivePeriod = 3 ;
                  break ;
               case "year":
                  settings.ArchivePeriod = 1 ;
                  break ;
               default:
                  // Invalid Arguments
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
                  retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "parition")) ;
                  return retVal ;
            }
            i++ ;
         }
         // timezone
         if(i + 1 < args.Length && String.Compare(args[i], "-timezone", true) == 0)
         {
            i++ ;
            settings.TimeZoneName = args[i++] ;
         }

         if(i != args.Length)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         WindowsIdentity id = WindowsIdentity.GetCurrent();
         settings.User = id.Name ;

         RemoteCollector collectionService = GetCollectionService(globals) ;

         if(collectionService != null)
         {
            try
            {
               retVal = collectionService.Archive(settings) ;
            }
            catch(Exception e)
            {
               return new CMCommandResult(e) ;
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed ;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port)) ;
         }
         return retVal ;
      }

      private CMCommandResult GroomProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         bool groomAll = false ;
         IntegrityCheckAction icAction = IntegrityCheckAction.PerformCheck ;
         int days ;
         string server = "" ;
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length != 3 && args.Length != 4)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         // Server Name first
         if(String.Compare(args[1], "-all", true) == 0)
            groomAll = true ;
         else
            server = args[1].ToUpper() ;

         // Days
         try
         {
            days = Int32.Parse(args[2]) ;
            if(days <= 0)
            {
               // Invalid Arguments
               retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
               retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "days")) ;
               return retVal ;
            }
         }
         catch(FormatException)
         {
            // Invalid Arguments
            retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
            retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "days")) ;
            return retVal ;
         }

         // Skip Integrity
         if(args.Length == 4)
         {
            if(String.Compare(args[3], "-nointegrity", true) == 0)
            {
               icAction = IntegrityCheckAction.SkipCheck  ;
            }
            else
            {
               return new CMCommandResult(ResultCode.InvalidArguments) ;
            }
         }

         RemoteCollector collectionService = GetCollectionService(globals) ;

         if(collectionService != null)
         {
            try
            {
               if(groomAll)
                  retVal = collectionService.GroomAll(days, icAction) ;
               else
                  retVal = collectionService.Groom(server, days, icAction) ;
            }
            catch(Exception e)
            {
               return new CMCommandResult(e) ;
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed ;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port)) ;
         }
         return retVal ;
      }

      private CMCommandResult CheckIntegrityProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         bool fixIntegrity = false ;
         string server ;
         StringBuilder builder = new StringBuilder() ;
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length != 2 && args.Length != 3)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         // Server Name first
         server = args[1].ToUpper() ;

         // Fix Integrity
         if(args.Length == 3)
         {
            if(String.Compare(args[2], "-fixintegrity", true) == 0)
               fixIntegrity = true ;
            else
               return new CMCommandResult(ResultCode.InvalidArguments) ;
         }

         RemoteCollector collectionService = GetCollectionService(globals) ;

         if(collectionService != null)
         {
            CheckResult ckResult ;
            try
            {
               ckResult = collectionService.CheckIntegrity(server, fixIntegrity) ;
            }
            catch(Exception e)
            {
               return new CMCommandResult(e) ;
            }

            if(ckResult.intact)
            {
               // Success
               builder.AppendFormat("Integrity check successful for {0}", server) ;
            }
            else
            {
               // Failure
               builder.AppendFormat("Integrity check failed for {0}", server) ;
               builder.Append("\nSummary:");
               builder.AppendFormat("\n  {0} added events", ckResult.numAdded) ;
               builder.AppendFormat("\n  {0} modified events", ckResult.numModified) ;
               builder.AppendFormat("\n  {0} event gaps", ckResult.numGaps) ;

               if(fixIntegrity)
                  builder.AppendFormat("\nThese issues were marked and integrity has been restored.") ;
            }
            retVal.AddResultString(builder.ToString()) ;
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed ;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port)) ;
         }
         return retVal ;
      }

      private CMCommandResult RegisterInstance(GlobalArguments globals, CMCommand command, string[] args)
      {
         int i = 1;
         StringBuilder builder = new StringBuilder();
         CMCommandResult retVal = new CMCommandResult();
         RegisterServerArgs registerArgs = new RegisterServerArgs();

         //The only thing required is the command name (which was set or we wouldn't be here), and the instance name.
         if (args.Length < 2)
            return new CMCommandResult(ResultCode.InvalidArguments);

         // Process commands and set our state information
         // Server Name first
         registerArgs.Instance = args[i].ToUpper();
         i++;

         if (!ValidateServerName(registerArgs))
         {
            retVal.AddResultString("The specified SQL Server instance name was not specified in a valid format. Enter '(local)'or 'instance_name' for a local instance or 'computer\\instance' for a remote instance.");
            retVal.ResultCode = ResultCode.Error;
            return retVal;
         }

         // Config file
         if (i < args.Length) //see if there are more arguments
         {
            if (String.Compare(args[i], "-config", true) == 0) //there are, see if it -config
            {
               if (i + 1 < args.Length) //see if the config file has been set
               {
                  i++; // gobble -config
                  registerArgs.ConfigFile = args[i++].Replace("\"", "");
               }
               else
               {
                  retVal.AddResultString("A valid config filename must be specified when using the -config option.");
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                  return retVal;
               }
            }
         }

         //use existing event database
         if (i < args.Length && String.Compare(args[i], "-deleteexisting", true) == 0)
            registerArgs.DeleteExisting = true;
         else
            registerArgs.DeleteExisting = false;

         WindowsIdentity id = WindowsIdentity.GetCurrent();
         registerArgs.User = id.Name;
         RemoteCollector collectionService = GetCollectionService(globals) ;

         if (collectionService != null)
         {
            retVal = collectionService.RegisterInstance(registerArgs);
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port));
         }
         return retVal;
      }

      private bool ValidateServerName(RegisterServerArgs args)
      {
         string localhost = Dns.GetHostName().ToUpper();

         args.Server = "";
         int pos = args.Instance.IndexOf(@"\");

         if (pos == -1)
         {
            if (args.Instance == "(LOCAL)" || args.Instance == ".")
            {
               args.Server = args.Instance = localhost;
            }
            else
            {
               args.Server = args.Instance;
            }
         }
         else if (pos == 0)
         {
            return false;
         }
         else // pos > 0; we have xxx/yyy
         {
            args.Server = args.Instance.Substring(0, pos);

            if (args.Instance.Substring(pos + 1).Length == 0)
            {
               return false;
            }
            else
            {
               if (args.Server == "(LOCAL)" || args.Server == ".")
               {
                  args.Server = localhost;
               }
            }
         }
         return true;
      }

      private CMCommandResult AuditDatabase(GlobalArguments globals, CMCommand command, string[] args)
      {
         int i = 1;
         StringBuilder builder = new StringBuilder();
         CMCommandResult retVal = new CMCommandResult();
         RegisterDatabaseArgs registerArgs = new RegisterDatabaseArgs();

         //The command name (which was set or we wouldn't be here), the instance name and the database name.
         if (args.Length < 3)
            return new CMCommandResult(ResultCode.InvalidArguments);

         // Server Name
         registerArgs.Instance = args[i++].ToUpper();

         //database name.  Case for the database name matters.
         registerArgs.Database = args[i++];

         // regulation
         if (i < args.Length) //see if there are more arguments
         {
            if (String.Compare(args[i], "-regulation", true) == 0) //there are, see if it is -regulation
            {
               if (i + 1 < args.Length) //see if the regulation name as been set
               {
                  i++; // gobble -config
                  registerArgs.Regulation = args[i++];

                  string[] regulations = registerArgs.Regulation.Split(',');

                  //no regulation has been set.
                  if (regulations.Length == 0)
                  {
                     retVal.AddResultString("A valid regulation name must be specified when using the -regulation option.");
                     retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                     return retVal;
                  }

                  if (!ValidateRegulations(regulations))
                  {
                     // Invalid Arguments
                     retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                     retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "-regulation"));
                     return retVal;
                  }
               }
               else
               {
                  retVal.AddResultString("A valid regulation name must be specified when using the -regulation option.");
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                  return retVal;
               }
            }
         }
                  
         // Config file
         if (i < args.Length) //see if there are more arguments
         {
            if (String.Compare(args[i], "-config", true) == 0) //there are, see if it -config
            {
               if (i + 1 < args.Length) //see if the config file has been set
               {
                  i++; // gobble -config
                  registerArgs.ConfigFile = args[i++].Replace("\"", "");
               }
               else
               {
                  retVal.AddResultString("A valid config filename must be specified when using the -config option.");
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                  return retVal;
               }
            }
         }
         WindowsIdentity id = WindowsIdentity.GetCurrent();
         registerArgs.User = id.Name;

         RemoteCollector collectionService = GetCollectionService(globals);

         if (collectionService != null)
         {
            retVal = collectionService.RegisterDatabase(registerArgs);
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port));
         }
         return retVal;
      }

      private bool ValidateRegulations(string[] regulations)
      {
         bool validRegulation = false;

         //we only support two regulations at this point
         if (regulations.Length > 2)
            return false;

         //make sure they are either PCI or HIPAA
         foreach (string regulation in regulations)
         {
            //ignore empty strings;
            if (String.IsNullOrEmpty(regulation))
               continue;

            if (regulation.ToUpper() == "PCI" || regulation.ToUpper() == "HIPAA")
            {
               validRegulation = true;
               continue;
            }
            else
               // we want to return false here because we found something wasn't a valid regualtion but wasn't an empty string.
               return false;
         }
         return validRegulation;
      }

      private CMCommandResult TimeZonesProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         StringBuilder builder = new StringBuilder() ;
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length != 1)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         TimeZoneInfo[] tzs = TimeZoneInfo.GetSystemTimeZones() ;

         foreach(TimeZoneInfo info in tzs)
         {
            builder.AppendFormat("Name:  {0}\nDescription:  {1}\n\n", info.TimeZoneStruct.StandardName, info.Name) ;
         }
         retVal.AddResultString(builder.ToString()) ;
         return retVal ;
      }

      private CMCommandResult ServerSettingsProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length != 1)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         RemoteCollector collectionService = GetCollectionService(globals) ;

         if(collectionService != null)
         {
            try
            {
               retVal.AddResultString(collectionService.GetServerSettings()) ;
            }
            catch(Exception e)
            {
               return new CMCommandResult(e) ;
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed ;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port)) ;
         }
         return retVal ;
      }

      private CMCommandResult UpdateIndexProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length != 2)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         RemoteCollector collectionService = GetCollectionService(globals) ;
         string databaseName = args[1] ;

         if(collectionService != null)
         {
            try
            {
               if(String.Compare(databaseName, "-all", true) == 0)
                  collectionService.UpdateAllIndexes() ;
               else
                  collectionService.UpdateIndexes(databaseName) ;
            }
            catch(Exception e)
            {
               return new CMCommandResult(e) ;
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed ;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port)) ;
         }
         return retVal ;
      }

      private CMCommandResult CollectNowProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         string instance;
         CMCommandResult retVal = new CMCommandResult();
         int i = 1;

         if (args.Length < 2)
            return new CMCommandResult(ResultCode.InvalidArguments);

         // Process commands and set our state information
         // Server Name first
         instance = args[i].ToUpper();
         i++;

         if (i != args.Length)
            return new CMCommandResult(ResultCode.InvalidArguments);

         AgentManager mgr = GetAgentManager(globals);

         if (mgr != null)
         {
            try
            {
               mgr.CollectTracesNow(instance);
               retVal.ResultCode = ResultCode.Success;
            }
            catch (Exception e)
            {
               return new CMCommandResult(e);
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.ConnectionFailed;
            retVal.AddResultString(String.Format("Unable to connect to the specified host: {0}:{1}", globals.Host, globals.Port));
         }
         return retVal;
      }

   }
}
