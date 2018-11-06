/* ******* */
/* SCHEMAS */
/* ******* */

if not exists (select schema_name from information_schema.schemata where schema_name = 'app')
begin
	exec sp_executesql N'create schema app'
end
go

if not exists (select schema_name from information_schema.schemata where schema_name = 'Cofoundry')
begin
	exec sp_executesql N'create schema Cofoundry'
end
go

if not exists (select schema_name from information_schema.schemata where schema_name = 'CofoundryPlugin')
begin
	exec sp_executesql N'create schema CofoundryPlugin'
end
go


/* ****** */
/* TABLES */
/* ****** */


/****** Cofoundry.AutoUpdateLock ******/

create table Cofoundry.AutoUpdateLock (

	IsLocked bit not null,

	constraint PK_AutoUpdateLock primary key (IsLocked)
)


/****** Cofoundry.ModuleUpdate ******/

create table Cofoundry.ModuleUpdate (

	Module varchar(200) not null,
	[Version] int not null,
	[Description] varchar(200) not null,
	ExecutionDate datetime2(4) not null,

	constraint PK_ModuleUpdate primary key (Module,[Version], [Description])
)

create index IX_ModuleUpdate_ModuleVersion on Cofoundry.ModuleUpdate (Module asc, [Version] desc)


/****** Cofoundry.ModuleUpdateError ******/

create table Cofoundry.ModuleUpdateError (

	ModuleUpdateErrorId int identity(1,1) not null,
	Module varchar(200) not null,
	[Version] int null,
	[Description] varchar(200) not null,
	ExecutionDate datetime2(4) not null,
	ExceptionMessage nvarchar(max) null,

	constraint PK_ModuleUpdateError primary key (ModuleUpdateErrorId)
)

create index IX_ModuleUpdateError_Date on Cofoundry.ModuleUpdateError (ExecutionDate desc)


/****** Cofoundry.Locale ******/

create table Cofoundry.Locale (
	LocaleId int identity(1,1) not null,
	ParentLocaleId int null,
	IETFLanguageTag nvarchar(16) not null,
	LocaleName nvarchar(64) not null,
	IsActive bit not null constraint DF_Locale_IsActive  default (0),

	constraint PK_Locale primary key (LocaleId),
	constraint FK_Locale_ParentLocale foreign key (ParentLocaleId) references Cofoundry.Locale (LocaleId)
)


/****** Cofoundry.UserArea ******/

create table Cofoundry.UserArea (

	UserAreaCode char(3) not null,
	Name nvarchar(20) null,

	constraint PK_UserArea primary key (UserAreaCode)
)


/****** Cofoundry.[Role] ******/

create table Cofoundry.[Role] (

	RoleId int identity(1,1) not null,
	Title nvarchar(50) not null,
	SpecialistRoleTypeCode char(3) null,
	UserAreaCode char(3) not null,

	constraint PK_Role primary key (RoleId),
	constraint FK_Role_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode)
)

create unique index UIX_Role_Title on Cofoundry.[Role] (UserAreaCode, Title)
create unique index UIX_Role_SpecialistRoleTypeCode on Cofoundry.[Role] (SpecialistRoleTypeCode) where (SpecialistRoleTypeCode is not null)


/****** Cofoundry.[User] ******/

create table Cofoundry.[User] (

	UserId int identity(1,1) not null,
	FirstName nvarchar(32) not null,
	LastName nvarchar(32) not null,
	Email nvarchar(150) null,
	Username nvarchar(150) not null,
	[Password] nvarchar(max) null,
	CreateDate datetime2(4) not null,
	IsDeleted bit not null constraint DF_User_IsDeleted default (0),
	LastPasswordChangeDate datetime2(4) not null,
	PreviousLoginDate datetime2(4) null,
	LastLoginDate datetime2(4) null,
	RequirePasswordChange bit not null constraint DF_User_RequirePasswordChange default (0),
	CreatorId int null,
	RoleId int not null,
	UserAreaCode char(3) not null,
	PasswordEncryptionVersion int null,

	constraint PK_User primary key (UserId),
	constraint FK_User_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_User_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId),
	constraint FK_User_UserArea foreign key (UserAreaCode) references Cofoundry.UserArea (UserAreaCode),
	constraint CK_CreatorIdNotSelf check (UserId <> CreatorId)
)

create unique index UIX_User_Email on Cofoundry.[User] (UserAreaCode, Email) where (Email is not null AND IsDeleted=0)
create unique index UIX_User_Username on Cofoundry.[User] (UserAreaCode, Username) where (IsDeleted=0)


/****** Cofoundry.EntityDefinition ******/

create table Cofoundry.EntityDefinition (

	EntityDefinitionCode char(6) not null,
	Name varchar(50) not null,

	constraint PK_EntityDefinition primary key (EntityDefinitionCode)
)

create unique index UIX_EntityDefinition_Name on Cofoundry.EntityDefinition (Name)


/****** Cofoundry.CustomEntityDefinition ******/

create table Cofoundry.CustomEntityDefinition (

	CustomEntityDefinitionCode char(6) not null,
	ForceUrlSlugUniqueness bit not null,
	IsOrderable bit not null,
	HasLocale bit not null,

	constraint PK_CustomEntityDefinition primary key (CustomEntityDefinitionCode),
	constraint FK_CustomEntityDefinition_EntityDefinition foreign key (CustomEntityDefinitionCode) references Cofoundry.EntityDefinition (EntityDefinitionCode)
)


/****** Cofoundry.Tag ******/

create table Cofoundry.Tag (

	TagId int identity(1,1) not null,
	TagText nvarchar(32) not null,
	CreateDate datetime2(4) not null,

	constraint PK_Tag primary key (TagId)
)


/****** Cofoundry.WebDirectory ******/

create table Cofoundry.WebDirectory (

	WebDirectoryId int identity(1,1) not null,
	ParentWebDirectoryId int null,
	Name nvarchar(64) not null,
	UrlPath nvarchar(64) not null,
	IsActive bit not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_WebDirectory primary key (WebDirectoryId),
	constraint FK_WebDirectory_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_WebDirectory_ParentWebDirectory foreign key (ParentWebDirectoryId) references Cofoundry.WebDirectory (WebDirectoryId)
)

