USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_enable_ExtendedEvents]    Script Date: 11/18/2016 8:28:40 PM ******/

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_enable_ExtendedEvents' and xtype='P')

DROP PROCEDURE [dbo].[sp_enable_ExtendedEvents]
GO

/****** Object:  StoredProcedure [dbo].[sp_enable_ExtendedEvents]    Script Date: 12/3/2016 11:53:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_enable_ExtendedEvents]
	@InstanceID varchar(max),
	@YesNo char(10) 

AS
BEGIN

DECLARE @Count int
SET @Count = (select COUNT(instance) from Servers where UPPER(instance) = UPPER(@InstanceID)  and sqlVersion >=11 and agentVersion like '5.4%' )


IF(@Count=1 and (UPPER(@YesNo)='YES' or UPPER(@YesNo)='NO'))
update Servers set auditCaptureSQLXE = REPLACE(REPLACE(UPPER(@YesNo),'YES',1),'NO',0) where instance = @InstanceID

ELSE IF (@Count=1 and (UPPER(@YesNo) !='YES' or UPPER(@YesNo) !='NO')) 
PRINT 'INVALID INPUT ENTERED!'

ELSE IF (@Count !=1 and (UPPER(@YesNo) ='YES' or UPPER(@YesNo) ='NO'))  
PRINT 'SQL VERSION NOT SUPPORTED!'

ELSE 
PRINT 'INVALID INPUT ENTERED OR SQL VERSION NOT SUPPORTED!'


END
GO
