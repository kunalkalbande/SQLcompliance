package com.idera.sqlcm.entities.database.properties;

import java.util.ArrayList;
import java.util.List;

import org.apache.commons.collections.CollectionUtils;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Joiner;
import com.google.common.base.Objects;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMStringCMTableEntity {

    @JsonProperty("Key")
    private String key;

    @JsonProperty("Value")
    private CMTable value;

    @JsonProperty("Type")
    private String type;

    @JsonProperty("columnList")
    private List<String> fullColumnList = new ArrayList<>();

    public List<String> getFullColumnList() {
	fullColumnList = new ArrayList<>();
	if (datasetTableList != null && datasetTableList.size() > 0) {
	    for (CMTable table : datasetTableList) {
		if (table.getColumnList() != null) {
		    fullColumnList.addAll(table.getColumnList());
		}
	    }
	}
	return fullColumnList;
    }

    public String getFullColumnListString() {
	if (CollectionUtils.isEmpty(getFullColumnList()))
	    return ELFunctions
		    .getLabel(SQLCMI18NStrings.DB_PROPS_DIALOG_ALL_COLUMNS);
	return Joiner.on(',').join(fullColumnList).toString();
    }

    public String getType() {
	return type;
    }

    public void setType(String type) {
	this.type = type;
    }

    @JsonProperty("datasetTableList")
    private List<CMTable> datasetTableList;

    public List<CMTable> getDatasetTableList() {
	return datasetTableList;
    }

    public void setDatasetTableList(List<CMTable> datasetTableList) {
	this.datasetTableList = datasetTableList;
    }

    public CMStringCMTableEntity() {
    }

    public CMStringCMTableEntity(String key, CMTable value) {
        this.key = key;
        this.value = value;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public CMTable getValue() {
        return value;
    }

    public void setValue(CMTable value) {
        this.value = value;
    }

    @Override
    public String toString() {
	return Objects.toStringHelper(this).add("key", key).add("value", value)
		.toString();
    }

	@Override
	public boolean equals(Object o) {
		if (this == o)
			return true;
		if (o == null || getClass() != o.getClass())
			return false;
		CMStringCMTableEntity that = (CMStringCMTableEntity) o;
		return Objects.equal(this.key, that.key)
				&& Objects.equal(this.value, that.value)
				&& Objects.equal(this.type, that.type)
				&& CompareDatasetTableList(this.datasetTableList,
						that.datasetTableList);
	}

	private boolean CompareDatasetTableList(List<CMTable> first,
      List<CMTable> second) {
		if(first != null){
			if(second != null){
				if(first.size() != second.size()){
					return false;
				}
				else{
					boolean isEqual = true;
					for (CMTable cmTableFirst : first) {
						isEqual = false;
						for (CMTable cmTableSecond : second) {
							if(cmTableFirst.getcolumnId() == cmTableSecond.getcolumnId()){
								isEqual = true;
								break;
							}
						}
					}
					return isEqual;
				}
			}
			else{
				return false;
			}
		}
		else if(second != null)
			return false;
		return true;
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(key, value);
    }
}
