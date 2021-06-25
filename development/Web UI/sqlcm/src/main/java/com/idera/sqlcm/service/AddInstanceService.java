package com.idera.sqlcm.service;

import com.idera.sqlcm.data.dialogs.DiscoveredInstance;
import com.idera.sqlcm.exception.ServiceException;

import java.util.List;

public interface AddInstanceService {

    static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(AddInstanceService.class);
    static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(AddInstanceService.class);

    public List<String> getInstanceWrapperList() throws ServiceException;
    public int checkSqlServerCredentials(DiscoveredInstance discoveredInstance) throws ServiceException;
    public boolean addServers(List<DiscoveredInstance> list) throws ServiceException;
    public DiscoveredInstance getInstanceById(int instanceId) throws ServiceException;

}
