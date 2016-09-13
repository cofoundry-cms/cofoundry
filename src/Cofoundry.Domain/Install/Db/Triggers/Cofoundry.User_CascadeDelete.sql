create trigger Cofoundry.User_CascadeDelete
   on  Cofoundry.[User]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.UserLoginLog
	from Cofoundry.UserLoginLog e
	inner join deleted d on e.UserId = d.UserId

    delete Cofoundry.UserPasswordResetRequest
	from Cofoundry.UserPasswordResetRequest e
	inner join deleted d on e.UserId = d.UserId
	
	-- Main Table
    delete Cofoundry.[User]
	from Cofoundry.[User] e
	inner join deleted d on e.UserId = d.UserId

end