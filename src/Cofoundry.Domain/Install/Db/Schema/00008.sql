-- ImageAsset

-- Remove soft deletes & tidy naming
delete from Cofoundry.ImageAsset where IsDeleted = 1
delete from Cofoundry.DocumentAsset where IsDeleted = 1
go

alter table Cofoundry.ImageAsset drop column IsDeleted
alter table Cofoundry.DocumentAsset drop constraint DF_DocumentAsset_IsDeleted
alter table Cofoundry.DocumentAsset drop column IsDeleted
alter table Cofoundry.ImageAsset alter column FileSize bigint not null
alter table Cofoundry.ImageAsset alter column FileDescription nvarchar(max) not null
alter table Cofoundry.DocumentAsset alter column [Title] nvarchar(130) not null
alter table Cofoundry.DocumentAsset alter column [FileName] nvarchar(130) not null
alter table Cofoundry.ImageAsset alter column [FileName] nvarchar(130) not null
alter table Cofoundry.ImageAsset add [Title] nvarchar(130) null
alter table Cofoundry.ImageAsset alter column Extension nvarchar(30) not null

alter table Cofoundry.ImageAsset add FileUpdateDate datetime2(4) null
alter table Cofoundry.DocumentAsset add FileUpdateDate datetime2(4) null
alter table Cofoundry.DocumentAsset alter column UpdateDate datetime2(4) not null
alter table Cofoundry.DocumentAsset add FileNameOnDisk varchar(50) null
alter table Cofoundry.ImageAsset add FileNameOnDisk varchar(50) null
alter table Cofoundry.ImageAsset add VerificationToken char(6) null
alter table Cofoundry.DocumentAsset add VerificationToken char(6) null
go

update Cofoundry.ImageAsset set 
	-- App command is limited to 120 so there shouldn't be any long data in there
	[Title] = cast(FileDescription as nvarchar(130)), 
	FileUpdateDate = UpdateDate,
	FileNameOnDisk = convert(varchar(50), ImageAssetId),
	-- Verification token is just to prevent enumeration and so doesn't have to be very unique
	VerificationToken = substring(convert(varchar(50), NewId()), 0, 7)

update Cofoundry.DocumentAsset set 
	FileUpdateDate = UpdateDate,
	FileNameOnDisk = convert(varchar(50), DocumentAssetId),
	VerificationToken = substring(convert(varchar(50), NewId()), 0, 7)

go

alter table Cofoundry.ImageAsset drop column [FileDescription]
alter table Cofoundry.ImageAsset alter column [Title] nvarchar(130) not null
alter table Cofoundry.DocumentAsset alter column FileUpdateDate datetime2(4) not null
alter table Cofoundry.ImageAsset alter column FileUpdateDate datetime2(4) not null
alter table Cofoundry.DocumentAsset alter column FileNameOnDisk varchar(50) not null
alter table Cofoundry.ImageAsset alter column FileNameOnDisk varchar(50) not null
alter table Cofoundry.ImageAsset alter column VerificationToken char(6) not null
alter table Cofoundry.DocumentAsset alter column VerificationToken char(6) not null
go

-- Improve naming
exec sp_rename 'Cofoundry.ImageAsset.Width' , 'WidthInPixels', 'column'
go
exec sp_rename 'Cofoundry.ImageAsset.Height' , 'HeightInPixels', 'column'
go
exec sp_rename 'Cofoundry.ImageAsset.Extension' , 'FileExtension', 'column'
go
exec sp_rename 'Cofoundry.ImageAsset.FileSize' , 'FileSizeInBytes', 'column'
go

-- TODO: 
-- When we add a queue, add soft-deleted assets to it
-- Image title is new? Might need to do some UI stuff for that

-- https://github.com/cofoundry-cms/cofoundry/issues/33
-- https://github.com/cofoundry-cms/cofoundry/issues/170