/****************************************************************************
	The Cofoundry auto-updater can also be used to create stored procedures, 
	triggers and functions. These scripts are run every time there is a version 
	update and are automatically dropped if they exist before the create script 
	is re-run.
*****************************************************************************/

create procedure app.CatLike_SetLiked
	(
		@CatId int,
		@UserId int,
		@IsLiked bit,
		@CreateDate datetime2
	)
	as
begin

	if (@IsLiked = 1)
	begin
		merge app.CatLike as destination
		using (values (@CatId, @UserId, @CreateDate)) src (CatCustomEntityId, UserId, CreateDate)
        on destination.UserId = src.UserId AND destination.CatCustomEntityId = src.CatCustomEntityId
		when not matched then
			insert (CatCustomEntityId, UserId, CreateDate)
			values (src.CatCustomEntityId, src.UserId, src.CreateDate);
	end
	else
	begin
		delete from app.CatLike where CatCustomEntityId = @CatId and UserId = @UserId
	end

	merge app.CatLikeCount as destination
		using (
			select @CatId, Count(UserId) 
			from app.CatLike cl
			right outer join Cofoundry.CustomEntity c on cl.CatCustomEntityId = c.CustomEntityId
			where cl.CatCustomEntityId = @CatId or c.CustomEntityId = @CatId
			group by cl.CatCustomEntityId
		) src (CatCustomEntityId, TotalLikes)
        on destination.CatCustomEntityId = src.CatCustomEntityId
	when matched then 
		update set destination.TotalLikes = src.TotalLikes
    when not matched then
        insert (CatCustomEntityId, TotalLikes)
        values (src.CatCustomEntityId, src.TotalLikes);

end