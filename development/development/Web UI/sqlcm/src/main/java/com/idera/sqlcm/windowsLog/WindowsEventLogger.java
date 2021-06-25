package com.idera.sqlcm.windowsLog;

import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.PatternLayout;
import org.apache.log4j.nt.NTEventLogAppender;
import org.apache.log4j.Logger;

public class WindowsEventLogger { 
	
	private static Logger logger = Logger.getLogger(WindowsEventLogger.class); 
	    	public void WIndowEventLog(){
	    		
	    		BasicConfigurator.configure(); NTEventLogAppender eventLogAppender = new NTEventLogAppender();
	    		//eventLogAppender.setSource(source);
	    		eventLogAppender.setLayout(new PatternLayout("%m"));
	    		eventLogAppender.activateOptions();
	    		logger.addAppender(eventLogAppender);
	    		logger.info("Hello World!"); 
	    		
	    	} 
}