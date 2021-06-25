using System ;
using System.IO ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Script.
	/// </summary>
	public partial class Form_Script : Form
	{
		public Form_Script()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
		}

      public string Script
      {
         get { return _tbScript.Text ; }
         set { _tbScript.Text = value ; }
      }


      private void Click_btnCopy(object sender, EventArgs e)
      {
         Clipboard.SetDataObject(_tbScript.Text) ;
      }

      private void Click_btnSave(object sender, EventArgs e)
      {
         Stream myStream ;
         SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
         saveFileDialog1.Filter = "Batch files (*.bat)|*.bat|All files (*.*)|*.*"  ;
         saveFileDialog1.FilterIndex = 1 ;
         saveFileDialog1.RestoreDirectory = true ;
 
         if(saveFileDialog1.ShowDialog() == DialogResult.OK)
         {
            if((myStream = saveFileDialog1.OpenFile()) != null)
            {
               StreamWriter writer = new StreamWriter(myStream) ;

               // Code to write the stream goes here.
               writer.Write(_tbScript.Text) ;
               writer.Close() ;
            }
         }
      }
	}
}
