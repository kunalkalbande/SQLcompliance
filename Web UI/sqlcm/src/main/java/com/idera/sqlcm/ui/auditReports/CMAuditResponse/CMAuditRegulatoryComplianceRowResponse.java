package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditRegulatoryComplianceRowResponse {

	@JsonProperty("FieldName")
	String FieldName;

	@JsonProperty("IsHeader")
    Boolean IsHeader;
    
	@JsonProperty("FieldType")
    int FieldType;
    
	@JsonProperty("IsFieldNameRed")
    Boolean IsFieldNameRed;
   
	@JsonProperty("IsCISRed")
    Boolean IsCISRed;
    
	@JsonProperty("IsCISChecked")
    Boolean IsCISChecked;
    
	@JsonProperty("IsDISASTIGRed")
    Boolean IsDISASTIGRed;
    
	@JsonProperty("IsDISASTIGChecked")
    Boolean IsDISASTIGChecked;
    
	@JsonProperty("IsFERPARed")
    Boolean IsFERPARed;
    
	@JsonProperty("IsFERPAChecked")
    Boolean IsFERPAChecked;
    
	@JsonProperty("IsGDPRRed")
    Boolean IsGDPRRed;
    
	@JsonProperty("IsGDPRChecked")
    Boolean IsGDPRChecked;
    
	@JsonProperty("IsHIPAARed")
    Boolean IsHIPAARed;
    
	@JsonProperty("IsHIPAAChecked")
    Boolean IsHIPAAChecked;
    
	@JsonProperty("IsNERCRed")
    Boolean IsNERCRed;
    
	@JsonProperty("IsNERCChecked")
    Boolean IsNERCChecked;
    
	@JsonProperty("IsPCIDSSRed")
    Boolean IsPCIDSSRed;
    
	@JsonProperty("IsPCIDSSChecked")
    Boolean IsPCIDSSChecked;

	@JsonProperty("IsSOXRed")
    Boolean IsSOXRed;
    
	@JsonProperty("IsSOXChecked")
    Boolean IsSOXChecked;

    public CMAuditRegulatoryComplianceRowResponse() 
    {}
    
	public String getFieldName() {
		return FieldName;
	}

	public void setFieldName(String fieldName) {
		FieldName = fieldName;
	}

	public Boolean getIsHeader() {
		return IsHeader;
	}

	public void setIsHeader(Boolean isHeader) {
		IsHeader = isHeader;
	}

	public int getFieldType() {
		return FieldType;
	}

	public void setFieldType(int fieldType) {
		FieldType = fieldType;
	}

	public Boolean getIsFieldNameRed() {
		return IsFieldNameRed;
	}

	public void setIsFieldNameRed(Boolean isFieldNameRed) {
		IsFieldNameRed = isFieldNameRed;
	}

	public Boolean getIsCISRed() {
		return IsCISRed;
	}

	public void setIsCISRed(Boolean isCISRed) {
		IsCISRed = isCISRed;
	}

	public Boolean getIsCISChecked() {
		return IsCISChecked;
	}

	public void setIsCISChecked(Boolean isCISChecked) {
		IsCISChecked = isCISChecked;
	}

	public Boolean getIsDISASTIGRed() {
		return IsDISASTIGRed;
	}

	public void setIsDISASTIGRed(Boolean isDISASTIGRed) {
		IsDISASTIGRed = isDISASTIGRed;
	}

	public Boolean getIsDISASTIGChecked() {
		return IsDISASTIGChecked;
	}

	public void setIsDISASTIGChecked(Boolean isDISASTIGChecked) {
		IsDISASTIGChecked = isDISASTIGChecked;
	}

	public Boolean getIsFERPARed() {
		return IsFERPARed;
	}

	public void setIsFERPARed(Boolean isFERPARed) {
		IsFERPARed = isFERPARed;
	}

	public Boolean getIsFERPAChecked() {
		return IsFERPAChecked;
	}

	public void setIsFERPAChecked(Boolean isFERPAChecked) {
		IsFERPAChecked = isFERPAChecked;
	}

	public Boolean getIsGDPRRed() {
		return IsGDPRRed;
	}

	public void setIsGDPRRed(Boolean isGDPRRed) {
		IsGDPRRed = isGDPRRed;
	}

	public Boolean getIsGDPRChecked() {
		return IsGDPRChecked;
	}

	public void setIsGDPRChecked(Boolean isGDPRChecked) {
		IsGDPRChecked = isGDPRChecked;
	}

	public Boolean getIsHIPAARed() {
		return IsHIPAARed;
	}

	public void setIsHIPAARed(Boolean isHIPAARed) {
		IsHIPAARed = isHIPAARed;
	}

	public Boolean getIsHIPAAChecked() {
		return IsHIPAAChecked;
	}

	public void setIsHIPAAChecked(Boolean isHIPAAChecked) {
		IsHIPAAChecked = isHIPAAChecked;
	}

	public Boolean getIsNERCRed() {
		return IsNERCRed;
	}

	public void setIsNERCRed(Boolean isNERCRed) {
		IsNERCRed = isNERCRed;
	}

	public Boolean getIsNERCChecked() {
		return IsNERCChecked;
	}

	public void setIsNERCChecked(Boolean isNERCChecked) {
		IsNERCChecked = isNERCChecked;
	}

	public Boolean getIsPCIDSSRed() {
		return IsPCIDSSRed;
	}

	public void setIsPCIDSSRed(Boolean isPCIDSSRed) {
		IsPCIDSSRed = isPCIDSSRed;
	}

	public Boolean getIsPCIDSSChecked() {
		return IsPCIDSSChecked;
	}

	public void setIsPCIDSSChecked(Boolean isPCIDSSChecked) {
		IsPCIDSSChecked = isPCIDSSChecked;
	}

	public Boolean getIsSOXRed() {
		return IsSOXRed;
	}

	public void setIsSOXRed(Boolean isSOXRed) {
		IsSOXRed = isSOXRed;
	}

	public Boolean getIsSOXChecked() {
		return IsSOXChecked;
	}

	public void setIsSOXChecked(Boolean isSOXChecked) {
		IsSOXChecked = isSOXChecked;
	}
	
}
