create procedure Cofoundry.CustomEntity_CopyBlocksToDraft
	(
		@CopyToCustomEntityId int,
		@CopyFromCustomEntityVersionId int
		)
	as
begin
	
	set nocount on;
	
	declare @CustomEntityPageBlockEntityDefinitionCode char(6) = 'COFCEB';
	declare @DraftWorkFlowStatus int = 1;
	declare @CopyToCustomEntityVersionId int;
	declare @ErrorMessage nvarchar(2048)

	-- Find the draft version to copy from

	select top 1 @CopyToCustomEntityVersionId = CustomEntityVersionId 
		from Cofoundry.CustomEntityVersion v
		inner join Cofoundry.CustomEntity p on p.CustomEntityId = v.CustomEntityId
		where p.CustomEntityId = @CopyToCustomEntityId 
			and WorkFlowStatusId =@DraftWorkFlowStatus
	
	if (@CopyToCustomEntityVersionId is null) 
	begin
		set @ErrorMessage = FORMATMESSAGE('CustomEntity_CopyBlocksToDraft: Unable to locate draft version for target custom entity, CustomEntityId: %i', @CopyToCustomEntityId);
		throw 50000, @ErrorMessage, 1;
	end

	-- Validate the templates for each version match

	declare @SourceVersionCustomEntityDefinitionCode char(6)
	declare @TargetVersionCustomEntityDefinitionCode char(6)
	
	select @SourceVersionCustomEntityDefinitionCode = ceSource.CustomEntityDefinitionCode, @TargetVersionCustomEntityDefinitionCode = ceTarget.CustomEntityDefinitionCode
		from Cofoundry.CustomEntityVersion ceVersionSource
		inner join Cofoundry.CustomEntity ceSource on ceSource.CustomEntityId = ceVersionSource.CustomEntityId
		cross join Cofoundry.CustomEntityVersion ceVersionTarget
		inner join Cofoundry.CustomEntity ceTarget on ceTarget.CustomEntityId = ceVersionTarget.CustomEntityId
		where ceVersionSource.CustomEntityVersionId = @CopyFromCustomEntityVersionId
		and ceVersionTarget.CustomEntityVersionId = @CopyToCustomEntityVersionId

	if (@SourceVersionCustomEntityDefinitionCode is null) 
	begin
		set @ErrorMessage = FORMATMESSAGE('CustomEntity_CopyBlocksToDraft: Source version does not exist, CustomEntityVersionId: %i', @CopyToCustomEntityVersionId);
		throw 50000, @ErrorMessage, 1;
	end
	
	if (@SourceVersionCustomEntityDefinitionCode <> @TargetVersionCustomEntityDefinitionCode) 
	begin
		set @ErrorMessage = FORMATMESSAGE('CustomEntity_CopyBlocksToDraft: Source version definition code does match target version definition code. Source CustomEntityVersionId: %i, Target CustomEntityVersionId: %i', @CopyFromCustomEntityVersionId, @CopyToCustomEntityVersionId);
		throw 50000, @ErrorMessage, 1;
	end

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
			PageBlockTypeTemplateId,
			PageId
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
			PageBlockTypeTemplateId,
			PageId
		)
		values
		(
			@CopyToCustomEntityVersionId,
			PageTemplateRegionId,
			PageBlockTypeId,
			SerializedData,
			Ordering,
			PageBlockTypeTemplateId,
			PageId
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