create trigger Cofoundry.PageVersionBlock_CascadeDelete
   on  Cofoundry.PageVersionBlock
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFPGB'

	-- Dependencies

	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.PageVersionBlockId and RootEntityDefinitionCode = @DefinitionCode

	-- Main Table
    delete Cofoundry.PageVersionBlock
	from Cofoundry.PageVersionBlock e
	inner join deleted d on e.PageVersionBlockId = d.PageVersionBlockId

end