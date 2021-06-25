package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import org.zkoss.zul.ListModelList;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditRegulatoryComplianceResponse {

	@JsonProperty("ServerName")
    String ServerName;

    @JsonProperty("DatabaseName")
    String DatabaseName;
    
    @JsonProperty("IsDatabase")
    Boolean IsDatabase;
    
    @JsonProperty("RowList")
    List<CMAuditRegulatoryComplianceRowResponse> RowList;
    
    @JsonProperty("ShowCIS")
    Boolean ShowCIS;
    
    @JsonProperty("ShowDISASTIG")
    Boolean ShowDISASTIG;
    
    @JsonProperty("ShowFERPA")
    Boolean ShowFERPA;
    
    @JsonProperty("ShowGDPR")
    Boolean ShowGDPR;
    
    @JsonProperty("ShowHIPAA")
    Boolean ShowHIPAA;
    
    @JsonProperty("ShowNERC")
    Boolean ShowNERC;
    
    @JsonProperty("ShowPCIDSS")
    Boolean ShowPCIDSS;
    
    @JsonProperty("ShowSOX")
    Boolean ShowSOX;

    public CMAuditRegulatoryComplianceResponse() 
    {}
    
	public String getServerName() {
		return ServerName;
	}

	public void setServerName(String serverName) {
		ServerName = serverName;
	}

	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public Boolean getIsDatabase() {
		return IsDatabase;
	}

	public void setIsDatabase(Boolean isDatabase) {
		IsDatabase = isDatabase;
	}

	public List<CMAuditRegulatoryComplianceRowResponse> getRowList() {
		return RowList;
	}

	public void setRowList(List<CMAuditRegulatoryComplianceRowResponse> rowList) {
		RowList = rowList;
	}

	public Boolean getShowCIS() {
		return ShowCIS;
	}

	public void setShowCIS(Boolean showCIS) {
		ShowCIS = showCIS;
	}

	public Boolean getShowDISASTIG() {
		return ShowDISASTIG;
	}

	public void setShowDISASTIG(Boolean showDISASTIG) {
		ShowDISASTIG = showDISASTIG;
	}

	public Boolean getShowFERPA() {
		return ShowFERPA;
	}

	public void setShowFERPA(Boolean showFERPA) {
		ShowFERPA = showFERPA;
	}

	public Boolean getShowGDPR() {
		return ShowGDPR;
	}

	public void setShowGDPR(Boolean showGDPR) {
		ShowGDPR = showGDPR;
	}

	public Boolean getShowHIPAA() {
		return ShowHIPAA;
	}

	public void setShowHIPAA(Boolean showHIPAA) {
		ShowHIPAA = showHIPAA;
	}

	public Boolean getShowNERC() {
		return ShowNERC;
	}

	public void setShowNERC(Boolean showNERC) {
		ShowNERC = showNERC;
	}

	public Boolean getShowPCIDSS() {
		return ShowPCIDSS;
	}

	public void setShowPCIDSS(Boolean showPCIDSS) {
		ShowPCIDSS = showPCIDSS;
	}

	public Boolean getShowSOX() {
		return ShowSOX;
	}

	public void setShowSOX(Boolean showSOX) {
		ShowSOX = showSOX;
	}

	@Override
	public String toString() {
		return "CMAuditRegulatoryComplianceResponse {"
				+ "ServerName=" + ServerName + 
				", DatabaseName=" + DatabaseName
				+ ", IsDatabase=" + IsDatabase
				+ ", RowList=" + RowList
				+ ", ShowCIS=" + ShowCIS
				+ ", ShowDISASTIG=" + ShowDISASTIG
				+ ", ShowFERPA=" + ShowFERPA
				+ ", ShowGDPR=" + ShowGDPR
				+ ", ShowHIPAA=" + ShowHIPAA
				+ ", ShowNERC=" + ShowNERC
				+ ", ShowPCIDSS=" + ShowPCIDSS
				+ ", ShowSOX=" + ShowSOX + "}";
	}
	
    
}
