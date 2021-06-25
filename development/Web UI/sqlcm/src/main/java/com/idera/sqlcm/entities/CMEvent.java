package com.idera.sqlcm.entities;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEvent extends CMEntity {

    @JsonProperty("databaseId")
    private long databaseId;

    @JsonProperty("databaseName")
    private String database;

    @JsonProperty("category")
	private String category;

    @JsonProperty("event")
	private String event;

    @JsonProperty("time")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date time;

    @JsonProperty("loginName")
    private String login;

    @JsonProperty("eventDatabase")
    private String eventDatabase;

    public long getDatabaseId() {
        return databaseId;
    }

    public void setDatabaseId(int databaseId) {
        this.databaseId = databaseId;
    }

    public String getDatabase() {
        return database;
    }

    public void setDatabase(String database) {
        this.database = database;
    }

    public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

    public Date getTime() {
        return time;
    }

    public void setTime(Date time) {
        this.time = time;
    }

    public String getLogin() {
        return login;
    }

    public void setLogin(String login) {
        this.login = login;
    }

	public String getEvent() {
		return event;
	}

	public void setEvent(String event) {
		this.event = event;
	}

    public String getEventDatabase() {
        return eventDatabase;
    }

    public void setEventDatabase(String eventDatabase) {
        this.eventDatabase = eventDatabase;
    }

    @Override
    public String toString() {
        return "CMEvent{" +
                "databaseId=" + databaseId +
                ", database='" + database + '\'' +
                ", category='" + category + '\'' +
                ", event='" + event + '\'' +
                ", time=" + time +
                ", login='" + login + '\'' +
                '}';
    }

}
