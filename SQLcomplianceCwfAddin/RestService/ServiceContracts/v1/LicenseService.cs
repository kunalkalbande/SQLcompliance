using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Licensing;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public bool CanAddOneMoreInstance()
        {
            using (_logger.InfoCall("CanAddOneMoreInstance"))
            {
                using (var connection = GetConnection())
                {
                    bool canAddMoreInstances = LicenseHelper.LicenseAllowsMoreInstances(connection);
                    return canAddMoreInstances;
                }
            }
        }

        public CmCombinedLicense GetCmLicenses()
        {
            using (_logger.InfoCall("GetCmLicenses"))
            {
                var prinicpal = GetPrincipalFromRequest();
                var connectionCredentials = GetConnectionCredentials(prinicpal);

                var query = QueryBuilder.Instance.GetCmLicenses();
                var result = QueryExecutor.Instance.GetCmLicenses(GetConnection(), query, Transformer.Instance.TranslateServerName(connectionCredentials.Location));
                return result;
            }
        }

        public AddLicenseResponse AddLicense(string licenseKey)
        {
            AddLicenseResponse result = new AddLicenseResponse();

            using (_logger.InfoCall("AddLicense"))
            {
                using (var connection = GetConnection())
                {
                    SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(connection);
                    BBSProductLicense licenseObject = configuration.LicenseObject;

                    BBSProductLicense.LicenseState addLicenseState;
                    if (!licenseObject.IsLicenseStringValid(licenseKey, out addLicenseState))
                    {
                        result.Success = false;

                        switch (addLicenseState)
                        {
                            case BBSProductLicense.LicenseState.InvalidKey:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalid, licenseKey);
                                break;
                            case BBSProductLicense.LicenseState.InvalidExpired:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseExpired);
                                break;
                            case BBSProductLicense.LicenseState.InvalidProductID:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalidProductID);
                                break;
                            case BBSProductLicense.LicenseState.InvalidProductVersion:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalidProductVersion);
                                break;
                            case BBSProductLicense.LicenseState.InvalidScope:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalidRepository,
                                    configuration.Server);
                                break;
                            case BBSProductLicense.LicenseState.InvalidDuplicateLicense:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalidDuplicate);
                                break;
                            default:
                                result.ErrorMessage = string.Format(CoreConstants.LicenseInvalid, licenseKey);
                                break;
                        }
                    }
                    else
                    {
                        if (!licenseObject.CombinedLicense.isTrial &&
                            licenseObject.IsLicenseStringTrial(licenseKey))
                        {
                            result.ErrorMessage = CoreConstants.CantAddTrialToPermamentLicense;
                        }
                        else
                        {
                            result.Success = true;

                            if (licenseObject.CombinedLicense.licState != BBSProductLicense.LicenseState.Valid ||
                                licenseObject.CombinedLicense.isTrial)
                            {
                                licenseObject.RemoveAllLicenses();
                            }

                            licenseObject.AddLicense(licenseKey);
                            configuration.ResetLicense(GetConnection());
                        }
                    }
                }

                return result;
            }
        }

        public void RemoveLicense(int id)
        {
            using (_logger.InfoCall("GetCmLicenses"))
            {
                using (var connection = GetConnection())
                {
                    SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(connection);
                    configuration.LicenseObject.RemoveLicense(id);
                    configuration.ResetLicense(connection);
                }
            }
        }
    }
}
