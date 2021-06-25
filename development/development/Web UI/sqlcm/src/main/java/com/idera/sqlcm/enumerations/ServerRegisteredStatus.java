package com.idera.sqlcm.enumerations;

public enum ServerRegisteredStatus {

    NOT_REGISTERED(0),
    IS_REGISTERED(1),
    WAS_REGISTERED(2);

    private int id;

    ServerRegisteredStatus(int id) {
        this.id = id;
    }

    public int getId() {
        return id;
    }

    public static ServerRegisteredStatus getById(long id) {
        for(ServerRegisteredStatus e : values()) {
            if(e.id == id) {
                return e;
            }
        }
        return null;
    }

    public static ServerRegisteredStatus getByIdOrException(long id) {
        ServerRegisteredStatus serverRegisteredStatus = getById(id);

        if (serverRegisteredStatus == null) {
            throw new RuntimeException(" Invalid server status id! ");
        }

        return serverRegisteredStatus;
    }

}
