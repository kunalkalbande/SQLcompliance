using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Licensing;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_License : Form
   {
      private Color warningColor = Color.OrangeRed;
      private Color errorColor = Color.Red;

      #region Fields

//      private List<BBSProductLicense.LicenseData> m_Licenses;
//      private BBSProductLicense.LicenseData m_CombinedLicense;
      private BBSProductLicense m_BBSProductLicense;

      #endregion

      #region CTOR

      public Form_License()
      {
         InitializeComponent();
         this.Icon = Properties.Resources.SQLcompliance_product_ico ;
//         m_BBSProductLicense = bbsProductLicense;
         LoadLicenseInformation();
      }

      #endregion

      #region Helpers

      private void LoadLicenseInformation()
      {
         m_BBSProductLicense = Globals.SQLcomplianceConfig.LicenseObject ;
//         m_Licenses = m_BBSProductLicense.Licenses;
//         m_CombinedLicense = m_BBSProductLicense.CombinedLicense;

         listview_Licenses.Items.Clear();
         textBox_DaysToExpire.Text = string.Empty;
         textBox_LicensedFor.Text = string.Empty;
         textBox_LicensedServers.Text = string.Empty;
         textBox_LicenseExpiration.Text = string.Empty;
         textBox_LicenseType.Text = string.Empty;

         foreach (BBSProductLicense.LicenseData licDataTemp in m_BBSProductLicense.Licenses)
         {
            BBSProductLicense.LicenseData licData = licDataTemp;
            if (licData.licState == BBSProductLicense.LicenseState.Valid)
            {
               ListViewItem li = listview_Licenses.Items.Add(licData.key);
               li.Tag = licData;
               li.SubItems.Add(licData.numLicensedServersStr);
               li.SubItems.Add(licData.daysToExpireStr);
               if (licData.isAboutToExpire)
               {
                  li.ForeColor = warningColor;
               }
            }
            else
            {
               string message = null;
               switch (licData.licState)
               {
                  case BBSProductLicense.LicenseState.InvalidKey:
                     licData.typeStr = "Invalid License";
                     licData.forStr = string.Empty;
                     licData.expirationDateStr = string.Empty;
                     licData.daysToExpireStr = string.Empty;
                     message = string.Format(CoreConstants.LicenseInvalid, licData.key);
                     break;
                  case BBSProductLicense.LicenseState.InvalidExpired:
                     message = CoreConstants.LicenseExpired;
                     licData.typeStr = message;
                     break;
                  case BBSProductLicense.LicenseState.InvalidProductID:
                     message = CoreConstants.LicenseInvalidProductID;
                     licData.typeStr = message;
                     break;
                  case BBSProductLicense.LicenseState.InvalidProductVersion:
                     message = CoreConstants.LicenseInvalidProductVersion;
                     licData.typeStr = message;
                     break;
                  case BBSProductLicense.LicenseState.InvalidScope:
                     message = string.Format(CoreConstants.LicenseInvalidRepository, m_BBSProductLicense.OrginalScopeString);
                     licData.typeStr = message;
                     break;
                  case BBSProductLicense.LicenseState.InvalidDuplicateLicense:
                     message = CoreConstants.LicenseInvalidDuplicate;
                     licData.typeStr = message;
                     break;
                  default:
                     licData.typeStr = "Invalid License";
                     licData.forStr = string.Empty;
                     licData.expirationDateStr = string.Empty;
                     licData.daysToExpireStr = string.Empty;
                     message = string.Format(CoreConstants.LicenseInvalid, licData.key);
                     break;
               }
//                    Utility.MsgBox.ShowWarning(CoreConstants.LicenseCaption, message);
               ListViewItem li = listview_Licenses.Items.Add(licData.key);
               li.Tag = licData;
               li.SubItems.Add(licData.numLicensedServersStr);
               li.SubItems.Add(licData.daysToExpireStr);
               li.ForeColor = errorColor;
            }
         }
         if (m_BBSProductLicense.Licenses.Count > 1)
         {
            ListViewItem li = listview_Licenses.Items.Add(m_BBSProductLicense.CombinedLicense.key);
            li.Tag = m_BBSProductLicense.CombinedLicense;
            li.SubItems.Add(m_BBSProductLicense.CombinedLicense.numLicensedServersStr);
            li.SubItems.Add(m_BBSProductLicense.CombinedLicense.daysToExpireStr);
            Font font = new Font(li.Font, FontStyle.Bold);
            li.Font = font;
            listview_Licenses.Focus();
            listview_Licenses.Items[li.Index].Focused = true;
            listview_Licenses.Items[li.Index].Selected = true;
            button_Delete.Enabled = false;
         }
         else if (listview_Licenses.Items.Count > 0)
         {
            listview_Licenses.Focus();
            listview_Licenses.Items[0].Selected = true;
         }
         if (listview_Licenses.Items.Count == 0)
         {
            button_Delete.Enabled = false;
         }
      }

      private void ShowHelpTopic()
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_LicenseManagement);
      }

      #endregion

      #region Events

      private void button_OK_Click(object sender, EventArgs e)
      {
          CheckInstancesCountForCombinedLicense() ;
         DialogResult = DialogResult.OK;
      }

      private static void CheckInstancesCountForCombinedLicense()
      {
          var servers = ServerRecord.GetServers(Globals.Repository.Connection, true);
          if (Globals.SQLcomplianceConfig.LicenseObject.IsLicneseGoodForServerCount(servers.Count)) return;

          MessageBox.Show(CoreConstants.LicenseTooManyRegisteredServers, CoreConstants.LicenseCaption);
      }

      private void button_Delete_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, CoreConstants.DeleteConfirmMsg, CoreConstants.DeleteLicenseCaption, MessageBoxButtons.YesNo)
             == DialogResult.Yes)
         {
            BBSProductLicense.LicenseData licData = (BBSProductLicense.LicenseData) listview_Licenses.SelectedItems[0].Tag;

            m_BBSProductLicense.RemoveLicense(licData.licenseRepositoryID);
            Globals.SQLcomplianceConfig.ResetLicense(Globals.Repository.Connection) ;
            m_BBSProductLicense = Globals.SQLcomplianceConfig.LicenseObject ;

            LoadLicenseInformation();
         }
      }

      private void button_Add_Click(object sender, EventArgs e)
      {
         Form_AddLicense form = new Form_AddLicense(m_BBSProductLicense);
         if (form.ShowDialog() == DialogResult.OK)
         {
            Globals.SQLcomplianceConfig.ResetLicense(Globals.Repository.Connection);
            m_BBSProductLicense = Globals.SQLcomplianceConfig.LicenseObject;
            LoadLicenseInformation();
         }
      }

      private void listview_Licenses_SelectedIndexChanged(object sender, EventArgs e)
      {
         bool enableDelete = true;
         if (listview_Licenses.SelectedItems.Count > 0)
         {
            BBSProductLicense.LicenseData licData = (BBSProductLicense.LicenseData) listview_Licenses.SelectedItems[0].Tag;
            textBox_LicensedFor.Text = licData.forStr;
            textBox_LicensedServers.Text = licData.numLicensedServersStr;
            textBox_LicenseType.Text = licData.typeStr;
            textBox_LicenseExpiration.Text = licData.expirationDateStr;
            textBox_DaysToExpire.Text = licData.daysToExpireStr;
            if (licData.licState != BBSProductLicense.LicenseState.Valid)
            {
               textBox_LicenseType.ForeColor = errorColor;
               textBox_LicenseType.BackColor = textBox_LicensedFor.BackColor;
            }
            else
            {
               textBox_LicenseType.ForeColor = SystemColors.ControlText;
               textBox_LicenseType.BackColor = textBox_LicensedFor.BackColor;
            }
            if (licData.isAboutToExpire)
            {
               textBox_DaysToExpire.ForeColor = warningColor;
               textBox_LicenseExpiration.ForeColor = warningColor;
               textBox_DaysToExpire.BackColor = textBox_LicensedFor.BackColor;
               textBox_LicenseExpiration.BackColor = textBox_LicensedFor.BackColor;
            }
            else
            {
               textBox_DaysToExpire.ForeColor = SystemColors.ControlText;
               textBox_LicenseExpiration.ForeColor = SystemColors.ControlText;
            }
            if (licData.key == BBSLicenseConstants.CombinedLicenses)
            {
               enableDelete = false;
            }
         }
         else
         {
            enableDelete = false;
         }
         button_Delete.Enabled = enableDelete;
      }

      private void _btn_Help_Click(object sender, EventArgs e)
      {
         ShowHelpTopic();
      }

      private void Form_License_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         ShowHelpTopic();
         hlpevent.Handled = true;
      }

      #endregion

      private void lnkCustomerPortal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
          Process.Start("http://www.idera.com/licensing");
      }

      private void button_LicenseManager_Click(object sender, EventArgs e)
      {
          string path = AppDomain.CurrentDomain.BaseDirectory;
          path = path + @"License Manager Utility.exe";
          #if DEBUG
             path = @"C:\Program Files\Idera\SQLcompliance\License Manager Utility.exe";
          #endif
          //MessageBox.Show(path);
          System.Diagnostics.Process.Start(path);
      }
   }
}