package com.idera.sqlcm.facade;

import com.idera.common.InstanceStatus;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.*;
import com.idera.sqlcm.entities.CMCheckAgentStatusResult;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEntity.EntityType;
import com.idera.sqlcm.entities.CMEntity.RuleType;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.CMManageInstanceCredentials;
import com.idera.sqlcm.entities.CMManagedInstance;
import com.idera.sqlcm.entities.CMManagedInstanceDetails;
import com.idera.sqlcm.entities.CMManagedInstances;
import com.idera.sqlcm.entities.CMPageSortRequest;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.sqlcm.entities.CMServerAuditingRequest;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.CMUpgradeAgentResponse;
import com.idera.sqlcm.entities.CMValidateInstanceCredentialResult;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.entities.InstanceAlert;
import com.idera.sqlcm.entities.StatisticData.Statistic;
import com.idera.sqlcm.entities.StatisticDataResponse;
import com.idera.sqlcm.entities.addserverwizard.*;
import com.idera.sqlcm.entities.instances.CMAgentProperties;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.entities.instances.CMRemoveInstanceResponse;
import com.idera.sqlcm.enumerations.Category;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.enumerations.State;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;
import com.idera.sqlcm.ui.instancedetails.AllSensitiveDetails;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.Set;


public class InstancesFacade extends CommonFacade {
	@Override
	public List<CMEntity> getAllEntities(Map<String, Object> filterRequest) {
		List<CMEntity> resultData = super.getAllEntities(filterRequest);
		try {
			List<AuditedInstanceBE> auditedInstanceBEList = SQLCMRestClient.getInstance().getAllAuditedInstances(filterRequest);
			resultData.addAll(mapList(auditedInstanceBEList));
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}

	public static List<String> getAllAuditedInstances() throws RestException{
		List<String> serversList = null;
		try {
			serversList = SQLCMRestClient.getInstance().GetAllAuditedInstances();
		} catch (RestException e) {
			throw new RestException(e);
		}
		return serversList;
	}
	
	public List<AuditedInstanceBE> getAllEntitiesInstances(Map<String, Object> filterRequest) {
		List<AuditedInstanceBE> auditedInstanceInstancesList = null;
		try {
			auditedInstanceInstancesList = SQLCMRestClient.getInstance().getAllAuditedInstances(filterRequest);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return auditedInstanceInstancesList;
	}
	
	public static CMInstance getInstanceDetails(long instId) throws RestException {

		try {
			return SQLCMRestClient.getInstance().getInstanceDetails(instId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMInstance getInstanceDetails(long instId, int days) throws RestException {

		try {
			return SQLCMRestClient.getInstance().getInstanceDetails(instId, days);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	/*public static List<Statistic> getInstanceStatsData(long instId, Interval interval,
            Category category) throws RestException {
		try {
			List<StatisticDataResponse> statisticDataResponse = SQLCMRestClient.getInstance().getInstanceStatsData(instId, interval.getDays(), category.getIndex());
			List<Statistic> statsData = new ArrayList<>();
			if(statisticDataResponse == null){
				return statsData;
			}
			for(StatisticDataResponse stat: statisticDataResponse){
				if(stat.getKey() == category.getIndex()){
					statsData = stat.getValue();
				}
			}
			return statsData;
		} catch (Exception e) {
			throw new RestException(e);
		}
	}*/

	public static List<Statistic> getInstanceStatsData(long instId, Interval interval,
            Category category) throws RestException {
  try {
   List<StatisticDataResponse> statisticDataResponse = SQLCMRestClient.getInstance().getInstanceStatsData(instId, interval.getDays(), category.getIndex());
   List<Statistic> statsData = new ArrayList<>();
   if(statisticDataResponse == null){
    return statsData;
   }
   
   for(StatisticDataResponse stat: statisticDataResponse){
    for(Statistic st: stat.getValue()){
     if (st.getCategory() == Category.EVENT_ALERT.getIndex())
      st.setCategoryName(Category.EVENT_ALERT.getLabel());
     if (st.getCategory() == Category.DDL.getIndex())
      st.setCategoryName(Category.DDL.getLabel());
     if (st.getCategory() == Category.FAILED_LOGIN.getIndex())
      st.setCategoryName(Category.FAILED_LOGIN.getLabel());
     if (st.getCategory() == Category.PRIVILEGED_USER.getIndex())
      st.setCategoryName(Category.PRIVILEGED_USER.getLabel());
     if (st.getCategory() == Category.SECURITY.getIndex())
      st.setCategoryName(Category.SECURITY.getLabel());
     statsData.add(st);
    }
   }
   /*for(StatisticDataResponse stat: statisticDataResponse){
     statsData = stat.getValue();
   }*/
   return statsData;
  } catch (Exception e) {
   throw new RestException(e);
  }
 }
	
	public static List<Statistic> getEnvironmentStatsData(Interval interval, Category category) throws RestException {
		try {
			List<StatisticDataResponse> statisticDataResponse = SQLCMRestClient.getInstance().getEnvironmentStatsData(interval.getDays(), category.getIndex());
			List<Statistic> statsData = new ArrayList<>();
			if(statisticDataResponse == null){
				return statsData;
			}
			for(StatisticDataResponse stat: statisticDataResponse){
				if(stat.getKey() == category.getIndex()){
					statsData = stat.getValue();
				}
			}
			return statsData;
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMInstanceProperties getInstanceProperties(long instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditedInstanceProperties(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

    public static List<CMInstance> getInstanceList() throws RestException {

        try {
            return SQLCMRestClient.getInstance().getInstances();
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

	public static ServerRegisteredStatusInfo checkServerRegisteredStatusByName(String name) throws RestException  {
		return SQLCMRestClient.getInstance().checkServerRegisteredStatusByName(name);
	}

	/*Mappers. Begin*/
	public static Instance map(AuditedInstanceBE auditedInstanceBE) {
		Instance instance = new Instance();
		instance.setId(auditedInstanceBE.getId());
		instance.setInstanceName(auditedInstanceBE.getInstance());
		instance.setInstanceOK(auditedInstanceBE.isServerStatus());
		instance.setStatusText(auditedInstanceBE.getStatusText());
		instance.setNumberOfAuditedDatabases(auditedInstanceBE.getAuditedDatabaseCount());
		instance.setSqlServerVersionEdition(auditedInstanceBE.getSqlVersionString());
		instance.setSevereAlerts(auditedInstanceBE.getSevereAlerts());
		instance.setHighAlerts(auditedInstanceBE.getHighAlerts());
		instance.setMediumAlert(auditedInstanceBE.getMediumAlert());
		instance.setLowAlerts(auditedInstanceBE.getLowAlerts());
		instance.setRecentAlertCount(auditedInstanceBE.getRecentAlertCount());
		instance.setAuditSettingsUpdateEnabled(auditedInstanceBE.isAuditSettingsUpdateEnabled());
		instance.setAuditEnabled(auditedInstanceBE.isEnabled());
		instance.setDeployed(auditedInstanceBE.isDeployed());

		if (auditedInstanceBE.isEnabled()) {
			instance.setAuditStatus(State.ENABLED.getLabel());
		} else {
			instance.setAuditStatus(State.DISABLED.getLabel());
		}
		instance.setLastAgentContact(auditedInstanceBE.getLastTimeAgentContacted());
		instance.setPrimary(auditedInstanceBE.isPrimary());
		return instance;
	}

	public static List<Instance> mapList(List<AuditedInstanceBE> auditedInstanceBEList) {
		List<Instance> instanceList = new ArrayList<>();
		for (AuditedInstanceBE auditedInstanceBE: auditedInstanceBEList) {
			instanceList.add(map(auditedInstanceBE));
		}
		return instanceList;
	}
	/*Mappers. End.*/

    public static CMPermissionInfo getPermissionInfo(long instId) throws RestException {
        try {
            return SQLCMRestClient.getInstance().getPermissionInfo(instId);
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

    public static CMInstanceUsersAndRoles getInstanceUsersAndRoles(long instId) throws RestException {
        try {
            return SQLCMRestClient.getInstance().getInstanceUsersAndRoles(instId);
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

	public static void changeAuditingForServers(CMServerAuditingRequest cmServerAuditingRequest) throws RestException {
		try {
			SQLCMRestClient.getInstance().changeAuditingForServers(cmServerAuditingRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String updateAuditConfigurationForServer(long instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().updateAuditConfigurationForServer(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String updateAuditServerProperties(CMInstanceProperties properties) throws RestException {
		try {
			return SQLCMRestClient.getInstance().updateAuditServerProperties(properties);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAgentProperties getAgentProperties(long instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAgentProperties(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMUpgradeAgentResponse upgradeAgent(long instanceId, String account, String password) throws RestException {
		try {
			return SQLCMRestClient.getInstance().upgradeAgent(instanceId, account, password);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String updateAgentProperties(CMAgentProperties properties) throws RestException {
		try {
			return SQLCMRestClient.getInstance().updateAgentProperties(properties);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMCheckAgentStatusResult checkAgentStatus(String instanceName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().checkAgentStatus(instanceName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMRemoveInstanceResponse> removeAuditServers(List serverIdList, boolean deleteEventsDatabase) throws RestException {
		try {
			return SQLCMRestClient.getInstance().removeAuditServers(serverIdList, deleteEventsDatabase);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<String> getAllNotRegisteredInstanceNameList() throws RestException {
		return SQLCMRestClient.getInstance().getAllNotRegisteredInstanceNameList();
	}

	public static String addInstanceConfig(ServerConfigEntity addDatabasesSaveEntity) throws RestException {
		return SQLCMRestClient.getInstance().addInstanceConfig(addDatabasesSaveEntity);
	}

	public static AddInstanceResult addInstance(AddServerEntity instanceEntity) throws RestException {
		List<AddInstanceResult> result = SQLCMRestClient.getInstance().addServers(Arrays.asList(instanceEntity));

		if (result != null && result.size() > 0) {
			return result.get(0);
		}

		throw new RestException(" AddInstanceResult is null or empty! ");
	}

	public static List<AddInstanceResult> addInstances(List<AddServerEntity> instanceEntityList) throws RestException {
		return SQLCMRestClient.getInstance().addServers(instanceEntityList);
	}

	public static List<AddInstanceResult> importInstances(Set<String> instances) throws RestException {
		return SQLCMRestClient.getInstance().importInstances(instances);
	}

	public static AgentDeploymentProperties getAgentDeploymentPropertiesForInstance(String instanceName) throws RestException {
		return SQLCMRestClient.getInstance().getAgentDeploymentPropertiesForInstance(instanceName);
	}

	public static InstanceAvailableResult isInstanceAvailable(String instanceName) throws RestException {
		return SQLCMRestClient.getInstance().isInstanceAvailable(instanceName);
	}

	public static CMManagedInstances getManagedInstances(CMPageSortRequest sortRequest) throws RestException {
		return SQLCMRestClient.getInstance().getManagedInstances(sortRequest);
	}

	public static CMManagedInstanceDetails getManagedInstanceDetails(long id) throws RestException {
		return SQLCMRestClient.getInstance().getManagedInstance(id);
	}

	public static void updateManageInstance(CMManagedInstance managedInstance) throws RestException {
		SQLCMRestClient.getInstance().updateManagedInstance(managedInstance);
	}

	public static List<CMValidateInstanceCredentialResult> validateInstanceCredentials(CMManageInstanceCredentials credentials, List<Long> instanceIdList) throws RestException {
		return SQLCMRestClient.getInstance().validateInstanceCredentials(credentials, instanceIdList);
	}

	public static void updateManagedInstancesCredentials(CMManageInstanceCredentials credentials, List<Long> instanceIdList) throws RestException {
		SQLCMRestClient.getInstance().updateManagedInstancesCredentials(credentials, instanceIdList);
	}
	
	public static Collection<InstanceAlert> getInstancesAlerts(
			RuleType alertType) throws RestException {

		try {
			return new SQLCMRestClient().getInstancesAlerts(alertType);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static List<InstanceStatus> getInstancesStatus(EntityType instanceType)
			throws RestException {

		try {
			return new SQLCMRestClient().getInstancesStatus(instanceType);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
 
	public String deleteInstances(Object[] instances) {
		String resultData = "";
		try {
			resultData =  new SQLCMRestClient().deleteInstances(instances);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}
	//Start SQLCm-5.4 
	//Requirement - 4.1.3.1
	public AllSensitiveDetails validateSensitiveColumn(String csvData) {
		AllSensitiveDetails resultData = new AllSensitiveDetails();
		try {
			resultData = new SQLCMRestClient().validateSensitiveColumns(csvData);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}
	//Start 5.4
	// Start Requirement - 4.1.3.1
	
	public void saveSensitiveColumnData(AllSensitiveDetails retVal) throws RestException {
		// TODO Auto-generated method stub
		AllSensitiveDetails resultData= new AllSensitiveDetails();
		try{
		    new SQLCMRestClient().saveSensitiveColumnData(retVal);
		   } 
		catch(RestException e)
		      {
			   e.printStackTrace();
		      }	
	}
	//End Requirement - 4.1.3.1	
	// Start Requirement - 4.1.4.1
	public AddWizardImportEntity validateAuditSettings(String xmlData) {
		AddWizardImportEntity resultData = new AddWizardImportEntity();
		try {
			resultData = new SQLCMRestClient().validateAuditSettings(xmlData);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}	
	
	public String ExportServerAuditSettings(String currentInstanceName)
	{
		String resultData = "";
		try {
			resultData =  new SQLCMRestClient().ExportServerAuditSettings(currentInstanceName);
		}
		catch(RestException e)
	      {
		   e.printStackTrace();
	      }
		return resultData;
	}
	
	//End Requirement - 4.1.3.1
	
	public String getLocalTime(){
		String resultData = "";
		try{
			resultData = new SQLCMRestClient().getLocalTime();
		}catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}
	
	public static boolean isLinqDllLoaded(){
		boolean result =  false;
		try{
			result = new SQLCMRestClient().isLinqDllLoaded();
		}
		catch(RestException e){
			e.printStackTrace();
		}
		return result;
	}	
	
	// Added for Custom Regulation Guideline Support
    public static RegulationCustomDetail getRegulationSettingsServer(com.idera.sqlcm.entities.RegulationSettings regulationSettings) throws RestException {
		try {
			RegulationCustomDetail regulationCustomDetail = SQLCMRestClient.getInstance().getRegulationSettingsServerLevel(regulationSettings);
			if (regulationCustomDetail == null) {
				return new RegulationCustomDetail();
			}
			return regulationCustomDetail;
		} catch (RestException e) {
			throw new RestException(e);
		}
	}
    
    public List<String> ExportServerRegulationAuditSettings(String currentInstanceName)
	{
		List<String> resultData = null;
		try {
			resultData =  new SQLCMRestClient().ExportServerRegulationAuditSettings(currentInstanceName);
		}
		catch(RestException e)
	      {
		   e.printStackTrace();
	      }
		return resultData;
	}
}