create unique index UIX_WebDirectory_UrlPath on Cofoundry.WebDirectory (ParentWebDirectoryId, UrlPath) where (IsActive=1)


/****** Cofoundry.WebDirectoryLocale ******/

create table Cofoundry.WebDirectoryLocale (

	WebDirectoryLocaleId int identity(1,1) not null,
	WebDirectoryId int not null,
	LocaleId int not null,
	UrlPath nvarchar(64) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_WebDirectoryLocale primary key (WebDirectoryLocaleId),
	constraint FK_WebDirectoryLocale_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_WebDirectoryLocale_Locale foreign key (LocaleId) references Cofoundry.Locale (LocaleId),
	constraint FK_WebDirectoryLocale_WebDirectory foreign key (WebDirectoryId) references Cofoundry.WebDirectory (WebDirectoryId)
)


/****** Cofoundry.WorkFlowStatus ******/

create table Cofoundry.WorkFlowStatus (

	WorkFlowStatusId int not null,
	Name nvarchar(32) not null,

	constraint PK_WorkFlowStatus primary key (WorkFlowStatusId)
)
go


/****** Cofoundry.DocumentAsset ******/

create table Cofoundry.DocumentAsset (
	DocumentAssetId int identity(1,1) not null,
	[FileName] nvarchar(100) not null,
	Title nvarchar(100) not null,
	[Description] nvarchar(max) not null,
	FileExtension nvarchar(30) not null,
	FileSizeInBytes bigint not null,
	ContentType nvarchar(100) null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	UpdateDate datetime not null,
	IsDeleted bit not null constraint DF_DocumentAsset_IsDeleted  default (0),
	UpdaterId int not null,

	constraint PK_DocumentAsset primary key (DocumentAssetId),
	constraint FK_DocumentAsset_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_DocumentAsset_UpdaterUser foreign key (UpdaterId) references Cofoundry.[User] (UserId)
)


/****** Cofoundry.DocumentAssetGroup ******/

create table Cofoundry.DocumentAssetGroup (

	DocumentAssetGroupId int identity(1,1) not null,
	GroupName nvarchar(64) not null,
	ParentDocumentAssetGroupId int null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	IsDeleted bit not null constraint DF_DocumentAssetGroup_IsDeleted default (0),

	constraint PK_DocumentAssetGroup primary key (DocumentAssetGroupId),
	constraint FK_DocumentAssetGroup_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_DocumentAssetGroup_ParentDocumentAssetGroup foreign key (ParentDocumentAssetGroupId) references Cofoundry.DocumentAssetGroup (DocumentAssetGroupId)
)


/****** Cofoundry.DocumentAssetGroupItem ******/

create table Cofoundry.DocumentAssetGroupItem (

	DocumentAssetId int not null,
	DocumentAssetGroupId int not null,
	CreatorId int not null,
	Ordering int not null,
	CreateDate datetime2(4) not null,

	constraint PK_DocumentAssetGroupItem primary key (DocumentAssetId, DocumentAssetGroupId),
	constraint FK_DocumentAssetGroupItem_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_DocumentAssetGroupItem_DocumentAsset foreign key (DocumentAssetId) references Cofoundry.DocumentAsset (DocumentAssetId),
	constraint FK_DocumentAssetGroupItem_DocumentAssetGroup foreign key (DocumentAssetGroupId) references Cofoundry.DocumentAssetGroup (DocumentAssetGroupId)
)


/****** Cofoundry.DocumentAssetTag ******/

create table Cofoundry.DocumentAssetTag (

	DocumentAssetId int not null,
	TagId int not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_DocumentAssetTag primary key (DocumentAssetId, TagId),
	constraint FK_DocumentAssetTag_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_DocumentAssetTag_DocumentAsset foreign key (DocumentAssetId) references Cofoundry.DocumentAsset (DocumentAssetId),
	constraint FK_DocumentAssetTag_Tag foreign key (TagId) references Cofoundry.Tag (TagId)
)


/****** Cofoundry.FailedAuthenticationAttempt ******/

create table Cofoundry.FailedAuthenticationAttempt (

	UserAreaCode char(3) not null,
	Username nvarchar(150) not null,
	IPAddress varchar(45) not null,
	AttemptDate datetime2(7) not null,
	FailedAuthenticationAttemptId int identity(1,1) not null,

	constraint PK_FailedAuthenticationAttempt primary key (FailedAuthenticationAttemptId)
)

create index IX_FailedAuthenticationAttempt_IPAddress on Cofoundry.FailedAuthenticationAttempt (UserAreaCode, IPAddress, AttemptDate)
create index IX_FailedAuthenticationAttempt_Username on Cofoundry.FailedAuthenticationAttempt (UserAreaCode, Username, AttemptDate)


/****** Cofoundry.ImageCropAnchorLocation ******/

create table Cofoundry.ImageCropAnchorLocation(

	ImageCropAnchorLocationId int not null,
	Title nvarchar(50) not null,

	constraint PK_ImageCropAnchorLocation primary key (ImageCropAnchorLocationId)
)


/****** Cofoundry.ImageAsset ******/

create table Cofoundry.ImageAsset (

	ImageAssetId int identity(1,1) not null,
	[FileName] nvarchar(128) not null,
	FileDescription nvarchar(512) not null,
	Width int not null,
	Height int not null,
	Extension nvarchar(5) not null,
	FileSize int not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	UpdateDate datetime2(4) not null,
	IsDeleted bit not null,
	ImageCropAnchorLocationId int null,
	UpdaterId int not null,

	constraint PK_ImageAsset primary key (ImageAssetId),
	constraint FK_ImageAsset_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_ImageAsset_ImageCropAnchorLocation foreign key (ImageCropAnchorLocationId) references Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId),
	constraint FK_ImageAsset_UpdaterUser foreign key (UpdaterId) references Cofoundry.[User] (UserId)
)


