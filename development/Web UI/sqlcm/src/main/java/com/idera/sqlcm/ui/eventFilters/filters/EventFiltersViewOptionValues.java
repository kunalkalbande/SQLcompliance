package com.idera.sqlcm.ui.eventFilters.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum EventFiltersViewOptionValues {
  	
	ENABLE_STATUS("enableStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.ENABLE_STATUS), 1, ELFunctions.getLabel(SQLCMI18NStrings.ENABLE), "EventFilter"),
    DISABLE_STATUS("disableStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.DISABLE_STATUS), 0, ELFunctions.getLabel(SQLCMI18NStrings.DISABLE), "EventFilterDisabled");
	
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, EventFiltersViewOptionValues> lookup = new HashMap<Integer, EventFiltersViewOptionValues>();
    static {
        for (EventFiltersViewOptionValues status : EventFiltersViewOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    EventFiltersViewOptionValues(String id, String value, int intValue, String label, String imageURL) {
        this.intValue = intValue;
        this.label = label;
        this.image = imageURL;
    }

    public int getIntValue() {
        return intValue;
    }

    public String getLabel() {
        return label;
    }

    public String getImage() {
        return image;
    }

    public static String findLabelByKey(int statusId) {
        return lookup.get(statusId).getLabel();
    }

    public static String findImageByKey(int statusId) {
        return lookup.get(statusId).getImage();
    }
}
