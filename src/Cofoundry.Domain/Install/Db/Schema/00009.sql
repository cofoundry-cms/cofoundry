/* 
	#187 Pages / Directories: Restrict by User Area
*/

-- Action to take when no access e.g.RedirectToLogin, Error, NotFound, RedirectToUrl
create table Cofoundry.AccessRuleViolationAction (
	AccessRuleViolationActionCode char(3) not null,
	Title varchar(20) not null,

	constraint PK_AccessRuleViolationAction primary key (AccessRuleViolationActionCode)
)

-- Used to restrict access to an individual page (access rules inherit from parent directories)
create table Cofoundry.PageAccessRule (
	PageAccessRuleId int identity(1,1) not null,
	PageId int not null,
	UserAreaCode char(3) null,
	RoleId int null,
	AccessRuleViolationActionCode char(3) not null,
	CreateDate datetime2(4) not null,

	constraint PK_PageAccessRule primary key (PageAccessRuleId),
	constraint FK_PageAccessRule_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PageAccessRule_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode),
	constraint FK_PageAccessRule_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId),
	constraint FK_PageAccessRule_AccessRuleViolationAction foreign key (AccessRuleViolationActionCode) references Cofoundry.AccessRuleViolationAction (AccessRuleViolationActionCode)
)

-- Used to restrict access to an individual directory (access rules inherit from parent directories)
create table Cofoundry.PageDirectoryAccessRule (
	PageDirectoryAccessRuleId int identity(1,1) not null,
	PageDirectoryId int not null,
	UserAreaCode char(3) null,
	RoleId int null,
	AccessRuleViolationActionCode char(3) not null,
	CreateDate datetime2(4) not null,

	constraint PK_PageDirectoryAccessRule primary key (PageDirectoryAccessRuleId),
	constraint FK_PageDirectoryAccessRule_Page foreign key (PageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId),
	constraint FK_PageDirectoryAccessRule_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode),
	constraint FK_PageDirectoryAccessRule_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId),
	constraint FK_PageDirectoryAccessRule_AccessRuleViolationAction foreign key (AccessRuleViolationActionCode) references Cofoundry.AccessRuleViolationAction (AccessRuleViolationActionCode)
)


-- The closure table can tell us all the directories that a directory is parented to so we can check for access rules up the heirarchy
create table Cofoundry.PageDirectoryClosure (
	AncestorPageDirectoryId int not null,
	DecendentPageDirectoryId int not null,
	Depth int not null,

	constraint PK_PageDirectoryClosure primary key (AncestorPageDirectoryId, DecendentPageDirectoryId),
	constraint FK_PageDirectoryClosure_AncestorPageDirectory foreign key (AncestorPageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId),
	constraint FK_PageDirectoryClosure_DecendentPageDirectory foreign key (DecendentPageDirectoryId) references Cofoundry.PageDirectory (PageDirectoryId)
)

