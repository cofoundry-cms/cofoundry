/* Make first and last name optional and add email confirmation */

alter table Cofoundry.[User] alter column FirstName nvarchar(32) null
alter table Cofoundry.[User] alter column LastName nvarchar(32) null
alter table Cofoundry.[User] add IsEmailConfirmed bit null

go

update Cofoundry.[User] set IsEmailConfirmed = 0

go

alter table Cofoundry.[User] alter column IsEmailConfirmed bit not null

go

