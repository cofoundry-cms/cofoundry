/*
	#187 Pages / Directories: Restrict by User Area
*/

-- Action to take when no access e.g.RedirectToLogin, Error, NotFound, RedirectToUrl
create table Cofoundry.AccessRuleViolationAction (
	AccessRuleViolationActionId int not null,
	Title varchar(20) not null,

	constraint PK_AccessRuleViolationAction primary key (AccessRuleViolationActionId)
)

create unique index UIX_AccessRuleViolationAction_Title on Cofoundry.AccessRuleViolationAction (Title)
go

insert into Cofoundry.AccessRuleViolationAction (AccessRuleViolationActionId, Title) values (0, 'Error')
insert into Cofoundry.AccessRuleViolationAction (AccessRuleViolationActionId, Title) values (1, 'RedirectToLogin')
insert into Cofoundry.AccessRuleViolationAction (AccessRuleViolationActionId, Title) values (2, 'NotFound')

go

alter table Cofoundry.[Page] add AccessRuleViolationActionId int null
alter table Cofoundry.[Page] add UserAreaCodeForLoginRedirect char(3) null
go
alter table Cofoundry.[Page] add constraint FK_Page_UserAreasForLoginRedirect foreign key (UserAreaCodeForLoginRedirect) references Cofoundry.UserArea (UserAreaCode)
alter table Cofoundry.[Page] add constraint CK_Page_UserAreaCodeForLoginRedirect_NotCofoundryAdmin check (UserAreaCodeForLoginRedirect <> 'COF')
go
update Cofoundry.[Page] set AccessRuleViolationActionId = 0
go
alter table Cofoundry.[Page] alter column AccessRuleViolationActionId int not null


-- Used to restrict access to an individual page (access rules inherit from parent directories)
create table Cofoundry.PageAccessRule (
	PageAccessRuleId int identity(1,1) not null,
	PageId int not null,
	UserAreaCode char(3) null,
	RoleId int null,
	CreatorId int not null,
	CreateDate datetime2(4) not null,

	constraint PK_PageAccessRule primary key (PageAccessRuleId),
	constraint FK_PageAccessRule_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PageAccessRule_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode),
	constraint FK_PageAccessRule_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId),
	constraint FK_PageAccessRule_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint CK_PageAccessRule_NotCofoundryAdmin check (UserAreaCode <> 'COF')
)


create unique index UIX_PageAccessRule_Rule on Cofoundry.PageAccessRule (PageId, UserAreaCode, RoleId)
go

alter table Cofoundry.PageDirectory add AccessRuleViolationActionId int null
alter table Cofoundry.PageDirectory add UserAreaCodeForLoginRedirect char(3) null
go
alter table Cofoundry.PageDirectory add constraint FK_PageDirectory_UserAreaForLoginRedirect foreign key (UserAreaCodeForLoginRedirect) references Cofoundry.UserArea (UserAreaCode)
alter table Cofoundry.PageDirectory add constraint CK_PageDirectory_UserAreaCodeForLoginRedirect_NotCofoundryAdmin check (UserAreaCodeForLoginRedirect <> 'COF')
go
update Cofoundry.PageDirectory set AccessRuleViolationActionId = 0
go

-- Used to restrict access to an individual directory (access rules inherit from parent directories)
create table Cofoundry.PageDirectoryAccessRule (
	PageDirectoryAccessRuleId int identity(1,1) not null,
	PageDirectoryId int not null,
	UserAreaCode char(3) null,
	RoleId int null,
	CreatorId int not null,
	CreateDate datetime2(4) not null,

	constraint PK_PageDirectoryAccessRule primary key (PageDirectoryAccessRuleId),
	constraint FK_PageDirectoryAccessRule_Page foreign key (PageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId),
	constraint FK_PageDirectoryAccessRule_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode),
	constraint FK_PageDirectoryAccessRule_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId),
	constraint FK_PageDirectoryAccessRule_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint CK_PageDirectoryAccessRule_NotCofoundryAdmin check (UserAreaCode <> 'COF')
)

create unique index UIX_PageDirectoryAccessRule_Rule on Cofoundry.PageDirectoryAccessRule (PageDirectoryId, UserAreaCode, RoleId)

-- The closure table can tell us all the directories that a directory is parented to so we can check for access rules up the heirarchy
create table Cofoundry.PageDirectoryClosure (
	AncestorPageDirectoryId int not null,
	DescendantPageDirectoryId int not null,
	Distance int not null,

	constraint PK_PageDirectoryClosure primary key (AncestorPageDirectoryId, DescendantPageDirectoryId),
	constraint FK_PageDirectoryClosure_AncestorPageDirectory foreign key (AncestorPageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId),
	constraint FK_PageDirectoryClosure_DescendantPageDirectory foreign key (DescendantPageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId)
)

