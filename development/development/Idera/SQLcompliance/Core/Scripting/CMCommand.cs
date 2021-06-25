using System.Collections ;

namespace Idera.SQLcompliance.Core.Scripting
{
   public delegate CMCommandResult ExecuteCommandImpl(GlobalArguments globals, CMCommand command, string[] args) ;

	/// <summary>
	/// Summary description for CMCommand.
	/// </summary>
	public class CMCommand
	{
      private string _command ;
      private bool _visible ;
      private string _description ;
      private string _usage ;
      private string _details ;
      private bool _adminRequired ;
      private ArrayList _arguments ;
      private ExecuteCommandImpl _executeProc ;

		public CMCommand(string command, ExecuteCommandImpl executeProc)
		{
         _command = command ;
         _executeProc = executeProc ;
         _arguments = new ArrayList() ;
         _description = "" ;
         _usage = "" ;
         _details = "" ;
         _adminRequired = true ;
		}

	   public string Command
	   {
	      get { return _command ; }
	   }

	   public bool Visible
	   {
	      get { return _visible ; }
	      set { _visible = value ; }
	   }

	   public string Description
	   {
	      get { return _description ; }
	      set { _description = value ; }
	   }

	   public string Usage
	   {
	      get { return _usage ; }
	      set { _usage = value ; }
	   }

      public string DetailedDescription
      {
         get { return _details ; }
         set { _details = value ; }
      }

      public CMCmdArgument[] Arguments
      {
         get
         {
            CMCmdArgument[] retVal = new CMCmdArgument[_arguments.Count];
            for(int i = 0 ; i < _arguments.Count ; i++)
               retVal[i] = (CMCmdArgument)_arguments[i] ;
            return retVal ;
         }
      }

      public bool IsAdminRequired
      {
         get { return _adminRequired ; }
         set { _adminRequired = value ; }
      }

      //
      // AddArgument
      //
      // This function simply stores information for display in the help
      //  for this command.
      //
      public void AddArgument(CMCmdArgument argument)
      {
         _arguments.Add(argument) ;
      }

      /*
      public bool Execute()
      {
         string[] args = new string[0] ;
         object result ;
         return Execute(new GlobalArguments(), args, out result) ;
      }*/

      //
      // Execute()
      // 
      // This function returns true if the arguments were properly processed.
      //  The actual results of the command are shown in the result object.
      //  False is returned when the supplied arguments could not be successfully parsed.
      //
      public CMCommandResult Execute(GlobalArguments globals, string[] args)
      {
         return _executeProc(globals, this, args) ;
      }
	}

   public class CMCmdArgument
   {
      private string _name ;
      private string _description ;

      public CMCmdArgument(string name, string description)
      {
         _name = name ;
         _description = description ;
      }

      public string Name
      {
         get { return _name ; }
         set { _name = value ; }
      }

      public string Description
      {
         get { return _description ; }
         set { _description = value ; }
      }
   }

   public class GlobalArguments
   {
      private string _host ;
      private int _port ;

      public GlobalArguments()
      {
         _host = "" ;
         _port = -1 ;
      }

      public string Host
      {
         get { return _host ; }
         set { _host = value ; }
      }

      public int Port
      {
         get { return _port ; }
         set { _port = value ; }
      }
   }
}