/****** Cofoundry.ImageAssetGroup ******/

create table Cofoundry.ImageAssetGroup (

	ImageAssetGroupId int identity(1,1) not null,
	GroupName nvarchar(64) not null,
	ParentImageAssetGroupId int null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	IsDeleted bit not null constraint DF_ImageAssetGroup_IsDeleted default (0),

	constraint PK_ImageAssetGroup primary key (ImageAssetGroupId),
	constraint FK_ImageAssetGroup_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_ImageAssetGroup_ParentImageAssetGroup foreign key (ParentImageAssetGroupId) references Cofoundry.ImageAssetGroup (ImageAssetGroupId)
)


/****** Cofoundry.ImageAssetGroupItem ******/

create table Cofoundry.ImageAssetGroupItem (

	ImageAssetId int not null,
	ImageAssetGroupId int not null,
	CreatorId int not null,
	CreateDate smalldatetime not null,
	Ordering int not null,

	constraint PK_ImageAssetGroupItem primary key (ImageAssetId, ImageAssetGroupId),
	constraint FK_ImageAssetGroupItem_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_ImageAssetGroupItem_ImageAsset foreign key (ImageAssetId) references Cofoundry.ImageAsset (ImageAssetId),
	constraint FK_ImageAssetGroupItem_ImageAssetGroup foreign key (ImageAssetGroupId) references Cofoundry.ImageAssetGroup (ImageAssetGroupId)
)


/****** Cofoundry.ImageAssetTag ******/

create table Cofoundry.ImageAssetTag (

	ImageAssetId int not null,
	TagId int not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_ImageAssetTag primary key (ImageAssetId, TagId),
	constraint FK_ImageAssetTag_ImageAsset foreign key (ImageAssetId) references Cofoundry.ImageAsset (ImageAssetId),
	constraint FK_ImageAssetTag_Tag foreign key (TagId) references Cofoundry.Tag (TagId),
	constraint FK_ImageAssetTag_User foreign key (CreatorId) references Cofoundry.[User] (UserId)
)


/****** Cofoundry.PageType ******/

create table Cofoundry.PageType (
	PageTypeId int not null,
	Name varchar(50) not null,

	constraint PK_PageType primary key (PageTypeId)
)


/****** Cofoundry.[Page] ******/

create table Cofoundry.[Page] (

	PageId int identity(1,1) not null,
	WebDirectoryId int not null,
	LocaleId int null,
	UrlPath nvarchar(70) not null,
	PageTypeId int not null,
	IsDeleted bit not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	CustomEntityDefinitionCode char(6) null,

	constraint PK_Page primary key (PageId),
	constraint FK_Page_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_Page_CustomEntityDefinition foreign key (CustomEntityDefinitionCode) references Cofoundry.CustomEntityDefinition (CustomEntityDefinitionCode),
	constraint FK_Page_Locale foreign key (LocaleId) references Cofoundry.Locale (LocaleId),
	constraint FK_Page_PageType foreign key (PageTypeId) references Cofoundry.PageType (PageTypeId),
	constraint FK_Page_WebDirectory foreign key (WebDirectoryId) references Cofoundry.WebDirectory (WebDirectoryId),
	constraint CK_Page_CustomEntityDefinition check (PageTypeId <> 2 or CustomEntityDefinitionCode is not null)
)

create unique index UIX_Page_Path on Cofoundry.[Page] (WebDirectoryId, LocaleId, UrlPath) where (IsDeleted=0)


/****** Cofoundry.PageGroup ******/

create table Cofoundry.PageGroup (

	PageGroupId int identity(1,1) not null,
	GroupName nvarchar(64) not null,
	ParentPageGroupId int null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	IsDeleted bit not null constraint DF_PageGroup_IsDeleted default (0),

	constraint PK_PageGroup primary key (PageGroupId),
	constraint FK_PageGroup_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageGroup_ParentPageGroup foreign key (ParentPageGroupId) references Cofoundry.PageGroup (PageGroupId)
)


/****** Cofoundry.PageGroupItem ******/

create table Cofoundry.PageGroupItem (

	PageId int not null,
	PageGroupId int not null,
	Ordering int not null,
	CreatorId int not null,
	CreateDate datetime2(4) not null,

	constraint PK_PageGroupItem primary key (PageId, PageGroupId),
	constraint FK_PageGroupItem_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageGroupItem_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PageGroupItem_PageGroup foreign key (PageGroupId) references Cofoundry.PageGroup (PageGroupId)
)


/****** Cofoundry.PageModuleType ******/

create table Cofoundry.PageModuleType (

	PageModuleTypeId int identity(1,1) not null,
	[Name] nvarchar(32) not null,
	[Description] nvarchar(max) not null,
	[FileName] nvarchar(32) not null,
	IsCustom bit not null constraint DF_PageModuleType_IsCustom  default (0),
	CreateDate datetime2(4) not null,

	constraint PK_PageModuleType primary key (PageModuleTypeId)
)

create unique index UIX_PageModuleType_Name on Cofoundry.PageModuleType([Name])


/****** Cofoundry.PageModuleTypeTemplate ******/

create table Cofoundry.PageModuleTypeTemplate (

	PageModuleTypeTemplateId int identity(1,1) not null,
	PageModuleTypeId int not null,
	[Name] nvarchar(32) not null,
	[FileName] nvarchar(32) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_PageModuleTypeTemplate primary key (PageModuleTypeTemplateId),
	constraint FK_PageModuleTypeTemplate_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageModuleTypeTemplate_PageModuleType foreign key (PageModuleTypeId) references Cofoundry.PageModuleType (PageModuleTypeId)
)


/****** Cofoundry.PageTag ******/

create table Cofoundry.PageTag (

	PageId int not null,
	TagId int not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_PageTag primary key (PageId, TagId),
	constraint FK_PageTag_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageTag_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PageTag_Tag foreign key (TagId) references Cofoundry.Tag (TagId)
)


/****** Cofoundry.PageTemplate ******/

