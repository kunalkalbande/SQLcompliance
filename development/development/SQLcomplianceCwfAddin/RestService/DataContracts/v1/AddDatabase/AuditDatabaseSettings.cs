using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{

    [DataContract(Name = "auditDatabaseSettings")]
    public class AuditDatabaseSettings
    {
        [DataMember(Order = 1, Name = "databaseList")]
        public List<AuditedDatabaseInfo> DatabaseList { get; set; }

        [DataMember(Order = 2, Name = "collectionLevel")]
        public AuditCollectionLevel CollectionLevel { get; set; }

        [DataMember(Order = 3, Name = "trustedRolesAndUsers")]
        public ServerRolesAndUsers TrustedRolesAndUsers { get; set; }

        [DataMember(Order = 4, Name = "privilegedRolesAndUsers")]
        public ServerRolesAndUsers PrivilegedRolesAndUsers { get; set; }

        [DataMember(Order = 5, Name = "regulationSettings")]
        public AuditRegulationSettings RegulationSettings { get; set; }

        [DataMember(Order = 6, Name = "availabilityGroupList")]
        public List<AvailabilityGroup> AvailabilityGroupList { get; set; }

        [DataMember(Order = 7, Name = "auditedActivities")]
        public AuditActivity AuditedActivities { get; set; }

        [DataMember(Order = 8, Name = "userAuditedActivities")]
        public AuditActivity UserAuditedActivities { get; set; }

        [DataMember(Order = 9, Name = "dmlSelectFilters")]
        public DmlSelectFilters DmlSelectFilters { get; set; }

        [DataMember(Order = 10, Name = "auditExceptions")]
        public bool AuditExceptions { get; set; }

        // these properties are only for Add Server wizard when users have abitility to continue adding databases 
        // after added server instance
        [DataMember(Order = 11, Name = "updateServerSettings")]
        public bool UpdateServerSettings { get; set; }

        [DataMember(Order = 12, Name = "serverSettingsToBeUpdated")]
        public ServerSettingsData ServerSettingsToBeUpdated { get; set; }
       
        [DataMember(Order = 13, Name = "auditedServerActivities")]
        public AuditActivity AuditedServerActivities { get; set; }

        [DataMember(Order = 14, Name = "isServerType")]
        public bool IsServerType { get; set; }
         }
    }
