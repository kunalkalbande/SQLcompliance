using System;
using System.Collections ;
using Idera.SQLcompliance.Core.Scripting ;

namespace Idera.SQLcompliance.Application.CLI
{
	/// <summary>
	/// Summary description for BuiltinCommands.
	/// </summary>
	internal class BuiltinCommands : ICommandProvider
	{
      private Hashtable _commands ;
      private SQLcomplianceCmd _engine ;

		public BuiltinCommands(SQLcomplianceCmd engine)
		{
         _engine = engine ;
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

         myCommand = new CMCommand("help", new ExecuteCommandImpl(HelpProc)) ;
         myCommand.Description = "General help." ;
         myCommand.Usage = "SQLcmCmd [global options] help [operation]" ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;

         myCommand = new CMCommand("batch", new ExecuteCommandImpl(BatchProc)) ;
         myCommand.Description = "Process a batch file of commands" ;
         myCommand.Usage = "SQLcmCmd [global options] batch {filename}" ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;
      }

	}
}
