create trigger Cofoundry.CustomEntity_CascadeDelete
   on  Cofoundry.CustomEntity
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFCEN'

	-- Dependencies
    delete Cofoundry.CustomEntityVersion
	from Cofoundry.CustomEntityVersion e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId

    delete Cofoundry.CustomEntityPublishStatusQuery
	from Cofoundry.CustomEntityPublishStatusQuery e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId
		
	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.CustomEntityId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.CustomEntityId and RelatedEntityDefinitionCode = @DefinitionCode)

	-- Main Table
    delete Cofoundry.CustomEntity
	from Cofoundry.CustomEntity e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId

end