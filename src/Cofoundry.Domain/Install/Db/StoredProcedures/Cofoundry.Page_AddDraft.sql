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

	if (@CopyFromPageVersionId is null)
	begin
		select top 1 @CopyFromPageVersionId = PageVersionId 
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where p.PageId = @PageId and p.IsDeleted = 0 and WorkFlowStatusId in (@DraftWorkFlowStatus, @PublishedWorkFlowStatus)
		order by 
			-- fall back to a deleted version if nothing else if found
			v.IsDeleted,
			-- then prefer published and draft over approved
			case 
				when WorkFlowStatusId = @PublishedWorkFlowStatus then 0 
				when WorkFlowStatusId = @DraftWorkFlowStatus then 1 
				else 2
			end,
			-- finally order by latest for status that allow duplicates
			v.CreateDate desc
	end
	
	-- if no draft to copy from we cannot go any further
	if (@CopyFromPageVersionId is null) throw 50000, 'Cofoundry.Page_AddDraft: No drafts available to copy from', 1;

	-- Copy version
	insert into Cofoundry.PageVersion (
		PageId,
		PageTemplateId,
		BasedOnPageVersionId,
		Title,
		MetaDescription,
		WorkFlowStatusId,
		IsDeleted,
		CreateDate,
		CreatorId,
		ExcludeFromSitemap,
		OpenGraphTitle,
		OpenGraphDescription,
		OpenGraphImageId
	) 
	select
		PageId,
		PageTemplateId,
		@CopyFromPageVersionId,
		Title,
		MetaDescription,
		@DraftWorkFlowStatus,
		IsDeleted,
		@CreateDate,
		@CreatorId,
		ExcludeFromSitemap,
		OpenGraphTitle,
		OpenGraphDescription,
		OpenGraphImageId
		
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