using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.AgentService
{
   public class SQLcomplianceAgentService : ServiceBase
   {
      //-------------       
      // Properties
      //-------------            
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private Container components = null;

      private SQLcomplianceAgent agent;

      private Thread agentThread = null;

      private Mutex agentMutex;

      //-------------
      // Constructor       
      //-------------       
      public SQLcomplianceAgentService()
      {
         // This call is required by the Windows.Forms Component Designer.
         InitializeComponent();

         // When the agent is monitoring a virtual instance, the virtual instance name is
         // passed in as a start parameter.  Agent uses the virtual instance to look up for
         // its registry key and other information.
         // Handle this in the Main
         //this.ServiceName = CoreConstants.AgentServiceName;
         try
         {
            agent = SQLcomplianceAgent.Instance;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(e.Message, ErrorLog.Severity.Error);
            throw(e);
         }

         this.AutoLog = false;
      }

      //---------------------------------------------
      // Main - The main entry point for the process
      //---------------------------------------------
      // TODO: threading model
      private static void Main(string[] args)
      {
         SQLcomplianceAgentService service = new SQLcomplianceAgentService();

         if (args.Length != 0)
            service.ServiceName = args[0];
         else
            service.ServiceName = CoreConstants.AgentServiceName;
         Run(service);
      }

      //---------------------------------------------------------------------------
      /// InitializeComponent - Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      //---------------------------------------------------------------------------
      private void
         InitializeComponent()
      {
      }

      //---------------------------------------------------------------------------
      /// Dispose - Clean up any resources being used.
      //---------------------------------------------------------------------------
      protected override void
         Dispose(
         bool disposing
         )
      {
         if (disposing)
         {
            if (components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose(disposing);
      }

      //---------------------------------------------------------------------------
      /// OnStart - Set things in motion so your service can do its work.
      //---------------------------------------------------------------------------
      protected override void
         OnStart(
         string[] args
         )
      {
         if (agent != null)
         {
            int i = ServiceName.IndexOf("$");
            if (i != -1)
            {
               agent.IsClustered = true;
               agent.VirtualServerName = ServiceName.Substring(i + 1);
            }

            bool gotOwnership;
            agentMutex = new Mutex(true,
                                   ServiceName,
                                   out gotOwnership);
            if (! gotOwnership)
            {
               // someone else has mutex; another instance of the agent is running; so we throw exception and quit
               // wait a few seconds just in case other instance still shutting down
               gotOwnership = agentMutex.WaitOne(5000, false);
               if (! gotOwnership)
               {
                  throw new Exception(CoreConstants.Exception_AgentAlreadyRunning);
               }
            }
            ErrorLog.Instance.Write("Starting agent", ErrorLog.Severity.Error);

            ThreadStart agentDelegate = new ThreadStart(StartAgent);
            agentThread = new Thread(agentDelegate);
            agentThread.Name = "SQLcomplianceAgent";
            agentThread.Start();
         }
         else
         {
            throw(new MissingMemberException(
               String.Format("{0} doesn't have an active instance", CoreConstants.AgentServiceName)));
         }
          CoreRemoteObjectsProvider.InitializeChannel();
      }

      private void StartAgent()
      {
         agent.Start();
      }

      //---------------------------------------------------------------------------
      /// OnStop - Stop this service.
      //---------------------------------------------------------------------------
      protected override void
         OnStop()
      {
         if (agent != null)
         {
            try
            {
               agent.Stop();
            }
            catch (Exception e)
            {
               // Log the error
               ErrorLog.Instance.Write(e.Message, ErrorLog.Severity.Error);
               throw(e);
            }
            finally
            {
               try
               {
                  agentMutex.ReleaseMutex();
                  agentMutex.Close();
                  agentMutex = null;
               }
               catch (Exception)
               {
                  // just eat this exception , main cause is we dont own it and we dont care since we were trying to release it
               }
            }
         }
         else
         {
            throw(new MissingMemberException(
               String.Format("{0} doesn't have an active instance", CoreConstants.AgentServiceName)));
         }
      }
   }
}