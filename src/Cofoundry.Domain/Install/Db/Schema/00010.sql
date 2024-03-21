/*
	Page access rules should always reference a user area and there shouldn't be
    any data in there without one.
*/

delete from Cofoundry.PageAccessRule where UserAreaCode is null
delete from Cofoundry.PageDirectoryAccessRule where UserAreaCode is null
go
drop index UIX_PageAccessRule_Rule on Cofoundry.PageAccessRule
drop index UIX_PageDirectoryAccessRule_Rule on Cofoundry.PageDirectoryAccessRule

alter table Cofoundry.PageAccessRule alter column UserAreaCode char(3) not null
alter table Cofoundry.PageDirectoryAccessRule alter column UserAreaCode char(3) not null

create unique index UIX_PageAccessRule_Rule on Cofoundry.PageAccessRule (PageId, UserAreaCode, RoleId)
create unique index UIX_PageDirectoryAccessRule_Rule on Cofoundry.PageDirectoryAccessRule (PageDirectoryId, UserAreaCode, RoleId)