-- This table is generated from the PageDirectoryClosure table and makes it easier for us to query heirarchy information
create table Cofoundry.PageDirectoryPath (
	PageDirectoryId int not null,
	FullUrlPath nvarchar(max) not null,
	Depth int not null,

	constraint PK_PageDirectoryPath primary key (PageDirectoryId),
	constraint FK_PageDirectoryPath_PageDirectory foreign key (PageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId)
)

-- Add missing foreign key to custom entity table

alter table Cofoundry.CustomEntity add constraint FK_CustomEntity_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId)
go

-- Seed the PageDirectoryClosure and PageDirectoryPath tables with existing data
-- This is a copy of the Cofoundry.PageDirectory_UpdatePath stored procedure  

with DirectoryCTE as 
(
	select 
		PageDirectoryId as AncestorPageDirectoryId, 
		PageDirectoryId as DescendantPageDirectoryId, 
		0 as Distance

	from Cofoundry.PageDirectory
	 
	union all

	select
		cte.AncestorPageDirectoryId,
		d.PageDirectoryId as DescendantPageDirectoryId,
		cte.Distance + 1 AS Distance
	from Cofoundry.PageDirectory as d
	inner join DirectoryCTE AS cte on d.ParentPageDirectoryId = cte.DescendantPageDirectoryId
	inner join Cofoundry.PageDirectory ancestorDirectory on cte.AncestorPageDirectoryId = ancestorDirectory.PageDirectoryId
)
merge into Cofoundry.PageDirectoryClosure as t
using (
	select AncestorPageDirectoryId, DescendantPageDirectoryId, Distance
	from DirectoryCTE
	) as s
on t.AncestorPageDirectoryId = s.AncestorPageDirectoryId and t.DescendantPageDirectoryId = s.DescendantPageDirectoryId
when matched and t.Distance <> s.Distance then 
	update set Distance = s.Distance
when not matched by target then
	insert (AncestorPageDirectoryId, DescendantPageDirectoryId, Distance)
	values (AncestorPageDirectoryId, DescendantPageDirectoryId, Distance)
when not matched by source then
	delete;

-- Upsert PageDirectoryPath

merge into Cofoundry.PageDirectoryPath as t
using (
	select 
		c.DescendantPageDirectoryId as PageDirectoryId, 
		IsNull((
			select Stuff((
				select N'/' + d.UrlPath 
				from Cofoundry.PageDirectoryClosure c2
				inner join Cofoundry.PageDirectory d on c2.AncestorPageDirectoryId = d.PageDirectoryId
				where c2.DescendantPageDirectoryId = c.DescendantPageDirectoryId and d.ParentPageDirectoryId is not null
				order by c2.Distance desc
				for xml path('')
			) ,1 ,1, N'')
		), '') as FullUrlPath,
		Max(c.Distance) as Depth
	from Cofoundry.PageDirectoryClosure c
	group by c.DescendantPageDirectoryId
) as s
on t.PageDirectoryId = s.PageDirectoryId
when matched and t.FullUrlPath <> s.FullUrlPath or t.Depth <> s.Depth  then 
	update set FullUrlPath = s.FullUrlPath, Depth = s.Depth
when not matched by target then
	insert (PageDirectoryId, FullUrlPath, Depth)
	values (PageDirectoryId, FullUrlPath, Depth)
when not matched by source then
	delete;
go


/*
	#463: Page Directories: extract ChangePageUrlCommand from UpdatePageDirectoryCommand
*/

declare @DirectoryDefinitionCode char(6) = 'COFDIR'
declare @UpdateUrlPermissionId int

-- If the entity definition is not installed yet, then we don't need to worry about migrating the permission
if (exists(select * from Cofoundry.EntityDefinition where EntityDefinitionCode = @DirectoryDefinitionCode))
begin
	insert into Cofoundry.Permission (EntityDefinitionCode, PermissionCode) values (@DirectoryDefinitionCode, 'UPDURL')
	set @UpdateUrlPermissionId = SCOPE_IDENTITY()
	
	-- Ensure anyone that had update permission also gets the new update url permission
	insert into Cofoundry.RolePermission (RoleId, PermissionId)
	select RoleId, @UpdateUrlPermissionId
	from Cofoundry.RolePermission rp
	inner join Cofoundry.Permission p on rp.PermissionId = p.PermissionId
	where p.EntityDefinitionCode = @DirectoryDefinitionCode and p.PermissionCode = 'COMUPD'
