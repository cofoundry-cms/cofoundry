/* Make first and last name optional and add email confirmation */

alter table Cofoundry.[User] alter column FirstName nvarchar(32) null
alter table Cofoundry.[User] alter column LastName nvarchar(32) null
alter table Cofoundry.[User] add IsEmailConfirmed bit null

go

update Cofoundry.[User] set IsEmailConfirmed = 0

go

alter table Cofoundry.[User] alter column IsEmailConfirmed bit not null

go

/* Role.SpecialistRoleTypeCode > Role.RoleCode */

drop index UIX_Role_SpecialistRoleTypeCode on Cofoundry.[Role]
go

exec sp_rename 'Cofoundry.[Role].SpecialistRoleTypeCode', 'RoleCode', 'column'
go

create unique index UIX_Role_RoleCode on Cofoundry.[Role] (RoleCode) where RoleCode is not null
go

/* Renaming WebDirectory to PageDirectory */

-- WebDirectory -> PageDirectory
exec sp_rename 'Cofoundry.WebDirectory', 'PageDirectory'
go
exec sp_rename 'Cofoundry.PageDirectory.WebDirectoryId' , 'PageDirectoryId', 'column'
go
exec sp_rename 'Cofoundry.PageDirectory.ParentWebDirectoryId' , 'ParentPageDirectoryId', 'column'
go
exec sp_rename 'Cofoundry.PK_WebDirectory', 'PK_PageDirectory', 'object'
go
exec sp_rename 'Cofoundry.FK_WebDirectory_CreatorUser', 'FK_PageDirectory_CreatorUser', 'object'
go
exec sp_rename 'Cofoundry.FK_WebDirectory_ParentWebDirectory', 'FK_PageDirectory_ParentPageDirectory', 'object'
go
exec sp_rename 'Cofoundry.PageDirectory.UIX_WebDirectory_UrlPath', 'UIX_PageDirectory_UrlPath', 'index'
go

-- WebDirectoryLocale -> PageDirectoryLocale
exec sp_rename 'Cofoundry.WebDirectoryLocale', 'PageDirectoryLocale'
go
exec sp_rename 'Cofoundry.PageDirectoryLocale.WebDirectoryLocaleId' , 'PageDirectoryLocaleId', 'column'
go
exec sp_rename 'Cofoundry.PageDirectoryLocale.WebDirectoryId' , 'PageDirectoryId', 'column'
go
exec sp_rename 'Cofoundry.PK_WebDirectoryLocale', 'PK_PageDirectoryLocale', 'object'
go
exec sp_rename 'Cofoundry.FK_WebDirectoryLocale_CreatorUser', 'FK_PageDirectoryLocale_CreatorUser', 'object'
go
exec sp_rename 'Cofoundry.FK_WebDirectoryLocale_Locale', 'FK_PageDirectoryLocale_Locale', 'object'
go
exec sp_rename 'Cofoundry.FK_WebDirectoryLocale_WebDirectory', 'FK_PageDirectoryLocale_PageDirectory', 'object'
go

-- Page.WebDirectoryId

exec sp_rename 'Cofoundry.[Page].WebDirectoryId' , 'PageDirectoryId', 'column'
go
exec sp_rename 'Cofoundry.FK_Page_WebDirectory', 'FK_Page_PageDirectory', 'object'
go

update Cofoundry.EntityDefinition set [Name] = 'Page Directory' where EntityDefinitionCode = 'COFDIR'
go

/* Correct PasswordHash terminology */

exec sp_rename 'Cofoundry.[User].PasswordEncryptionVersion' , 'PasswordHashVersion', 'column'
go

/* Re-naming of template sections to regions and page modules to page blocks */

-- PageTemplateSection to PageTemplateRegion
exec sp_rename 'Cofoundry.PageTemplateSection', 'PageTemplateRegion'
go
exec sp_rename 'Cofoundry.PageTemplateRegion.PageTemplateSectionId' , 'PageTemplateRegionId', 'column'
go
exec sp_rename 'Cofoundry.PageTemplateRegion.IsCustomEntitySection' , 'IsCustomEntityRegion', 'column'
go
exec sp_rename 'Cofoundry.PK_PageTemplateSection', 'PK_PageTemplateRegion', 'object'
go
exec sp_rename 'Cofoundry.FK_PageTemplateSection_PageTemplate', 'FK_PageTemplateRegion_PageTemplate', 'object'
go
exec sp_rename 'Cofoundry.PageTemplateRegion.UIX_PageTemplateSection_Name', 'UIX_PageTemplateRegion_Name', 'index'
go

