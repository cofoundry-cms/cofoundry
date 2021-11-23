create trigger Cofoundry.PageVersionBlock_CascadeDelete
   on  Cofoundry.PageVersionBlock
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFPGB'

	-- Dependencies

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.PageVersionBlockId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.PageVersionBlockId and RelatedEntityDefinitionCode = @DefinitionCode)
	
	-- Main Table
    delete Cofoundry.PageVersionBlock
	from Cofoundry.PageVersionBlock e
	inner join deleted d on e.PageVersionBlockId = d.PageVersionBlockId

end