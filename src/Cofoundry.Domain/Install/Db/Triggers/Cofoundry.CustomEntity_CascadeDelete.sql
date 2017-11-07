create trigger Cofoundry.CustomEntity_CascadeDelete
   on  Cofoundry.CustomEntity
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFCEN'
	declare @RelatedEntityCascadeActionId_None int = 1

	-- Dependencies
    delete Cofoundry.CustomEntityVersion
	from Cofoundry.CustomEntityVersion e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId

    delete Cofoundry.CustomEntityPublishStatusQuery
	from Cofoundry.CustomEntityPublishStatusQuery e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId

	-- Delete any relations that allow cascades
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RelatedEntityId = d.CustomEntityId and RelatedEntityDefinitionCode = @DefinitionCode
	where RelatedEntityCascadeActionId <> @RelatedEntityCascadeActionId_None

	-- Main Table
    delete Cofoundry.CustomEntity
	from Cofoundry.CustomEntity e
	inner join deleted d on e.CustomEntityId = d.CustomEntityId

end