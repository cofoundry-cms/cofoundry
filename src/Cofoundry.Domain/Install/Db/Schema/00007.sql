/* 
	#225 Increase the size of the CustomEntityModelType field in the PageTemplate 
	table to account for longer namespaces
 */
alter table Cofoundry.PageTemplate drop constraint CK_PageTemplate_CustomEntityDefinition

alter table Cofoundry.PageTemplate alter column CustomEntityModelType nvarchar(400) null

alter table Cofoundry.PageTemplate add constraint CK_PageTemplate_CustomEntityDefinition check (PageTypeId <> 2 or CustomEntityDefinitionCode is not null and CustomEntityModelType is not null)

go

/*
#227 PageBlockTypes: Missing unique index on filename.
*/

-- name does not have to be unique
drop index UIX_PageBlockType_Name on Cofoundry.PageBlockType
create unique index UIX_PageBlockType_FileName on Cofoundry.PageBlockType ([FileName]) where IsArchived = 0

create unique index UIX_PageBlockTypeTemplate_FileName on Cofoundry.PageBlockTypeTemplate ([FileName])

go

