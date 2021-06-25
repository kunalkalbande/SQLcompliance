using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.CollectionService
{
	/// <summary>
	/// Summary description for CollectionServiceProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class CollectionServiceProjectInstaller : System.Configuration.Install.Installer
	{
      private System.ServiceProcess.ServiceProcessInstaller CollectionServiceProcessInstaller;
      private System.ServiceProcess.ServiceInstaller CollectionServiceInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CollectionServiceProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.CollectionServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
         this.CollectionServiceInstaller = new System.ServiceProcess.ServiceInstaller();
         // 
         // CollectionServiceProcessInstaller
         // 
         this.CollectionServiceProcessInstaller.Password = null;
         this.CollectionServiceProcessInstaller.Username = null;
         // 
         // CollectionServiceInstaller
         // 
         this.CollectionServiceInstaller.DisplayName = "SQLcompliance Collection Service";
         this.CollectionServiceInstaller.ServiceName = "SQLcomplianceCollectionService";
         this.CollectionServiceInstaller.StartType   = System.ServiceProcess.ServiceStartMode.Automatic;
         // 
         // CollectionServiceProjectInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                                                                                  this.CollectionServiceProcessInstaller,
                                                                                  this.CollectionServiceInstaller});

      }
		#endregion
	}
}