create table Cofoundry.PageTemplate (

	PageTemplateId int identity(1,1) not null,
	[FileName] nvarchar(100) not null,
	[Description] nvarchar(max) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	FullPath nvarchar(400) not null,
	Name nvarchar(100) not null,
	CustomEntityDefinitionCode char(6) null,
	CustomEntityModelType varchar(100) null,
	PageTypeId int not null,

	constraint PK_PageTemplate primary key (PageTemplateId),
	constraint FK_PageTemplate_CustomEntityDefinition foreign key (CustomEntityDefinitionCode) references Cofoundry.CustomEntityDefinition (CustomEntityDefinitionCode),
	constraint FK_PageTemplate_PageType foreign key (PageTypeId) references Cofoundry.PageType (PageTypeId),
	constraint FK_PageTemplate_User foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint CK_PageTemplate_CustomEntityDefinition check (PageTypeId <> 2 or CustomEntityDefinitionCode is not null and CustomEntityModelType is not null)
)

create unique index UIX_PageTemplate_FullPath on Cofoundry.PageTemplate (FullPath)
create unique index UIX_PageTemplate_Name on Cofoundry.PageTemplate ([Name])


/****** Cofoundry.PageTemplateSection ******/

create table Cofoundry.PageTemplateSection (

	PageTemplateSectionId int identity(1,1) not null,
	PageTemplateId int not null,
	Name nvarchar(100) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	IsCustomEntitySection bit not null,

	constraint PK_PageTemplateSection primary key (PageTemplateSectionId),
	constraint FK_PageTemplateSection_PageTemplate foreign key (PageTemplateId) references Cofoundry.PageTemplate (PageTemplateId),
	constraint FK_PageTemplateSection_User foreign key (CreatorId) references Cofoundry.[User] (UserId)
)

create unique index UIX_PageTemplateSection_Name on Cofoundry.PageTemplateSection (PageTemplateId, Name)


/****** Cofoundry.PageTemplateSectionPageModuleType ******/

create table Cofoundry.PageTemplateSectionPageModuleType (

	PageTemplateSectionId int not null,
	PageModuleTypeId int not null,
	
	constraint PK_PageTemplateSectionPageModuleType primary key (PageTemplateSectionId, PageModuleTypeId),
	constraint FK_PageTemplateSectionPageModuleType_PageModuleType foreign key (PageModuleTypeId) references Cofoundry.PageModuleType (PageModuleTypeId),
	constraint FK_PageTemplateSectionPageModuleType_PageTemplateSection foreign key (PageTemplateSectionId) references Cofoundry.PageTemplateSection (PageTemplateSectionId)
)

/****** Cofoundry.PageVersion ******/

create table Cofoundry.PageVersion (

	PageVersionId int identity(1,1) not null,
	PageId int not null,
	PageTemplateId int not null,
	BasedOnPageVersionId int null,
	Title nvarchar(70) not null,
	MetaDescription nvarchar(256) not null,
	MetaKeywords nvarchar(256) not null,
	WorkFlowStatusId int not null,
	IsDeleted bit not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	ExcludeFromSitemap bit not null constraint DF_PageVersion_ExcludeFromSitemap default (0),
	OpenGraphTitle nvarchar(64) null,
	OpenGraphDescription nvarchar(max) null,
	OpenGraphImageId int null,

	constraint PK_PageVersion primary key (PageVersionId),
	constraint FK_PageVersion_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageVersion_OpenGraphImageAsset foreign key (OpenGraphImageId) references Cofoundry.ImageAsset (ImageAssetId),
	constraint FK_PageVersion_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PageVersion_PageTemplate foreign key (PageTemplateId) references Cofoundry.PageTemplate (PageTemplateId),
	constraint FK_PageVersion_PageVersionOf foreign key (BasedOnPageVersionId) references Cofoundry.PageVersion (PageVersionId),
	constraint FK_PageVersion_WorkFlowStatus foreign key (WorkFlowStatusId) references Cofoundry.WorkFlowStatus (WorkFlowStatusId)
)

create unique index UIX_PageVersion_DraftVersion on Cofoundry.PageVersion (PageId) where (WorkFlowStatusId=1 AND IsDeleted=0)
create unique index UIX_PageVersion_PublishedVersion on Cofoundry.PageVersion (PageId) where (WorkFlowStatusId=(4) AND IsDeleted=(0))


/****** Cofoundry.PageVersionModule ******/

create table Cofoundry.PageVersionModule (

	PageVersionModuleId int identity(1,1) not null,
	PageVersionId int not null,
	PageTemplateSectionId int not null,
	PageModuleTypeId int not null,
	SerializedData nvarchar(max) not null,
	Ordering int not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	UpdateDate datetime2(4) not null,
	PageModuleTypeTemplateId int null,

	constraint PK_PageVersionModule primary key (PageVersionModuleId),
	constraint FK_PageVersionModule_CreatorUser foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint FK_PageVersionModule_PageModuleType foreign key (PageModuleTypeId) references Cofoundry.PageModuleType (PageModuleTypeId),
	constraint FK_PageVersionModule_PageModuleTypeTemplate foreign key (PageModuleTypeTemplateId) references Cofoundry.PageModuleTypeTemplate (PageModuleTypeTemplateId),
	constraint FK_PageVersionModule_PageTemplateSection foreign key (PageTemplateSectionId) references Cofoundry.PageTemplateSection (PageTemplateSectionId),
	constraint FK_PageVersionModule_PageVersion foreign key (PageVersionId) references Cofoundry.PageVersion (PageVersionId)
)


/****** Cofoundry.CustomEntity ******/

create table Cofoundry.CustomEntity (

	CustomEntityId int identity(1,1) not null,
	CustomEntityDefinitionCode char(6) not null,
	LocaleId int null,
	UrlSlug nvarchar(200) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,
	Ordering int null,

	constraint PK_CustomEntity primary key (CustomEntityId),
	constraint FK_CustomEntity_CustomEntityDefinition foreign key (CustomEntityDefinitionCode) references Cofoundry.CustomEntityDefinition (CustomEntityDefinitionCode),
	constraint FK_CustomEntity_Locale foreign key (LocaleId) references Cofoundry.Locale (LocaleId)
)


