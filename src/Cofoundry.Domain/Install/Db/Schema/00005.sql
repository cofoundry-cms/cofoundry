/* Make first and last name optional and add email confirmation */

alter table Cofoundry.[User] alter column FirstName nvarchar(32) null
alter table Cofoundry.[User] alter column LastName nvarchar(32) null
alter table Cofoundry.[User] add IsEmailConfirmed bit null

go

update Cofoundry.[User] set IsEmailConfirmed = 0

go

alter table Cofoundry.[User] alter column IsEmailConfirmed bit not null

go

/* Role. SpecialistRoleTypeCode > Role.RoleCode */

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
