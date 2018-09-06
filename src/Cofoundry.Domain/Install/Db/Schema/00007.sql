/* 
	#225 Increase the size of the CustomEntityModelType field in the PageTemplate 
	table to account for longer namespaces
 */
alter table Cofoundry.PageTemplate drop constraint CK_PageTemplate_CustomEntityDefinition

alter table Cofoundry.PageTemplate alter column CustomEntityModelType nvarchar(400) null

alter table Cofoundry.PageTemplate add constraint CK_PageTemplate_CustomEntityDefinition check (PageTypeId <> 2 or CustomEntityDefinitionCode is not null and CustomEntityModelType is not null)

go

/*
	#227 PageBlockTypes: Missing unique index on filename
*/

-- name does not have to be unique
drop index UIX_PageBlockType_Name on Cofoundry.PageBlockType
create unique index UIX_PageBlockType_FileName on Cofoundry.PageBlockType ([FileName]) where IsArchived = 0

create unique index UIX_PageBlockTypeTemplate_FileName on Cofoundry.PageBlockTypeTemplate ([FileName])

go

/*
	#228 Custom Entity: Block data needs to be partitioned by page
*/

-- First Check to see if there are already multiple custom entity details pages
-- using the same template. This isn't supported in the upgrade, but wouldn't
-- have been working correctly anyway so this state is unlikely to occur.
declare @NumTemplatesInUseWithMultiplePages int;

select @NumTemplatesInUseWithMultiplePages = count(*) from (
	select pv.PageTemplateId, pv.PageId
	from Cofoundry.CustomEntityVersionPageBlock b
	inner join Cofoundry.PageTemplateRegion r on b.PageTemplateRegionId = r.PageTemplateRegionId
	inner join Cofoundry.PageVersion pv on pv.PageTemplateId = r.PageTemplateId
	inner join Cofoundry.PageTemplate pt on pt.PageTemplateId = pv.PageTemplateId
	inner join Cofoundry.[Page] p on p.PageId = pv.PageId
	where p.IsDeleted = 0 and pt.IsArchived = 0 and p.CustomEntityDefinitionCode is not null
	group by pv.PageTemplateId, pv.PageId
	) as PagesPerTemplate
group by PageTemplateId
having Count(*) > 1

if (@NumTemplatesInUseWithMultiplePages > 1) throw 50000, 'Detected multiple custom entity pages that use the same template. This is not supported in the upgrade to v0.4, please remove one before updating.', 1;

go

alter table Cofoundry.CustomEntityVersionPageBlock add PageId int null
alter table Cofoundry.CustomEntityVersionPageBlock add 
	constraint FK_CustomEntityVersionPageBlock_Page foreign key (PageId) references Cofoundry.[Page] (PageId)
go

-- First update based on active pages/templates to give them priority
update Cofoundry.CustomEntityVersionPageBlock
set PageId = pv.PageId
from Cofoundry.CustomEntityVersionPageBlock b
inner join Cofoundry.PageTemplateRegion r on b.PageTemplateRegionId = r.PageTemplateRegionId
inner join Cofoundry.PageVersion pv on pv.PageTemplateId = r.PageTemplateId
inner join Cofoundry.PageTemplate pt on pt.PageTemplateId = pv.PageTemplateId
inner join Cofoundry.[Page] p on p.PageId = pv.PageId
where p.IsDeleted = 0 and pt.IsArchived = 0

-- update the remaining blocks to ensure they have a value
-- ensuring we don't overwrite any good data with old pages/template references
update Cofoundry.CustomEntityVersionPageBlock
set PageId = pv.PageId
from Cofoundry.CustomEntityVersionPageBlock b
inner join Cofoundry.PageTemplateRegion r on b.PageTemplateRegionId = r.PageTemplateRegionId
inner join Cofoundry.PageVersion pv on pv.PageTemplateId = r.PageTemplateId
where b.PageId is null

go

alter table Cofoundry.CustomEntityVersionPageBlock alter column PageId int not null

go

/*
	#67 Page/Entities version numbers to be stored in database
*/


delete from Cofoundry.PageVersion where IsDeleted = 1

go
-- drop BasedOnPageVersionId since it's not used at all
alter table Cofoundry.PageVersion drop constraint FK_PageVersion_PageVersionOf
alter table Cofoundry.PageVersion drop column BasedOnPageVersionId

alter table Cofoundry.PageVersion add DisplayVersion int null
alter table Cofoundry.CustomEntityVersion add DisplayVersion int null

drop index UIX_PageVersion_DraftVersion on Cofoundry.PageVersion
alter table Cofoundry.PageVersion drop column IsDeleted

go

with PageCTE as (
select 
	ROW_NUMBER() over (partition by PageId order by WorkFlowStatusId desc, CreateDate) as RowNum, 
	DisplayVersion
from Cofoundry.PageVersion 
)
update PageCTE set DisplayVersion = RowNum;

with CustomEntityCTE as (
select 
	ROW_NUMBER() over (partition by CustomEntityId order by WorkFlowStatusId desc, CreateDate) as RowNum, 
	DisplayVersion
from Cofoundry.CustomEntityVersion 
)
update CustomEntityCTE set DisplayVersion = RowNum;

go
alter table Cofoundry.PageVersion alter column DisplayVersion int not null
alter table Cofoundry.CustomEntityVersion alter column DisplayVersion int not null

create unique index UIX_PageVersion_DraftVersion 
	on Cofoundry.PageVersion (PageId) 
	where WorkFlowStatusId = 1

create unique index UIX_PageVersion_DisplayVersion on Cofoundry.PageVersion (PageId, DisplayVersion)
create unique index UIX_CustomEntityVersion_DisplayVersion on Cofoundry.CustomEntityVersion (CustomEntityId, DisplayVersion)
