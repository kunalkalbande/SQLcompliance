using System ;
using System.Threading ;
using Idera.SQLcompliance.Application.GUI.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI
{
   class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         // Set the thread name
         Thread.CurrentThread.Name = CoreConstants.ManagementConsoleName;
         // set default connection name
         Repository.ApplicationName = CoreConstants.ManagementConsoleName;

         try
         {
            // Setup the logging
            ErrorLog.Instance.LogToEventLog = true;
            ErrorLog.Instance.EventLogSource = CoreConstants.EventLogSource_ManagementConsole;
         }
         catch
         {
         }

         System.Windows.Forms.Application.EnableVisualStyles();
         System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

         SplashScreen splasher = new SplashScreen();

         SQLcmApplicationContext context = new SQLcmApplicationContext(splasher);

         System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
         AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

         /* Commenting the Secure Implementation due to Agent Issues
         RemotingCompositeChannel cc = new RemotingCompositeChannel(CoreConstants.GUIClientName, UriComparisonMode.WildcardMatch);

         // Add the secure TCP channel
         Hashtable tcpChannelProps = new Hashtable();
         tcpChannelProps["name"] = CoreConstants.GUIClientName;
         tcpChannelProps["secure"] = true;
         tcpChannelProps["protectionLevel"] = ProtectionLevel.EncryptAndSign;
         tcpChannelProps["tokenImpersonationLevel"] = TokenImpersonationLevel.Impersonation;
         TcpClientChannel secureChannel = new TcpClientChannel(tcpChannelProps, new BinaryClientFormatterSinkProvider());
         secureChannel.IsSecured = true;
         cc.Add(@"^tcp:\/\/.*(?<!\+Open)$", secureChannel);

         // Register the channel
         ChannelServices.RegisterChannel(cc, false);
          * */
         GUIRemoteObjectsProvider.InitializeChannel();
         System.Windows.Forms.Application.Run(context);
      }

      /// <summary>
      /// This handler is called for unhandled exceptions thrown from all non-main threads
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         if (e.ExceptionObject is Exception)
         {
            Exception ex = (Exception)e.ExceptionObject;
            string errorString = String.Format("Message:  {0}\r\n\r\nStack Trace:  {1}", ex.Message, ex.StackTrace);
            if (e.IsTerminating)
            {
               Form_UnhandledError errorBox = new Form_UnhandledError(errorString);
               errorBox.ShowDialog();
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Always, errorString, ErrorLog.Severity.Error);
         }
         else
         {
            ErrorLog.Instance.Write(e.ExceptionObject.ToString());
            string errorString = String.Format("Message:  {0}", e.ExceptionObject.ToString());
            if (e.IsTerminating)
            {
               Form_UnhandledError errorBox = new Form_UnhandledError(errorString);
               errorBox.ShowDialog();
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Always, e.ExceptionObject.ToString(), ErrorLog.Severity.Error);
         }
      }

      /// <summary>
      /// This handler is called for unhandled exceptions thrown from the main thread
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
      {
         ErrorLog.Instance.Write(e.Exception, true);
      }
   }
}
