using Idera.SQLcompliance.Core.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "cmCombinedLicense")]
    public class CmCombinedLicense: CmLicense
    {
        #region constructor \ destructor

        public CmCombinedLicense()
        {
            Licenses = new List<CmLicense>();  
            State = CmLicenseStateFlag.InvalidKey;
        }

        #endregion

        [DataMember(Order = 0, Name = "licenses")]
        public List<CmLicense> Licenses { get; set; }

        [DataMember(Order = 1, Name = "monitoredServers")]
        public int MonitoredServers { get; set; }

        [DataMember(Order = 2, Name = "repositoryServer")]
        public string RepositoryServer { get; set; }

        [DataMember(Order = 3, Name = "licensedServers")]
        public int LicensedServers { get; set; }

        internal override void Populate(BBSProductLicense LicenseObject, List<CmLicense> licenses = null)
        {
            var isPopulated = false;
            foreach (var license in Licenses)
            {
                // copy license details from the first license
                if (!isPopulated)
                {
                    Id = -1;
                    CreatedBy = license.CreatedBy;
                    CreatedTime = license.CreatedTime;
                    DaysToExpiration = license.DaysToExpiration;
                    ExpirationDate = license.ExpirationDate;
                    IsAboutToExpire = license.IsAboutToExpire;
                    IsTrial = license.IsTrial;
                    LicenseType = license.LicenseType;
                    Key = license.Key;
                    State = license.State;
                    Scope = license.Scope;

                    isPopulated = true;
                    continue;
                }

                Key = CombinedLicenses;
                if (!license.IsTrial && license.State == CmLicenseStateFlag.Valid)
                {
                    if (license.NumberOfLicensedServers == -1)
                        NumberOfLicensedServers = -1;
                    else if (NumberOfLicensedServers != -1)
                        NumberOfLicensedServers += license.NumberOfLicensedServers;
                }

                if (ExpirationDate != license.ExpirationDate)
                {
                    if (license.IsAboutToExpire)
                        IsAboutToExpire = true;

                    ExpirationDate = CombinedLicensesMultiExpirationDates;
                    DaysToExpiration = CombinedLicensesMultiExpirationDates;
                }

                if (Scope != license.Scope)
                    Scope = string.Format(CombinedLicensesMultiTypes, Scope);
            }

            LicensedServers = Licenses.Sum(cmLicense => cmLicense.NumberOfLicensedServers);

            if (string.IsNullOrEmpty(NumberOfLicensedServersString))
            {
                NumberOfLicensedServersString = LicensedServers.ToString();
            }
        }
    }
}
