using System ;
using System.Collections ;
using System.Configuration ;
using System.Reflection ;
using System.Text ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Scripting ;
using Idera.SQLcompliance.Core.Service;

namespace Idera.SQLcompliance.Application.CLI
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class SQLcomplianceCmd
	{
      #region Members

      private Hashtable _commands = new Hashtable() ;
      private CMCommand _helpCmd ;
      private CMCommand _batchCmd ;
      private CMCommand _encryptCmd ;
      private GlobalArguments _globals ;
      private bool _isAdmin ;
      private string _prefix = "SQLcmCmd [global-options]" ;
      private string _tab = "   " ;
      private int _exitCode = 0 ;

      public readonly string UnknownOperation = "Error - Unknown operation:  {0}" ;

      #endregion

      #region Initialization

      public SQLcomplianceCmd(string[] args)
      {
         _isAdmin = false ;
         _helpCmd = new CMCommand("help", new ExecuteCommandImpl(HelpProc)) ;
         _helpCmd.IsAdminRequired = false ;
         _helpCmd.Description = "Print help message" ;
         _helpCmd.Usage = "SQLcmCmd help [operation]" ;
         _helpCmd.DetailedDescription = "Print detailed help about the specified operation.  If not operation is specified, a list of supported commands is printed." ;
         _helpCmd.AddArgument(new CMCmdArgument("operation", "the SQLcmCmd operation to display detailed information about")) ;
         _helpCmd.Visible = true ;

         _batchCmd = new CMCommand("batch", new ExecuteCommandImpl(BatchProc)) ;
         _batchCmd.IsAdminRequired = false ;
         _batchCmd.Description = "Process a batch file of commands" ;
         _batchCmd.Usage = "SQLcmCmd [global-options] batch {filename}" ;
         _batchCmd.Visible = true ;

         _encryptCmd = new CMCommand("encrypt", new ExecuteCommandImpl(EncryptProc));
         _encryptCmd.IsAdminRequired = false;
         _encryptCmd.Description = "Encrypt a string";
         _encryptCmd.Usage = "SQLcmCmd [global-options] encrypt string";
         _encryptCmd.Visible = false ;

         _globals = new GlobalArguments();

         LoadCommands() ;
         DoCommand(args) ;
      }

      #endregion

      #region Properties

      public CMCommand[] Commands
      {
         get
         {
            CMCommand[] retVal = new CMCommand[_commands.Count] ;
            ArrayList tmpList = new ArrayList(_commands.Values) ;
            for(int i = 0 ; i < _commands.Count ; i++)
               retVal[i] = (CMCommand)tmpList[i] ;

            return retVal ;
         }
      }

      #endregion

      private void ShowHelp(string command)
      {
         CMCommandResult result ;
         string[] args = new string[2] ;
         args[0] = "help" ;
         args[1] = command ;

         result = _helpCmd.Execute(_globals, args) ;
         if(result.ResultCode != ResultCode.Success)
            Console.WriteLine(result.ResultString) ;
      }

      private void ShowHelp()
      {
         CMCommandResult result ;
         string[] args = new string[1] ;
         args[0] = "help" ;

         result = _helpCmd.Execute(_globals, args) ;
         if(result.ResultCode != ResultCode.Success)
            Console.WriteLine(result.ResultString) ;
      }

      private void GetPrivileges()
      {
         RemoteCollector collector = CollectionServiceCommands.GetCollectionService(_globals) ;
         if(collector != null)
         {
            Repository.ApplicationName = "SQLcompliance Command-Line" ;
            Repository.ServerInstance = collector.GetRepositoryInstance() ;
            Repository rep = new Repository() ;
            try
            {
               rep.OpenConnection() ;
               _isAdmin = RawSQL.IsCurrentUserSysadmin(rep.connection);
            }
            catch(Exception)
            {
               Console.WriteLine("Cannot obtain privileges from {0}.  Assuming non-administrative.", Repository.ServerInstance) ;
               _isAdmin = false ;
            }
         }
         else
         {
            Console.WriteLine("Unable to connect to the specified host: {0}:{1}", _globals.Host, _globals.Port) ;
            Console.WriteLine("Cannot obtain privileges.  Assuming non-administrative.") ;
            _isAdmin = false ;
         }
      }

      private void DoCommand(string[] args)
      {
         int i = 0 ;
         if(args.Length == 0)
         {
            ShowHelp() ;
            return ;
         }

         // Strip globals
         if(i + 1 < args.Length && String.Compare("-host", args[i], true) == 0)
         {
            _globals.Host = args[++i] ;
            i++ ;
         }
         if(i + 1 < args.Length && String.Compare("-port", args[i], true) == 0)
         {
            try
            {
               _globals.Port = Int32.Parse(args[++i]) ;
               i++ ;
               if(_globals.Port <= 0)
               {
                  Console.WriteLine("Error:  Invalid argument format:  port") ;
                  _exitCode = -1 ;
                  return ;
               }
            }
            catch(Exception)
            {
               Console.WriteLine("Error:  Invalid argument format:  port") ;
               _exitCode = -1 ;
               return ;
            }
         }

         string[] newArgs ;
         if(i > 0)
         {
            newArgs = new string[args.Length - i];
            for(int j = 0 ; j < newArgs.Length ; j++)
               newArgs[j] = args[i + j] ;
         }
         else
         {
            newArgs = args ;
         }

         if(newArgs.Length == 0)
         {
            Console.WriteLine("Invalid arguments") ;
            _exitCode = -1 ;
            return ;
         }

         CMCommand command = (CMCommand)_commands[newArgs[0].ToLower()] ;

         if(command != null)
         {
            CMCommandResult result ;

            GetPrivileges() ;
            if(command.IsAdminRequired && !_isAdmin)
            {
               Console.WriteLine("ERROR:  SQLcompliance Administrator privileges are required for this operation:  {0}", command.Command) ;
               _exitCode = -1 ;
               return; 
            }
            else
            {
               result = command.Execute(_globals, newArgs) ;
               Console.WriteLine(result.ResultString) ;
               if(result.ResultCode == ResultCode.InvalidArgumentFormat ||
                  result.ResultCode == ResultCode.InvalidArguments)
               {
                  ShowHelp(command.Command) ;
               }
               _exitCode = (int)result.ResultCode ;
            }
         }
         else
         {
            Console.WriteLine("Error:  Invalid operation {0}", newArgs[0]) ;
            _exitCode = -1 ;
            ShowHelp() ;
         }
      }

      private bool FilterICommandProvider(Type theType, object criteria)
      {
         if(theType.FullName.Equals(criteria))
            return true ;
         else
            return false ;
      }

      private bool ImplementsICommandProvider(Type theType)
      {
         TypeFilter myFilter = new TypeFilter(FilterICommandProvider) ;
         Type[] typeArray = theType.FindInterfaces(myFilter, "Idera.SQLcompliance.Core.Scripting.ICommandProvider") ;
         if(typeArray.Length > 0)
            return true ;
         else
            return false ;
      }

      private void LoadCommands()
      {
         ArrayList commands = new ArrayList() ;

         string assemblyList = ConfigurationSettings.AppSettings["provider-assemblies"] ;
         string[] assemblies ;

         CollectionServiceCommands collectionService = new CollectionServiceCommands() ;
         AgentServiceCommands agentCommands = new AgentServiceCommands() ;
         DirectCommands directCommands = new DirectCommands();

         commands.AddRange(collectionService.GetAvailableCommands()) ;
         commands.AddRange(agentCommands.GetAvailableCommands()) ;
         commands.AddRange(directCommands.GetAvailableCommands());

         if(assemblyList != null)
         {
            assemblies = assemblyList.Split(new char[] {','}) ;

            foreach(string assemblyName in assemblies)
            {
               Assembly myAsm = Assembly.LoadFrom(assemblyName) ;
               foreach(Type myType in myAsm.GetTypes())
               {
                  if(myType.IsClass && ImplementsICommandProvider(myType))
                  {
                     ICommandProvider provider = (ICommandProvider)Activator.CreateInstance(myType) ;
                     commands.AddRange(provider.GetAvailableCommands()) ;
                  }
               }
            }
         }

         // Add Builtin Commands
         commands.Add(_helpCmd) ;
         commands.Add(_encryptCmd) ;
         //commands.Add(_batchCmd) ;

         foreach(CMCommand myCommand in commands)
         {
            _commands[myCommand.Command.ToLower()] = myCommand ;
         }
      }

      #region Command Procs

      private CMCommandResult HelpProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult() ;

         if(args.Length == 0 || args.Length == 1)
         {
            string title = "SQLcmCmd", copyright = "" ;
            Assembly assm = Assembly.GetExecutingAssembly() ;
            object[] o = assm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if(o.Length > 0)
            {
               AssemblyCopyrightAttribute attr = (AssemblyCopyrightAttribute)o[0] ;
               copyright = attr.Copyright ;
            }
            o = assm.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if(o.Length > 0)
            {
               AssemblyTitleAttribute attr = (AssemblyTitleAttribute)o[0] ;
               title = attr.Title ;
            }
            Console.WriteLine("{0}{1}\n{0}{2}", _tab, title, copyright);
            Console.WriteLine("\n{0}{1} operation [args]\n", _tab, _prefix) ;

            Console.WriteLine("{0}global-options\n{0}{0}-host    the machine hosting the collection service", _tab) ;
            Console.WriteLine("{0}{0}-port    the port used to communicate with the collection service\n\n{0}Available operations:", _tab) ;

            ArrayList sortedKeys = new ArrayList(_commands.Keys) ;
            sortedKeys.Sort() ;
            foreach(string key in sortedKeys)
            {
               CMCommand cmd = (CMCommand)_commands[key] ;
               if(cmd.Visible)
                  Console.WriteLine("{0}{0}{1,-20}{2}", _tab, cmd.Command, cmd.Description) ;
            }
         }
         else if(args.Length == 2)
         {
            CMCommand cmd = (CMCommand)_commands[args[1].ToLower()] ;

            if(cmd != null)
            {
               string[] lines ;

               Console.WriteLine("\n{0}{1} -- {2}\n", _tab, cmd.Command, cmd.Description) ;
               lines = FormatString(cmd.Usage, 1, _tab.Length, 80) ;
               foreach(string s in lines)
                  Console.WriteLine(s) ;
               Console.WriteLine() ;
               lines = FormatString(cmd.DetailedDescription, 2, 0, 80) ;
               foreach(string s in lines)
                  Console.WriteLine(s) ;
               Console.WriteLine() ;
               foreach(CMCmdArgument argument in cmd.Arguments)
               {
                  lines = FormatString(String.Format("{0,-16}{1}\n", argument.Name + ":", argument.Description), 2, 16, 80);
                  foreach(string s in lines)
                     Console.WriteLine(s) ;
               }
            }
            else
            {
               Console.WriteLine("{0}\n", String.Format(UnknownOperation, args[1])) ;
            }
         }
         else
         {
            retVal.ResultCode = ResultCode.InvalidArguments ;
         }
         return retVal ;
      }

      private string[] FormatString(string s, int tabCount, int extraLineIndent, int screenWidth)
      {
         string[] tokens = s.Split(new char[] {' '}) ;
         ArrayList tmpList = new ArrayList() ;
         StringBuilder builder = new StringBuilder() ;

         for(int i = 0 ; i < tabCount ; i++)
            builder.Append(_tab) ;

         foreach(string token in tokens)
         {
            if(builder.Length + token.Length > screenWidth)
            {
               // Start a newline
               tmpList.Add(builder.ToString()) ;
               builder.Remove(0, builder.Length) ;
               for(int i = 0 ; i < tabCount ; i++)
                  builder.Append(_tab) ;
               for(int i = 0 ; i < extraLineIndent ; i++)
                  builder.Append(' ') ;
            }
            builder.AppendFormat("{0} ", token) ;
         }
         if(builder.Length > 0)
            tmpList.Add(builder.ToString()) ;

         string[] retVal = new string[tmpList.Count];
         for(int i = 0 ; i < tmpList.Count ; i++)
            retVal[i] = (string)tmpList[i] ;

         return retVal ;
      }

      private CMCommandResult BatchProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         return new CMCommandResult() ;
      }

      private CMCommandResult EncryptProc(GlobalArguments globals, CMCommand command, string[] args)
      {
         CMCommandResult retVal = new CMCommandResult();

         if (args.Length != 2)
            return new CMCommandResult(ResultCode.InvalidArguments) ;
         else
         {
            retVal.AddResultString(args[1]) ;
            retVal.AddResultString(AgentServiceManager.EncryptString(args[1])) ;
         }
         return retVal;
      }
      
      #endregion // Command Procs

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
         SQLcomplianceCmd parser = new SQLcomplianceCmd(args) ;
         System.Environment.Exit(parser._exitCode) ;
      }
	}
}
