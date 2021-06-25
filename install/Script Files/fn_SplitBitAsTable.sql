
IF OBJECT_ID('dbo.fn_SplitBitAsTable') IS NOT NULL
  DROP FUNCTION dbo.[fn_SplitBitAsTable]
GO

CREATE FUNCTION [dbo].[fn_SplitBitAsTable]
(
	@Row_Data NVARCHAR(MAX),
	@Delimiter NVARCHAR(5)
)  
RETURNS @temp table 
(
	data BIT
) 
AS  
BEGIN
	WHILE (CHARINDEX(@Delimiter, @Row_Data) > 0)
	BEGIN
		INSERT INTO @temp(data)
			SELECT
				data = LTRIM(RTRIM(SUBSTRING(@Row_Data, 1, CHARINDEX(@Delimiter, @Row_Data) - 1)))

		SET @Row_Data = SUBSTRING(@Row_Data, CHARINDEX(@Delimiter, @Row_Data) + 1, LEN(@Row_Data))
	END
	
	INSERT INTO @temp(data)
		SELECT
			data = LTRIM(RTRIM(@Row_Data))

	RETURN
END;
