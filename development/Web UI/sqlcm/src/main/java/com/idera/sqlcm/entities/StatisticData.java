package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import org.zkoss.zul.*;

import java.util.Date;
import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class StatisticData {

	private List<Statistic> statics;

	public List<Statistic> getStatics() {
		return statics;
	}

	public void setStatics(List<Statistic> statics) {
		this.statics = statics;
	}

    public SimpleXYModel getDataXYModel() {
        SimpleXYModel model = new SimpleXYModel();
        if (statics != null && statics.size() > 0) {
            for (Statistic st : statics) {
            	model.addValue(st.getCategoryName(), st.getDate().getTime(), st.getCount()); 
            	//SCM-4 Start
            	if (st.getCount() > st.getCriCriticalThresholdValue())
                {
            		model.addValue("Critical", st.getDate().getTime(), st.getCriCriticalThresholdValue());
                }
            	else
            	{
            		model.addValue("Critical", st.getDate().getTime(), 0);
            	}
            	
            	if(st.getCount() > st.getWarningThresholdValue())
                {
            		model.addValue("Warning", st.getDate().getTime(), st.getWarningThresholdValue());
                }
            	else
            	{
            		model.addValue("Warning", st.getDate().getTime(), 0);
            	}
            	//SCM-4 End
            }
        }
        return model;
    }

	public PieModel getPieModel() {
		PieModel model = new SimplePieModel();
		if (getStatics() != null && getStatics().size() > 0) {
			for (Statistic st : getStatics()) {
				String key = st.getCategoryName();
				if (model.getValue(key) != null) {
					model.setValue(key,
							model.getValue(key).intValue() + st.getCount());
				} else {
					model.setValue(key, st.getCount());
				}
			}
		}
		return model;
	}

	@JsonIgnoreProperties(ignoreUnknown = true)
	public static class Statistic {

		private int databaseId;
		@JsonDeserialize(using = DataContractDateDeserializer.class)
		@JsonSerialize(using = DataContractUtcDateSerializer.class)
		private Date date;
		@JsonDeserialize(using = DataContractDateDeserializer.class)
		@JsonSerialize(using = DataContractUtcDateSerializer.class)
		private Date lastUpdated;
		private int category;
		private String categoryName;
		private int count;
		
		//SCM-4 Start
		@JsonProperty("criticalThreshold")
		private int criticalThreshold;
		@JsonProperty("warningThreshold")
		private int warningThreshold;

		public int getCriCriticalThresholdValue() {
			return criticalThreshold;
		}

		public void setCriCriticalThresholdValue(int criticalThreshold) {
			this.criticalThreshold = criticalThreshold;
		}
		
		public int getWarningThresholdValue() {
			return warningThreshold;
		}

		public void setWarningThresholdValue(int warningThreshold) {
			this.warningThreshold = warningThreshold;
		}		
		//SCM-4 End
		
		public int getDatabaseId() {
			return databaseId;
		}

		public void setDatabaseId(int databaseId) {
			this.databaseId = databaseId;
		}

		public Date getDate() {
			return date;
		}

		public void setDate(Date date) {
			this.date = date;
		}

		public Date getLastUpdated() {
			return lastUpdated;
		}

		public void setLastUpdated(Date lastUpdated) {
			this.lastUpdated = lastUpdated;
		}

		public int getCategory() {
			return category;
		}

		public void setCategory(int category) {
			this.category = category;
		}

		public String getCategoryName() {
			return categoryName;
		}

		public void setCategoryName(String categoryName) {
			this.categoryName = categoryName;
		}

		public int getCount() {
			return count;
		}

		public void setCount(int count) {
			this.count = count;
		}

	}
}