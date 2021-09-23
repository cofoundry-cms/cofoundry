create procedure Cofoundry.Page_AddDraft
	(
		@PageId int,
		@CopyFromPageVersionId int,
		@CreateDate datetime2,
		@CreatorId int,
		@PageVersionId int output
		)
	as
begin
	
	set nocount on;
	
	declare @PageVersionBlockEntityDefinitionCode char(6) = 'COFPGB';
	declare @PublishedWorkFlowStatus int = 4;
	declare @DraftWorkFlowStatus int = 1;
	declare @MaxDisplayVersion int = 1;
	declare @ErrorMessage nvarchar(2048);

	if (@CopyFromPageVersionId is null)
	begin
		declare @LatestWorkFlowStatusId int

		select top 1 
			@CopyFromPageVersionId = PageVersionId,
			@LatestWorkFlowStatusId = WorkFlowStatusId
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where p.PageId = @PageId and WorkFlowStatusId in (@DraftWorkFlowStatus, @PublishedWorkFlowStatus)
		order by 
			-- prefer published and draft over approved
			case 
				when WorkFlowStatusId = @DraftWorkFlowStatus then 0 
				when WorkFlowStatusId = @PublishedWorkFlowStatus then 1 
				else 2
			end,
			-- finally order by latest for status that allow duplicates
			v.CreateDate desc

		if (@LatestWorkFlowStatusId is null) 
		begin
			set @ErrorMessage = FORMATMESSAGE('Page_AddDraft: Unable to locate a version to copy from for page, PageId: %i', @PageId);
			throw 51001, @ErrorMessage, 1;
		end 
		
		if (@LatestWorkFlowStatusId = @DraftWorkFlowStatus) 
		begin
			set @ErrorMessage = FORMATMESSAGE('Page_AddDraft: Page already has a draft version, PageId: %i', @PageId);
			throw 51002, @ErrorMessage, 1;
		end 
	end
	
	select @MaxDisplayVersion = max(DisplayVersion)
	from Cofoundry.PageVersion
	where PageId = @PageId

	-- Copy version
	insert into Cofoundry.PageVersion (
		PageId,
		PageTemplateId,
		Title,
		MetaDescription,
		WorkFlowStatusId,
		CreateDate,
		CreatorId,
		ExcludeFromSitemap,
		OpenGraphTitle,
		OpenGraphDescription,
		OpenGraphImageId,
		DisplayVersion
	) 
	select
		PageId,
		PageTemplateId,
		Title,
		MetaDescription,
		@DraftWorkFlowStatus,
		@CreateDate,
		@CreatorId,
		ExcludeFromSitemap,
		OpenGraphTitle,
		OpenGraphDescription,
		OpenGraphImageId,
		@MaxDisplayVersion + 1
		
	from Cofoundry.PageVersion
	where PageVersionId =  @CopyFromPageVersionId

	set @PageVersionId = SCOPE_IDENTITY()
	
	-- Copy Blocks

	exec Cofoundry.Page_CopyBlocksToDraft 
		@CopyToPageId = @PageId, 
		@CopyFromPageVersionId = @CopyFromPageVersionId,
		@CreateDate = @CreateDate,
		@CreatorId = @CreatorId

	-- Update page publish status lookup table
	exec Cofoundry.PagePublishStatusQuery_Update @PageId = @PageId;
end