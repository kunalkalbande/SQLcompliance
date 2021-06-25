package com.idera.sqlcm.smtpConfiguration;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class UpdateSMTPConfiguration {

    @JsonProperty("smtpServer")
    String smtpServer;

	@JsonProperty("smtpPort")
    int smtpPort;
    
    @JsonProperty("smtpAuthType")
    int smtpAuthType;
    
    @JsonProperty("smtpSsl")
    int smtpSsl;
    
    @JsonProperty("smtpUsername")
    String smtpUsername;
    
    @JsonProperty("smtpPassword")
    String smtpPassword;
    
    @JsonProperty("smtpSenderAddress")
    String smtpSenderAddress;
    
    @JsonProperty("smtpSenderName")
    String smtpSenderName;
    
   
    public String getSmtpServer() {
		return smtpServer;
	}

	public void setSmtpServer(String smtpServer) {
		this.smtpServer = smtpServer;
	}

	public int getSmtpPort() {
		return smtpPort;
	}

	public void setSmtpPort(int smtpPort) {
		this.smtpPort = smtpPort;
	}

	public int getSmtpAuthType() {
		return smtpAuthType;
	}

	public void setSmtpAuthType(int smtpAuthType) {
		this.smtpAuthType = smtpAuthType;
	}

	public int getSmtpSsl() {
		return smtpSsl;
	}

	public void setSmtpSsl(int smtpSsl) {
		this.smtpSsl = smtpSsl;
	}

	public String getSmtpUsername() {
		return smtpUsername;
	}

	public void setSmtpUsername(String smtpUsername) {
		this.smtpUsername = smtpUsername;
	}

	public String getSmtpPassword() {
		return smtpPassword;
	}

	public void setSmtpPassword(String smtpPassword) {
		this.smtpPassword = smtpPassword;
	}

	public String getSmtpSenderAddress() {
		return smtpSenderAddress;
	}

	public void setSmtpSenderAddress(String smtpSenderAddress) {
		this.smtpSenderAddress = smtpSenderAddress;
	}

	public String getSmtpSenderName() {
		return smtpSenderName;
	}

	public void setSmtpSenderName(String smtpSenderName) {
		this.smtpSenderName = smtpSenderName;
	}

	@Override
    public String toString() {
        return "UpdateSMTPConfiguration{" +
            "smtpServer=" + smtpServer +
            ", smtpPort=" + smtpPort +
            ", smtpAuthType=" + smtpAuthType +
            ", smtpSsl=" + smtpSsl +
            ", smtpUsername=" + smtpUsername +
            ", smtpPassword=" + smtpPassword +
            ", smtpSenderAddress=" + smtpSenderAddress +
            ", smtpSenderName=" + smtpSenderName +
            '}';
    }
}