end
go
-- Add missing constraint here to prevent self-referencing directories
alter table Cofoundry.PageDirectory add constraint CK_PageDirectory_ParentNotSelf check (ParentPageDirectoryId <> PageDirectoryId)
go


/*
	#464: Page Directories: "Name" property is superflous
	#466 : Pages / Directories: Increase maximum url slug length
*/

drop index UIX_Page_Path on Cofoundry.[Page]
drop index UIX_PageDirectory_UrlPath on Cofoundry.PageDirectory
go
alter table Cofoundry.PageDirectory alter column [Name] nvarchar(200) null
alter table Cofoundry.PageDirectory alter column UrlPath nvarchar(200) not null
alter table Cofoundry.[Page] alter column UrlPath nvarchar(200) not null
go
create index UIX_Page_UrlPath on Cofoundry.[Page] (PageDirectoryId, LocaleId, UrlPath)
create index UIX_PageDirectory_UrlPath on Cofoundry.PageDirectory (ParentPageDirectoryId, UrlPath)
go


/*
	#288: Page & Custom Entity: Add update date
*/

alter table Cofoundry.[Page] add LastPublishDate datetime2(7) null
alter table Cofoundry.CustomEntity add LastPublishDate datetime2(7) null
go
update Cofoundry.[Page] set LastPublishDate = PublishDate where PublishDate is not null
update Cofoundry.CustomEntity set LastPublishDate = PublishDate where PublishDate is not null
go

-- Also increase precision of other dates to match c# DateTime type which can otherwise cause issues
-- This index needs to be dropped before date precision, but isn't necessary, so let's not re-add it unless a need comes apparent
drop index IX_ModuleUpdateError_Date on Cofoundry.ModuleUpdateError
go
alter table Cofoundry.AssetFileCleanupQueueItem alter column CreateDate datetime2(7) not null
alter table Cofoundry.AssetFileCleanupQueueItem alter column LastAttemptDate datetime2(7) null
alter table Cofoundry.AssetFileCleanupQueueItem alter column CompletedDate datetime2(7) null
alter table Cofoundry.AssetFileCleanupQueueItem alter column AttemptPermittedDate datetime2(7) not null

alter table Cofoundry.CustomEntity alter column CreateDate datetime2(7) not null
alter table Cofoundry.CustomEntityVersion alter column CreateDate datetime2(7) not null

alter table Cofoundry.DocumentAsset alter column CreateDate datetime2(7) not null
alter table Cofoundry.DocumentAsset alter column UpdateDate datetime2(7) not null
alter table Cofoundry.DocumentAsset alter column FileUpdateDate datetime2(7) not null
alter table Cofoundry.DocumentAssetGroup alter column CreateDate datetime2(7) not null
alter table Cofoundry.DocumentAssetGroupItem alter column CreateDate datetime2(7) not null
alter table Cofoundry.DocumentAssetTag alter column CreateDate datetime2(7) not null

alter table Cofoundry.ImageAsset alter column CreateDate datetime2(7) not null
alter table Cofoundry.ImageAsset alter column UpdateDate datetime2(7) not null
alter table Cofoundry.ImageAsset alter column FileUpdateDate datetime2(7) not null
alter table Cofoundry.ImageAssetGroup alter column CreateDate datetime2(7) not null
alter table Cofoundry.ImageAssetGroupItem alter column CreateDate datetime2(7) not null
alter table Cofoundry.ImageAssetTag alter column CreateDate datetime2(7) not null

alter table Cofoundry.ModuleUpdate alter column ExecutionDate datetime2(7) not null
alter table Cofoundry.ModuleUpdateError alter column ExecutionDate datetime2(7) not null

alter table Cofoundry.[Page] alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageAccessRule alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageBlockType alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageBlockType alter column UpdateDate datetime2(7) not null
alter table Cofoundry.PageBlockTypeTemplate alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageDirectory alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageDirectoryAccessRule alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageDirectoryLocale alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageGroup alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageAccessRule alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageGroupItem alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageTag alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageTemplate alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageTemplate alter column UpdateDate datetime2(7) not null
alter table Cofoundry.PageTemplateRegion alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageTemplateRegion alter column UpdateDate datetime2(7) not null
alter table Cofoundry.PageVersion alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageVersionBlock alter column CreateDate datetime2(7) not null
alter table Cofoundry.PageVersionBlock alter column UpdateDate datetime2(7) not null
alter table Cofoundry.RewriteRule alter column CreateDate datetime2(7) not null
alter table Cofoundry.Setting alter column CreateDate datetime2(7) not null
alter table Cofoundry.Setting alter column UpdateDate datetime2(7) not null
alter table Cofoundry.Tag alter column CreateDate datetime2(7) not null
alter table Cofoundry.[User] alter column CreateDate datetime2(7) not null
alter table Cofoundry.[User] alter column LastPasswordChangeDate datetime2(7) not null
alter table Cofoundry.[User] alter column PreviousLoginDate datetime2(7) null
alter table Cofoundry.[User] alter column LastLoginDate datetime2(7) null

