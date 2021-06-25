package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zul.ListModelList;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyObjectsViewModel.Objects;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyPrivilegedUserViewModel.PrivilegedUserName;

public class RegulationSettings {

	
    @JsonProperty("sqlServer")
    private boolean sqlServer;
    
    @JsonProperty("objectName")
    private boolean objectName;
    
    @JsonProperty("databaseName")
    private boolean databaseName;
    
    @JsonProperty("hostName")
    private boolean hostName;
    
    @JsonProperty("applicationName")
    private boolean applicationName;
    
    @JsonProperty("loginName")
    private boolean loginName;
    
    @JsonProperty("accessCheckPassed")
    private boolean accessCheckPassed;
    
    @JsonProperty("accessCheckPassedChk")
    private boolean accessCheckPassedChk;
    
    @JsonProperty("isPrivilegedUser")
    private boolean isPrivilegedUser;
    
    @JsonProperty("isPrivilegedCheck")
    private boolean isPrivilegedCheck;
    
    @JsonProperty("privilegedUserName")
    private boolean privilegedUserName;
    
	@JsonProperty("excludeCertainEventType")
    private boolean excludeCertainEventType;
    
    @JsonProperty("database")
    private List database;
    
    @JsonProperty("rowCountWithTimeInterval")
	private boolean rowCountWithTimeInterval;


    
    @JsonProperty("targetInstances")
	public Map<String , Object> targetInstances = new HashMap<String , Object>();
    
    @JsonProperty("dbNameList")
   	public ListModelList<Data> dbNameList = new ListModelList<>();
    
    @JsonProperty("objectNameList")
   	public ListModelList<Objects> objectNameList = new ListModelList<>();
   	
    @JsonProperty("hostNameList")
   	public ListModelList<Host> hostNameList = new ListModelList<>();
   	
    @JsonProperty("appNameList")
   	public ListModelList<App> appNameList = new ListModelList<>();
   	
    @JsonProperty("loginNameList")
   	public ListModelList<Login> loginNameList = new ListModelList<>();
    
   	@JsonProperty("PrivilegedUserNameList")
   	public ListModelList<PrivilegedUserName> privilegedUserNameList = new ListModelList<>();
   	
	@JsonProperty("dbRadioSelected")
    private String dbRadioSelected;
    
    @JsonProperty("objectRadioSelected")
    private String objectRadioSelected;
    
    @JsonProperty("hostRadioSelected")
    private String hostRadioSelected;
    
    @JsonProperty("appRadioSelected")
    private String appRadioSelected;
    
    @JsonProperty("loginRadioSelected")
    private String loginRadioSelected;
    
	@JsonProperty("rowCountMatchString")
	public String rowCountMatchString;

	@JsonProperty("rowCountFieldId")
	public int rowCountFieldId;

	public int getRowCountFieldId() {
		return rowCountFieldId;
	}

	public void setRowCountFieldId(int rowCountFieldId) {
		this.rowCountFieldId = rowCountFieldId;
	}
	public String getRowCountMatchString() {
		return rowCountMatchString;
	}

	public void setRowCountMatchString(String rowCountMatchString) {
		this.rowCountMatchString = rowCountMatchString;
	}
	
	public boolean isRowCountWithTimeInterval() {
		return rowCountWithTimeInterval;
	}

	public void setRowCountWithTimeInterval(boolean rowCountWithTimeInterval) {
		this.rowCountWithTimeInterval = rowCountWithTimeInterval;
	}
    
    public boolean getSQLServer() {
        return sqlServer;
    }

    public void setSQLServer(boolean sqlServer) {
        this.sqlServer = sqlServer;
    }

    public boolean getObjectName() {
        return objectName;
    }

    public void setObjectName(boolean objectName) {
        this.objectName = objectName;
    }

    public boolean getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(boolean databaseName) {
        this.databaseName = databaseName;
    }
    
    public boolean getHostName() {
        return hostName;
    }

    public void setHostName(boolean hostName) {
        this.hostName = hostName;
    }
    
    public boolean getApplicationName() {
        return applicationName;
    }

    public void setApplicationName(boolean applicationName) {
        this.applicationName = applicationName;
    }
    
    public boolean getLoginName() {
        return loginName;
    }

    public void setLoginName(boolean loginName) {
        this.loginName = loginName;
    }
    
    public boolean getPrivilegedUserName()
    {
    	return privilegedUserName;
    }
    
    public void setPrivilegedUserName(boolean privilegedUserName)
    {
    	this.privilegedUserName = privilegedUserName;
    }
    
    public boolean getAccessCheckPassed() {
        return accessCheckPassed;
    }

