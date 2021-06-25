package com.idera.sqlcm.service.rest;

import java.util.List;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.data.dialogs.DiscoveredInstance;
import com.idera.sqlcm.exception.ServiceException;
import com.idera.sqlcm.service.AddInstanceService;

public class AddInstanceServiceRest implements AddInstanceService {

    @Override
    public List<String> getInstanceWrapperList() throws ServiceException {
        try {
            String url = SQLCMRestMethods.GET_AVAILABLE_INSTANCES.getMethodName();
            return SQLCMRestClient.getInstance().getProductRestResponseCore(url, new TypeReference<List<String>>() {});
        } catch (RestException e) {
            throw new ServiceException(e.getMessage(), e);
        }
    }

    @Override
    public int checkSqlServerCredentials(DiscoveredInstance discoveredInstance) throws ServiceException {
        try {
            String url = SQLCMRestMethods.CHECK_SQL_SERVER_CREDENTIALS.getMethodName();
            return SQLCMRestClient.getInstance().postProductRestResponseCore(url, discoveredInstance, new TypeReference<Integer>() {});
        } catch (RestException e) {
            throw new ServiceException(e.getMessage(), e);
        }
    }

    @Override
    public boolean addServers(List<DiscoveredInstance> list) throws ServiceException {
        try {
            String url = SQLCMRestMethods.ADD_SERVERS.getMethodName();
            return SQLCMRestClient.getInstance().postProductRestResponseCore(url, list, new TypeReference<Boolean>() {});
        } catch (RestException e) {
            throw new ServiceException(e.getMessage(), e);
        }
    }

    @Override
    public DiscoveredInstance getInstanceById(int instanceId) throws ServiceException {
        try {
            String url = SQLCMRestMethods.GET_INSTANCE_BY_ID.getMethodName() + String.valueOf(instanceId);
            return SQLCMRestClient.getInstance().getProductRestResponseCore(url, new TypeReference<DiscoveredInstance>() {});
        } catch (RestException e) {
            throw new ServiceException(e.getMessage(), e);
        }
    }

}
