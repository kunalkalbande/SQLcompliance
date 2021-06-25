
-- Idera SQL compliance Manager version 5.6
-- Last modification date: 11/27/2018
--
-- (c) Copyright 2004-2018 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetStandardRegulationSettings' and xtype='P')
drop procedure [sp_cmreport_GetStandardRegulationSettings]
GO

CREATE procedure sp_cmreport_GetStandardRegulationSettings
@regulationId int
as
begin
	
	--Temporary table for storing combined regulation settings of server and database for each regulation
	CREATE TABLE #tempRegulationSettings
	(
		regulationId int,
		serverEvents int,
		databaseEvents int
	);

	DECLARE @regId int
	DECLARE @srvEvents int
	DECLARE @dbEvents int
	
	DECLARE db_cursor CURSOR FOR 
	SELECT regulationId, serverEvents, databaseEvents
	FROM RegulationMap 

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @regId, @srvEvents, @dbEvents  

	--Fetch regulation settings for each regulation
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
	      
	      IF(NOT EXISTS(SELECT * FROM #tempRegulationSettings 
	      					WHERE regulationId = @regId))
	      BEGIN
	      	INSERT INTO #tempRegulationSettings 
	      		VALUES(@regId, @srvEvents, @dbEvents)
	      END
	      ELSE
	      BEGIN
	      	UPDATE #tempRegulationSettings
	      		SET serverEvents = serverEvents | @srvEvents, databaseEvents = databaseEvents | @dbEvents
	      			WHERE regulationId = @regId
	      END

	      FETCH NEXT FROM db_cursor INTO @regId, @srvEvents, @dbEvents 
	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

    --Return one row containing the server and database settings for the queried regulation
    SELECT serverEvents, databaseEvents FROM #tempRegulationSettings WHERE regulationId = @regulationId

    --Delete temporary table
    IF OBJECT_ID('tempdb..#tempRegulationSettings') IS NOT NULL
    DROP TABLE #tempRegulationSettings

end