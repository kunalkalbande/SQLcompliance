package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

public class SnmpClasses {
	 public enum SNMPTrap
	    {
	        SqlCmTrap,
	        SqlCmTrapTest
	    }

	    public static class SNMPHelper
	    {
	        public final String IderaOid = "1.3.6.1.4.1.24117";
	        public final byte ProductLine = 1; // SQL Servers
	        public final byte Product = 6; // SQL Compliance Manager

	        private final int GenericTrap = 6;

	        public enum SqlCmVariable 
	        {
	            AlertType,
	            Instance,
	            Created,
	            Level,
	            EventType,
	            Message,
	            RuleName,
	            MessageTitle,
	            Severity,
	            ComputerName
	        }
 }
}
