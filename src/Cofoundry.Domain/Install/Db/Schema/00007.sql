/* 
	#225 Increase the size of the CustomEntityModelType field in the PageTemplate 
	table to account for longer namespaces
 */
 alter table Cofoundry.PageTemplate alter column CustomEntityModelType nvarchar(400) null
 go