    public void setAccessCheckPassed(boolean accessCheckPassed) {
        this.accessCheckPassed = accessCheckPassed;
    }
    

    public boolean getAccessCheckPassedChk() {
        return accessCheckPassedChk;
    }

    public void setAccessCheckPassedChk(boolean accessCheckPassedChk) {
        this.accessCheckPassedChk = accessCheckPassedChk;
    }
    
    public boolean getIsPrivilegedCheck() {
        return isPrivilegedCheck;
    }

    public void setIsPrivilegedCheck(boolean isPrivilegedCheck) {
        this.isPrivilegedCheck = isPrivilegedCheck;
    }

    public boolean getIsPrivilegedUser() {
        return isPrivilegedUser;
    }

    public void setIsPrivilegedUser(boolean isPrivilegedUser) {
        this.isPrivilegedUser = isPrivilegedUser;
    }
    
    public boolean getExcludeCertainEventType() {
        return excludeCertainEventType;
    }

    public void setExcludeCertainEventType(boolean excludeCertainEventType) {
        this.excludeCertainEventType = excludeCertainEventType;
    }
    
    public Map<String, Object> getTargetInstances() {
        return targetInstances;
    }

    public void setTargetInstances(Map<String, Object> targetInstances) {
        this.targetInstances = targetInstances;
    } 
    
    public ListModelList<Data> getDbNameList() {
        return dbNameList;
    }

    public void setDbNameList(ListModelList<Data> dbNameList) {
        this.dbNameList = dbNameList;
    } 
    
    public ListModelList<Objects> getObjectNameList() {
        return objectNameList;
    }

    public void setObjectNameList(ListModelList<Objects> objectNameList) {
        this.objectNameList = objectNameList;
    }
    
    public ListModelList<Host> getHostNameList() {
        return hostNameList;
    }

    public void setHostNameList(ListModelList<Host> hostNameList) {
        this.hostNameList = hostNameList;
    }
    
    public ListModelList<App> getAppNameList() {
        return appNameList;
    }

    public void setAppNameList(ListModelList<App> appNameList) {
    	 this.appNameList = appNameList;
    }
    
    
    public ListModelList<Login> getLoginNameList() {
        return loginNameList;
    }

    public void setLoginNameList(ListModelList<Login> loginNameList) {
    	 this.loginNameList = loginNameList;
    }
    
    public String getDbRadioSelected() {
        return dbRadioSelected;
    }

    public void setDbRadioSelected(String dbRadioSelected) {
        this.dbRadioSelected = dbRadioSelected;
    } 
    
    public String getObjectRadioSelected() {
        return objectRadioSelected;
    }

    public void setObjectRadioSelected(String objectRadioSelected) {
        this.objectRadioSelected = objectRadioSelected;
    } 
    
    public String getHostRadioSelected() {
        return hostRadioSelected;
    }

    public void setHostRadioSelected(String hostRadioSelected) {
        this.hostRadioSelected = hostRadioSelected;
    } 
    
    public String getAppRadioSelected() {
        return appRadioSelected;
    }

    public void setAppRadioSelected(String appRadioSelected) {
        this.appRadioSelected = appRadioSelected;
    } 
    
    public String getLoginRadioSelected() {
        return loginRadioSelected;
    }

    public void setLoginRadioSelected(String loginRadioSelected) {
        this.loginRadioSelected = loginRadioSelected;
    } 
    
    public ListModelList<PrivilegedUserName> getPrivilegedUserNameList() {
		return privilegedUserNameList;
	}

	public void setPrivilegedUserNameList(
			ListModelList<PrivilegedUserName> privilegedUserNameList) {
		this.privilegedUserNameList = privilegedUserNameList;
	}
    
    
    // MatchString Data for event rule Condition 
    
    @JsonProperty("dbMatchString")
    private String dbMatchString;
    
    @JsonProperty("objectMatchString")
    private String objectMatchString;

    @JsonProperty("hostMatchString")
    private String hostMatchString;
    
    @JsonProperty("appMatchString")
    private String appMatchString;
    
    @JsonProperty("loginMatchString")
    private String loginMatchString;
    
    @JsonProperty("accessChkMatchString")
    private String accessChkMatchString;
    
    @JsonProperty("privilegedUserMatchString")
    private String privilegedUserMatchString;
    
    @JsonProperty("privilegedUserNameMatchString")
    private String privilegedUserNameMatchString;
    
    public String getPrivilegedUserNameMatchString() {
		return privilegedUserNameMatchString;
	}

	public void setPrivilegedUserNameMatchString(
			String privilegedUserNameMatchString) {
		this.privilegedUserNameMatchString = privilegedUserNameMatchString;
	}


