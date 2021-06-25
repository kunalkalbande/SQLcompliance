using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BBS.License;
using SQLcomplianceCwfAddin.Helpers;
using Idera.SQLcompliance.Core.Licensing;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "cmLicenseStateFlag")]
    public enum CmLicenseStateFlag: byte
    {
        [EnumMember(Value = "valid")]
        Valid,

        [EnumMember(Value = "invalidKey")]
        InvalidKey,

        [EnumMember(Value = "invalidProductId")]
        InvalidProductId,

        [EnumMember(Value = "invalidScope")]
        InvalidScope,

        [EnumMember(Value = "invalidExpired")]
        InvalidExpired,

        [EnumMember(Value = "invalidMixedTypes")]
        InvalidMixedTypes,

        [EnumMember(Value = "invalidDuplicateLicense")]
        InvalidDuplicateLicense,

        [EnumMember(Value = "invalidProductVersion")]
        InvalidProductVersion
    }

    [DataContract(Name = "cmLicense")]
    public class CmLicense
    {
        #region members

        protected const string LicenseTypeProduction = "Production";
        protected const string LicenseTypeTrial = "Trial";
        protected const string LicenseTypeEnterprise = "Enterprise";
        protected const string LicenseNoExpirationDate = "None";
        protected const string LicenseExpired = "License Expired";
        protected const string CombinedLicenses = "Resulting Combined Licenses";
        protected const string CombinedLicensesMultiExpirationDates = "Multiple Expiration Dates";
        protected const string CombinedLicensesMultiTypes = "Mixed Enterprise and {0}";
        protected const string LicenseCountNa = "Not Applicable";
        protected const string LicenseCountUnlimited = "Unlimited";

        protected const int ExpirationDayToWarnProduction = 45;
        protected const int ExpirationDayToWarnTrial = 7;

        private readonly BBSLic _license;
        private const int ProductId = 1400;
        private string _licenseScope;
        private int _numberOfLicensedServers;
        private DateTime _createdTime;

        #endregion

        #region constructor \ destructor

        public CmLicense()
        {
            _license = new BBSLic();
            CreatedTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public CmLicense(string licenseScope): this()
        {
            Scope = licenseScope;
        }

        #endregion

        #region properties

        [DataMember(Order = 1, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 2, Name = "key")]
        public string Key { get; set; }

        [DataMember(Order = 3, Name = "createdBy")]
        public string CreatedBy { get; set; }

        internal DateTime CreatedTime
        {
            get { return _createdTime; }
            set
            {
                _createdTime = value;
                CreatedTimeString = Transformer.Instance.GetDateString(_createdTime);
            }
        }

        [DataMember(Order = 4, Name = "createdTime", EmitDefaultValue = false)]
        public string CreatedTimeString { get; set; }

        [DataMember(Order = 5, Name = "state")]
        public CmLicenseStateFlag State { get; set; }

        internal bool IsTrial { get; set; }

        [DataMember(Order = 6, Name = "expirationDate")]
        internal string ExpirationDate { get; set; }

        [DataMember(Order = 7, Name = "scope")]
        public string Scope
        {
            get { return _licenseScope; }
            set { _licenseScope = value; }
        }

        internal int NumberOfLicensedServers
        {
            get 
            { 
                return _numberOfLicensedServers; 
            }
            set
            {
                _numberOfLicensedServers = value;

                // set licensed servers string
                if (_numberOfLicensedServers == BBSLic.Unlimited)
                    NumberOfLicensedServersString = LicenseCountUnlimited;
                else if (_numberOfLicensedServers == BBSLic.NotApplicable)
                    NumberOfLicensedServersString = LicenseNoExpirationDate;
                else
                    NumberOfLicensedServersString = _numberOfLicensedServers.ToString();
            }
        }

        [DataMember(Order = 8, Name = "licensedServerCount")]
        public string NumberOfLicensedServersString { get; set; }

        [DataMember(Order = 9, Name = "licenseType")]
        public string LicenseType { get; set; }

        internal int DaysToExpire { get; set; }

        internal string DaysToExpiration { get; set; }

        internal bool IsAboutToExpire { get; set; }

        #endregion

        private CmLicenseStateFlag ConvertToCmLicenseStateFlag(BBSProductLicense.LicenseState licState)
        {
            byte value = (byte)licState;
            CmLicenseStateFlag convertedValue = (CmLicenseStateFlag)Enum.ToObject(typeof(CmLicenseStateFlag), value);
            return convertedValue;
        }

        internal virtual void Populate(BBSProductLicense LicenseObject, List<CmLicense> licensesAddedToCombinedLicense = null)
        {
            if (_license.LoadKeyString(Key) == LicErr.OK)
            {
                Scope = _license.IsEnterprise ? LicenseCountUnlimited : Scope;
                IsTrial = _license.IsTrial;
                NumberOfLicensedServers = _license.Limit1;
                LicenseType = _license.IsTrial ? LicenseTypeTrial : LicenseTypeProduction;
                ExpirationDate = _license.IsPermanent ? LicenseNoExpirationDate : _license.ExpirationDate.ToShortDateString();

                DaysToExpire = _license.DaysToExpiration;
                if (_license.IsPermanent)
                    DaysToExpiration = LicenseNoExpirationDate;
                else if (_license.IsExpired)
                    DaysToExpiration = LicenseExpired;
                else
                    DaysToExpiration = _license.DaysToExpiration.ToString();

                if (LicenseType == LicenseTypeProduction && 
                    DaysToExpire <= ExpirationDayToWarnProduction ||
                    LicenseType == LicenseTypeTrial && 
                    DaysToExpire <= ExpirationDayToWarnTrial)
                    IsAboutToExpire = true;
                else
                    IsAboutToExpire = false;

                if (_license.ProductID != ProductId)
                {
                    State = CmLicenseStateFlag.InvalidProductId;
                }
                else if (!(_license.IsEnterprise || _license.CheckScopeHash(Scope)))
                {
                    State = CmLicenseStateFlag.InvalidScope;
                }
                else if (_license.IsExpired)
                {
                    State = CmLicenseStateFlag.InvalidExpired;
                }
                else if (_license.IsTrial)
                {
                    State = CmLicenseStateFlag.Valid;
                }
                else if (_license.ProductVersion >= BBSProductLicense.SUPPORTED_PREVIOUS_LICENSE_VERSION)
                {
                    State = CmLicenseStateFlag.Valid;
                }
                else
                {
                    State = CmLicenseStateFlag.InvalidProductVersion;
                }

                // check for duplicate license
                if (licensesAddedToCombinedLicense != null)
                {
                    foreach (var license in licensesAddedToCombinedLicense)
                    {
                        if (license.Key.Equals(Key))
                        {
                            State = CmLicenseStateFlag.InvalidDuplicateLicense;
                            break;
                        }
                    }
                }
            }
            else
                State = CmLicenseStateFlag.InvalidKey;
        }
    }
}
