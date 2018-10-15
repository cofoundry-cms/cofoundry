create procedure Cofoundry.Page_CopyBlocksToDraft
	(
		@CopyToPageId int,
		@CopyFromPageVersionId int,
		@CreateDate datetime2,
		@CreatorId int
		)
	as
begin
	
	set nocount on;
	
	declare @PageVersionBlockEntityDefinitionCode char(6) = 'COFPGB';
	declare @DraftWorkFlowStatus int = 1;
	declare @CopyToPageVersionId int;
	declare @ErrorMessage nvarchar(2048)

	-- Find the draft version to copy from

	select top 1 @CopyToPageVersionId = PageVersionId 
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where p.PageId = @CopyToPageId 
			and WorkFlowStatusId =@DraftWorkFlowStatus
	
	if (@CopyToPageVersionId is null) 
	begin
		set @ErrorMessage = FORMATMESSAGE('Page_CopyBlocksToDraft: Unable to locate draft version for target page, PageId: %i', @CopyToPageId);
		throw 50000, @ErrorMessage, 1;
	end

	-- Validate the templates for each version match

	declare @SourceVersionTemplateId int
	declare @TargetVersionTemplateId int

	select @SourceVersionTemplateId = pvSource.PageTemplateId, @TargetVersionTemplateId = pvTarget.PageTemplateId
		from Cofoundry.PageVersion pvSource
		cross join Cofoundry.PageVersion pvTarget
		where pvSource.PageVersionId = @CopyFromPageVersionId
		and pvTarget.PageVersionId = @CopyToPageVersionId

	if (@SourceVersionTemplateId is null) 
	begin
		set @ErrorMessage = FORMATMESSAGE('Page_CopyBlocksToDraft: Source version does not exist, PageVersionId: %i', @CopyToPageId);
		throw 50000, @ErrorMessage, 1;
	end
	
	if (@SourceVersionTemplateId <> @TargetVersionTemplateId) 
	begin
		set @ErrorMessage = FORMATMESSAGE('Page_CopyBlocksToDraft: Source version template does match target version template. Source PageVersionId: %i, Target PageVersionId: %i', @CopyFromPageVersionId, @CopyToPageVersionId);
		throw 50000, @ErrorMessage, 1;
	end

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
			@CopyToPageVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			@CreateDate,
			@CreatorId,
			@CreateDate,
			PageBlockTypeTemplateId
		) 
	output src.PageVersionBlockId, inserted.PageVersionBlockId
	into @BlocksToCopy (SourcePageVersionBlockId, DestinationPageVersionBlockId);

	-- Copy Page Block Dependencies
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