/****** Cofoundry.CustomEntityVersion ******/

create table Cofoundry.CustomEntityVersion (

	CustomEntityVersionId int identity(1,1) not null,
	CustomEntityId int not null,
	Title nvarchar(200) not null,
	WorkFlowStatusId int not null,
	SerializedData nvarchar(max) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_CustomEntityVersion primary key (CustomEntityVersionId),
	constraint FK_CustomEntityVersion_CustomEntity foreign key (CustomEntityId) references Cofoundry.CustomEntity (CustomEntityId),
	constraint FK_CustomEntityVersion_WorkFlowStatus foreign key (WorkFlowStatusId) references Cofoundry.WorkFlowStatus (WorkFlowStatusId)
)

create unique index UIX_CustomEntityVersion_DraftVersion on Cofoundry.CustomEntityVersion (CustomEntityId) where (WorkFlowStatusId=1)
create unique index UIX_CustomEntityVersion_PublishedVersion on Cofoundry.CustomEntityVersion (CustomEntityId) where (WorkFlowStatusId=4)


/****** Cofoundry.CustomEntityVersionPageModule ******/

create table Cofoundry.CustomEntityVersionPageModule (

	CustomEntityVersionPageModuleId int identity(1,1) not null,
	CustomEntityVersionId int not null,
	PageTemplateSectionId int not null,
	PageModuleTypeId int not null,
	SerializedData nvarchar(max) not null,
	Ordering int not null,
	PageModuleTypeTemplateId int null,

	constraint PK_CustomEntityVersionPageModule primary key (CustomEntityVersionPageModuleId),
	constraint FK_CustomEntityVersionPageModule_CustomEntityVersion foreign key (CustomEntityVersionId) references Cofoundry.CustomEntityVersion (CustomEntityVersionId),
	constraint FK_CustomEntityVersionPageModule_PageModuleType foreign key (PageModuleTypeId) references Cofoundry.PageModuleType (PageModuleTypeId),
	constraint FK_CustomEntityVersionPageModule_PageModuleTypeTemplate foreign key (PageModuleTypeTemplateId) references Cofoundry.PageModuleTypeTemplate (PageModuleTypeTemplateId),
	constraint FK_CustomEntityVersionPageModule_PageTemplateSection foreign key (PageTemplateSectionId) references Cofoundry.PageTemplateSection (PageTemplateSectionId)
)


/****** Cofoundry.Permission ******/

create table Cofoundry.Permission (

	PermissionId int identity(1,1) not null,
	EntityDefinitionCode char(6) null,
	PermissionCode char(6) not null,

	constraint PK_Permission primary key (PermissionId),
	constraint FK_Permission_EntityDefinition foreign key (EntityDefinitionCode) references Cofoundry.EntityDefinition (EntityDefinitionCode)
)

create unique index UIX_Permission_PermissionCode on Cofoundry.Permission (EntityDefinitionCode, PermissionCode)


/****** Cofoundry.RolePermission ******/

create table Cofoundry.RolePermission (

	RoleId int not null,
	PermissionId int not null,

	constraint PK_RolePermission primary key (RoleId, PermissionId),
	constraint FK_RolePermission_Permission foreign key (PermissionId) references Cofoundry.Permission (PermissionId),
	constraint FK_RolePermission_Role foreign key (RoleId) references Cofoundry.[Role] (RoleId)
)


/****** Cofoundry.RelatedEntityCascadeAction ******/

create table Cofoundry.RelatedEntityCascadeAction (

	RelatedEntityCascadeActionId int not null,
	Name varchar(25) null,

	constraint PK_RelatedEntityCascadeAction primary key (RelatedEntityCascadeActionId)
)


/****** Cofoundry.RewriteRule ******/

create table Cofoundry.RewriteRule (

	RewriteRuleId int identity(1,1) not null,
	WriteFrom nvarchar(2000) not null,
	WriteTo nvarchar(2000) not null,
	CreateDate datetime2(4) not null,
	CreatorId int not null,

	constraint PK_RewriteRule primary key (RewriteRuleId),
	constraint FK_RewriteRule_User foreign key (CreatorId) references Cofoundry.[User] (UserId),
	constraint CK_RewriteRule_CircularRoute check (WriteFrom <> WriteTo)
)


/****** Cofoundry.Setting ******/

create table Cofoundry.Setting (

	SettingId int identity(1,1) not null,
	SettingKey varchar(30) not null,
	SettingValue nvarchar(max) not null,
	CreateDate datetime2(4) not null,
	UpdateDate datetime2(4) not null,

	constraint PK_Setting primary key (SettingId)
)

create unique index UIX_Setting_SettingKey on Cofoundry.Setting (SettingKey)


/****** Cofoundry.UnstructuredDataDependency ******/

create table Cofoundry.UnstructuredDataDependency (

	RootEntityDefinitionCode char(6) not null,
	RootEntityId int not null,
	RelatedEntityDefinitionCode char(6) not null,
	RelatedEntityId int not null,
	RelatedEntityCascadeActionId int not null,

	constraint PK_UnstructuredDataDependency primary key (RootEntityDefinitionCode, RootEntityId,RelatedEntityDefinitionCode, RelatedEntityId),
	constraint FK_UnstructuredDataDependency_RelatedEntityCascadeAction foreign key (RelatedEntityCascadeActionId) references Cofoundry.RelatedEntityCascadeAction (RelatedEntityCascadeActionId),
	constraint FK_UnstructuredDataDependency_RelatedEntityDefinition foreign key (RelatedEntityDefinitionCode) references Cofoundry.EntityDefinition (EntityDefinitionCode),
	constraint FK_UnstructuredDataDependency_RootEntityDefinition foreign key (RootEntityDefinitionCode) references Cofoundry.EntityDefinition (EntityDefinitionCode),
	constraint CK_UnstructuredDataDependency_NotSelf check (RootEntityDefinitionCode<>RelatedEntityDefinitionCode or RootEntityId<>RelatedEntityId)
)


