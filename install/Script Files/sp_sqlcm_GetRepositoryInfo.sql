USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetRepositoryInfo]    Script Date: 11/13/2018 7:33:58 PM ******/
IF (OBJECT_ID('sp_sqlcm_GetRepositoryInfo') IS NOT NULL)
BEGIN
	DROP PROCEDURE [dbo].[sp_sqlcm_GetRepositoryInfo]
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetRepositoryInfo]
AS
BEGIN
	SELECT
		[Name],
		[Internal_Value],
		[Character_Value]
	FROM
		[RepositoryInfo] (NOLOCK)
END
GO
