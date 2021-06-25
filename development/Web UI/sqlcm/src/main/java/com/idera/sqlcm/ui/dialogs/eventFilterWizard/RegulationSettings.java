package com.idera.sqlcm.ui.dialogs.eventFilterWizard; 


import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zul.ListModelList;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.ui.eventFilters.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.eventFilters.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.eventFilters.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.eventFilters.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.eventFilters.SpecifyObjectsViewModel.Objects;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel.SessionLogin;

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
    
    @JsonProperty("sessionLoginName")
    private boolean sessionLoginName;
    
    @JsonProperty("accessCheckPassed")
    private boolean accessCheckPassed;
    
    @JsonProperty("accessCheckPassedChk")
    private boolean accessCheckPassedChk;
    
    @JsonProperty("isPrivilegedUser")
    private boolean isPrivilegedUser;
    
    @JsonProperty("isPrivilegedCheck")
    private boolean isPrivilegedCheck;
    
    @JsonProperty("excludeCertainEventType")
    private boolean excludeCertainEventType;
    
    @JsonProperty("database")
    private List database;
    
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
    
    @JsonProperty("sessionLoginNameList")
    public ListModelList<SessionLogin> sessionLoginNameList = new ListModelList<>();
    
    
    
    @JsonProperty("dbMatchString")
    public String dbMatchString;
    
	@JsonProperty("objectMatchString")
    public String objectMatchString;
    
    @JsonProperty("hostMatchString")
    public String hostMatchString;
    
    @JsonProperty("appMatchString")
    public String appMatchString;
    
    @JsonProperty("loginMatchString")
    public String loginMatchString;
    
    
    
    @JsonProperty("sessionLoginMatchString")
    public String sessionLoginMatchString;
    
    @JsonProperty("sessionPrivilegedMatchString")
    public String sessionPrivilegedMatchString;
    

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
    
    @JsonProperty("sessionLoginRadioSelected")
    private String sessionLoginRadioSelected;
    
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
    
    public boolean getSessionLoginName() {
        return sessionLoginName;
    }

    public void setSessionLoginName(boolean sessionLoginName) {
        this.sessionLoginName = sessionLoginName;
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

    public ListModelList<SessionLogin> getSessionLoginNameList() {
        return sessionLoginNameList;
    }

    public void setSessionLoginNameList(ListModelList<SessionLogin> sessionLoginNameList) {
      this.sessionLoginNameList = sessionLoginNameList;
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
    
    public String getSessionLoginRadioSelected() {
        return sessionLoginRadioSelected;
    }

    public void setSessionLoginRadioSelected(String sessionLoginRadioSelected) {
        this.sessionLoginRadioSelected = sessionLoginRadioSelected;
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

	public String getSessionLoginMatchString() {
		return sessionLoginMatchString;
	}

	public void setSessionLoginMatchString(String sessionLoginMatchString) {
		this.sessionLoginMatchString = sessionLoginMatchString;
	}
	
    public String getSessionPrivilegedMatchString() {
		return sessionPrivilegedMatchString;
	}

	public void setSessionPrivilegedMatchString(String sessionPrivilegedMatchString) {
		this.sessionPrivilegedMatchString = sessionPrivilegedMatchString;
	}

}