/****** Cofoundry.UserLoginLog ******/

create table Cofoundry.UserLoginLog (

	UserId int not null,
	IPAddress varchar(45) not null,
	LoginDate datetime2(7) not null,
	UserLoginLogId int identity(1,1) not null,

	constraint PK_UserLoginLog primary key (UserLoginLogId),
	constraint FK_UserLoginLog_User foreign key (UserId) references Cofoundry.[User] (UserId)
)


/****** Cofoundry.UserPasswordResetRequest ******/

create table Cofoundry.UserPasswordResetRequest (

	UserPasswordResetRequestId uniqueidentifier not null,
	UserId int not null,
	Token varchar(max) not null,
	CreateDate datetime2(7) not null,
	IsComplete bit not null,
	IPAddress varchar(45) not null,

	constraint PK_UserPasswordResetRequest primary key (UserPasswordResetRequestId),
	constraint FK_UserPasswordResetRequest_User foreign key (UserId) references Cofoundry.[User] (UserId)
)


/* ****** */
/*  DATA  */
/* ****** */


/****** Cofoundry.AutoUpdateLock ******/

insert Cofoundry.AutoUpdateLock (IsLocked) values (0)


/****** Cofoundry.ImageCropAnchorLocation ******/

insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (1, N'TopLeft')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (2, N'TopCenter')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (4, N'TopRight')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (16, N'MiddleLeft')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (32, N'MiddleCenter')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (64, N'MiddleRight')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (256, N'BottomLeft')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (512, N'BottomCenter')
insert Cofoundry.ImageCropAnchorLocation (ImageCropAnchorLocationId, Title) values (1024, N'BottomRight')

set identity_insert Cofoundry.Locale on 

insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (1, NULL, N'af', N'Afrikaans', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (2, 1, N'af-ZA', N'Afrikaans – South Africa', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (3, NULL, N'sq', N'Albanian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (4, 3, N'sq-AL', N'Albanian – Albania', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (5, NULL, N'ar', N'Arabic', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (6, 5, N'ar-DZ', N'Arabic – Algeria', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (7, 5, N'ar-BH', N'Arabic – Bahrain', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (8, 5, N'ar-EG', N'Arabic – Egypt', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (9, 5, N'ar-IQ', N'Arabic – Iraq', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (10, 5, N'ar-JO', N'Arabic – Jordan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (11, 5, N'ar-KW', N'Arabic – Kuwait', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (12, 5, N'ar-LB', N'Arabic – Lebanon', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (13, 5, N'ar-LY', N'Arabic – Libya', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (14, 5, N'ar-MA', N'Arabic – Morocco', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (15, 5, N'ar-OM', N'Arabic – Oman', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (16, 5, N'ar-QA', N'Arabic – Qatar', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (17, 5, N'ar-SA', N'Arabic – Saudi Arabia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (18, 5, N'ar-SY', N'Arabic – Syria', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (19, 5, N'ar-TN', N'Arabic – Tunisia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (20, 5, N'ar-AE', N'Arabic – United Arab Emirates', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (21, 5, N'ar-YE', N'Arabic – Yemen', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (22, NULL, N'hy', N'Armenian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (23, 22, N'hy-AM', N'Armenian – Armenia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (24, NULL, N'az', N'Azeri', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (25, 24, N'az-AZ-Cyrl', N'Azeri (Cyrillic) – Azerbaijan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (26, 24, N'az-AZ-Latn', N'Azeri (Latin) – Azerbaijan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (27, NULL, N'eu', N'Basque', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (28, 27, N'eu-ES', N'Basque – Basque', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (29, NULL, N'be', N'Belarusian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (30, 29, N'be-BY', N'Belarusian – Belarus', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (31, NULL, N'bg', N'Bulgarian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (32, 31, N'bg-BG', N'Bulgarian – Bulgaria', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (33, NULL, N'ca', N'Catalan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (34, 33, N'ca-ES', N'Catalan – Catalan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (35, NULL, N'zh-HK', N'Chinese – Hong Kong SAR', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (36, NULL, N'zh-MO', N'Chinese – Macao SAR', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (37, NULL, N'zh-CN', N'Chinese – China', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (38, NULL, N'zh-CHS', N'Chinese (Simplified)', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (39, NULL, N'zh-SG', N'Chinese – Singapore', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (40, NULL, N'zh-TW', N'Chinese – Taiwan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (41, NULL, N'zh-CHT', N'Chinese (Traditional)', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (42, NULL, N'hr', N'Croatian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (43, 42, N'hr-HR', N'Croatian – Croatia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (44, NULL, N'cs', N'Czech', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (45, 44, N'cs-CZ', N'Czech – Czech Republic', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (46, NULL, N'da', N'Danish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (47, 46, N'da-DK', N'Danish – Denmark', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (48, NULL, N'div', N'Dhivehi', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (49, 48, N'div-MV', N'Dhivehi – Maldives', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (50, NULL, N'nl', N'Dutch', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (51, 50, N'nl-BE', N'Dutch – Belgium', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (52, 50, N'nl-NL', N'Dutch – The Netherlands', 0)
-- Enable English by default
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (53, NULL, N'en', N'English', 1)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (54, 53, N'en-AU', N'English – Australia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (55, 53, N'en-BZ', N'English – Belize', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (56, 53, N'en-CA', N'English – Canada', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (57, 53, N'en-CB', N'English – Caribbean', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (58, 53, N'en-IE', N'English – Ireland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (59, 53, N'en-JM', N'English – Jamaica', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (60, 53, N'en-NZ', N'English – New Zealand', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (61, 53, N'en-PH', N'English – Philippines', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (62, 53, N'en-ZA', N'English – South Africa', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (63, 53, N'en-TT', N'English – Trinidad and Tobago', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (64, 53, N'en-GB', N'English – United Kingdom', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (65, 53, N'en-US', N'English – United States', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (66, 53, N'en-ZW', N'English – Zimbabwe', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (67, NULL, N'et', N'Estonian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (68, 67, N'et-EE', N'Estonian – Estonia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (69, NULL, N'fo', N'Faroese', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (70, 69, N'fo-FO', N'Faroese – Faroe Islands', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (71, NULL, N'fa', N'Farsi', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (72, 71, N'fa-IR', N'Farsi – Iran', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (73, NULL, N'fi', N'Finnish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (74, 73, N'fi-FI', N'Finnish – Finland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (75, NULL, N'fr', N'French', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (76, 75, N'fr-BE', N'French – Belgium', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (77, 75, N'fr-CA', N'French – Canada', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (78, 75, N'fr-FR', N'French – France', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (79, 75, N'fr-LU', N'French – Luxembourg', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (80, 75, N'fr-MC', N'French – Monaco', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (81, 75, N'fr-CH', N'French – Switzerland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (82, NULL, N'gl', N'Galician', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (83, 81, N'gl-ES', N'Galician – Galician', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (84, NULL, N'ka', N'Georgian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (85, 84, N'ka-GE', N'Georgian – Georgia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (86, NULL, N'de', N'German', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (87, 86, N'de-AT', N'German – Austria', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (88, 86, N'de-DE', N'German – Germany', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (89, 86, N'de-LI', N'German – Liechtenstein', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (90, 86, N'de-LU', N'German – Luxembourg', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (91, 86, N'de-CH', N'German – Switzerland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (92, NULL, N'el', N'Greek', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (93, 92, N'el-GR', N'Greek – Greece', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (94, NULL, N'gu', N'Gujarati', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (95, 94, N'gu-IN', N'Gujarati – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (96, NULL, N'he', N'Hebrew', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (97, 96, N'he-IL', N'Hebrew – Israel', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (98, NULL, N'hi', N'Hindi', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (99, 98, N'hi-IN', N'Hindi – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (100, NULL, N'hu', N'Hungarian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (101, 100, N'hu-HU', N'Hungarian – Hungary', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (102, NULL, N'is', N'Icelandic', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (103, 102, N'is-is', N'Icelandic – Iceland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (104, NULL, N'id', N'Indonesian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (105, 104, N'id-ID', N'Indonesian – Indonesia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (106, NULL, N'it', N'Italian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (107, 106, N'it-IT', N'Italian – Italy', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (108, 106, N'it-CH', N'Italian – Switzerland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (109, NULL, N'ja', N'Japanese', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (110, 109, N'ja-JP', N'Japanese – Japan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (111, NULL, N'kn', N'Kannada', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (112, 111, N'kn-IN', N'Kannada – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (113, NULL, N'kk', N'Kazakh', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (114, 113, N'kk-KZ', N'Kazakh – Kazakhstan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (115, NULL, N'kok', N'Konkani', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (116, 115, N'kok-IN', N'Konkani – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (117, NULL, N'ko', N'Korean', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (118, 117, N'ko-KR', N'Korean – Korea', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (119, NULL, N'ky', N'Kyrgyz', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (120, 119, N'ky-KG', N'Kyrgyz – Kyrgyzstan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (121, NULL, N'lv', N'Latvian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (122, 121, N'lv-LV', N'Latvian – Latvia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (123, NULL, N'lt', N'Lithuanian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (124, 123, N'lt-LT', N'Lithuanian – Lithuania', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (125, NULL, N'mk', N'Macedonian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (126, 125, N'mk-MK', N'Macedonian – Former Yugoslav Republic of Macedonia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (127, NULL, N'ms', N'Malay', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (128, 127, N'ms-BN', N'Malay – Brunei', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (129, NULL, N'ms-MY', N'Malay – Malaysia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (130, NULL, N'mr', N'Marathi', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (131, 130, N'mr-IN', N'Marathi – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (132, NULL, N'mn', N'Mongolian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (133, 132, N'mn-MN', N'Mongolian – Mongolia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (134, NULL, N'no', N'Norwegian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (135, 134, N'nb-NO', N'Norwegian (Bokm?l) – Norway', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (136, 134, N'nn-NO', N'Norwegian (Nynorsk) – Norway', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (137, NULL, N'pl', N'Polish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (138, 137, N'pl-PL', N'Polish – Poland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (139, NULL, N'pt', N'Portuguese', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (140, 139, N'pt-BR', N'Portuguese – Brazil', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (141, 139, N'pt-PT', N'Portuguese – Portugal', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (142, NULL, N'pa', N'Punjabi', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (143, 142, N'pa-IN', N'Punjabi – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (144, NULL, N'ro', N'Romanian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (145, 144, N'ro-RO', N'Romanian – Romania', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (146, NULL, N'ru', N'Russian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (147, 146, N'ru-RU', N'Russian – Russia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (148, NULL, N'sa', N'Sanskrit', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (149, 148, N'sa-IN', N'Sanskrit – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (150, NULL, N'sr-SP-Cyrl', N'Serbian (Cyrillic) – Serbia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (151, NULL, N'sr-SP-Latn', N'Serbian (Latin) – Serbia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (152, NULL, N'sk', N'Slovak', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (153, 152, N'sk-SK', N'Slovak – Slovakia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (154, NULL, N'sl', N'Slovenian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (155, 154, N'sl-SI', N'Slovenian – Slovenia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (156, NULL, N'es', N'Spanish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (157, 156, N'es-AR', N'Spanish – Argentina', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (158, 156, N'es-BO', N'Spanish – Bolivia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (159, 156, N'es-CL', N'Spanish – Chile', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (160, 156, N'es-CO', N'Spanish – Colombia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (161, 156, N'es-CR', N'Spanish – Costa Rica', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (162, 156, N'es-DO', N'Spanish – Dominican Republic', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (163, 156, N'es-EC', N'Spanish – Ecuador', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (164, 156, N'es-SV', N'Spanish – El Salvador', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (165, 156, N'es-GT', N'Spanish – Guatemala', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (166, 156, N'es-HN', N'Spanish – Honduras', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (167, 156, N'es-MX', N'Spanish – Mexico', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (168, 156, N'es-NI', N'Spanish – Nicaragua', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (169, 156, N'es-PA', N'Spanish – Panama', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (170, 156, N'es-PY', N'Spanish – Paraguay', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (171, 156, N'es-PE', N'Spanish – Peru', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (172, 156, N'es-PR', N'Spanish – Puerto Rico', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (173, 156, N'es-ES', N'Spanish – Spain', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (174, 156, N'es-UY', N'Spanish – Uruguay', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (175, 156, N'es-VE', N'Spanish – Venezuela', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (176, NULL, N'sw', N'Swahili', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (177, 176, N'sw-KE', N'Swahili – Kenya', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (178, NULL, N'sv', N'Swedish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (179, 178, N'sv-FI', N'Swedish – Finland', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (180, 178, N'sv-SE', N'Swedish – Sweden', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (181, NULL, N'syr', N'Syriac', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (182, 181, N'syr-SY', N'Syriac – Syria', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (183, NULL, N'ta', N'Tamil', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (184, 183, N'ta-IN', N'Tamil – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (185, NULL, N'tt', N'Tatar', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (186, 185, N'tt-RU', N'Tatar – Russia', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (187, NULL, N'te', N'Telugu', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (188, 187, N'te-IN', N'Telugu – India', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (189, NULL, N'th', N'Thai', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (190, 189, N'th-TH', N'Thai – Thailand', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (191, NULL, N'tr', N'Turkish', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (192, 191, N'tr-TR', N'Turkish – Turkey', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (193, NULL, N'uk', N'Ukrainian', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (194, 193, N'uk-UA', N'Ukrainian – Ukraine', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (195, NULL, N'ur', N'Urdu', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (196, 195, N'ur-PK', N'Urdu – Pakistan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (197, NULL, N'uz', N'Uzbek', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (198, 197, N'uz-UZ-Cyrl', N'Uzbek (Cyrillic) – Uzbekistan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (199, 197, N'uz-UZ-Latn', N'Uzbek (Latin) – Uzbekistan', 0)
insert Cofoundry.Locale (LocaleId, ParentLocaleId, IETFLanguageTag, LocaleName, IsActive) values (200, NULL, N'vi', N'Vietnamese', 0)

set identity_insert Cofoundry.Locale off


/****** Cofoundry.PageModuleType ******/

insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Plain text', N'A simple module when you want just a bit of text. Consider using rich text or raw html if you need more control over the rendering', N'PlainText', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Image', N'', N'Image', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Single line of text', N'A single line of text, no formatting', N'SingleLineText', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Quotation', N'Quotation with optional citation and title', N'Quotation', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Text list', N'Text list items', N'TextList', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Raw html', N'Raw html', N'RawHtml', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Document download', N'Download a document asset', N'Document', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Horizontal line', N'Horizontal line', N'HorizontalLine', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'YouTube video', N'Embed a YouTube video', N'YoutubeVideo', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Vimeo video', N'Embed a Vimeo video', N'VimeoVideo', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'Rich Text', N'Text entry with simple formatting like headings and lists.', N'RichText', 0, GetUtcDate())
insert Cofoundry.PageModuleType (Name, [Description], [FileName], IsCustom, CreateDate) values (N'RichTextWithMedia', N'Text entry with simple formatting and image/video media functionality.', N'RichTextWithMedia', 0, GetUtcDate())


/****** Cofoundry.PageType ******/

insert Cofoundry.PageType (PageTypeId, Name) values (1, N'Generic')
insert Cofoundry.PageType (PageTypeId, Name) values (2, N'CustomEntityDetails')
insert Cofoundry.PageType (PageTypeId, Name) values (5, N'NotFound')


/****** Cofoundry.RelatedEntityCascadeAction ******/

insert Cofoundry.RelatedEntityCascadeAction (RelatedEntityCascadeActionId, Name) values (1, N'None')
insert Cofoundry.RelatedEntityCascadeAction (RelatedEntityCascadeActionId, Name) values (2, N'CascadeProperty')


/****** Cofoundry.UserArea ******/

insert Cofoundry.UserArea (UserAreaCode, Name) values (N'COF', N'Cofoundry')


/****** Cofoundry.[Role] ******/

insert Cofoundry.[Role] (Title, SpecialistRoleTypeCode, UserAreaCode) values (N'Super Administrator', N'SUP', N'COF')


/****** Cofoundry.Setting ******/

insert Cofoundry.Setting (SettingKey, SettingValue, CreateDate, UpdateDate) values ('ApplicationName', N'"Cofoundry"', GetUtcDate(), GetUtcDate())
insert Cofoundry.Setting (SettingKey, SettingValue, CreateDate, UpdateDate) values ('RobotsTxt', N'"User-agent: *"', GetUtcDate(), GetUtcDate())
insert Cofoundry.Setting (SettingKey, SettingValue, CreateDate, UpdateDate) values ('HumansTxt', N'""', GetUtcDate(), GetUtcDate())
insert Cofoundry.Setting (SettingKey, SettingValue, CreateDate, UpdateDate) values ('GoogleAnalyticsUAId', N'""', GetUtcDate(), GetUtcDate())
insert Cofoundry.Setting (SettingKey, SettingValue, CreateDate, UpdateDate) values ('IsSetup', N'false', GetUtcDate(), GetUtcDate())


/****** Cofoundry.WorkFlowStatus ******/

insert Cofoundry.WorkFlowStatus (WorkFlowStatusId, Name) values (1, N'Draft')
insert Cofoundry.WorkFlowStatus (WorkFlowStatusId, Name) values (2, N'Waiting Approval')
insert Cofoundry.WorkFlowStatus (WorkFlowStatusId, Name) values (3, N'Rejected')
insert Cofoundry.WorkFlowStatus (WorkFlowStatusId, Name) values (4, N'Published')
insert Cofoundry.WorkFlowStatus (WorkFlowStatusId, Name) values (5, N'Approved')
