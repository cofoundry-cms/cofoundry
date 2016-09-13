create procedure Cofoundry.CustomEntity_AddDraft
	(
		@CustomEntityId int,
		@CopyFromCustomEntityVersionId int,
		@CreateDate datetime2,
		@CreatorId int,
		@CustomEntityVersionId int output
		)
	as
begin
	
	declare @CustomEntityVersionEntityDefinitionCode char(6) = 'COFCEV';
	declare @CustomEntityPageModuleEntityDefinitionCode char(6) = 'COFCEM';
	declare @PublishedWorkFlowStatus int = 4;
	declare @ApprovedWorkFlowStatus int = 5;
	declare @DraftWorkFlowStatus int = 1;

	if (@CopyFromCustomEntityVersionId is null)
	begin
		select top 1 @CopyFromCustomEntityVersionId = CustomEntityVersionId 
		from Cofoundry.CustomEntityVersion
		where CustomEntityId = @CustomEntityId and (WorkFlowStatusId = @PublishedWorkFlowStatus or WorkFlowStatusId = @DraftWorkFlowStatus)
		order by case when WorkFlowStatusId = @PublishedWorkFlowStatus then 0 else 1 end
	end

	-- Copy version
	insert into Cofoundry.CustomEntityVersion (
		CustomEntityId,
		Title,
		WorkFlowStatusId,
		SerializedData,
		CreateDate,
		CreatorId
	) 
	select
		CustomEntityId,
		Title,
		@DraftWorkFlowStatus,
		SerializedData,
		@CreateDate,
		@CreatorId
		
	from Cofoundry.CustomEntityVersion
	where CustomEntityVersionId =  @CopyFromCustomEntityVersionId

	set @CustomEntityVersionId = SCOPE_IDENTITY()
	
	-- Copy Version Dependencies
	insert into Cofoundry.UnstructuredDataDependency (
		RootEntityDefinitionCode,
		RootEntityId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	)
	select 
		RootEntityDefinitionCode,
		@CustomEntityVersionId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	from Cofoundry.UnstructuredDataDependency
	where RootEntityDefinitionCode = @CustomEntityVersionEntityDefinitionCode and RootEntityId = @CopyFromCustomEntityVersionId
	
	-- Copy Modules
	-- Technique take from http://sqlmag.com/t-sql/copying-data-dependencies
	declare @ModulesToCopy table
	(
		SourceCustomEntityVersionPageModuleId int,
		DestinationCustomEntityVersionPageModuleId int
	)

	merge into Cofoundry.CustomEntityVersionPageModule as destination
	using (select 
			CustomEntityVersionPageModuleId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			PageModuleTypeTemplateId
		from Cofoundry.CustomEntityVersionPageModule
		where CustomEntityVersionId = @CopyFromCustomEntityVersionId
		) as src
		on 1= 2
	when not matched then 
		insert 
		 (
			CustomEntityVersionId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			PageModuleTypeTemplateId
		)
		values
		(
			@CustomEntityVersionId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			PageModuleTypeTemplateId
		) 
	output src.CustomEntityVersionPageModuleId, inserted.CustomEntityVersionPageModuleId
	into @ModulesToCopy (SourceCustomEntityVersionPageModuleId, DestinationCustomEntityVersionPageModuleId);
	
	-- Copy Custom Entity Page Module Dependencies
	insert into Cofoundry.UnstructuredDataDependency (
		RootEntityDefinitionCode,
		RootEntityId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	)
	select 
		RootEntityDefinitionCode,
		s.DestinationCustomEntityVersionPageModuleId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	from @ModulesToCopy s
	inner join Cofoundry.UnstructuredDataDependency d on d.RootEntityId = s.SourceCustomEntityVersionPageModuleId and RootEntityDefinitionCode = @CustomEntityPageModuleEntityDefinitionCode
	
end