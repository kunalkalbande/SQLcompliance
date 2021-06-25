package com.idera.sqlcm.common.grid;

import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.utils.SQLCMConstants;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class CommonFacade implements SQLCMConstants{

    public List<CMEntity> getAllEntities(Map<String, Object> filterRequest) {
        return new ArrayList<>();
    }
}
