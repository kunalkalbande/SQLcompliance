package com.idera.sqlcm.ui.dialogs;

import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class UpdateSNMPThresholdConfiguration {
	
		@JsonProperty("senderEmail")
	    String senderEmail;

		@JsonProperty("instanceName")
	    String instanceName;
	    
		@JsonProperty("snmpPermission")
	    boolean snmpPermission;
		
		@JsonProperty("logsPermission")
		boolean logsPermission;
		
		@JsonProperty("sendMailPermission")
		boolean sendMailPermission;
		
		//SQLCM-5.3.1 Start- Threshold notifications are not saved
		@JsonProperty("snmpServerAddress")
	    String snmpServerAddress;

		@JsonProperty("port")
	    int port;
	    
	    @JsonProperty("community")
	    String community;
	    
	    @JsonProperty("severity")
	    int severity;
	    
	    @JsonProperty("srvId")
	    long srvId;
	    
	    @JsonProperty("messageData")
	    String messageData;
	    
		public String getMessageData() {
			return messageData;
		}

		public void setMessageData(String messageData) {
			this.messageData = messageData;
		}

		public long getSrvId() {
			return srvId;
		}

		public void setSrvId(long srvId) {
			this.srvId = srvId;
		}

		public int getSeverity() {
			return severity;
		}

		public void setSeverity(int severity) {
			this.severity = severity;
		}

		public String getSnmpServerAddress() {
			return snmpServerAddress;
		}

		public void setSnmpServerAddress(String snmpServerAddress) {
			this.snmpServerAddress = snmpServerAddress;
		}

		public int getPort() {
			return port;
		}

		public void setPort(int port) {
			this.port = port;
		}

		public String getCommunity() {
			return community;
		}

		public void setCommunity(String community) {
			this.community = community;
		}
	    //End
	    
		public String getInstanceName() {
			return instanceName;
		}
		
	    public void setInstanceName(String instanceName) {
			this.instanceName = instanceName;
		}

	    public String getSenderEmail() {
			return senderEmail;
		}
	    
	    public void setSenderEmail(String senderEmail) {
			this.senderEmail = senderEmail;
		}
	    
	    public boolean getSnmpPermission() {
			return snmpPermission;
		}
	    
		public void setSnmpPermission(boolean snmpPermission) {
			this.snmpPermission = snmpPermission;
		}
		
		public boolean getLogsPermission() {
			return logsPermission;
		}
	    
		public void setLogsPermission(boolean logsPermission) {
			this.logsPermission = logsPermission;
		}
		
		public boolean getSendMailPermission() {
			return sendMailPermission;
		}
	  
		public void setSendMailPermission(boolean sendMailPermission) {
			this.sendMailPermission = sendMailPermission;
		}

	    @Override
	    public String toString() {
	        return "UpdateSNMPConfiguration{"+
	            "instance_name=" + instanceName +
	            ", sender_email=" + senderEmail +
	            ", send_mail_permission=" + sendMailPermission +
	            ", snmp_permission=" + snmpPermission +
	            ", logs_permission=" + logsPermission +
	            ", snmpServerAddress=" + snmpServerAddress +
	            ", port=" + port +
	            ", community=" + community +
	            ", severity=" + severity +
	            ", srvId=" + srvId +
	            ", messageData=" + messageData +
	            '}';
	    }
}

