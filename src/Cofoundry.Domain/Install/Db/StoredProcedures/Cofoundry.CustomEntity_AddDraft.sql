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
	declare @CustomEntityPageBlockEntityDefinitionCode char(6) = 'COFCEB';
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
	
	-- Copy Blocks
	-- Technique take from http://sqlmag.com/t-sql/copying-data-dependencies
	declare @BlocksToCopy table
	(
		SourceCustomEntityVersionPageBlockId int,
		DestinationCustomEntityVersionPageBlockId int
	)

	merge into Cofoundry.CustomEntityVersionPageBlock as destination
	using (select 
			CustomEntityVersionPageBlockId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			PageBlockTypeTemplateId
		from Cofoundry.CustomEntityVersionPageBlock
		where CustomEntityVersionId = @CopyFromCustomEntityVersionId
		) as src
		on 1= 2
	when not matched then 
		insert 
		 (
			CustomEntityVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			PageBlockTypeTemplateId
		)
		values
		(
			@CustomEntityVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			PageBlockTypeTemplateId
		) 
	output src.CustomEntityVersionPageBlockId, inserted.CustomEntityVersionPageBlockId
	into @BlocksToCopy (SourceCustomEntityVersionPageBlockId, DestinationCustomEntityVersionPageBlockId);
	
	-- Copy Custom Entity Page Block Dependencies
	insert into Cofoundry.UnstructuredDataDependency (
		RootEntityDefinitionCode,
		RootEntityId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	)
	select 
		RootEntityDefinitionCode,
		s.DestinationCustomEntityVersionPageBlockId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	from @BlocksToCopy s
	inner join Cofoundry.UnstructuredDataDependency d on d.RootEntityId = s.SourceCustomEntityVersionPageBlockId and RootEntityDefinitionCode = @CustomEntityPageBlockEntityDefinitionCode
	
end