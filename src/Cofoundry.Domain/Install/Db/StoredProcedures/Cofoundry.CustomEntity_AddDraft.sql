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
	declare @MaxDisplayVersion int = 1;
	declare @ErrorMessage nvarchar(2048);

	if (@CopyFromCustomEntityVersionId is null)
	begin
		declare @LatestWorkFlowStatusId int

		select top 1 
			@CopyFromCustomEntityVersionId = CustomEntityVersionId,
			@LatestWorkFlowStatusId = WorkFlowStatusId
		from Cofoundry.CustomEntityVersion
		where CustomEntityId = @CustomEntityId and WorkFlowStatusId in (@DraftWorkFlowStatus, @PublishedWorkFlowStatus)
		order by 
			-- detect draft first
			case 
				when WorkFlowStatusId = @DraftWorkFlowStatus then 0 
				when WorkFlowStatusId = @PublishedWorkFlowStatus then 1 
				else 2
			end,
			-- finally order by latest for status that allow duplicates
			CreateDate desc

		if (@LatestWorkFlowStatusId is null) 
		begin
			set @ErrorMessage = FORMATMESSAGE('CustomEntity_AddDraft: Unable to locate a version to copy from for custom entity, CustomEntityId: %i', @CustomEntityId);
			throw 50000, @ErrorMessage, 1;
		end 
		
		if (@LatestWorkFlowStatusId = @DraftWorkFlowStatus) 
		begin
			set @ErrorMessage = FORMATMESSAGE('CustomEntity_AddDraft: Custom entity already has a draft version, CustomEntityId: %i', @CustomEntityId);
			throw 50000, @ErrorMessage, 1;
		end 
	end
	
	select @MaxDisplayVersion = max(DisplayVersion)
	from Cofoundry.CustomEntityVersion
	where CustomEntityId = @CustomEntityId

	-- Copy version
	insert into Cofoundry.CustomEntityVersion (
		CustomEntityId,
		Title,
		WorkFlowStatusId,
		SerializedData,
		CreateDate,
		CreatorId,
		DisplayVersion
	) 
	select
		CustomEntityId,
		Title,
		@DraftWorkFlowStatus,
		SerializedData,
		@CreateDate,
		@CreatorId,
		@MaxDisplayVersion + 1
		
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