package com.idera.sqlcm.exception;

import util.LocalizedException;

public class ServiceException extends LocalizedException {

    protected static final mazz.i18n.Logger LOG = mazz.i18n.LoggerFactory.getLogger(ServiceException.class);

    private static final String CAN_NOT_MAP_JSON_TO_OBJECT = "Cannot Map JSON string to Object!";
    private static final String ERROR_DURING_GETTING_DATA = "Cannot get data!";

    private Exception internalException;

    public ServiceException(final String msg, final Exception internalException) {
        super(msg, internalException);
        this.internalException = internalException;
    }

    public ServiceException(final String msg) {
        super(msg);
    }

    public static ServiceException cannotMapToObject(final Exception internalException) {
        return new ServiceException(CAN_NOT_MAP_JSON_TO_OBJECT, internalException);
    }

    public static ServiceException errorDuringGettingData() {
        return new ServiceException(ERROR_DURING_GETTING_DATA);
    }

}
