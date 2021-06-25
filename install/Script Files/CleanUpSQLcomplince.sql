--------------------------------------------------------------------------------------------------
--                                                                            
--  File Name:   CleanUpSQLcomplince.sql                                   
--                                                                            
--  Description: Repository Uninstall code - deletes events databases and SQLcompliance databases     
--                                                                            
---------------------------------------------------------------------------------------------------

USE master  
GO  

--------------------------------------------
-- Drop the last databases - SQLcompliance and SQLcomplianceProcessing--
--------------------------------------------

IF EXISTS(SELECT * FROM sysdatabases WHERE name='SQLcompliance')  
BEGIN
DROP DATABASE [SQLcompliance]
END

GO 

-- If the database already exists, drop it  

IF EXISTS(SELECT * FROM sysdatabases WHERE name='SQLcompliance')  
BEGIN
RAISERROR ('SQLcompliance Database was not deleted.  Please be sure there are no open connection to this database.', 16, 1)
END

GO
     
