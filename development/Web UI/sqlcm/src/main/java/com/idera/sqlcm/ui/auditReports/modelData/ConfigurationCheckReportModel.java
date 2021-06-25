package com.idera.sqlcm.ui.auditReports.modelData;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.apache.log4j.Logger;
import org.zkoss.zul.ListModelList;

import com.idera.sqlcm.ui.auditReports.AuditReportGridViewModel;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditConfigurationDatabaseResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditIderaDefaultValuesResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListConfigurationResponse;

public class ConfigurationCheckReportModel {

	private ListModelList<ConfigurationServerCheckEvents> configCheckEvents;
	Map<String, String> dbDefaults;
	private Map<String, String> serverDefaults;

	public ListModelList<ConfigurationServerCheckEvents> createReportModel(int auditSetting, int defaultStatus,
			CMAuditListConfigurationResponse cmAuditListConfigurationResponse,
			CMAuditIderaDefaultValuesResponse defServerValues, CMAuditIderaDefaultValuesResponse defDBValues) {
		// TODO Auto-generated method stub
		dbDefaults = defDBValues.getDefaultMap();
		serverDefaults = defServerValues.getDefaultMap();
		configCheckEvents = new ListModelList<ConfigurationServerCheckEvents>();
		List<CMAuditConfigurationResponse> conditionConfigurationEvents = (List) cmAuditListConfigurationResponse
				.getAuditConfiguration();
		int conditionEventsCount = conditionConfigurationEvents.size();

		// Process all servers
		for (int i = 0; i < conditionEventsCount; i++) {

			CMAuditConfigurationResponse currSerData = conditionConfigurationEvents.get(i);
			if (!(auditSetting == 24 || auditSetting == 17 || auditSetting == 18 || auditSetting == 19
					|| auditSetting == 20 || auditSetting == 21 || auditSetting == 22 || auditSetting == 23))
				createEntityForServer(auditSetting, defaultStatus, currSerData);

		}
		
		// Process all databases
		for (int i = 0; i < conditionEventsCount; i++) {
			CMAuditConfigurationResponse currSerData = conditionConfigurationEvents.get(i);
			List<CMAuditConfigurationDatabaseResponse> databases = currSerData.getDatabaseConfigList();
			/*
			 * Logger logger = Logger.getLogger(AuditReportGridViewModel.class);
			 * logger.info("[Configuration DB Count] : " +databases.size() ) ;
			 */
			if (auditSetting != 9) {
				for (CMAuditConfigurationDatabaseResponse cmAuditConfigurationDatabaseResponse : databases) {
					createEntityForDatabase(cmAuditConfigurationDatabaseResponse, auditSetting, defaultStatus);
				}
			}
		}

		return configCheckEvents;
	}

