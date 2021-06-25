package com.idera.server;

import util.LocalizedException;

import java.io.Serializable;

public class ServerException extends LocalizedException {

    private static final long serialVersionUID = 1L;

    public ServerException() {
        super();
    }

    public ServerException(String msg) {
        super(msg);
    }

    public ServerException(String msgKey, Serializable... varargs) {
        super(msgKey, varargs);
    }

    public ServerException(Throwable t, String msgKey) {
        super(t, msgKey);
    }

    public ServerException(Throwable t, String msgKey, Serializable... varargs) {
        super(t, msgKey, varargs);
    }

    public ServerException(Throwable t) {
        super(t);
    }
}
