package com.idera.sqlcm.entities;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.idera.sqlcm.ui.converter.DateConverter;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InstanceStatus extends CMEntity {

	private int eventCategories;
	private int collectedEventCount;
	private int databaseCount;
	private CMStatus agentStatus;
	private String message;
	private Date lastHeartbeat;
	private DateConverter dateConverter = new DateConverter();

	public int getEventCategories() {
		return eventCategories;
	}

	public void setEventCategories(int eventCategories) {
		this.eventCategories = eventCategories;
	}

	public int getCollectedEventCount() {
		return collectedEventCount;
	}

	public void setCollectedEventCount(int collectedEventCount) {
		this.collectedEventCount = collectedEventCount;
	}

	public int getDatabaseCount() {
		return databaseCount;
	}

	public void setDatabaseCount(int databasesCount) {
		this.databaseCount = databasesCount;
	}

	public CMStatus getAgentStatus() {
		return agentStatus;
	}

	public void setAgentStatus(CMStatus agentStatus) {
		this.agentStatus = agentStatus;
	}

	public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	public Date getLastHeartbeat() {
		return lastHeartbeat;
	}

	/**
	 * @return the dateConverter
	 */
	public DateConverter getDateConverter() {
		return dateConverter;
	}

	/**
	 * @param dateConverter the dateConverter to set
	 */
	public void setDateConverter(DateConverter dateConverter) {
		this.dateConverter = dateConverter;
	}

	public void setLastHeartbeat(Date time) {
		this.lastHeartbeat = time;
	}

}
