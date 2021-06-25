using System;
using System.Collections ;
using Idera.SQLcompliance.Core.Scripting ;

namespace Idera.SQLcompliance.Application.CLI
{
	/// <summary>
	/// Summary description for InterpreterCommands.
	/// </summary>
	public class InterpreterCommands
	{
      private Hashtable _commands ;

      public InterpreterCommands()
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

         myCommand = new CMCommand("exit", new ExecuteCommandImpl(ExitProc)) ;
         myCommand.Description = "exit the interpreter" ;
         myCommand.Usage = "exit" ;
         myCommand.Visible = true ;
         _commands[myCommand.Command] = myCommand ;
      }

      private bool ExitProc(CMCommand command, string[] args, out object result)
      {
         result = null ;
         return false ;
      }



   }
}
