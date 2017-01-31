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
	
	declare @PageVersionModuleEntityDefinitionCode char(6) = 'COFPGM';
	declare @PublishedWorkFlowStatus int = 4;
	declare @ApprovedWorkFlowStatus int = 5;
	declare @DraftWorkFlowStatus int = 1;

	if (@CopyFromPageVersionId is null)
	begin
		select top 1 @CopyFromPageVersionId = PageVersionId 
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where p.PageId = @PageId and p.IsDeleted = 0 and (WorkFlowStatusId = @PublishedWorkFlowStatus or WorkFlowStatusId = @DraftWorkFlowStatus)
		order by case when WorkFlowStatusId = @PublishedWorkFlowStatus then 0 else 1 end
	end

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
	
	-- Copy Modules
	-- Technique take from http://sqlmag.com/t-sql/copying-data-dependencies
	declare @ModulesToCopy table
	(
		SourcePageVersionModuleId int,
		DestinationPageVersionModuleId int
	)

	merge into Cofoundry.PageVersionModule as destination
	using (select 
			PageVersionModuleId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageModuleTypeTemplateId
		from Cofoundry.PageVersionModule
		where PageVersionId = @CopyFromPageVersionId
		) as src
		on 1 = 2
	when not matched then 
		insert 
		 (
			PageVersionId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageModuleTypeTemplateId
		)
		values
		(
			@PageVersionId,
			PageTemplateSectionId,
			PageModuleTypeId,
			SerializedData,
			Ordering,
			CreateDate,
			CreatorId,
			UpdateDate,
			PageModuleTypeTemplateId
		) 
	output src.PageVersionModuleId, inserted.PageVersionModuleId
	into @ModulesToCopy (SourcePageVersionModuleId, DestinationPageVersionModuleId);

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
		s.DestinationPageVersionModuleId,
		RelatedEntityDefinitionCode,
		RelatedEntityId,
		RelatedEntityCascadeActionId
	from @ModulesToCopy s
	inner join Cofoundry.UnstructuredDataDependency d on d.RootEntityId = s.SourcePageVersionModuleId and RootEntityDefinitionCode = @PageVersionModuleEntityDefinitionCode
	
end