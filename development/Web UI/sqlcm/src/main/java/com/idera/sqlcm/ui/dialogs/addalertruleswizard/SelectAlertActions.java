package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import org.zkoss.zul.Spinner;
import org.zkoss.zul.Textbox;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SelectAlertActions {
	
	@JsonProperty("emailAddress")
    private String emailAddress;
        
    @JsonProperty("eventLogEntry")
    private String eventLogEntry;
    
    @JsonProperty("address")
    private String address;
    
    @JsonProperty("port")
    private int port;
    
    @JsonProperty("community")
    private String community;
        
    @JsonProperty("snmpTrap")
    private boolean snmpTrap;
    
    @JsonProperty("message")
    private String message;
    
    @JsonProperty("snmpServerAddress")
    private String snmpServerAddress;
    
    public String getSnmpServerAddress() {
    	return snmpServerAddress;
	}

	public void setSnmpServerAddress(String snmpServerAddress) {
		this.snmpServerAddress = snmpServerAddress;
	}

	public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	@JsonProperty("emailMessage")
    private boolean emailMessage;
    
    @JsonProperty("logMessage")
    private boolean logMessage;
    
    public boolean isEmailMessage() {
		return emailMessage;
	}

	public void setEmailMessage(boolean emailMessage) {
		this.emailMessage = emailMessage;
	}

	public boolean isLogMessage() {
		return logMessage;
	}

	public void setLogMessage(boolean logMessage) {
		this.logMessage = logMessage;
	}

	public boolean isSnmpTrap() {
		return snmpTrap;
	}

	public void setSnmpTrap(boolean snmpTrap) {
		this.snmpTrap = snmpTrap;
	}
	
    public String getEmailAddress() {
        return emailAddress;
    }

    public void setEmailAddress(String txtEmailAddress) {
        this.emailAddress = txtEmailAddress;
    }
    
    public String getEventLogEntry() {
        return eventLogEntry;
    }

    public void setEventLogEntry(String eventLogEntry) {
        this.eventLogEntry = eventLogEntry;
    }
    
    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
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
}
