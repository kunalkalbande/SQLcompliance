USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetSNMPThresholdConfiguration]    Script Date: 3/7/2016 12:31:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_DeleteThresholdConfiguration') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DeleteThresholdConfiguration
END
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_DeleteThresholdConfiguration]
@InstanceName nvarchar(255)

AS
BEGIN

DELETE FROM SQLcompliance..ThresholdConfiguration where instance_name=@InstanceName;


END


GO