	private void createEntityForServer(int auditSetting, int defaultStatus, CMAuditConfigurationResponse currSerData) {
		ConfigurationServerCheckEvents configEventData;
		configEventData = new ConfigurationServerCheckEvents();
		configEventData.setServId(currSerData.getSrvId());
		configEventData.setInstance(currSerData.getInstance());
		configEventData.setDB(false);
		configEventData.setAgentVersion(currSerData.getAgentVersion());
		List<DataRow> entities = new ArrayList<DataRow>();
		boolean isChecked;
		DataRow dr;
		/**
		 * AuditedEvents
		 */
		if (!(auditSetting != 0 && auditSetting != 1 && auditSetting != 2 && auditSetting != 3 && auditSetting != 4
				&& auditSetting != 5 && auditSetting != 6 && auditSetting != 9)) {
			dr = new DataRow();
			dr.setName("Audited Activities");
			dr.setIsHeader(true);
			entities.add(dr);

			if (auditSetting == 0 || auditSetting == 1) {
				dr = new DataRow();
				dr.setName("Logins");
				isChecked = getBool(currSerData.getAuditLogins());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditLogins")));
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 2) {
				dr = new DataRow();
				dr.setName("Logouts");
				isChecked = getBool(currSerData.getAuditLogouts());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditLogouts")));
				dr.setIsCheckBox(true);
				dr.setIsGreyed((currSerData.getAuditLogins() == 0) ? true : false);

				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 3) {
				dr = new DataRow();
				dr.setName("Failed Logins");
				isChecked = getBool(currSerData.getAuditFailedLogins());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditFailedLogins")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 4) {
				dr = new DataRow();
				dr.setName("Security Changes (e.g. GRANT, REVOKE, LOGIN CHANGE PWD)");
				isChecked = getBool(currSerData.getAuditSecurity());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditSecurity")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}
			
			if (auditSetting == 0 || auditSetting == 5) {
				dr = new DataRow();
				dr.setName("Administrative Actions (e.g. DBCC)");
				isChecked = getBool(currSerData.getAuditAdmin());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditAdmin")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 6) {
				dr = new DataRow();
				dr.setName("Database Definition (DDL)(e.g. CREATE or DROP DATABASE)");
				isChecked = getBool(currSerData.getAuditDDL());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditDDL")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 9) {
				dr = new DataRow();
				dr.setName("User Defined Events (custom SQL Server event type)");
				isChecked = getBool(currSerData.getAuditUDE());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(serverDefaults.get("auditUDE")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}
		}
		boolean val, isSame;
		boolean isGreyed;

		// Access check filter
		if (!((auditSetting != 0 && auditSetting != 10 && auditSetting != 11)
				|| ((currSerData.getAuditFailures() != Integer.parseInt(serverDefaults.get("auditFailures")))
						&& defaultStatus == 1)
				|| ((currSerData.getAuditFailures() == Integer.parseInt(serverDefaults.get("auditFailures")))
						&& defaultStatus == 2))) {
			dr = new DataRow();
			dr.setName("Access Check Filter");
			dr.setIsHeader(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 10 || auditSetting == 11) {
			dr = new DataRow();
			dr.setName("Filter events based on access check");
			val = currSerData.getAuditFailures() == 0 || currSerData.getAuditFailures() == 2;
			isGreyed = currSerData.getAuditFailures() == 1;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			dr.setIsSame(currSerData.getAuditFailures() == Integer.parseInt(serverDefaults.get("auditFailures")));
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 10) {
			dr = new DataRow();
			dr.setName("Passed");
			if (currSerData.getAuditFailures() == 0 || currSerData.getAuditFailures() == 1) {
				val = true;
			} else
				val = false;
			isGreyed = false;
			if (currSerData.getAuditFailures() == 1)
				isGreyed = true;
			else
				isGreyed = false;

			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			dr.setIsSame(currSerData.getAuditFailures() == Integer.parseInt(serverDefaults.get("auditFailures")));
			dr.setIsCheckBox(false);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 11) {
			dr = new DataRow();
			dr.setName("Failed");
			if (currSerData.getAuditFailures() == 2) {
				val = true;
			} else
				val = false;
			if (currSerData.getAuditFailures() == 1)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			isSame = currSerData.getAuditFailures() == Integer.parseInt(serverDefaults.get("auditFailures"));
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);
		}
		
		/**
		 * //Capture DML and Select activiesisAuditLogEnabled
		 */

		if ((auditSetting == 0 || auditSetting == 12) && ((defaultStatus == 0) || (defaultStatus == 1 && ((getBool(
				currSerData.getAuditCaptureSqlXE()) == getBool(serverDefaults.get("auditCaptureSQLXE")))
				&& (getBool(currSerData.getIsAuditLogEnabled()) == getBool(serverDefaults.get("isAuditLogEnabled")))))
				|| (defaultStatus == 2 && !((getBool(currSerData.getAuditCaptureSqlXE()) == getBool(
						serverDefaults.get("auditCaptureSQLXE")))
						&& (getBool(currSerData.getIsAuditLogEnabled()) == getBool(
								serverDefaults.get("isAuditLogEnabled"))))))) {
			dr = new DataRow();
			dr.setName("Capture DML and SELECT Activities");
			dr.setIsHeader(true);
			entities.add(dr);
		}

		if ((auditSetting == 0 || auditSetting == 12)) {
			dr = new DataRow();
			dr.setName("Via Trace Events");
			isSame = true;
			if (currSerData.getAuditCaptureSqlXE() == 0 && currSerData.getIsAuditLogEnabled() == 0) {
				val = true;
			} else
				val = false;

			dr.setChecked(val);
			if ((getBool(currSerData.getAuditCaptureSqlXE()) == getBool(serverDefaults.get("auditCaptureSQLXE")))
					&& (getBool(currSerData.getIsAuditLogEnabled()) == getBool(
							serverDefaults.get("isAuditLogEnabled"))))
				isSame = true;
			else
				isSame = false;

			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Via Extended Events");
			if (currSerData.getAuditCaptureSqlXE() == 1) {
				val = true;
			} else
				val = false;

			dr.setChecked(val);
			dr.setIsSame(val == getBool(serverDefaults.get("auditCaptureSQLXE")));
			dr.setIsCheckBox(false);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Via SQL Server Audit Specification");
			if (currSerData.getAuditCaptureSqlXE() == 0 && currSerData.getIsAuditLogEnabled() == 1) {
				val = true;
			} else
				val = false;
			dr.setChecked(val);
			if ((getBool(currSerData.getAuditCaptureSqlXE()) == getBool(serverDefaults.get("auditCaptureSQLXE")))
					&& getBool(currSerData.getIsAuditLogEnabled()) == getBool(serverDefaults.get("isAuditLogEnabled")))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

		}

		// Priviledged user Auditing
		if (!(auditSetting == 12 || auditSetting == 24 || auditSetting == 17 || auditSetting == 18 || auditSetting == 19
				|| auditSetting == 20 || auditSetting == 21 || auditSetting == 22 || auditSetting == 23)) {
			dr = new DataRow();
			dr.setName("Privileged User Auditing");
			dr.setIsHeader(true);
			entities.add(dr);
			dr = new DataRow();
			dr.setName("Audited Activity");
			dr.setIsHeader(true);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit all activities done by privileged users");
			if (currSerData.getAuditUserAll() == 1 && currSerData.getAuditUsersList() != null
					&& !currSerData.getAuditUsersList().isEmpty()) {
				val = true;
			} else
				val = false;
			if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty())
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currSerData.getAuditUserAll()) == (getBool(serverDefaults.get("auditUserAll"))))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit selected activities done by privileged users");
			if (currSerData.getAuditUserLogins() == 0) {
				val = true;
			} else
				val = false;
			if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
					|| currSerData.getAuditUserAll() == 1)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);

			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);
			configEventData.setRowEntities(entities);

			if (auditSetting == 0 || auditSetting == 1 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Logins");
				if (currSerData.getAuditUserLogins() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserLogins()) == (getBool(serverDefaults.get("auditUserLogins"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 2 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Logouts");
				if (currSerData.getAuditUserLogouts() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserLogouts()) == (getBool(serverDefaults.get("auditUserLogouts"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 3 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Failed Logins");
				if (currSerData.getAuditUserFailedLogins() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData
						.getAuditUserFailedLogins()) == (getBool(serverDefaults.get("auditUserFailedLogins"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 4 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Security Changes");
				if (currSerData.getAuditUserSecurity() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserSecurity()) == (getBool(serverDefaults.get("auditUserSecurity"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 5 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Administrative Actions");
				if (currSerData.getAuditUserAdmin() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserAdmin()) == (getBool(serverDefaults.get("auditUserAdmin"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 6 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database Definition (DDL)");
				if (currSerData.getAuditUserDDL() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserDDL()) == (getBool(serverDefaults.get("auditUserDDL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 7 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database Modification (DML)");
				if (currSerData.getAuditUserDML() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);

				if (getBool(currSerData.getAuditUserDML()) == (getBool(serverDefaults.get("auditUserDML"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 8 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database SELECT operations");
				if (currSerData.getAuditUserSELECT() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserSELECT()) == (getBool(serverDefaults.get("auditUserSELECT"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 9 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("User Defined Events");
				if (currSerData.getAuditUserUDE() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData.getAuditUserUDE()) == (getBool(serverDefaults.get("auditUserUDE"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 10 || auditSetting == 16 || auditSetting == 11) {
				dr = new DataRow();
				dr.setName("Filter events based on access check");
				val = currSerData.getAuditUserFailures() == 0 || currSerData.getAuditUserFailures() == 2;
				isGreyed = currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				isSame = currSerData.getAuditUserFailures() == Integer
						.parseInt(serverDefaults.get("auditUserFailures"));
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);

				if (auditSetting == 0 || auditSetting == 10) {
					dr = new DataRow();
					dr.setName("Passed");
					val = currSerData.getAuditUserFailures() == 0 || currSerData.getAuditUserFailures() == 1;
					isGreyed = currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
							|| currSerData.getAuditUserAll() == 1 || currSerData.getAuditUserFailures() == 1;
					dr.setChecked(val);
					dr.setIsGreyed(isGreyed);
					isSame = currSerData.getAuditUserFailures() == Integer
							.parseInt(serverDefaults.get("auditUserFailures"));
					dr.setIsSame(isSame);
					dr.setIsCheckBox(false);
					entities.add(dr);
				}

				if (auditSetting == 0 || auditSetting == 11) {
					dr = new DataRow();
					dr.setName("Failed");
					val = currSerData.getAuditUserFailures() == 2;
					isGreyed = currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
							|| currSerData.getAuditUserAll() == 1;
					dr.setChecked(val);
					dr.setIsGreyed(isGreyed);
					isSame = currSerData.getAuditUserFailures() == Integer
							.parseInt(serverDefaults.get("auditUserFailures"));
					dr.setIsSame(isSame);
					dr.setIsCheckBox(false);
					entities.add(dr);
				}
			}
			if (auditSetting == 0 || auditSetting == 16 || auditSetting == 13) {
				dr = new DataRow();
				dr.setName("Capture SQL statements for DML and SELECT activities");
				if (currSerData.getAuditUserCaptureSQL() != 0) {
					val = true;
				} else
					val = false;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1 || currSerData.getAuditUserDML() == 0
						|| currSerData.getAuditUserSELECT() == 0)
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(
						currSerData.getAuditUserCaptureSQL()) == (getBool(serverDefaults.get("auditUserCaptureSQL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 16 || auditSetting == 14) {
				dr = new DataRow();
				dr.setName("Capture Transaction Status for DML Activity");
				if (currSerData.getAuditUserCaptureTrans() != 0) {
					val = true;
				} else
					val = false;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserDML() == 0 || currSerData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currSerData
						.getAuditUserCaptureTrans()) == (getBool(serverDefaults.get("auditUserCaptureTrans"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 16 || auditSetting == 15) {
				dr = new DataRow();
				dr.setName("Capture SQL statements for DDL and Security Changes");
				if (currSerData.getAuditUserCaptureDDL() == 0) {
					val = false;
				} else
					val = true;
				if (currSerData.getAuditUsersList() == null || currSerData.getAuditUsersList().isEmpty()
						|| currSerData.getAuditUserAll() == 1
						|| (currSerData.getAuditUserDDL() == 0 && currSerData.getAuditUserSecurity() == 0))
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(
						currSerData.getAuditUserCaptureDDL()) == (getBool(serverDefaults.get("auditUserCaptureDDL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 24) {
				dr = new DataRow();
				dr.setName("Trusted Users");
				if (currSerData.getAuditTrustedUsersList() != null
						&& currSerData.getAuditTrustedUsersList().isEmpty()) {
					val = false;
				} else
					val = true;
				dr.setChecked(val);
				if (currSerData.getAuditTrustedUsersList() == null
						|| currSerData.getAuditTrustedUsersList().isEmpty()) {
					isSame = true;
				} else
					isSame = false;
				// isSame= val;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}
		}

		configEventData.setRowEntities(entities);
		configCheckEvents.add(configEventData);

	}

	private void createEntityForDatabase(CMAuditConfigurationDatabaseResponse currDbData, int auditSetting,
			int defaultStatus) {

		ConfigurationServerCheckEvents configEventData = new ConfigurationServerCheckEvents();
		configEventData.setServId(currDbData.getSrvId());
		configEventData.setInstance(currDbData.getInstance());
		configEventData.setDB(true);
		configEventData.setAgentVersion(currDbData.getAgentVersion());
		configEventData.setDataBase(currDbData.getName());
		configEventData.setDatabaseId(currDbData.getSqlDatabaseId());

		List<DataRow> entities = new ArrayList<DataRow>();
		// AuditedEvents
		DataRow dr;
		boolean val;
		boolean isGreyed = false;
		boolean isSame;
		boolean isChecked;

		/**
		 * Audited Activities
		 */
		if (!(auditSetting != 0 && auditSetting != 8 && auditSetting != 4 && auditSetting != 5 && auditSetting != 6
				&& auditSetting != 7)) {
			dr = new DataRow();
			dr.setName("Audited Activities");
			dr.setIsHeader(true);
			entities.add(dr);

			if (auditSetting == 0 || auditSetting == 4) {
				dr = new DataRow();
				dr.setName("Security Changes");
				if (currDbData.getAuditSecurity() == 0) {
					val = false;
				} else
					val = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditSecurity()) == (getBool(dbDefaults.get("auditSecurity"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 5) {
				dr = new DataRow();
				dr.setName("Administrative Actions");
				if (currDbData.getAuditAdmin() == 0) {
					val = false;
				} else
					val = true;
				dr.setChecked(val);
				if (getBool(currDbData.getAuditAdmin()) == (getBool(dbDefaults.get("auditAdmin"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 6) {
				dr = new DataRow();
				dr.setName("Database Definition (DDL)");
				isChecked = getBool(currDbData.getAuditDDL());
				dr.setChecked(isChecked);
				dr.setIsSame(isChecked == getBool(dbDefaults.get("auditDDL")));
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 7) {
				dr = new DataRow();
				dr.setName("Database Modification (DML)");
				if (currDbData.getAuditDML() == 0) {
					val = false;
				} else
					val = true;
				dr.setChecked(val);
				if (getBool(currDbData.getAuditDML()) == (getBool(dbDefaults.get("auditDML"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 8) {
				dr = new DataRow();
				dr.setName("Database SELECT operations");
				isChecked = (currDbData.getAuditSELECT() == 0) ? false : true;
				dr.setChecked(isChecked);
				dr.setIsSame((getBool(currDbData.getAuditSELECT()) == getBool(dbDefaults.get("auditSELECT"))) ? true
						: false);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}
		}
		/**
		 * Access Check Filter
		 */
		if (!((auditSetting != 0 && auditSetting != 10 && auditSetting != 11)
				|| ((currDbData.getAuditFailures() != Integer.parseInt(dbDefaults.get("auditFailures")))
						&& defaultStatus == 1)
				|| ((currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures")))
						&& defaultStatus == 2))) {
			dr = new DataRow();
			dr.setName("Access Check Filter");
			dr.setIsHeader(true);
			entities.add(dr);
		}
		if (auditSetting == 0 || auditSetting == 10 || auditSetting == 11) {
			dr = new DataRow();
			dr.setName("Filter events based on access check");
			val = currDbData.getAuditFailures() == 0 || currDbData.getAuditFailures() == 2;
			dr.setChecked(val);
			isGreyed = currDbData.getAuditFailures() == 1;
			dr.setIsGreyed(isGreyed);
			dr.setIsSame(currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures")));
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 10) {
			dr = new DataRow();
			dr.setName("Passed");
			if (currDbData.getAuditFailures() == 0 || currDbData.getAuditFailures() == 1) {
				val = true;
			} else
				val = false;
			isGreyed = false;
			if (currDbData.getAuditFailures() == 1)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			dr.setIsSame(currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures")));
			dr.setIsCheckBox(false);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 11) {
			dr = new DataRow();
			dr.setName("Failed");
			if (currDbData.getAuditFailures() == 2) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditFailures() == 1)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			isSame = currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures"));
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 13) {
			dr = new DataRow();
			dr.setName("Capture SQL statements for DML and SELECT activities");
			if (currDbData.getAuditCaptureSql() == 0) {
				val = false;
			} else
				val = true;
			if (currDbData.getAuditDML() == 0 && currDbData.getAuditSELECT() == 0)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditCaptureSql()) == (getBool(dbDefaults.get("auditCaptureSQL"))))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 14) {
			dr = new DataRow();
			dr.setName("Capture Transaction Status for DML Activity");
			if (currDbData.getAuditCaptureTrans() == 0) {
				val = false;
			} else
				val = true;
			if (currDbData.getAuditDML() == 0)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditCaptureTrans()) == (getBool(dbDefaults.get("auditCaptureTrans"))))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 15) {
			dr = new DataRow();
			dr.setName("Capture SQL statements for DDL and Security Changes");
			val = (currDbData.getAuditCaptureDDL() == 0) ? false : true;
			isGreyed = (currDbData.getAuditDDL() == 0 && currDbData.getAuditSecurity() == 0) ? true : false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditCaptureDDL()) == (getBool(dbDefaults.get("auditCaptureDDL"))))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		// Previledged user Auditing
		if (!(auditSetting == 12 || auditSetting == 21 || auditSetting == 17 || auditSetting == 18 || auditSetting == 19
				|| auditSetting == 20)) {
			dr = new DataRow();
			dr.setName("Privileged User Auditing");
			dr.setIsHeader(true);
			entities.add(dr);
			dr = new DataRow();
			dr.setName("Audited Activity");
			dr.setIsHeader(true);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit all activities done by privileged users");
			if (currDbData.getAuditUserAll() == 1 && currDbData.getAuditPrivUsersList() != null
					&& !currDbData.getAuditPrivUsersList().isEmpty()) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty())
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditUserAll()) == (getBool(dbDefaults.get("auditUserAll"))))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit selected activities done by privileged users");
			if (currDbData.getAuditPrivUsersList() != null && !currDbData.getAuditPrivUsersList().isEmpty()
					&& currDbData.getAuditUserAll() != 1) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
					|| currDbData.getAuditUserAll() == 1)
				isGreyed = true;
			else
				isGreyed = false;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			if (auditSetting == 0 || auditSetting == 1 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Logins");
				if (currDbData.getAuditUserLogins() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserLogins()) == (getBool(dbDefaults.get("auditUserLogins"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 2 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Logouts");
				if (currDbData.getAuditLogouts() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserLogouts()) == (getBool(dbDefaults.get("auditUserLogouts"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 3 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Failed Logins");
				if (currDbData.getAuditUserFailedLogins() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(
						currDbData.getAuditUserFailedLogins()) == (getBool(dbDefaults.get("auditUserFailedLogins"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 4 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Security Changes");
				if (currDbData.getAuditUserSecurity() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserSecurity()) == (getBool(dbDefaults.get("auditUserSecurity"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 5 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Administrative Actions");
				if (currDbData.getAuditUserAdmin() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserAdmin()) == (getBool(dbDefaults.get("auditUserAdmin"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 6 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database Definition (DDL)");
				if (currDbData.getAuditUserDDL() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserDDL()) == (getBool(dbDefaults.get("auditUserDDL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 7 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database Modification (DML)");
				if (currDbData.getAuditUserDML() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);

				if (getBool(currDbData.getAuditUserDML()) == (getBool(dbDefaults.get("auditUserDML"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 8 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Database SELECT operations");
				if (currDbData.getAuditUserSELECT() == 0) {
					val = false;
				} else
					val = true;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserSELECT()) == (getBool(dbDefaults.get("auditUserSELECT"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 9 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("User Defined Events");
				isChecked = getBool(currDbData.getAuditUDE());
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setIsGreyed(isGreyed);
				dr.setChecked(isChecked);
				dr.setIsSame(getBool(currDbData.getAuditUserUDE()) == getBool(dbDefaults.get("auditUserUDE")));
				dr.setIsCheckBox(true);
				// set disabled
				entities.add(dr);
			}

			if ((auditSetting == 0 || auditSetting == 16 || auditSetting == 10 || auditSetting == 11)
					&& ((currDbData.getAuditUserFailures() == Integer.parseInt(dbDefaults.get("auditUserFailures"))
							&& defaultStatus == 1)
							|| (currDbData.getAuditUserFailures() != Integer
									.parseInt(dbDefaults.get("auditUserFailures")) && defaultStatus == 2)
							|| defaultStatus == 0)) {
				dr = new DataRow();
				dr.setName("Filter events based on access check");
				val = currDbData.getAuditUserFailures() == 0 || currDbData.getAuditUserFailures() == 2;
				isGreyed = currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				isSame = currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures"));
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);

				if (auditSetting == 0 || auditSetting == 10) {
					dr = new DataRow();
					dr.setName("Passed");
					val = currDbData.getAuditUserFailures() == 0 || currDbData.getAuditUserFailures() == 1;
					isGreyed = currDbData.getAuditPrivUsersList() == null
							|| currDbData.getAuditPrivUsersList().isEmpty() || currDbData.getAuditUserAll() == 1
							|| currDbData.getAuditUserFailures() == 1;
					dr.setChecked(val);
					dr.setIsGreyed(isGreyed);
					isSame = currDbData.getAuditFailures() == Integer.parseInt(dbDefaults.get("auditFailures"));
					dr.setIsSame(isSame);
					dr.setIsCheckBox(false);
					entities.add(dr);
				}

				if (auditSetting == 0 || auditSetting == 11) {
					dr = new DataRow();
					dr.setName("Failed");
					val = currDbData.getAuditUserFailures() == 2;
					isGreyed = currDbData.getAuditPrivUsersList() == null
							|| currDbData.getAuditPrivUsersList().isEmpty() || currDbData.getAuditUserAll() == 1;
					dr.setChecked(val);
					dr.setIsGreyed(isGreyed);
					isSame = currDbData.getAuditUserFailures() == Integer.parseInt(dbDefaults.get("auditUserFailures"));
					dr.setIsSame(isSame);
					dr.setIsCheckBox(false);
					entities.add(dr);
				}
			}
			if (auditSetting == 0 || auditSetting == 13 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Capture SQL Statements for DML and SELECT activities");
				if (currDbData.getAuditUserCaptureSQL() == 1) {
					val = true;
				} else
					val = false;
				if ((currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
						|| (currDbData.getAuditUserDML() == 0 && currDbData.getAuditUserSELECT() == 0))
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserCaptureSQL()) == (getBool(dbDefaults.get("auditUserCaptureSQL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 14 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Capture Transaction Status for DML Activity");
				if (currDbData.getAuditUserCaptureTrans() == 1) {
					val = true;
				} else
					val = false;

				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1 /*
																 * || currDbData.getAuditUserDML() ==0 ||
																 * currDbData.getAuditUserSELECT() == 0
																 */)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(
						currDbData.getAuditUserCaptureTrans()) == (getBool(dbDefaults.get("auditUserCaptureTrans"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 15 || auditSetting == 16) {
				dr = new DataRow();
				dr.setName("Capture SQL Statements for DDL and Security Changes");
				if (currDbData.getAuditUserCaptureDDL() == 2) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditPrivUsersList() == null || currDbData.getAuditPrivUsersList().isEmpty()
						|| currDbData.getAuditUserAll() == 1)
					isGreyed = true;
				else
					isGreyed = false;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserCaptureDDL()) == (getBool(dbDefaults.get("auditUserCaptureDDL"))))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}
		}

		if (auditSetting == 0 || auditSetting == 21 || auditSetting == 17 || auditSetting == 18 || auditSetting == 19
				|| auditSetting == 20) {
			dr = new DataRow();
			dr.setName("DML/SELECT Filters");
			dr.setIsHeader(true);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit all database objects");
			if (currDbData.getAuditDMLAll() == 1) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
				isGreyed = false;
			else
				isGreyed = true;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditDMLAll()))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			dr = new DataRow();
			dr.setName("Audit the following database objects");
			if (currDbData.getAuditDMLAll() == 0) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
				isGreyed = false;
			else
				isGreyed = true;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (getBool(currDbData.getAuditDMLAll()))
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(false);
			entities.add(dr);

			if ((auditSetting == 0 || auditSetting == 17 || auditSetting == 18)
					&& ((getBool(currDbData.getAuditUserTables()) && defaultStatus == 1)
							|| (!getBool(currDbData.getAuditUserTables()) && defaultStatus == 2)
							|| (defaultStatus == 0))) {
				dr = new DataRow();
				dr.setName("User Tables");
				dr.setIsHeader(true);
				entities.add(dr);
			}
			if (auditSetting == 0 || auditSetting == 18 || auditSetting == 17) {
				dr = new DataRow();
				dr.setName("Audit all user tables");
				if (currDbData.getAuditUserTables() == 1) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserTables()))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(false);
				entities.add(dr);

				dr = new DataRow();
				dr.setName("Audit selected user tables");
				if (currDbData.getAuditUserTables() == 2) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserTables()))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(false);
				entities.add(dr);

				dr = new DataRow();
				dr.setName("Don't audit user tables");
				if (currDbData.getAuditUserTables() == 0) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (getBool(currDbData.getAuditUserTables()))
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(false);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 19 || auditSetting == 17) {
				dr = new DataRow();
				dr.setName("Audit system tables");
				if (currDbData.getAuditSystemTables() == 1) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (currDbData.getAuditSystemTables() == 0)
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 20 || auditSetting == 17) {
				dr = new DataRow();
				dr.setName("Audit stored procedures");
				if (currDbData.getAuditStoredProcedures() == 1) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (currDbData.getAuditStoredProcedures() == 0)
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}

			if (auditSetting == 0 || auditSetting == 21 || auditSetting == 17) {
				dr = new DataRow();
				dr.setName("Audit all other object types (views, indexes, etc.)");
				if (currDbData.getAuditDMLOther() == 1) {
					val = true;
				} else
					val = false;
				if (currDbData.getAuditDML() == 1 || currDbData.getAuditSELECT() == 1)
					isGreyed = false;
				else
					isGreyed = true;
				dr.setChecked(val);
				dr.setIsGreyed(isGreyed);
				if (currDbData.getAuditDMLOther() == 0)
					isSame = true;
				else
					isSame = false;
				dr.setIsSame(isSame);
				dr.setIsCheckBox(true);
				entities.add(dr);
			}
		}
		if (auditSetting == 0 || auditSetting == 22) {
			dr = new DataRow();
			dr.setName("Before After Data");
			if (currDbData.getAuditDataChanges() == 1) {
				val = true;
			} else
				val = false;
			if (currDbData.getAuditDML() == 1)
				isGreyed = false;
			else
				isGreyed = true;
			dr.setChecked(val);
			dr.setIsGreyed(isGreyed);
			if (currDbData.getAuditDataChanges() == 0)
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 23) {
			dr = new DataRow();
			dr.setName("Sensitive Columns");
			if (currDbData.getAuditSensitiveColumns() == 1) {
				val = true;
			} else
				val = false;
			dr.setChecked(val);
			if (currDbData.getAuditSensitiveColumns() == 0)
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		if (auditSetting == 0 || auditSetting == 24) {
			dr = new DataRow();
			dr.setName("Trusted Users");
			if (currDbData.getAuditUsersList() != null && !currDbData.getAuditUsersList().isEmpty()) {
				val = true;
			} else
				val = false;
			dr.setChecked(val);
			if (currDbData.getAuditUsersList() == null || currDbData.getAuditUsersList().isEmpty())
				isSame = true;
			else
				isSame = false;
			dr.setIsSame(isSame);
			dr.setIsCheckBox(true);
			entities.add(dr);
		}

		configEventData.setRowEntities(entities);
		configCheckEvents.add(configEventData);
	}

	private DataRow getDataRowObj(int flagValue, boolean defaultValue) {
		DataRow dr = new DataRow();
		boolean val = (flagValue == 0) ? false : true;
		dr.setChecked(val);
		dr.setIsSame(val == defaultValue ? true : false);
		return dr;
	}

	private boolean getBool(int val) {
		if (val == 0)
			return false;
		else
			return true;
	}

	private boolean getBool(String boolString) {
		if (boolString.equalsIgnoreCase("true"))
			return true;
		else
			return false;
	}

}
