package com.idera.sqlcm.facade;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMApplyReindexForArchiveRequest;
import com.idera.sqlcm.entities.CMArchive;
import com.idera.sqlcm.entities.CMArchiveProperties;
import com.idera.sqlcm.entities.CMArchivedDatabase;
import com.idera.sqlcm.entities.CMAttachArchive;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMAvailabilityInfo;
import com.idera.sqlcm.entities.CMColumnDetails;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDatabaseAuditedActivity;
import com.idera.sqlcm.entities.CMDatabaseAuditingRequest;
import com.idera.sqlcm.entities.CMEvent;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.CMTableDetails;
import com.idera.sqlcm.entities.CMUpdatedArchiveProperties;
import com.idera.sqlcm.entities.ProfilerObject;
import com.idera.sqlcm.entities.StatisticDataResponse;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;

public class DatabasesFacade {

	public static CMAuditedDatabase getDatabaseDetails(String instanceId, String databaseId) throws RestException {

		try {
			return SQLCMRestClient.getInstance().getDatabaseDetails(instanceId, databaseId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static List<String> getDatabaseByServerName(String serverName) throws RestException{
		try {
			return SQLCMRestClient.getInstance().GetDatabaseByServerName(serverName);
		} catch (RestException e) {
			throw new RestException(e);
		}
	}

	public static List<CMEvent> getDatabaseAuditEvents(String instanceId, String databaseId, Interval interval)
			throws RestException {

		try {
			return SQLCMRestClient.getInstance().getDatabaseAuditEvents(instanceId, databaseId, interval.getDays());
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<StatisticDataResponse> getRecentDatabaseActivity(Long instanceId, Interval interval,
			Long databaseId) throws RestException {
		try {
			List<StatisticDataResponse> statisticDataResponse = SQLCMRestClient.getInstance()
					.getRecentDatabaseActivity(instanceId, interval.getDays(), databaseId);
			if (statisticDataResponse == null) {
				return new ArrayList<StatisticDataResponse>();
			}
			return statisticDataResponse;
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMDatabase> getAuditedDatabasesForInstance(Long instanceId) throws RestException {
		try {
			List<CMDatabase> auditedDatabases = SQLCMRestClient.getInstance()
					.getAuditedDatabasesForInstance(instanceId.toString());
			if (auditedDatabases == null) {
				return new ArrayList<CMDatabase>();
			}
			return auditedDatabases;
		} catch (RestException e) {
			throw new RestException(e);
		}
	}

	public static List<String> getEventsDatabasesForInstance(Long instanceId) throws RestException {
		try {
			List<String> databases = SQLCMRestClient.getInstance()
					.getEventsDatabasesForInstance(instanceId.toString());
			if (databases == null) {
				return new ArrayList<String>();
			}
			return databases;
		} catch (RestException e) {
			throw new RestException(e);
		}
	}
	
	public static CMAuditedActivities getRegulationSettingsForDatabase(RegulationSettings regulationSettings)
			throws RestException {
		try {
			CMAuditedActivities cmAuditedActivities = SQLCMRestClient.getInstance()
					.getRegulationSettingsForDatabase(regulationSettings);
			if (cmAuditedActivities == null) {
				return new CMAuditedActivities();
			}
			return cmAuditedActivities;
		} catch (RestException e) {
			throw new RestException(e);
		}
	}

	public static CMDatabaseAuditedActivity getAuditedActivityForDatabase(Long instanceId, Long databaseId)
			throws RestException {
		try {
			CMDatabaseAuditedActivity cmDatabaseAuditedActivity = SQLCMRestClient.getInstance()
					.getAuditedActivityForDatabase(instanceId.toString(), databaseId.toString());
			if (cmDatabaseAuditedActivity == null) {
				return new CMDatabaseAuditedActivity();
			}
			return cmDatabaseAuditedActivity;
		} catch (RestException e) {
			throw new RestException(e);
		}
	}

	public static List<CMDatabase> getDatabaseList(long instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getDatabaseList(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMAvailabilityInfo> getAvailabilityInfoList(List<CMDatabase> databaseList) throws RestException {
		return SQLCMRestClient.getInstance().getAvailabilityInfoList(databaseList);
	}

	public static List<CMTable> getTableList(long instanceId, String databaseName, String filterText)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getTables(instanceId, databaseName, filterText);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<String> getColumnList(Long databaseId, String tableName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getColumns(databaseId, tableName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<String> getNewDatabaseColumnList(String tableName, String instanceName, String databaseName)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getNewDatabaseColumns(tableName, instanceName, databaseName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void changeAuditingForDatabase(CMDatabaseAuditingRequest cmDatabaseAuditingRequest)
			throws RestException {
		try {
			SQLCMRestClient.getInstance().changeAuditingForDatabase(cmDatabaseAuditingRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void removeDatabase(Map databaseMap) throws RestException {
		try {
			SQLCMRestClient.getInstance().removeDatabase(databaseMap);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMArchivedDatabase> getArchivesList(String instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getArchivesList(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMArchiveProperties getArchiveProperties(String archiveName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getArchiveProperties(archiveName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMDatabaseProperties getDatabaseProperties(Long instanceId) throws RestException {
		CMDatabaseProperties databaseProperties = null;
		try {
			databaseProperties = SQLCMRestClient.getInstance().getDatabaseProperties(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return databaseProperties;
	}

	public static String updateDatabaseProperties(CMDatabaseProperties databaseProperties) throws RestException {
		try {
			return SQLCMRestClient.getInstance().updateDatabaseProperties(databaseProperties);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void updateArchiveProperties(CMUpdatedArchiveProperties archiveProperties) throws RestException {
		try {
			SQLCMRestClient.getInstance().updateArchiveProperties(archiveProperties);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void detachArchive(CMArchive archive) throws RestException {
		try {
			SQLCMRestClient.getInstance().detachArchive(archive);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMAttachArchive> getDatabasesForArchiveAttachment(long parentId, boolean showAll)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getDatabasesForArchiveAttachment(parentId, showAll);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void attachArchive(String archiveName) throws RestException {
		try {
			SQLCMRestClient.getInstance().attachArchive(archiveName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void upgradeEventDatabaseSchema(String archiveName) throws RestException {
		try {
			SQLCMRestClient.getInstance().upgradeEventDatabaseSchema(archiveName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static Boolean checkNeedIndexUpdatesForArchive(String archiveName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().checkNeedIndexUpdatesForArchive(archiveName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void updateIndexesForEventDatabase(String archiveName) throws RestException {
		try {
			SQLCMRestClient.getInstance().updateIndexesForEventDatabase(archiveName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static Boolean isIndexStartTimeForArchiveDatabaseIsValid() throws RestException {
		try {
			return SQLCMRestClient.getInstance().isIndexStartTimeForArchiveDatabaseIsValid();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void applyReindexForAttach(CMApplyReindexForArchiveRequest archive) throws RestException {
		try {
			SQLCMRestClient.getInstance().applyReindexForAttach(archive);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String addAlertRules(AddAlertRulesSaveEntity addDatabasesSaveEntity) throws RestException {
		try {
			return new SQLCMRestClient().addAlertRules(addDatabasesSaveEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String addDataAlertRules(AddDataAlertRulesSaveEntity addDataAlertRulesSaveEntity)
			throws RestException {
		try {
			return new SQLCMRestClient().addDataAlertRules(addDataAlertRulesSaveEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String addStatusAlertRules(AddStatusAlertRulesSaveEntity addStatusAlertRulesSaveEntity)
			throws RestException {
		try {
			return new SQLCMRestClient().addStatusAlertRules(addStatusAlertRulesSaveEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String addDatabases(AddDatabasesSaveEntity addDatabasesSaveEntity) throws RestException {
		try {
			return new SQLCMRestClient().addDatabases(addDatabasesSaveEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	// SQLCM 5.4 Start
	public String ExportDatabaseAuditSettings(int database) {
		String resultData = "";
		try {
			resultData = new SQLCMRestClient().ExportDatabaseAuditSettings(database);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}

	public static void importDatabaseAuditSetting(AddWizardImportEntity wizardSaveEntity) throws RestException {
		try {
			new SQLCMRestClient().importDatabaseAuditSetting(wizardSaveEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}

	}

	public static List<CMDatabase> getAllDatabaseList(long instanceId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAllDatabaseList(instanceId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMTableDetails> getTableDetailSummaryForAll(List databaseName, long instanceId,
			String profileName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getTablesDetailsForAll(databaseName, instanceId, profileName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<CMColumnDetails> getColumnDetailSummaryForAll(List databaseName, long instanceId,
			String profileName) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getColumnDetailsForAll(databaseName, instanceId, profileName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static List<ProfilerObject> getProfileDetails() throws RestException {
		try {
			return SQLCMRestClient.getInstance().getProfiles();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String getActiveProfile() throws RestException {
		try {
			return SQLCMRestClient.getInstance().getActiveProfile();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void deleteStringSeleted(ProfilerObject deletionList) throws RestException {
		try {
			SQLCMRestClient.getInstance().deleteStringSeleted(deletionList);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void insertNewString(ProfilerObject insertionList) throws RestException {
		try {
			SQLCMRestClient.getInstance().insertNewString(insertionList);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void updateString(List<ProfilerObject> updateStrings) throws RestException {
		try {
			SQLCMRestClient.getInstance().updateString(updateStrings);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void activateProfile(String profileName) throws RestException {
		try {
			SQLCMRestClient.getInstance().activateProfile(profileName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void resetData() throws RestException {
		try {
			SQLCMRestClient.getInstance().resetData();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void createProfile(List<ProfilerObject> newProfileDetails) throws RestException {
		try {
			SQLCMRestClient.getInstance().createProfile(newProfileDetails);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void updateCurrentProfile(List<ProfilerObject> currentProfileDetails) throws RestException {
		try {
			SQLCMRestClient.getInstance().updateCurrentProfile(currentProfileDetails);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void deleteProfile(String profileName) throws RestException {
		try {
			SQLCMRestClient.getInstance().deleteProfile(profileName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static void updateIsUpdated(String value) throws RestException {
		try {
			SQLCMRestClient.getInstance().updateIsUpdated(value);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static String getIsUpdated() throws RestException {
		try {
			return SQLCMRestClient.getInstance().getIsUpdated();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	// SQLCM 5.4 End
	public List<String> ExportDatabaseRegulationAuditSettings(List<CMDatabase> dblist) {
		List<String> resultData = new ArrayList<>();
		try {
			resultData = new SQLCMRestClient().ExportDatabaseRegulationAuditSettings(dblist);
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}
}
