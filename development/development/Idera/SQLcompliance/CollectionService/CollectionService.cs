using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.CollectionService
{
   public class CollectionService : ServiceBase
   {
      #region Properties

      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private Container components = null;

      private static CollectionServer collectionServer;

      private Thread serverThread = null;

      private Mutex serverMutex;

      #endregion

      public CollectionService()
      {
         // This call is required by the Windows.Forms Component Designer.
         InitializeComponent();
         collectionServer = CollectionServer.Instance;
         this.AutoLog = false;
      }

      // The main entry point for the process
      private static void Main()
      {
         ServiceBase[] ServicesToRun;

         // More than one user Service may run within the same process. To add
         // another service to this process, change the following line to
         // create a second service object. For example,
         //
         //   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new CollectionService(), new MySecondUserService()};
         //
         ServicesToRun = new ServiceBase[] {new CollectionService()};

         Run(ServicesToRun);
      }

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         // 
         // CollectionService
         // 
         this.ServiceName = CoreConstants.CollectionServiceName;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose(bool disposing)
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

      /// <summary>
      /// Set things in motion so your service can do its work.
      /// </summary>
      protected override void OnStart(string[] args)
      {
         bool gotOwnership;
         serverMutex = new Mutex(true,
                                 CoreConstants.CollectionServiceName,
                                 out gotOwnership);
         if (! gotOwnership)
         {
            // wait a few seconds just in case other instance still shutting down
            gotOwnership = serverMutex.WaitOne(5000, false);
            if (! gotOwnership)
            {
               // someone else has it; so we throw exception and quit
               throw new Exception(CoreConstants.Exception_ServerAlreadyRunning);
            }
         }

         ThreadStart serverDelegate = new ThreadStart(StartServer);
         serverThread = new Thread(serverDelegate);
         serverThread.Name = "CollectionServer";
         serverThread.Start();
          CoreRemoteObjectsProvider.InitializeChannel();
      }

      private void StartServer()
      {
         collectionServer.Start();
      }

      /// <summary>
      /// Stop this service.
      /// </summary>
      protected override void OnStop()
      {
         collectionServer.Stop();

         try
         {
            //serverMutex.ReleaseMutex();
            serverMutex.Close();
            serverMutex = null;
         }
         catch (Exception)
         {
            // just eat this exception , main cause is we dont own it and we dont care since we were trying to release it
         }

         collectionServer = null;
      }
   }
}