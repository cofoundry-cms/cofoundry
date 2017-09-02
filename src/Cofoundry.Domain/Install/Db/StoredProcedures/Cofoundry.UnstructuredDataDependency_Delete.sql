create procedure Cofoundry.UnstructuredDataDependency_Delete
	(
		@EntityDefinitionCode char(6),
		@EntityId int
		)
	as
begin	
	
	set nocount on;
	
	declare @RelatedEntityCascadeActionId_None int = 1

	-- Delete any relations that allow cascades
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	where (e.RelatedEntityId = @EntityId 
			and RelatedEntityDefinitionCode = @EntityDefinitionCode
			and RelatedEntityCascadeActionId <> @RelatedEntityCascadeActionId_None) 
		or (e.RootEntityId = @EntityId and RootEntityDefinitionCode = @EntityDefinitionCode) 

end