	@JsonProperty("excludeCertainMatchString")
    private String excludeCertainMatchString;
    
	@JsonProperty("dbFieldId")
    private int dbFieldId;
    
    @JsonProperty("objectFieldId")
    private int objectFieldId;

    @JsonProperty("hostFieldId")
    private int hostFieldId;
    
    @JsonProperty("appFieldId")
    private int appFieldId;
    
    @JsonProperty("loginFieldId")
    private int loginFieldId;
    
    @JsonProperty("privilegedFieldId")
    private int privilegedFieldId;
    
	@JsonProperty("excludeCertainFieldId")
    private int excludeCertainFieldId;

	@JsonProperty("accessChkFieldId")
    private int accessChkFieldId;
    
    @JsonProperty("privilegedUserFieldId")
    private int privilegedUserFieldId;
    
    public int getPrivilegedFieldId() {
		return privilegedFieldId;
	}

	public void setPrivilegedFieldId(int privilegedFieldId) {
		this.privilegedFieldId = privilegedFieldId;
	}
	
    public String getExcludeCertainMatchString() {
		return excludeCertainMatchString;
	}

	public void setExcludeCertainMatchString(String excludeCertainMatchString) {
		this.excludeCertainMatchString = excludeCertainMatchString;
	}
    
    public int getExcludeCertainFieldId() {
		return excludeCertainFieldId;
	}

	public void setExcludeCertainFieldId(int excludeCertainFieldId) {
		this.excludeCertainFieldId = excludeCertainFieldId;
	}
       
    public boolean isSqlServer() {
		return sqlServer;
	}

	public void setSqlServer(boolean sqlServer) {
		this.sqlServer = sqlServer;
	}

	public int getDbFieldId() {
		return dbFieldId;
	}

	public void setDbFieldId(int dbFieldId) {
		this.dbFieldId = dbFieldId;
	}

	public int getObjectFieldId() {
		return objectFieldId;
	}

	public void setObjectFieldId(int objectFieldId) {
		this.objectFieldId = objectFieldId;
	}

	public int getHostFieldId() {
		return hostFieldId;
	}

	public void setHostFieldId(int hostFieldId) {
		this.hostFieldId = hostFieldId;
	}

	public int getAppFieldId() {
		return appFieldId;
	}

	public void setAppFieldId(int appFieldId) {
		this.appFieldId = appFieldId;
	}

	public int getLoginFieldId() {
		return loginFieldId;
	}

	public void setLoginFieldId(int loginFieldId) {
		this.loginFieldId = loginFieldId;
	}

	public int getAccessChkFieldId() {
		return accessChkFieldId;
	}

	public void setAccessChkFieldId(int accessChkFieldId) {
		this.accessChkFieldId = accessChkFieldId;
	}

	public int getPrivilegedUserFieldId() {
		return privilegedUserFieldId;
	}

	public void setPrivilegedUserFieldId(int privilegedUserFieldId) {
		this.privilegedUserFieldId = privilegedUserFieldId;
	}

	public void setPrivilegedUser(boolean isPrivilegedUser) {
		this.isPrivilegedUser = isPrivilegedUser;
	}

	public void setPrivilegedCheck(boolean isPrivilegedCheck) {
		this.isPrivilegedCheck = isPrivilegedCheck;
	}

	public String getAccessChkMatchString() {
		return accessChkMatchString;
	}

	public void setAccessChkMatchString(String accessChkMatchString) {
		this.accessChkMatchString = accessChkMatchString;
	}

	public String getPrivilegedUserMatchString() {
		return privilegedUserMatchString;
	}

	public void setPrivilegedUserMatchString(String privilegedUserMatchString) {
		this.privilegedUserMatchString = privilegedUserMatchString;
	}

    public String getDbMatchString() {
		return dbMatchString;
	}

	public void setDbMatchString(String dbMatchString) {
		this.dbMatchString = dbMatchString;
	}

	public String getObjectMatchString() {
		return objectMatchString;
	}

	public void setObjectMatchString(String objectMatchString) {
		this.objectMatchString = objectMatchString;
	}

	public String getHostMatchString() {
		return hostMatchString;
	}

	public void setHostMatchString(String hostMatchString) {
		this.hostMatchString = hostMatchString;
	}

	public String getAppMatchString() {
		return appMatchString;
	}

	public void setAppMatchString(String appMatchString) {
		this.appMatchString = appMatchString;
	}

	public String getLoginMatchString() {
		return loginMatchString;
	}

	public void setLoginMatchString(String loginMatchString) {
		this.loginMatchString = loginMatchString;
	}
}