package com.idera.sqlcm.service.rest;

import java.io.IOException;
import java.net.URLEncoder;

import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.common.rest.RestResponse;
import com.idera.core.CoreConfiguration;
import com.idera.cwf.model.Product;
import com.idera.i18n.I18NStrings;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

public class SQLCMRestClient implements SQLCMConstants {
    protected static final Logger log = Logger.getLogger(SQLCMRestClient.class);
    private static SQLCMRestClient _instance;

    SQLCMRestClient() {
        log.debug("Creating SQLCMRestClient");
    }

    public synchronized static SQLCMRestClient getInstance() {
        if (_instance == null) {
            _instance = new SQLCMRestClient();
        }
        return _instance;
    }

    private String getProductBaseURL(String restUrl) throws IOException {
        return CoreConfiguration.getIderaCoreServiceHost() + restUrl;
    }

    public <T> T getProductRestResponseCore(String methodCallUrl, TypeReference<T> responseObjectTypeReference)
            throws RestException {
        try {
            Product productInstance = (Product) SessionUtil.getSessionVariable(PROD_INSTANCE_SESSION_PARAM_NAME);
            if (productInstance == null) {
                productInstance = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
                SessionUtil.setSessionVariable(PROD_INSTANCE_SESSION_PARAM_NAME, productInstance);
            }
            log.debug("Making a call to method " + methodCallUrl + " of instance " + productInstance.getInstanceName());

            String restUrl = productInstance.getRestUrl();
            Credentials curr = null;
            try {
                curr = SessionUtil.getCurrentUserCredentials();
                if (curr != null) {
                    SessionUtil.setSessionVariable(USER_STGS_SESSION_PARAM_NAME, curr);
                    log.debug("Setting the Credentials " + curr.getUserPrincipal());
                }
            } catch (Exception e) {
                log.error("Error in sqlsafe-rest-client while getting credentials ", e);
            }
            if (curr == null) {
                curr = (Credentials) SessionUtil.getSessionVariable(USER_STGS_SESSION_PARAM_NAME);
                log.debug("Got from session " + curr);
            }
            if (curr == null) {
                log.debug("ERROR: ********* Credentials is still empty");
            }
            String baseUrl = getProductBaseURL(restUrl);
            String encodedMethodCallUrl = URLEncoder.encode(methodCallUrl.replace("#", "%23"), "UTF-8");
            encodedMethodCallUrl = encodedMethodCallUrl.replace("+", "%20");
            log.debug("Making a call to method " + encodedMethodCallUrl + " of " + restUrl);
            RestResponse<T> restResponse = CoreRestClient.getInstance()
                    .getWithProductBaseURL(baseUrl, encodedMethodCallUrl, responseObjectTypeReference, curr);
            log.debug("Response: code: " + restResponse.getHttpReturnCode() + ", message:" + restResponse.getMessage());
            if (restResponse.getResultObject() != null) log.debug(", object class:" + restResponse.getResultObject().getClass());
            return restResponse.getResultObject();
        } catch (Exception x) {
            log.debug("Rest exception in rest get for method " + methodCallUrl, x);
            throw getRestException(I18NStrings.EXCEPTION_OCCURRED_DURING_REST_API_CALL, x, methodCallUrl);
        }
    }

    public <T> T postProductRestResponseCore(String methodCallUrl, Object requestObject, TypeReference<T> responseObjectTypeReference)
            throws RestException {
        try {
            Product productInstance = (Product) SessionUtil.getSessionVariable(PROD_INSTANCE_SESSION_PARAM_NAME);
            if (productInstance == null) {
                productInstance = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
                SessionUtil.setSessionVariable(PROD_INSTANCE_SESSION_PARAM_NAME, productInstance);
            }
            log.debug("Making a call to method " + methodCallUrl + " of instance " + productInstance.getInstanceName());

            String restUrl = productInstance.getRestUrl();
            Credentials curr = null;
            try {
                curr = SessionUtil.getCurrentUserCredentials();
                if (curr != null) {
                    SessionUtil.setSessionVariable(USER_STGS_SESSION_PARAM_NAME, curr);
                    log.debug("Setting the Credentials " + curr.getUserPrincipal());
                }
            } catch (Exception e) {
                log.error("Error in sqlsafe-rest-client while getting credentials ", e);
            }
            if (curr == null) {
                curr = (Credentials) SessionUtil.getSessionVariable(USER_STGS_SESSION_PARAM_NAME);
                log.debug("Got from session " + curr);
            }
            if (curr == null) {
                log.debug("ERROR: ********* Credentials is still empty");
            }

            String baseUrl = getProductBaseURL(restUrl);
            String encodedMethodCallUrl = URLEncoder.encode(methodCallUrl.replace("#", "%23"), "UTF-8");
            encodedMethodCallUrl = encodedMethodCallUrl.replace("+", "%20");
            log.debug("Making a call to method " + encodedMethodCallUrl + " of " + restUrl);
            RestResponse<T> restResponse = CoreRestClient.getInstance()
                    .postWithProductBaseURL(baseUrl, encodedMethodCallUrl, requestObject,
                            responseObjectTypeReference, curr);
            log.debug("Response: code: " + restResponse.getHttpReturnCode() + ", message:" + restResponse.getMessage());
            if (restResponse.getResultObject() != null) log.debug(", object class:" + restResponse.getResultObject().getClass());
            return restResponse.getResultObject();
        } catch (Exception x) {
            log.debug("Rest exception in rest post for method " + methodCallUrl, x);
            throw getRestException(I18NStrings.EXCEPTION_OCCURRED_DURING_REST_API_CALL, x, methodCallUrl);
        }
    }

    private RestException getRestException(String exceptionMessage, Exception restException, String callUrl) {
        return CoreRestClient.getInstance().getRestException(exceptionMessage, restException, callUrl);
    }

}
