using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BBS.License;
using Idera.SQLcompliance.Core.Encryption;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.Licensing
{
   /// <summary>
   /// SQLcompliance License Manager
   /// </summary>
   internal sealed class LicenseManager
   {
      #region Properties

      public static bool licenseExpiredAlready = false;

      #endregion

      #region Public Methods

      //-------------------------------------------------------------------
      // StartupCheck
      //
      // if license in repository, done
      // if license not in repository
      //    get license from registry and copy to repository
      //    if no license in registry, create one
      //-------------------------------------------------------------------
      public static bool StartupCheck(string instanceName)
      {
         SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
         Repository rep = null;

         try
         {
            rep = new Repository();
            rep.OpenConnection();

            config.Read(rep.connection);
            
            // look in repository
            if (config.LicenseObject.LicenseLoaded && config.LicenseObject.IsProductLicensed())
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "SQLcompliance license loaded from Repository");
               return true;
            }

            // check to see if we need to convert an old license
            IderaLicense oldLicense = new IderaLicense(config.OldLicenseKey) ;

            if(oldLicense.IsValid() && !oldLicense.IsTrialLicense() && !config.LicenseObject.HasLicenseConversionBeenDone())
            {
               string newKey = ConvertLicense(oldLicense) ;
               config.LicenseObject.TagLicenseConversionUsed() ;
               config.LicenseObject.AddLicense(newKey) ;
               ErrorLog.Instance.Write("A 45-day temporary license has been generated.  Please contact IDERA support for an updated license key.") ;
            }
            else if((oldLicense.IsValid() && oldLicense.IsTrialLicense()) || 
               !config.LicenseObject.HasTrialLicenseBeenUsed())
            {
               // If we have a valid 2.1 trial license, just generate a new one
               // if not in repository or registry, create a trial license
               string licenseKey = config.LicenseObject.GenerateTrialLicense();
               config.LicenseObject.AddLicense(licenseKey);
               config.LicenseObject.TagTrialLicenseUsed() ;
               // Store trials only in registry for easy recovery
               AddLicenseToRegistry(licenseKey) ;
               ErrorLog.Instance.Write("14 day trial license generated");
            }else
            {
               string licenseKey = ReadLicenseFromRegistry() ;
               BBSProductLicense.LicenseState licState ;
               if(config.LicenseObject.IsLicenseStringValid(licenseKey, out licState))
               {
                  config.LicenseObject.AddLicense(licenseKey) ;
                  ErrorLog.Instance.Write("Loading previously generated trial license") ;
               }
            }

            return true;
         }
         finally
         {
            if(rep != null)
               rep.CloseConnection();
         }
      }

      private static string ConvertLicense(IderaLicense oldLicense)
      {
         BBSLic newLicense = new BBSLic();

         newLicense.IsTrial = true;
         newLicense.KeyID = 0;
         newLicense.DaysToExpiration = CoreConstants.LicenseConversionDaysLimit;
         newLicense.ProductID = CoreConstants.ProductID;
         newLicense.Limit1 = oldLicense.Product.LicensedInstances;
         if (oldLicense.IsEnterprise)
            newLicense.MakeEnterprise() ;
         else
            newLicense.SetScopeHash(oldLicense.Server);
         Version v = Assembly.GetExecutingAssembly().GetName().Version;
         Version v2 = new Version(v.Major, v.Minor);
         newLicense.ProductVersion = v2;


         return newLicense.GetKeyString(PW());
      }

      private static byte[] PW()
      {
         Process currentProcess = Process.GetCurrentProcess();
         string data = currentProcess.MachineName + currentProcess.Id;
         return BBSLic.GetHash(data);
      }


      //---------------------------------------------------------------------------
      /// CheckLicense - workhorse for licenseCHeck event - seperated out
      //                 so that we can call driectlry from startup as well
      //---------------------------------------------------------------------------
      public static void CheckLicense()
      {
         SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
         Repository rep = null;

         try
         {
            rep = new Repository();
            rep.OpenConnection();
            config.Read(rep.connection);

            if (config.LicenseObject.LicenseLoaded && !config.LicenseObject.IsProductLicensed())
            {
               string sql = String.Format("SELECT instance FROM {0} where isEnabled=1 AND isAuditedServer=1", CoreConstants.RepositoryServerTable);
               using (SqlCommand selectCmd = new SqlCommand(sql, rep.connection))
               {
                  using (SqlDataReader reader = selectCmd.ExecuteReader())
                  {
                     while (reader.Read())
                     {
                        DisableInstance(reader.GetString(0));
                     }
                  }
               }

               if (!licenseExpiredAlready)
               {
                  if (!config.IsLicenseOk())
                  {
                     ErrorLog.Instance.Write("The installed license is invalid. Auditing will be disabled until a valid license has been installed. Please contact IDERA for assistance.",
                                             ErrorLog.Severity.Error);
                  }
                  else
                  {
                     ErrorLog.Instance.Write(CoreConstants.Info_TrialLicenseHasExpired, ErrorLog.Severity.Error);
                  }
                  licenseExpiredAlready = true;
               }
               // Note: agent will pick up new settings on next heartbeat
            }
            else
            {
               if ( config.LicenseObject.CombinedLicense.licState == BBSProductLicense.LicenseState.InvalidExpired )
               {
                  licenseExpiredAlready = true;
                  ErrorLog.Instance.Write(CoreConstants.Info_TrialLicenseHasExpired, ErrorLog.Severity.Error );
               }
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("An error occurred reading configuration record to perform license check.",
                                    ex);
         }
         finally
         {
            if(rep != null)
               rep.CloseConnection();
         }
      }

      #endregion


      #region Disable Instance Auditing

      //----------------------------------------------------------------------------
      /// DisableInstance - Mark an instance disabled because of license expiration
      //----------------------------------------------------------------------------
      private static void DisableInstance(string instanceName)
      {
         Repository rep = null;

         try
         {
            rep = new Repository();
            rep.OpenConnection();

            // Disable Server            
            string sql = String.Format("UPDATE {0} " +
                                       "SET isEnabled=0,configVersion=configVersion+1 " +
                                       "WHERE instance={1}",
                                       CoreConstants.RepositoryServerTable,
                                       SQLHelpers.CreateSafeString(instanceName));
            using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
            {
               cmd.ExecuteNonQuery();
            }

            // Log that it was disabled
            LogRecord.WriteLog(rep.connection,
                               LogType.DisableServer,
                               instanceName,
                               CoreConstants.Info_DisabledByLicenseCheck);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("An error disabling auditing for a registerd SQL Server instance",
                                    instanceName,
                                    ex);
         }
         finally
         {
            if(rep != null)
               rep.CloseConnection();
         }
      }

      #endregion

      //----------------------------------------------------------------------		
      // ReadLicenseFromRegistry
      //----------------------------------------------------------------------		
      private static string ReadLicenseFromRegistry()
      {
         string licenseString = "";
         RegistryKey licenseHive = OpenLicenseHive();
         RegistryKey licenseKey = null;

         try
         {
            licenseKey = licenseHive.OpenSubKey(CoreConstants.RegistryHiveLicensingStart.ToString());
            if (licenseKey != null)
            {
               string encryptedLicenseString = (string)licenseKey.GetValue(CoreConstants.RegistryKeyLicensingData);
               licenseString = DecryptRegistryLicense(encryptedLicenseString);
            }
         }
         finally
         {
            // Close all registry keys
            if (licenseHive != null) licenseHive.Close();
            if (licenseKey != null) licenseKey.Close();
         }

         return licenseString;
      }

      //----------------------------------------------------------------------		
      // AddLicenseToRegistry
      //----------------------------------------------------------------------		
      private static void AddLicenseToRegistry(string license)
      {
         string encryptedLicense = EncryptionEngine.QuickEncrypt(license);

         RegistryKey licenseHive = OpenLicenseHive();
         RegistryKey newLicenseKey = null;

         try
         {
            newLicenseKey = licenseHive.CreateSubKey(CoreConstants.RegistryHiveLicensingStart.ToString());
            newLicenseKey.SetValue(CoreConstants.RegistryKeyLicensingData, encryptedLicense);
            newLicenseKey.Flush();
         }
         finally
         {
            // Close all registry keys
            if (licenseHive != null) licenseHive.Close();
            if (newLicenseKey != null) newLicenseKey.Close();
         }
      }

      //----------------------------------------------------------------------		
      // OpenLicenseHive
      //----------------------------------------------------------------------		
      private static RegistryKey OpenLicenseHive()
      {
         RegistryKey hklm;
         RegistryKey hkSoftware = null;
         RegistryKey hkMicrosoft = null;
         RegistryKey hkLicensing;

         try
         {
            hklm = Registry.LocalMachine;
            hkSoftware = hklm.OpenSubKey("SOFTWARE");
            hkMicrosoft = hkSoftware.OpenSubKey("Microsoft", true);
            hkLicensing = hkMicrosoft.OpenSubKey(CoreConstants.RegistryHiveLicensing, true);

            if (hkLicensing == null)
            {
               hkLicensing = hkMicrosoft.CreateSubKey(CoreConstants.RegistryHiveLicensing);
            }

            return hkLicensing;
         }
         finally
         {
            if (hkSoftware != null) hkSoftware.Close();
            if (hkMicrosoft != null) hkMicrosoft.Close();
         }
      }

      private static string DecryptRegistryLicense(string encryptedLicenseString)
      {
         string licenseString = EncryptionEngine.QuickDecrypt(encryptedLicenseString);

         return licenseString;
      }
   }
}