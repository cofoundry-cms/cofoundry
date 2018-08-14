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
	
	set nocount on;
	
	declare @CustomEntityVersionEntityDefinitionCode char(6) = 'COFCEV';
	declare @CustomEntityPageBlockEntityDefinitionCode char(6) = 'COFCEB';
	declare @PublishedWorkFlowStatus int = 4;
	declare @DraftWorkFlowStatus int = 1;

	if (@CopyFromCustomEntityVersionId is null)
	begin
		select top 1 @CopyFromCustomEntityVersionId = CustomEntityVersionId 
		from Cofoundry.CustomEntityVersion
		where CustomEntityId = @CustomEntityId and (WorkFlowStatusId = @PublishedWorkFlowStatus or WorkFlowStatusId = @DraftWorkFlowStatus)
		order by case when WorkFlowStatusId = @PublishedWorkFlowStatus then 0 else 1 end, createdate desc
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
	
	exec Cofoundry.CustomEntity_CopyBlocksToDraft 
		@CopyToCustomEntityId = @CustomEntityId, 
		@CopyFromCustomEntityVersionId = @CopyFromCustomEntityVersionId
	
	exec Cofoundry.CustomEntityPublishStatusQuery_Update @CustomEntityId = @CustomEntityId;
end