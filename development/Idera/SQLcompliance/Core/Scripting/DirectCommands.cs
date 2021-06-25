using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Idera.SQLcompliance.Core.Triggers;

namespace Idera.SQLcompliance.Core.Scripting
{
   //
   // Direct commands perform direct connections to SQL Server to perform their operation.  The services in the product
   //  are not required.  As such, admin is not required - however, SQL Server admin permissions will likely be required for
   //  some of the commands.  These commands are available to support customers even after the product is removed, etc.
   public class DirectCommands : ICommandProvider
   {
      private Hashtable _commands ;

      public DirectCommands()
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

         myCommand = new CMCommand("listtriggers", new ExecuteCommandImpl(ListTriggersProc));
         myCommand.IsAdminRequired = false;
         myCommand.Description = "List the CLR triggers for DML auditing ";
         myCommand.Usage = "SQLcmCmd [global-options] listtriggers server";
         myCommand.DetailedDescription = "The listtriggers operation allows you to view all SQL Compliance Manager DML auditing triggers installed on the target server.";
         myCommand.AddArgument(new CMCmdArgument("server", "specify the target server to list trigger information for"));
         myCommand.Visible = true;
         _commands[myCommand.Command] = myCommand;

         myCommand = new CMCommand("removetriggers", new ExecuteCommandImpl(RemoveTriggersProc));
         myCommand.IsAdminRequired = false;
         myCommand.Description = "Remove the CLR triggers for DML auditing ";
         myCommand.Usage = "SQLcmCmd [global-options] removetriggers server database";
         myCommand.DetailedDescription = "The removetriggers operation allows you to remove all DML auditing triggers from the specified database.";
         myCommand.AddArgument(new CMCmdArgument("server", "specify the target server to remove triggers from"));
         myCommand.AddArgument(new CMCmdArgument("database", "specify the target database to remove triggers from"));
         myCommand.Visible = true;
         _commands[myCommand.Command] = myCommand;
      }

      private SqlConnection GetConnection(string instance)
      {
         if( instance == null )
            return null;
         string connString = "server=" + instance
                  + ";integrated security=SSPI"
                  + ";Connect Timeout=30"  
                  + ";Application Name='SqlcmCmd'" ;
         SqlConnection conn = new SqlConnection(connString);
         conn.Open();
         return conn; 
      }

      private CMCommandResult ListTriggersProc(GlobalArguments globals, CMCommand command, string[] args)
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
         try
         {
            using (SqlConnection conn = GetConnection(instance))
            {
               foreach (RawDatabaseObject dbo in RawSQL.GetUserDatabases(conn))
               {
                  foreach (DMLTriggerInfo info in TriggerHelpers.GetTriggers(instance, dbo.name))
                  {
                     retVal.AddResultString(String.Format("{0} on {1}.{2}", info.Name, dbo.name, info.FullTableName));
                  }
               }
            }
         }
         catch (Exception e)
         {
            return new CMCommandResult(e);
         }
         return retVal;
      }

      private CMCommandResult RemoveTriggersProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         string instance, database;
         CMCommandResult retVal = new CMCommandResult();
         int i = 1;

         if (args.Length < 3)
            return new CMCommandResult(ResultCode.InvalidArguments);

         // Process commands and set our state information
         // Server Name first
         instance = args[i].ToUpper();
         i++;
         database = args[i];
         i++;

         if (i != args.Length)
            return new CMCommandResult(ResultCode.InvalidArguments);
         try
         {
            TriggerManager.CleanUpTriggerSetup(instance, database);
         }
         catch (Exception e)
         {
            return new CMCommandResult(e);
         }
         return retVal;
      }
   }
}
