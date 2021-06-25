package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SelectDataAlertActions {
	
	@JsonProperty("emailNotification")
    private boolean emailNotification;
	
	@JsonProperty("emailAddress")
    private String emailAddress;
        
    @JsonProperty("windowEventLogEntry")
    private boolean windowEventLogEntry;
    
    @JsonProperty("eventLogEntry")
    private String eventLogEntry;
    
    @JsonProperty("snmpTrap")
    private boolean snmpTrap;
    
    @JsonProperty("address")
    private String address;
    
    @JsonProperty("port")
    private int port;
    
    @JsonProperty("community")
    private String community;
    
    @JsonProperty("message")
    private String message;
    
    public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	public String getEmailAddress() {
        return emailAddress;
    }

    public void setEmailAddress(String emailAddress) {
        this.emailAddress = emailAddress;
    }
   
    
    public String getEventLogEntry() {
        return eventLogEntry;
    }

    public void setEventLogEntry(String eventLogEntry) {
        this.eventLogEntry = eventLogEntry;
    }
    
    public boolean isEmailNotification() {
		return emailNotification;
	}

	public void setEmailNotification(boolean emailNotification) {
		this.emailNotification = emailNotification;
	}

	public boolean isWindowEventLogEntry() {
		return windowEventLogEntry;
	}

	public void setWindowEventLogEntry(boolean windowEventLogEntry) {
		this.windowEventLogEntry = windowEventLogEntry;
	}

	public boolean isSnmpTrap() {
		return snmpTrap;
	}

	public void setSnmpTrap(boolean snmpTrap) {
		this.snmpTrap = snmpTrap;
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
