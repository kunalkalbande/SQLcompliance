package com.idera;

import java.util.List;

public interface Wrapper {

    public void start(List<String> dependecies) throws StartupException;

    public void shutdown() throws ShutdownException;

    public Boolean isStarted();

    public String getName();
}
