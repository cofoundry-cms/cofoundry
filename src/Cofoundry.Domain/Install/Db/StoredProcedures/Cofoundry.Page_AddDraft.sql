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
	
	declare @PageVersionBlockEntityDefinitionCode char(6) = 'COFPGB';
	declare @PublishedWorkFlowStatus int = 4;
	declare @ApprovedWorkFlowStatus int = 5;
	declare @DraftWorkFlowStatus int = 1;

	if (@CopyFromPageVersionId is null)
	begin
		select top 1 @CopyFromPageVersionId = PageVersionId 
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where p.PageId = @PageId and p.IsDeleted = 0 and WorkFlowStatusId in (@DraftWorkFlowStatus, @PublishedWorkFlowStatus, @ApprovedWorkFlowStatus)
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
	
	-- if no draft to copy from, make an empty new version
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
	-- Technique take from http://sqlmag.com/t-sql/copying-data-dependencies
	declare @BlocksToCopy table
	(
		SourcePageVersionBlockId int,
		DestinationPageVersionBlockId int
	)

	merge into Cofoundry.PageVersionBlock as destination
	using (select 
			PageVersionBlockId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageBlockTypeTemplateId
		from Cofoundry.PageVersionBlock
		where PageVersionId = @CopyFromPageVersionId
		) as src
		on 1 = 2
	when not matched then 
		insert 
		 (
			PageVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageBlockTypeTemplateId
		)
		values
		(
			@PageVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageBlockTypeTemplateId
		) 
	output src.PageVersionBlockId, inserted.PageVersionBlockId
	into @BlocksToCopy (SourcePageVersionBlockId, DestinationPageVersionBlockId);

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
		s.DestinationPageVersionBlockId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	from @BlocksToCopy s
	inner join Cofoundry.UnstructuredDataDependency d on d.RootEntityId = s.SourcePageVersionBlockId and RootEntityDefinitionCode = @PageVersionBlockEntityDefinitionCode
	
end