IF (OBJECT_ID('trg_sqlcm_UpdateServers') IS NOT NULL)
BEGIN
	DROP TRIGGER [trg_sqlcm_UpdateServers]
END
GO
		

CREATE TRIGGER trg_sqlcm_UpdateServers
	ON [SQLcompliance]..[Servers]
	AFTER UPDATE
	AS
	BEGIN
		IF Update([timeLastCollection]) OR Update([description]) -- 5.1: OR Update([owner]) OR Update([location]) OR Update([comments])
		begin
			UPDATE [SQLcompliance]..[Servers]
			SET [isSynchronized] = 0
			WHERE srvId in (
			select i.srvId from Inserted i
			left outer join Deleted d on d.srvId = i.srvId
			WHERE 
				(ISNULL(i.[timeLastCollection], '') <> ISNULL(d.[timeLastCollection], '')))
		end
		
	END