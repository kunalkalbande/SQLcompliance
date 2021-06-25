package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Joiner;
import com.google.common.base.Strings;
import com.idera.sqlcm.enumerations.NumbersOfRows;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.apache.commons.collections.CollectionUtils;

import java.util.ArrayList;
import java.util.List;

public class ProfilerObject {
	
	@JsonProperty("profileName")
    private String profileName;

	@JsonProperty("category")
    private String category;

    @JsonProperty("searchStringName")
    private String searchStringName;

    @JsonProperty("definition")
    private String definition;
    
    @JsonProperty("isStringChecked")
    private boolean isStringChecked;
    
    @JsonProperty("isProfileActive")
    private boolean isProfileActive;

	public boolean getIsProfileActive() {
		return isProfileActive;
	}

	public void setIsProfileActive(boolean isProfileActive) {
		this.isProfileActive = isProfileActive;
	}

	public boolean getIsStringChecked() {
		return isStringChecked;
	}

	public void setIsStringChecked(boolean isStringChecked) {
		this.isStringChecked = isStringChecked;
	}

	public String getDefinition() {
		return definition;
	}

	public void setDefinition(String definition) {
		this.definition = definition;
	}

	public String getProfileName() {
		return profileName;
	}

	public void setProfileName(String profile) {
		this.profileName = profile;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public String getSearchStringName() {
		return searchStringName;
	}

	public void setSearchStringName(String searchStringName) {
		this.searchStringName = searchStringName;
	}
	
	@Override
	public boolean equals(Object other) {
	    if (!(other instanceof ProfilerObject)) {
	        return false;
	    }
	    ProfilerObject that = (ProfilerObject) other;
	    if(this.category.equals(that.category) && this.definition.equals(that.definition) && this.searchStringName.equals(that.searchStringName) 
	    		&& (this.isStringChecked==that.isStringChecked))
	    	return true;
	    else
	    	return false;
	}

}