-- PageModuleType to PageBlockType

exec sp_rename 'Cofoundry.PageModuleType', 'PageBlockType'
go
exec sp_rename 'Cofoundry.PageBlockType.PageModuleTypeId' , 'PageBlockTypeId', 'column'
go
exec sp_rename 'Cofoundry.PK_PageModuleType', 'PK_PageBlockType', 'object'
go
exec sp_rename 'Cofoundry.PageBlockType.UIX_PageModuleType_Name', 'UIX_PageBlockType_Name', 'index'
go

-- PageModuleTypeTemplate to PageBlockTypeTemplate

exec sp_rename 'Cofoundry.PageModuleTypeTemplate', 'PageBlockTypeTemplate'
go
exec sp_rename 'Cofoundry.PageBlockTypeTemplate.PageModuleTypeTemplateId' , 'PageBlockTypeTemplateId', 'column'
go
exec sp_rename 'Cofoundry.PageBlockTypeTemplate.PageModuleTypeId' , 'PageBlockTypeId', 'column'
go
exec sp_rename 'Cofoundry.PK_PageModuleTypeTemplate', 'PK_PageBlockTypeTemplate', 'object'
go
exec sp_rename 'Cofoundry.FK_PageModuleTypeTemplate_PageModuleType', 'FK_PageBlockTypeTemplate_PageBlockType', 'object'
go

-- PageVersionModule to PageVersionBlock

exec sp_rename 'Cofoundry.PageVersionModule', 'PageVersionBlock'
go
exec sp_rename 'Cofoundry.PageVersionBlock.PageVersionModuleId' , 'PageVersionBlockId', 'column'
go
exec sp_rename 'Cofoundry.PageVersionBlock.PageTemplateSectionId' , 'PageTemplateRegionId', 'column'
go
exec sp_rename 'Cofoundry.PageVersionBlock.PageModuleTypeId' , 'PageBlockTypeId', 'column'
go
exec sp_rename 'Cofoundry.PageVersionBlock.PageModuleTypeTemplateId' , 'PageBlockTypeTemplateId', 'column'
go
exec sp_rename 'Cofoundry.PK_PageVersionModule', 'PK_PageVersionBlock', 'object'
go
exec sp_rename 'Cofoundry.FK_PageVersionModule_CreatorUser', 'FK_PageVersionBlock_CreatorUser', 'object'
go
exec sp_rename 'Cofoundry.FK_PageVersionModule_PageModuleType', 'FK_PageVersionBlock_PageBlockType', 'object'
go
exec sp_rename 'Cofoundry.FK_PageVersionModule_PageModuleTypeTemplate', 'FK_PageVersionBlock_PageBlockTypeTemplate', 'object'
go
exec sp_rename 'Cofoundry.FK_PageVersionModule_PageTemplateSection', 'FK_PageVersionBlock_PageTemplateRegion', 'object'
go
exec sp_rename 'Cofoundry.FK_PageVersionModule_PageVersion', 'FK_PageVersionBlock_PageVersion', 'object'
go

-- CustomEntityVersionPageModule to CustomEntityVersionPageBlock

exec sp_rename 'Cofoundry.CustomEntityVersionPageModule', 'CustomEntityVersionPageBlock'
go
exec sp_rename 'Cofoundry.CustomEntityVersionPageBlock.CustomEntityVersionPageModuleId' , 'CustomEntityVersionPageBlockId', 'column'
go
exec sp_rename 'Cofoundry.CustomEntityVersionPageBlock.PageTemplateSectionId' , 'PageTemplateRegionId', 'column'
go
exec sp_rename 'Cofoundry.CustomEntityVersionPageBlock.PageModuleTypeId' , 'PageBlockTypeId', 'column'
go
exec sp_rename 'Cofoundry.CustomEntityVersionPageBlock.PageModuleTypeTemplateId' , 'PageBlockTypeTemplateId', 'column'
go
exec sp_rename 'Cofoundry.PK_CustomEntityVersionPageModule', 'PK_CustomEntityVersionPageBlock', 'object'
go
exec sp_rename 'Cofoundry.FK_CustomEntityVersionPageModule_CustomEntityVersion', 'FK_CustomEntityVersionPageBlock_CustomEntityVersion', 'object'
go
exec sp_rename 'Cofoundry.FK_CustomEntityVersionPageModule_PageModuleType', 'FK_CustomEntityVersionPageBlock_PageBlockType', 'object'
go
exec sp_rename 'Cofoundry.FK_CustomEntityVersionPageModule_PageModuleTypeTemplate', 'FK_CustomEntityVersionPageBlock_PageBlockTypeTemplate', 'object'
go
exec sp_rename 'Cofoundry.FK_CustomEntityVersionPageModule_PageTemplateSection', 'FK_CustomEntityVersionPageBlock_PageTemplateRegion', 'object'
go

