using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Idera.SQLcompliance.AgentService
{
   /// <summary>
   /// Summary description for ProjectInstaller.
   /// </summary>
   [RunInstaller(true)]
   public class SQLcomplianceAgentProjectInstaller : Installer
   {
      private ServiceProcessInstaller SQLcomplianceAgentProcessInstaller;
      private ServiceInstaller SQLcomplianceAgentInstaller;

      /// <summary>
      /// Required designer variable.
      /// </summary>
      private Container components = null;

      public SQLcomplianceAgentProjectInstaller()
      {
         // This call is required by the Designer.
         InitializeComponent();

         // TODO: Add any initialization after the InitializeComponent call
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

      public override void Install(IDictionary stateSaver)
      {
         StringDictionary parameters = this.Context.Parameters;
         bool isVirtual = false;

         if (parameters.ContainsKey("displayname"))
         {
            SQLcomplianceAgentInstaller.DisplayName = parameters["displayname"];
         }

         if (parameters.ContainsKey("servicename"))
         {
            SQLcomplianceAgentInstaller.ServiceName = parameters["servicename"];
            isVirtual = true;
         }

         if (parameters.ContainsKey("username"))
         {
            SQLcomplianceAgentProcessInstaller.Username = parameters["username"];
         }

         if (parameters.ContainsKey("password"))
         {
            SQLcomplianceAgentProcessInstaller.Password = parameters["password"];
         }
         if (isVirtual)
            SQLcomplianceAgentInstaller.StartType = ServiceStartMode.Manual;

         base.Install(stateSaver);
         if (isVirtual)
         {
            RegistryKey system,
                        currentControlSet,
                        services,
                        service;
            try
            {
               //Open the HKEY_LOCAL_MACHINE\SYSTEM key
               system = Registry.LocalMachine.OpenSubKey("System");
               //Open CurrentControlSet
               currentControlSet = system.OpenSubKey("CurrentControlSet");
               //Go to the services key
               services = currentControlSet.OpenSubKey("Services");
               //Open the key for your service, and allow writing
               service = services.OpenSubKey(this.SQLcomplianceAgentInstaller.ServiceName, true);
               //Add your service's description as a REG_SZ value named "Description"
               service.SetValue("Description", this.SQLcomplianceAgentInstaller.ServiceName + " Description");
               //(Optional) Add some custom information your service will use...
               //Console.WriteLine(service.GetValue("ImagePath"));
               string path = service.GetValue("ImagePath") + " " + this.SQLcomplianceAgentInstaller.ServiceName;
               service.SetValue("ImagePath", path);
               service.Close();
               services.Close();
               currentControlSet.Close();
               system.Close();
            }
            catch
            {
            }
         }
      }

      public override void Uninstall(IDictionary savedState)
      {
         StringDictionary parameters = this.Context.Parameters;

         if (parameters.ContainsKey("ServiceName"))
         {
            SQLcomplianceAgentInstaller.ServiceName = parameters["ServiceName"];
         }

         base.Uninstall(savedState);
      }

      #region Component Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.SQLcomplianceAgentProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
         this.SQLcomplianceAgentInstaller = new System.ServiceProcess.ServiceInstaller();
         // 
         // SQLcomplianceAgentProcessInstaller
         // 
         this.SQLcomplianceAgentProcessInstaller.Password = null;
         this.SQLcomplianceAgentProcessInstaller.Username = null;
         // 
         // SQLcomplianceAgentInstaller
         // 
         this.SQLcomplianceAgentInstaller.DisplayName = "SQLcompliance Agent";
         this.SQLcomplianceAgentInstaller.ServiceName = "SQLcomplianceAgent";
         this.SQLcomplianceAgentInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
         // 
         // SQLcomplianceAgentProjectInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[]
                                     {
                                        this.SQLcomplianceAgentInstaller,
                                        this.SQLcomplianceAgentProcessInstaller
                                     });
      }

      #endregion
   }
}