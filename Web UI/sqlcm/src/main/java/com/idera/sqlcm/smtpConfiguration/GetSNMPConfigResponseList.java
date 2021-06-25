package com.idera.sqlcm.smtpConfiguration;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class GetSNMPConfigResponseList {
	
	    @JsonProperty("sender_email")
	    String SenderEmail;
	    
	    @JsonProperty("logs_permission")
	    Boolean LogsPermission;
	    
	    @JsonProperty("send_mail_permission")
	    Boolean SendMailPermission;
	    
	    @JsonProperty("SnmpCommunity")
	    String SnmpCommunity;
	    
	    @JsonProperty("snmp_permission")
	    Boolean SnmpPermission;
	    
	    @JsonProperty("SnmpPort")
	    int SnmpPort;
	    
	    @JsonProperty("SnmpServerAddress")
	    String SnmpServerAddress;
	    
	    @JsonProperty("severity")
	    int severity;
	    
	    @JsonProperty("messageData")
	    String messageData;

		public String getMessageData() {
			return messageData;
		}

		public void setMessageData(String messageData) {
			this.messageData = messageData;
		}

		public int getSeverity() {
			return severity;
		}

		public void setSeverity(int severity) {
			this.severity = severity;
		}

		public String getSenderEmail() {
			return SenderEmail;
		}

		public void setSenderEmail(String senderEmail) {
			SenderEmail = senderEmail;
		}

		public Boolean getLogsPermission() {
			return LogsPermission;
		}

		public void setLogsPermission(Boolean logsPermission) {
			LogsPermission = logsPermission;
		}

		public Boolean getSendMailPermission() {
			return SendMailPermission;
		}

		public void setSendMailPermission(Boolean sendMailPermission) {
			SendMailPermission = sendMailPermission;
		}

		public String getSnmpCommunity() {
			return SnmpCommunity;
		}

		public void setSnmpCommunity(String snmpCommunity) {
			SnmpCommunity = snmpCommunity;
		}

		public Boolean getSnmpPermission() {
			return SnmpPermission;
		}

		public void setSnmpPermission(Boolean snmpPermission) {
			SnmpPermission = snmpPermission;
		}

		public int getSnmpPort() {
			return SnmpPort;
		}

		public void setSnmpPort(int snmpPort) {
			SnmpPort = snmpPort;
		}

		public String getSnmpServerAddress() {
			return SnmpServerAddress;
		}

		public void setSnmpServerAddress(String snmpServerAddress) {
			SnmpServerAddress = snmpServerAddress;
		}

		@Override
	    public String toString() {
	        return "GetSNMPConfigResponseList{" +
	            "SenderEmail=" + SenderEmail +
	            "LogsPermission=" + LogsPermission +
	            "SendMailPermission=" + SendMailPermission +
	            "SnmpCommunity=" + SnmpCommunity +
	            "SnmpPermission=" + SnmpPermission +
	            "SnmpPort=" + SnmpPort +
	            "SnmpServerAddress=" + SnmpServerAddress  +
	            "Severity=" + severity  +
	            "MessageData=" + messageData  +
	            '}';
	    }
}