-- remove renamed triggers if this isn't first install
if (ObjectProperty(object_id('Cofoundry.CustomEntityVersionPageModule_CascadeDelete'), 'IsTrigger') = 1)
begin
	drop trigger Cofoundry.CustomEntityVersionPageModule_CascadeDelete
end

if (ObjectProperty(object_id('Cofoundry.PageTemplateSection_CascadeDelete'), 'IsTrigger') = 1)
begin
	drop trigger Cofoundry.PageTemplateSection_CascadeDelete
end

if (ObjectProperty(object_id('Cofoundry.PageVersionModule_CascadeDelete'), 'IsTrigger') = 1)
begin
	drop trigger Cofoundry.PageVersionModule_CascadeDelete
end

go

-- update entity definitions to match the new terms
insert into Cofoundry.EntityDefinition (EntityDefinitionCode, [Name]) values ('COFPGB', 'Page Version Block')
update Cofoundry.UnstructuredDataDependency set RootEntityDefinitionCode = 'COFPGB' where RootEntityDefinitionCode = 'COFPGM'
update Cofoundry.UnstructuredDataDependency set RelatedEntityDefinitionCode = 'COFPGB' where RelatedEntityDefinitionCode = 'COFPGM'
delete Cofoundry.EntityDefinition where EntityDefinitionCode = 'COFPGM'

insert into Cofoundry.EntityDefinition (EntityDefinitionCode, [Name]) values ('COFCEB', 'Custom Entity Version Page Block')
update Cofoundry.UnstructuredDataDependency set RootEntityDefinitionCode = 'COFCEB' where RootEntityDefinitionCode = 'COFCEM'
update Cofoundry.UnstructuredDataDependency set RelatedEntityDefinitionCode = 'COFCEB' where RelatedEntityDefinitionCode = 'COFCEM'
delete Cofoundry.EntityDefinition where EntityDefinitionCode = 'COFCEM'

go

/*  Add missing unique indexes, mistakenly created as non-unique */

-- remove duplicates
with pageTemplateDuplicates as (
  select 
	[FileName], 
	IsArchived, 
    row_number() over (partition by [FileName], IsArchived order by UpdateDate) as [RowNumber]
  from Cofoundry.PageTemplate
)
delete pageTemplateDuplicates where [RowNumber] > 1;


with pageBlockDuplicates as (
  select 
	[FileName], 
	IsArchived, 
    row_number() over (partition by  [FileName], IsArchived order by UpdateDate) as [RowNumber]
  from Cofoundry.PageBlockType
)
delete pageBlockDuplicates where [RowNumber] > 1;

go

-- recreate indexes

drop index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate
drop index UIX_PageTemplate_Name on Cofoundry.PageTemplate
drop index UIX_PageTemplateRegion_Name on Cofoundry.PageTemplateRegion
drop index UIX_PageBlockType_Name on Cofoundry.PageBlockType
drop index UIX_User_IsSystemAccount on Cofoundry.[User]

go

create unique index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate ([FullPath]) where IsArchived = 0
create unique index UIX_PageTemplate_Name on Cofoundry.PageTemplate ([Name]) where IsArchived = 0
create unique index UIX_PageTemplateRegion_Name on Cofoundry.PageTemplateRegion (PageTemplateId, [Name])
create unique index UIX_PageBlockType_Name on Cofoundry.PageBlockType ([Name]) where IsArchived = 0
create unique index UIX_User_IsSystemAccount on Cofoundry.[User] (IsSystemAccount) where IsSystemAccount = 1

go