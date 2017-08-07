/**
* Make Page Templates auto-bootstrapable
* This involes making data that is automatically added/removed by the system
* soft deletable to avoid any accidental mistakes and allow for data migration
* should templates/modules change
*/

-- No need for this relation, it is now defined in the template
drop table Cofoundry.PageTemplateSectionPageModuleType

-- Page Template Soft Deletes

alter table Cofoundry.PageTemplate drop constraint FK_PageTemplate_User
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
alter table Cofoundry.PageTemplate alter column [Description] nvarchar(max) null

create unique index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate ([FullPath]) where IsArchived = 0
create unique index UIX_PageTemplate_Name on Cofoundry.PageTemplate ([Name]) where IsArchived = 0

-- Page Template Section Soft Deletes
drop index UIX_PageTemplateSection_Name on Cofoundry.PageTemplateSection

alter table Cofoundry.PageTemplateSection drop constraint FK_PageTemplateSection_User
alter table Cofoundry.PageTemplateSection drop column CreatorId
alter table Cofoundry.PageTemplateSection add UpdateDate datetime2(4) null
alter table Cofoundry.PageTemplateSection alter column [Name] nvarchar(50) not null

create unique index UIX_PageTemplateSection_Name on Cofoundry.PageTemplateSection (PageTemplateId, [Name])

go

update Cofoundry.PageTemplateSection set UpdateDate = CreateDate

go

alter table Cofoundry.PageTemplateSection alter column UpdateDate datetime2(4) not null


-- Page Module Type Soft Deletes

drop index UIX_PageModuleType_Name on Cofoundry.PageModuleType

alter table Cofoundry.PageModuleType add IsArchived bit null
alter table Cofoundry.PageModuleType add UpdateDate datetime2(4) null
alter table Cofoundry.PageModuleType alter column [Name] nvarchar(50) not null
alter table Cofoundry.PageModuleType alter column [FileName] nvarchar(50) not null
alter table Cofoundry.PageModuleType alter column [Description] nvarchar(max) null

alter table Cofoundry.PageModuleType drop constraint DF_PageModuleType_IsCustom
alter table Cofoundry.PageModuleType drop column IsCustom

-- Tidy up PageModuleTypeTemplate and add a description
alter table Cofoundry.PageModuleTypeTemplate drop constraint FK_PageModuleTypeTemplate_CreatorUser
alter table Cofoundry.PageModuleTypeTemplate drop column CreatorId
alter table Cofoundry.PageModuleTypeTemplate alter column [Name] nvarchar(50) not null
alter table Cofoundry.PageModuleTypeTemplate alter column [FileName] nvarchar(50) not null
alter table Cofoundry.PageModuleTypeTemplate add [Description] nvarchar(max) null

go

update Cofoundry.PageModuleType set IsArchived = 0
update Cofoundry.PageModuleType set UpdateDate = CreateDate

go

alter table Cofoundry.PageModuleType alter column IsArchived bit not null
alter table Cofoundry.PageModuleType alter column UpdateDate datetime2(4) not null

create unique index UIX_PageModuleType_Name on Cofoundry.PageModuleType ([Name]) where IsArchived = 0
go

-- Add system level user to allow for data imports before the application has been created

alter table Cofoundry.[User] add IsSystemAccount bit null

go
update Cofoundry.[User] set IsSystemAccount = 0

declare @RoleId int
select @RoleId = RoleId from Cofoundry.[Role] where UserAreaCode = 'COF' and SpecialistRoleTypeCode = 'SUP'

-- The system user is never allowed to be loggged in, the application forbids this. 
-- As an extra measure we pass in a guid as the password hash so the value is junk and at least different between applications
insert into Cofoundry.[User] (FirstName, LastName, [Username], [Password], CreateDate, IsDeleted, LastPasswordChangeDate, RequirePasswordChange, RoleId, UserAreaCode, IsSystemAccount) 
values ('System', 'User', 'System', NEWID(), GetUtcDate(), 0, GetUtcDate(), 0, @RoleId, 'COF', 1)
go
-- Make sure that only 1 system account can be created
create unique index UIX_User_IsSystemAccount on Cofoundry.[User] (IsSystemAccount) where IsSystemAccount = 1
go

-- Add the root directory if it doesn't exist
declare @sysUserId int
select @sysUserId = UserId from Cofoundry.[User] where IsSystemAccount = 1

if (not exists(select * from Cofoundry.WebDirectory where ParentWebDirectoryId is null))
begin
	insert into Cofoundry.WebDirectory ([Name], UrlPath, IsActive, CreateDate, CreatorId) 
		values  ('Root', '', 1, GetUtcDate(), @sysUserId)
end

