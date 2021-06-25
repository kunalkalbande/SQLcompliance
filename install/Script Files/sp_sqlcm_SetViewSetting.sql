IF (OBJECT_ID('sp_sqlcm_SetViewSetting') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_SetViewSetting
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_SetViewSetting]
@UserId nvarchar(256),
@ViewId nvarchar(256),
@ViewName nvarchar(256),
@Timeout int,
@Filter [nvarchar](MAX)
AS BEGIN

IF EXISTS( SELECT * FROM [SQLcompliance]..[ViewSettings] WHERE UserId = @UserId AND (@ViewName = ViewName) AND (@ViewId = ViewId))
BEGIN
UPDATE [SQLcompliance]..[ViewSettings]
    SET
        Timeout = (CASE WHEN @Timeout IS NULL THEN Timeout ELSE @Timeout END),
		Filter = (CASE WHEN @Filter IS NULL THEN Filter ELSE @Filter END)
    WHERE UserId = @UserId AND (@ViewName = ViewName)
END
ELSE
BEGIN
	INSERT INTO [SQLcompliance]..[ViewSettings](UserId, ViewId, Timeout, Filter,ViewName) VALUES
		(@UserId, @ViewId, @Timeout, @Filter,@ViewName)
END
END
GO