using System.ServiceModel;
using SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    [ServiceContract]
    public interface IRestService : 
        IUtility, 
        IArchivesService, 
        IEnvironmentService, 
        ILicenseService, 
        IEventService, 
        IAlertService, 
        IDatabasesService, 
        IInstanceService, 
        IServerService,
        ISettingService,
        IAgentService,
        IUserSettingsService,
        IActivityLogsViewService,
        IChangeLogsViewService,
        IAlertRules,		
        IAuditReportService,
        ILicenseManager,
        Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager
    {
    }
}
