package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

//@JsonIgnoreProperties(ignoreUnknown = true)
public abstract class CMEntity {

	public enum EntityType {
		All,
		Audited,
		Registered;
	}

    // TODO IR can remove ???
    @Deprecated // use com.idera.sqlcm.ui.BaseViewModel.Category
	public enum EventStatCategories {
		Overall_Activity,
		AuditedInstance,
		AuditedDatabase,
		ProccessedEvents,
		Alerts,
		PrivUserEvents,
		FailedLogin,
		UserDefinedEvents,
		Admin,
		DDL,
		Security,
		DML,
		Insert,
		Update,
		Delete,
		Select,
		Logins,
		HdSpace,
		IntegrityCheck,
		Execute,
		EventReceived,
		EventProcessed,
		EventFiltered,
		MaxValue, Category;
	}

	public enum InstanceStatus {
		Ok(0),
		Disabled(1),
		Down(2),
		Error(3),
		Slow(4),
		Up(5),
		Unknown(6);

        private long id;

        InstanceStatus(long id) {
            this.id = id;
        }

        public long getId() {
            return id;
        }

        public static InstanceStatus getById(long id) {
            for(InstanceStatus e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return Unknown;
        }

    }

	public enum CMStatus {
		Unknown,
		Low,
		Medium,
		High,
		Severe,
		Ok,
		Informational,
		Warning,
		Critical;
	}

	public enum RuleType {
		None,
		Event,
		Status,
		Data,
		All;
	}

	public enum NodeType {
		Root,
		Server,
		Database,
		Table,
		Column;
	}

    @JsonProperty("id")
	protected long id;
    
 	@JsonProperty("name")
	protected String name;
 	
 	protected boolean boolType;

	public boolean isBoolType() {
		return boolType;
	}

	public void setBoolType(boolean boolType) {
		this.boolType = boolType;
	}

	public final long getId() {
		return id;
	}

	public final void setId(long id) {
		this.id = id;
	}

	public final String getName() {
		return name;
	}

	public final void setName(String name) {
		this.name = name;
	}

}
