
IF OBJECT_ID('dbo.fn_sqlsm_Split') IS NOT NULL
  DROP FUNCTION dbo.[fn_sqlsm_Split]
GO

CREATE FUNCTION [dbo].[fn_sqlsm_Split] 
(
	@List    nvarchar(max),
	@SplitOn varchar(5)
)  
RETURNS @Return table 
(		
	ID    int identity(1,1),
	Value nvarchar(max)
)
AS  
BEGIN
	WHILE(CHARINDEX(@SplitOn, @List) > 0)
	BEGIN
		INSERT INTO @Return (Value) SELECT Value = LTRIM(RTRIM( SUBSTRING(@List,1, CHARINDEX(@SplitOn,@List)-1) ));
		SET @List = SUBSTRING(@List, CHARINDEX(@SplitOn,@List) + LEN(@SplitOn), LEN(@List));
	END;

	INSERT INTO @Return (Value)
	SELECT Value = LTRIM(RTRIM(@List));
	
	RETURN;
END;
