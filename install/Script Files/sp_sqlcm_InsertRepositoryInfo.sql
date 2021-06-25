USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_InsertRepositoryInfo]    Script Date: 11/13/2018 7:33:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_InsertRepositoryInfo') IS NOT NULL)
BEGIN
	DROP PROCEDURE [dbo].[sp_sqlcm_InsertRepositoryInfo] 
END
GO

CREATE procedure [dbo].[sp_sqlcm_InsertRepositoryInfo]  
(
    @Name nvarchar(max),
	@Internal_Value int,
	@Character_Value nvarchar(max)
)
AS
begin
	declare @err int
	declare @ruleid int
	declare @xmlDoc int
	
	-- Add entry to permission table and get the rule ID.
	insert into [dbo].[RepositoryInfo] ([Name], [Internal_Value], [Character_Value])
	values (@Name, @Internal_Value, @Character_Value)

	select @err = @@error
	return @err
end
 

GO

