create trigger Cofoundry.User_CascadeDelete
   on  Cofoundry.[User]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFUSR'

	-- Dependencies
    delete Cofoundry.UserLoginLog
	from Cofoundry.UserLoginLog e
	inner join deleted d on e.UserId = d.UserId

    delete Cofoundry.AuthorizedTask
	from Cofoundry.AuthorizedTask e
	inner join deleted d on e.UserId = d.UserId

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.UserId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.UserId and RelatedEntityDefinitionCode = @DefinitionCode)
	
	-- Main Table
    delete Cofoundry.[User]
	from Cofoundry.[User] e
	inner join deleted d on e.UserId = d.UserId

end