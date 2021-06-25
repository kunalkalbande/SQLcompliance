using System;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Cluster
{
   /// <summary>
   /// This class contains Help context id string constants and methods to show help.
   /// </summary>
   public class HelpAlias
   {
      private HelpAlias()
      {}

      //---------------------------------------------------------------------
      // ShowHelp - This method brings up the SQLsafe Help file topic.
      //---------------------------------------------------------------------
      public static void
         ShowHelp(
         System.Windows.Forms.Control control,
         string                       topic
         )
      {
         string helpfilepath = AppDomain.CurrentDomain.BaseDirectory+SQLSECURECHMFILE;
			
         try 
         {
            IntPtr hWnd;
            if (!Idera.WebHelp.WebHelpLauncher.TryShowWebHelp(topic, out hWnd))
            {
               Help.ShowHelp(control, helpfilepath, HelpNavigator.Topic, topic);
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(control, String.Format("Unable to display the SQLCompliance Manager help file. Message: {0}", ex.Message),
               "Cluster ConfigurationConsole  Help") ;
         }
      }
		
      //---------------------
      // Help File Constants
      //---------------------
      public static string SQLSECURECHMFILE  = "SQL Compliance Manager Help.chm";

      public static string CLUSTERHELP_Form_Main = @"Windows\Cluster Configuration Console Window.htm";
      public static string CLUSTERHELP_Form_General = @"Windows\Add Agent Service General.htm";
      public static string CLUSTERHELP_Form_CollectionServer = @"Windows\Add Agent Service Collection Server.htm";
      public static string CLUSTERHELP_Form_AgentServiceAccount = @"Windows\Add Agent Service Account.htm";
      public static string CLUSTERHELP_Form_TraceDirectory = @"Windows\Add Agent Service Trace Directory.htm";
      public static string CLUSTERHELP_FORM_AssemblyDirectory = @"Windows\Add Agent Service CLR Directory.htm";
      public static string CLUSTERHELP_Form_Summary = @"Windows\Add Agent Service Summary.htm";
      public static string CLUSTERHELP_Form_AddFollowUp = @"Windows\New Registered SQL Server SQLServer Cluster.htm";
      public static string CLUSTERHELP_Form_Properties = @"Windows\SQLcompliance Agent Details.htm";
      public static string CLUSTERHELP_AuditVirtualInstance = @"Audit Virtual Instance.htm";
      public static string CLUSTERHELP_SetAssemblyDirectory = @"Windows\Specify CLR Trigger Directory Window.htm";
   }
}
