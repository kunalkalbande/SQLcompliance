using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_CreateIndex.
	/// </summary>
	public partial class Form_CreateIndex : Form
	{
		public Form_CreateIndex(bool isNewAttachment)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

         if(isNewAttachment)
         {
            string sText ;
            sText = "This database was created using an earlier version of SQL Compliance Manager," +
               " and does not contain optimized indexes.  Optimized indexes improve Management Console" +
               " performance when viewing audited events.  You can add these indexes now or later." ;
            _lnkText.Text = sText + "  Tell me more..." ;
            LinkArea link = new LinkArea(sText.Length + 2, 15);
            _lnkText.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked_lnkText);
            _lnkText.LinkArea = link ;
            _btnNo.Text = "Later";
            _lblUpdateNow.Text = "The update process requires free disk space and may take some" +
               " time to complete.  Do you want to update indexes now?" ;
         }
         else
         {
            string sText ;
            sText = "This update process requires free disk space, may be resource-intensive," +
               " and may take some time to complete.  You can update the indexes now or later." ;
            _lnkText.Text = sText;
            LinkArea link = new LinkArea(0, 0);
            _lnkText.LinkArea = link ;
            this.Text = "Update Indexes Now";
            _lblUpdateNow.Text = "Do you want to update indexes now?" ;
            _btnNo.Text = "No";
         }
		}


      private void LinkClicked_lnkText(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Index_Update_Information);
      }
   }
}