go

/*
	#468: Email Uniqueness: Support IDNs and improve uniqueness handling
*/

create table Cofoundry.EmailDomain (
	EmailDomainId int not null identity(1,1),
	[Name] nvarchar(255) not null,
	NameHash binary(32) not null,
	CreateDate datetime2(7) not null,

	constraint PK_EmailDomain primary key (EmailDomainId)
)

create unique index UIX_EmailDomain_NameHash on Cofoundry.EmailDomain (NameHash)

alter table Cofoundry.[User] add EmailDomainId int null
alter table Cofoundry.[User] add UniqueEmail nvarchar(150) null
alter table Cofoundry.[User] add UniqueUsername nvarchar(150) null
go

declare @dateNow datetime2(7) = GetUtcDate()
insert into Cofoundry.EmailDomain ([Name], NameHash, CreateDate)
select distinct
	lower(right(u.Email, len(u.Email) - charindex('@', u.Email))), 
	HashBytes('SHA2_256', lower(right(u.Email, len(u.Email) - charindex('@', u.Email)))), 
	@dateNow
from Cofoundry.[User] u
where u.Email is not null

update Cofoundry.[User] 
set UniqueUsername = lower([Username]), 
	UniqueEmail = lower([Email]),
	EmailDomainId = (select EmailDomainId from Cofoundry.EmailDomain where [Name] = lower(right(Email, len(Email) - charindex('@', Email))))

go
alter table Cofoundry.[User] alter column UniqueUsername nvarchar(150) not null
drop index UIX_User_Email on Cofoundry.[User]
create unique index UIX_User_UniqueUsername on Cofoundry.[User] (UserAreaCode, UniqueUsername) where UniqueUsername is not null and IsDeleted = 0

go

/*
	#478: Users: Security stamp / session invalidation 
*/

alter table Cofoundry.[User] add SecurityStamp nvarchar(max) null
go
update Cofoundry.[User] set SecurityStamp = convert(nvarchar(max), CRYPT_GEN_RANDOM(16), 2)
go
alter table Cofoundry.[User] alter column SecurityStamp nvarchar(max) not null
go


/*
	Tables for IPAddresses
*/

create table Cofoundry.IPAddress (
	IPAddressId bigint identity (1,1) not null,
	[Address] varchar(45) not null,
	CreateDate datetime2(7) not null,

	constraint PK_IPAddress primary key (IPAddressId)
)
go

create unique index UIX_IPAddress_IPAddress on Cofoundry.IPAddress ([Address])

/*
	#485: Users: Confirm Account
*/

alter table Cofoundry.[User] add AccountVerifiedDate datetime2(7) null
go
update Cofoundry.[User] set AccountVerifiedDate = GetUtcDate() where IsEmailConfirmed = 1
go
alter table Cofoundry.[User] drop column IsEmailConfirmed
go

create table Cofoundry.AuthorizedTask (
	AuthorizedTaskId uniqueidentifier not null,
	ClusterId bigint identity(1,1) not null, -- Used in the clustered index to avoid excessive fragmentation. Not otherwise used and can be ignored.
	UserId int not null,
	AuthorizedTaskTypeCode char(6) not null,
	AuthorizationCode varchar(max) not null,
	IPAddressId bigint null,
	TaskData varchar(max) null,
	CreateDate datetime2(7) not null,
	InvalidatedDate datetime2(7) null,
	ExpiryDate datetime2(7) null,
	CompletedDate datetime2(7) null,

	constraint PK_AuthorizedTask primary key nonclustered (AuthorizedTaskId),
	constraint FK_AuthorizedTask_User foreign key (UserId) references Cofoundry.[User] (UserId), 
	constraint FK_AuthorizedTask_IPAddress foreign key (IPAddressId) references Cofoundry.IPAddress (IPAddressId), 
)

go
create unique clustered index CIX_UserAuthorizedTask_ClusterId on Cofoundry.AuthorizedTask (ClusterId)
go

-- Since reset requests have a short lifecycle, we won't migrate them
drop table Cofoundry.UserPasswordResetRequest
go