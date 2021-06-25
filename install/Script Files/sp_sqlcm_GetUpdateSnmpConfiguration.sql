USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetUpdateSnmpConfiguration]    Script Date: 10/28/2015 9:10:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO
IF (OBJECT_ID('sp_sqlcm_GetUpdateSnmpConfiguration') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetUpdateSnmpConfiguration
END
GO

Create PROCEDURE [dbo].[sp_sqlcm_GetUpdateSnmpConfiguration]
@SnmpServerAddress nvarchar(255),
@SnmpPort int,
@Community nvarchar(255) 

AS
BEGIN

UPDATE SQLcompliance..Configuration SET snmpServerAddress =@SnmpServerAddress , snmpPort =@SnmpPort , snmpCommunity = @Community

END
GO

