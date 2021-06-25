using System;
using System.Collections ;
using System.Net ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.Core.Scripting
{
	/// <summary>
	/// Summary description for AgentServiceCommands.
	/// </summary>
	public class AgentServiceCommands : ICommandProvider
	{
      private Hashtable _commands ;
      
      public AgentServiceCommands()
		{
         _commands = new Hashtable() ;
         RegisterCommands() ;
      }

	   public CMCommand[] GetAvailableCommands()
	   {
         CMCommand[] retVal = new CMCommand[_commands.Count] ;
         ArrayList tmpList = new ArrayList(_commands.Values) ;
         for(int i = 0 ; i < _commands.Count ; i++)
            retVal[i] = (CMCommand)tmpList[i] ;

         return retVal ;
      }

      private void RegisterCommands()
      {
         CMCommand myCommand ;
         myCommand = new CMCommand("agentsettings", new ExecuteCommandImpl(AgentSettingsProc)) ;
         myCommand.IsAdminRequired = false ;
         myCommand.Description = "List agent settings" ;
         myCommand.Usage = "SQLcmCmd agentsettings agentHost [port]" ;
         myCommand.DetailedDescription = "The agentsettings operation displays information about the target agent configuration." ;
         myCommand.AddArgument(new CMCmdArgument("agentHost", "specify the machine hosting the target agent")) ;
         myCommand.AddArgument(new CMCmdArgument("port", "specify the port used to communicate with the agent")) ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("explodeuserlist", ExplodeUserListProc);
         myCommand.IsAdminRequired = false;
         myCommand.Description = "List all privileged users";
         myCommand.Usage = "SQLcmCmd explodeuserlist targetInstance agentHost [port]";
         myCommand.DetailedDescription = "The explodeuserlist operation displays the final list of privileged users.";
         myCommand.AddArgument(new CMCmdArgument("targetInstance", "specify the instance to generate a privileged user list for"));
         myCommand.AddArgument(new CMCmdArgument("agentHost", "specify the machine hosting the target agent"));
         myCommand.AddArgument(new CMCmdArgument("port", "specify the port used to communicate with the agent"));
         myCommand.Visible = false;
         _commands[myCommand.Command] = myCommand;

      }

      private AgentCommand GetAgentCommand(string host, int port)
      {
         try
         {
             AgentCommand agent = CoreRemoteObjectsProvider.AgentCommand(host, port);
            agent.Ping() ;

            return agent ;
         }
         catch(Exception)
         {
            return null ;
         }
      }



      private CMCommandResult AgentSettingsProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult(); 
         string server ;
         int port = CoreConstants.AgentServerTcpPort ;

         if(args.Length != 2 && args.Length != 3)
            return new CMCommandResult(ResultCode.InvalidArguments) ;

         // Process commands and set our state information
         // Server Name first
         server = args[1] ;

         // Port
         if(args.Length == 3)
         {
            try
            {
               port = Int32.Parse(args[2]) ;
               if(port <= 0)
               {
                  // Invalid Arguments
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
                  retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "port")) ;
                  return retVal ;
               }
            }
            catch(FormatException)
            {
               // Invalid Arguments
               retVal.ResultCode = ResultCode.InvalidArgumentFormat ;
               retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "port")) ;
               return retVal ;
            }
         }

         AgentCommand agent = GetAgentCommand(server, port) ;

         if(agent != null)
         {
            try
            {
               retVal.AddResultString(agent.GetAgentSettings()) ;
               return retVal ;
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
            return retVal ;
         }
      }

      private CMCommandResult ExplodeUserListProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult();
         string instance ;
         string server;
         int port = CoreConstants.AgentServerTcpPort;

         if (args.Length != 3 && args.Length != 4)
            return new CMCommandResult(ResultCode.InvalidArguments);

         // Process commands and set our state information
         // Server Name first
         instance = args[1] ;
         server = args[2];

         // Port
         if (args.Length == 4)
         {
            try
            {
               port = Int32.Parse(args[2]);
               if (port <= 0)
               {
                  // Invalid Arguments
                  retVal.ResultCode = ResultCode.InvalidArgumentFormat;
                  retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "port"));
                  return retVal;
               }
            }
            catch (FormatException)
            {
               // Invalid Arguments
               retVal.ResultCode = ResultCode.InvalidArgumentFormat;
               retVal.AddResultString(String.Format(CMCommandResult.InvalidArgumentFormat, "port"));
               return retVal;
            }
         }

         AgentCommand agent = GetAgentCommand(server, port);

         if (agent != null)
         {
            try
            {
               RemoteUserList remoteList = agent.GetUserList(instance) ;
               UserList list = new UserList(remoteList) ;
               UserCache userCache = new UserCache(instance) ;

               string[] results = UserList.ExplodeGroupsToUserList(instance, list.UserNames, userCache) ;
               foreach (string s in results)
                  retVal.AddResultString(s);
               results = UserList.GetAuditedServerRoleUsers(instance, list.ServerRoleIdList, userCache);
               foreach (string s in results)
                  retVal.AddResultString(s);
               return retVal;
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
            return retVal;
         }
      }

	}
}
