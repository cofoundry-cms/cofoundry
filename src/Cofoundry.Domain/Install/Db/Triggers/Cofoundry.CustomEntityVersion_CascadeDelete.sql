create trigger Cofoundry.CustomEntityVersion_CascadeDelete
   on  Cofoundry.CustomEntityVersion
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFCEV'

	-- Dependencies
    delete Cofoundry.CustomEntityVersionPageBlock
	from Cofoundry.CustomEntityVersionPageBlock e
	inner join deleted d on e.CustomEntityVersionId = d.CustomEntityVersionId
	
    delete Cofoundry.CustomEntityPublishStatusQuery
	from Cofoundry.CustomEntityPublishStatusQuery e
	inner join deleted d on e.CustomEntityVersionId = d.CustomEntityVersionId

	-- At some point we could allow cascading of deletions, but for now
	-- we just delete any related entities
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.CustomEntityVersionId and RootEntityDefinitionCode = @DefinitionCode

	-- Main Table
    delete Cofoundry.CustomEntityVersion
	from Cofoundry.CustomEntityVersion e
	inner join deleted d on e.CustomEntityVersionId = d.CustomEntityVersionId

end