--------------------------------------------------------------------------------------------------
--                                                                            
--  File Name:   CleanUpProcessing.sql                                   
--                                                                            
--  Description: Repository Uninstall code - deletes events databases and SQLcompliance databases     
--                                                                            
---------------------------------------------------------------------------------------------------

USE master  
GO  

--------------------------------------------
-- Drop the last databases - SQLcompliance and SQLcomplianceProcessing--
--------------------------------------------
IF EXISTS(SELECT * FROM sysdatabases WHERE name='SQLcomplianceProcessing')  
BEGIN
DROP DATABASE [SQLcomplianceProcessing]
END

GO 

-- If the database already exists, drop it  
IF EXISTS(SELECT * FROM sysdatabases WHERE name='SQLcomplianceProcessing')  
BEGIN
RAISERROR ('SQLcomplianceProcessing Database was not deleted.  Please be sure there are no open connection to this database.', 16, 1)
END

GO
     
