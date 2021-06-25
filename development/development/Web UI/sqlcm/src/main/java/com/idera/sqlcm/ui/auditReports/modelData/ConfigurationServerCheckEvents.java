package com.idera.sqlcm.ui.auditReports.modelData;

import java.util.List;

public class ConfigurationServerCheckEvents {
		boolean isDB;
		int servId;
		String instance;
		String agentVersion;
		List<DataRow> rowEntities;
		String DataBase;
		int DatabaseId;
		public boolean getIsDB() {
			return isDB;
		}
		public void setDB(boolean isDB) {
			this.isDB = isDB;
		}
		public int getServId() {
			return servId;
		}
		public void setServId(int servId) {
			this.servId = servId;
		}
		public String getInstance() {
			return instance;
		}
		public void setInstance(String instance) {
			this.instance = instance;
		}
		public String getAgentVersion() {
			return agentVersion;
		}
		public void setAgentVersion(String agentVersion) {
			this.agentVersion = agentVersion;
		}
		public List<DataRow> getRowEntities() {
			return rowEntities;
		}
		public void setRowEntities(List<DataRow> rowEntities) {
			this.rowEntities = rowEntities;
		}
		public String getDataBase() {
			return DataBase;
		}
		public void setDataBase(String dataBase) {
			DataBase = dataBase;
		}
		public int getDatabaseId() {
			return DatabaseId;
		}
		public void setDatabaseId(int databaseId) {
			DatabaseId = databaseId;
		}
		
		
	
}
