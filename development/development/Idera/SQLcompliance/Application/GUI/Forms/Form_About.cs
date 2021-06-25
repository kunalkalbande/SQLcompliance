using System ;
using System.Reflection ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Beta ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public partial class AboutForm : Form
	{
		string version = "0.0.1";

		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			Initialize();
			
			// Beta Label
			labelBetaVersion.Visible = BetaHelper.IsBeta;
		}

		private void Initialize() 
		{
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			if (version != null && version.Equals("0.0.0.0")) 
			{
				version = "1.1";
			}

			//Console.WriteLine(assembly.ToString());

			if (version != null && version.StartsWith("v"))
				version = version.Substring(1, version.Length -1);
            string[] versionArr = version.Split('.');
            if (versionArr.Length > 3)
                version = string.Join(".", versionArr, 0, 3);

			this.labelSQLsecure.Text = "SQL Compliance Manager " + version;
		}


		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
