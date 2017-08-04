create trigger Cofoundry.CustomEntityVersionPageBlock_CascadeDelete
   on  Cofoundry.CustomEntityVersionPageBlock
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFCEB'

	-- Dependencies

	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.CustomEntityVersionPageBlockId and RootEntityDefinitionCode = @DefinitionCode

	-- Main Table
    delete Cofoundry.CustomEntityVersionPageBlock
	from Cofoundry.CustomEntityVersionPageBlock e
	inner join deleted d on e.CustomEntityVersionPageBlockId = d.CustomEntityVersionPageBlockId

end