package com.idera.sqlcm.ui.components.filter;

import com.idera.sqlcm.utils.SQLCMConstants;

import java.util.Arrays;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public class FilterData extends TreeMap<String, String> {

    private String source;

    public String getSource() {
        return source;
    }

    public void setSource(String source) {
        this.source = source;
    }

    public String generateRequestParameters() {
        StringBuilder requestParams = new StringBuilder();
        Iterator<Map.Entry<String, String>> i = entrySet().iterator();
        while (i.hasNext()) {
            Map.Entry<String, String> e = i.next();
            requestParams.append(e.getKey()).append(SQLCMConstants.GET_REQUEST_PARAM_VAL_PREF)
                         .append(e.getValue()).append(SQLCMConstants.GET_REQUEST_PARAM_SEPARATOR);
        }
        if (requestParams.length() == 0) return requestParams.toString();
        requestParams.setLength(requestParams.length() - 1);
        return requestParams.toString();
    }

    public boolean removeFilterValue(String key, String valueToDelete) {
        List<String> currentValues = new LinkedList<>(Arrays.asList(this.get(key).split(SQLCMConstants.FILTER_VALUES_SEP_REGEXP)));
        if (currentValues != null) {
            currentValues.remove(valueToDelete);
        }
        if (!currentValues.isEmpty()) {
            StringBuilder values = new StringBuilder();
            for (String value : currentValues) {
                values.append(value).append(SQLCMConstants.FILTER_VALUES_SEP);
            }
            values.setLength(values.length() - 1);
            this.put(key, values.toString());
            return true;
        }
        return false;
    }

}
