using System;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Licensing;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_AddLicense : Form
   {
      private BBSProductLicense m_BBSProductLicense = null;

      #region CTOR

      public Form_AddLicense(BBSProductLicense bpl)
      {
         m_BBSProductLicense = bpl;

         InitializeComponent();
         this.Icon = Properties.Resources.SQLcompliance_product_ico ;


//            if (!AllowGenerateTrialLicense())
//            {
//                if (Program.gController.Repository.IsLicenseOk())
//                {
//                    label_NewUser1.Text = CoreConstants.LicenseInterestText;
//                    label_NewUser2.Text = CoreConstants.LicenseEnterProductionText;
//                }
//                else if (Program.gController.Repository.RepositoryComputerName != System.Environment.MachineName)
//                {
//                    label_NewUser2.Text = CoreConstants.LicenseUnsupportedConfigText;
//                }
//                else if (m_BBSProductLicense.HasTrialLicneseBeenUsed())
//                {
//                    label_NewUser2.Text = CoreConstants.LicenseTrialExpiredText;
//                }
//                button_GenerateTrialLicense.Visible = false;
//            }

         textBox_NewKey.Text = string.Empty;
      }

      #endregion

      #region Helpers

//        private bool AllowGenerateTrialLicense()
//        {
//            bool isAllowed = false;
//            if (!Program.gController.Repository.IsLicenseOk() && !m_BBSProductLicense.HasTrialLicneseBeenUsed())
//            {
//                if (Program.gController.Repository.RepositoryComputerName == System.Environment.MachineName)
//                {
//                    isAllowed = true;
//                }
//            }
//
//            return isAllowed;
//        }

      private bool AddNewLicenseString(string licenseStr)
      {
         bool isOK = true;

         if (!string.IsNullOrEmpty(licenseStr))
         {
             BBSProductLicense.LicenseState licState;
            if (!m_BBSProductLicense.IsLicenseStringValid(licenseStr, out licState))
            {
               string message;
               switch (licState)
               {
                  case BBSProductLicense.LicenseState.InvalidKey:
                     message = string.Format(CoreConstants.LicenseInvalid, licenseStr);
                     break;
                  case BBSProductLicense.LicenseState.InvalidExpired:
                     message = string.Format(CoreConstants.LicenseExpired);
                     break;
                  case BBSProductLicense.LicenseState.InvalidProductID:
                     message = string.Format(CoreConstants.LicenseInvalidProductID);
                     break;
                  case BBSProductLicense.LicenseState.InvalidProductVersion:
                     message = string.Format(CoreConstants.LicenseInvalidProductVersion);
                     break;
                  case BBSProductLicense.LicenseState.InvalidScope:
                     message = string.Format(CoreConstants.LicenseInvalidRepository, Globals.RepositoryServer);
                     break;
                  case BBSProductLicense.LicenseState.InvalidDuplicateLicense:
                     message = string.Format(CoreConstants.LicenseInvalidDuplicate);
                     break;
                  default:
                     message = string.Format(CoreConstants.LicenseInvalid, licenseStr);
                     break;
               }
               var messageBox = new Form_MessageBox(this, message, CoreConstants.LicenseCaption, @"http://www.idera.com/licensing");
               messageBox.ShowDialog();
               isOK = false;
            }

            if (isOK)
            {
               if (!Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.isTrial)
               {
                  if (m_BBSProductLicense.IsLicenseStringTrial(licenseStr))
                  {
                      var messageBox = new Form_MessageBox(this, CoreConstants.CantAddTrialToPermamentLicense, CoreConstants.LicenseCaption, @"http://www.idera.com/licensing");
                      messageBox.ShowDialog();
                     isOK = false;
                  }
               }

               if (isOK)
               {
                  if (Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.licState
                      != BBSProductLicense.LicenseState.Valid ||
                      Globals.SQLcomplianceConfig.LicenseObject.CombinedLicense.isTrial)
                  {
                     m_BBSProductLicense.RemoveAllLicenses();
                  }
//                  if (m_BBSProductLicense.IsLicenseStringTrial(licenseStr))
//                  {
//                     m_BBSProductLicense.TagTrialLicenseUsed();
//                  }
                  m_BBSProductLicense.AddLicense(licenseStr);
                  Globals.SQLcomplianceConfig.ResetLicense(Globals.Repository.Connection);
               }
            }
         }
         return isOK;
      }

      #endregion

      #region Events

      private void button_GenerateTrialLicense_Click(object sender, EventArgs e)
      {
         string licenseKey = m_BBSProductLicense.GenerateTrialLicense();

         BBSProductLicense.LicenseState licState;
         if (m_BBSProductLicense.IsLicenseStringValid(licenseKey, out licState))
         {
            textBox_NewKey.Text = licenseKey;
         }
      }

      private void button_OK_Click(object sender, EventArgs e)
      {
         if (AddNewLicenseString(textBox_NewKey.Text.Trim()))
         {
            DialogResult = DialogResult.OK;
         }
         else
         {
            return;
         }
      }

      private void button_Cancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
      }

      #endregion

      private void lnkCustomerPortal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
          Process.Start("http://www.idera.com/licensing");
      }
   }
}