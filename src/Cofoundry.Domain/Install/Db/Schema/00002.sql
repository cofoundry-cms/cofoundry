/**
* Make Page Templates auto-bootstrapable
* This involes making data that is automatically added/removed by the system
* soft deletable to avoid any accidental mistakes and allow for data migration
* should templates/modules change
*/

-- No need for this relation, it is now defined in the template
drop table Cofoundry.PageTemplateSectionPageModuleType

-- Page Template Soft Deletes
alter table Cofoundry.PageTemplate drop column CreatorId
alter table Cofoundry.PageTemplate add IsArchived bit null
alter table Cofoundry.PageTemplate add UpdateDate datetime2(4) null

drop index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate
drop index UIX_PageTemplate_Name on Cofoundry.PageTemplate
go

update Cofoundry.PageTemplate set IsArchived = 0
update Cofoundry.PageTemplate set UpdateDate = CreateDate

go

alter table Cofoundry.PageTemplate alter column IsArchived bit not null
alter table Cofoundry.PageTemplate alter column UpdateDate datetime2(4) not null

create index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate ([FullPath]) where IsArchived = 0
create index UIX_PageTemplate_Name on Cofoundry.PageTemplate ([Name]) where IsArchived = 0

-- Page Template Section Soft Deletes

alter table Cofoundry.PageTemplateSection drop column CreatorId
alter table Cofoundry.PageTemplateSection add UpdateDate datetime2(4) null
alter table Cofoundry.PageTemplateSection alter column [Name] nvarchar(50) not null

go

update Cofoundry.PageTemplateSection set UpdateDate = CreateDate

go

alter table Cofoundry.PageTemplateSection alter column UpdateDate datetime2(4) not null


-- Page Module Type Soft Deletes

alter table Cofoundry.PageModuleType add IsArchived bit null
alter table Cofoundry.PageModuleType add UpdateDate datetime2(4) null
alter table Cofoundry.PageModuleType alter column [Name] nvarchar(50) not null
alter table Cofoundry.PageModuleType alter column [FileName] nvarchar(50) not null
alter table Cofoundry.PageModuleType drop column IsCustom
alter table Cofoundry.PageModuleTypeTemplate drop column CreatorId
alter table Cofoundry.PageModuleTypeTemplate alter column [Name] nvarchar(50) not null
alter table Cofoundry.PageModuleTypeTemplate alter column [FileName] nvarchar(50) not null
alter table Cofoundry.PageModuleTypeTemplate add [Description] nvarchar(max) null

drop index UIX_PageModuleType_Name on Cofoundry.PageModuleType

go

update Cofoundry.PageModuleType set IsArchived = 0
update Cofoundry.PageModuleType set UpdateDate = CreateDate

go

alter table Cofoundry.PageModuleType alter column IsArchived bit not null
alter table Cofoundry.PageModuleType alter column UpdateDate datetime2(4) not null

create index UIX_PageModuleType_Name on Cofoundry.PageModuleType ([Name]) where IsArchived = 0

