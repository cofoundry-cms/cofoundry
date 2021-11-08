﻿/* 
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

/* Add missing foreign key to custom entity table */

alter table Cofoundry.CustomEntity add constraint FK_CustomEntity_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId)
go

/* ***************************************************************************** */
/* Seed the PageDirectoryClosure and PageDirectoryPath tables with existing data */
/* This is a copy of the Cofoundry.PageDirectory_UpdatePath stored procedure     */
/* ***************************************************************************** */

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

