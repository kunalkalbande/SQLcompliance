﻿using System;
using System.Collections.Generic;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Service;
using Idera.SQLcompliance.Core.Licensing;
using BBS.License;
using System.Net;
using System.Data.SqlClient;
using SQLcomplianceCwfAddin.Errors;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using Microsoft.VisualBasic;
using SQLcomplianceCwfAddin.Repository;
using SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService : ILicenseManager, Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager
    {
        #region ILicenseManager Members

        public DataContracts.v1.License.LicenseDetails GetLicense()
        {
            using (_logger.InfoCall("LM Utility GetLicense"))
            {
                using (var connection = GetConnection())
                {
                    return new DataContracts.v1.License.LicenseDetails(); //ConvertToDataContract.ToDC(RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo));
                }
            }
        }

        #endregion

        #region Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager

        public bool ApplyLicenseKey(Idera.LicenseManager.ProductPlugin.DataContracts.License license)
        {
            {
                try
                {
                    SetConnectionCredentiaslFromCWFHost();
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                    throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
                }
                try
                {
                    var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                    //LicenseSummary licSummary = new LicenseSummary();
                    BBS.License.BBSLic lic = RepositoryHelper.LoadKeyString(license.Key);
                    if (lic != null)
                    {
                        // Key was loaded successfully.  Check for invalid properties.
                        if (lic.ProductID != CoreConstants.ProductID)
                        {
                            throw new LicenseManagerException("The specified key is for a different product.");
                        }
                        else if (lic.IsExpired)
                        {
                            throw new LicenseManagerException("The specified key has already expired.");
                        }
                        else if (!lic.CheckScopeHash(licSummary.Repository))
                        {
                            throw new LicenseManagerException(string.Format("The specified key cannot be used with repository {0}", licSummary.Repository));
                        }
                        else if (!LicenseSummary.IsLicenseReasonable(lic))
                        {
                            throw new LicenseManagerException("The specified key is invalid");
                        }
                        else
                        {
                            RepositoryHelper.AddOrReplaceKey(lic, license.Key);
                            return true;
                        }
                    }
                    else
                        throw new LicenseManagerException("No key was specified");
                }
                catch (LicenseManagerException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred while Applying Key : {0}", ex);
                    throw new LicenseManagerException("An error occurred while Applying Key : " + ex.Message);
                }
            }
        }

        public Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData GetEnvironmentalData()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData EnvironmentalData = new Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData();
                EnvironmentalData.CPUCount = Convert.ToString(Environment.ProcessorCount);
                EnvironmentalData.Memory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory.ToString();
                EnvironmentalData.OSVersion = Convert.ToString(Environment.OSVersion);
                using (SqlConnection connection = RestServiceConfiguration.SQLConnectInfo.GetConnection(CoreConstants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    EnvironmentalData.SQLServerVersion = connection.ServerVersion;
                }
                return EnvironmentalData;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching environmental info : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching Environmental Data : " + ex.Message); ;
            }
        }

        List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey> Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager.GetLicenseDetails()
        {

            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey> LicenseKeysDetails = new List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey>();
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                //LicenseSummary licSummary = new LicenseSummary();
                foreach (var checkedKey in licSummary.CheckedKeys)
                {
                    Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey licKey = new Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey();
                    licKey.Expiration = Convert.ToString(checkedKey.KeyObject.ExpirationDate);
                    licKey.Instances = checkedKey.KeyObject.Limit1;
                    licKey.IsUnlimited = (checkedKey.KeyObject.Limit1 == BBSLic.Unlimited);
                    licKey.Key = checkedKey.KeyString;
                    licKey.LicenseStatus = Convert.ToString(licSummary.Status);
                    licKey.Scope = licSummary.Repository;
                    licKey.IsEnterprise = checkedKey.KeyObject.IsEnterprise;//Incorporated new build of licence manager.
                    if (checkedKey.KeyObject.IsTrial)
                    {
                        licKey.Type = "Trial";
                    }
                    else
                        licKey.Type = "Production";
                    LicenseKeysDetails.Add(licKey);
                }
                return LicenseKeysDetails;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching license details : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license details : " + ex.Message);
            }
        }

        public Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary GetLicenseSummary()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary licenseSummary = new Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary();
                //fill the values from repository
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                //LicenseSummary licSummary = new LicenseSummary();
                licenseSummary.IsUnlimited = licSummary.IsUnlimited;
                licenseSummary.TotalKeys = Convert.ToString(licSummary.LicensedServers);
                licenseSummary.KeysUsed = Convert.ToString(licSummary.MonitoredServers);
                return licenseSummary;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching License summary : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license summary : " + ex.Message);
            }
        }

        public Idera.LicenseManager.ProductPlugin.DataContracts.Product GetProductInfo()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                _logX.InfoCall("Enter to GetProductInfo");
                Idera.LicenseManager.ProductPlugin.DataContracts.Product product = new Idera.LicenseManager.ProductPlugin.DataContracts.Product();
                product.ProductCode = CoreConstants.ProductID;
                product.ProductName = CoreConstants.PRODUCT_SHORT_NAME;
                _logX.InfoCall(product.ProductCode.ToString());
                _logX.InfoCall(product.ProductName);
                _logX.InfoCall("check assembly call");
                product.ProductVersion = CoreConstants.LmProductVersion;
                return product;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching product information : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license summary : " + ex.Message);
            }
        }

        public string Ping()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            return "Success";
        }

        public bool ReleaseLicenseKey(Idera.LicenseManager.ProductPlugin.DataContracts.License license)
        {
            {
                try
                {
                    SetConnectionCredentiaslFromCWFHost();
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                    throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
                }
                try
                {
                    List<string> keys = new List<string>();
                    Dictionary<String, String> licensesServerNumber = new Dictionary<string, string>();
                    var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                    //LicenseSummary licSummary = new LicenseSummary();
                    List<CheckedKey> keyList = new List<CheckedKey>();
                    CheckedKey ck = new CheckedKey(license.Key);
                    keyList.Add(ck);
                    foreach (CheckedKey item in keyList)
                    {
                        CheckedKey listObject = item;
                        keys.Add(listObject.KeyString);
                        licensesServerNumber.Add(listObject.KeyString, listObject.KeyObject.Limit1.ToString());
                    }
                    int flag = 0;
                    foreach (SQLcomplianceCwfAddin.Helpers.CheckedKey item in licSummary.CheckedKeys)
                    {
                        if (item.KeyString.Equals(ck.KeyString))
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        throw new LicenseManagerException("Specified Key Does Not Exist");
                    }
                    try
                    {
                        //to audit licenses delete
                        foreach (var licenses in licensesServerNumber)
                        {
                            RepositoryHelper.SetAuditableEntity(LicenseKeyOperation.Remove, licenses.Key, licenses.Value);
                        }

                        LicenseSummary license1 = RepositoryHelper.SetLicenseKeys(LicenseKeyOperation.Remove, keys);
                        return true;

                    }
                    catch (Exception ex)
                    {
                        _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                        throw new LicenseManagerException("An error occurred while removing license keys.");
                    }
                }
                catch (LicenseManagerException ex)
                {
                    _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                    throw new LicenseManagerException("An error occurred while removing license keys : " + ex.Message);
                }
            }
        }

        public bool UpdateUsedLicenses(string usedLicense)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
