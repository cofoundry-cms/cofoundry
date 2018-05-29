create procedure Cofoundry.CustomEntity_ReOrder
	(
		@CustomEntityDefinitionCode char(6),
		@CustomEntityIds varchar(max),
		@LocaleId int,
		@UpdatedIds varchar(max) output
		)
	as
begin

	-- Need to first convert the ids and set the ordering
	declare @AllOrderedIds table
	(
		CustomEntityId int not null, 
		Ordering int identity (1, 1) not null
	)

	-- can't do any modifications here because we dont want to affect the natural ordering
	insert into @AllOrderedIds (CustomEntityId) 
	select i.number
	from  (select number from Cofoundry.IntListToTbl(@CustomEntityIds)) i
			
	-- Create entity ordering
	declare @OrderedEntities table
	(
		CustomEntityId int not null, 
		Ordering int identity (1, 1) not null,
		OriginalOrdering int null
	)

	insert into @OrderedEntities (CustomEntityId, OriginalOrdering) 
	select c.CustomEntityId, min(c.Ordering)
	from @AllOrderedIds i
	inner join Cofoundry.CustomEntity c on c.CustomEntityId = i.CustomEntityId 
		and c.CustomEntityDefinitionCode = @CustomEntityDefinitionCode
		and (@LocaleId is null or c.LocaleId = @LocaleId)
	group by c.CustomEntityId
	order by min(i.Ordering)

	-- Find out which entities are having thier ordering rmeoved
	declare @OrderingRemovedEntities table
	(
		CustomEntityId int not null
	)

	insert into @OrderingRemovedEntities
	select c.CustomEntityId
	from Cofoundry.CustomEntity c
	left outer join @OrderedEntities oe on c.CustomEntityId = oe.CustomEntityId
	where CustomEntityDefinitionCode = @CustomEntityDefinitionCode 
		  and oe.CustomEntityId is null
		  and c.Ordering is not null
		  and (@LocaleId is null or c.LocaleId = @LocaleId)

	-- Update the ordering records
	update Cofoundry.CustomEntity 
	set Ordering = oe.Ordering
	from Cofoundry.CustomEntity c
	inner join @OrderedEntities oe on c.CustomEntityId = oe.CustomEntityId
	where IsNull(oe.Ordering, -1) <> IsNull(oe.OriginalOrdering, -1)

	update Cofoundry.CustomEntity 
	set Ordering = null
	from Cofoundry.CustomEntity c
	inner join @OrderingRemovedEntities ore on c.CustomEntityId = ore.CustomEntityId

	-- Return a list of modified entity ids
	select @UpdatedIds = (
		select cast(convert(varchar(10), CustomEntityId) + ',' AS varchar(max))
		from (
			select CustomEntityId
			from @OrderedEntities
			where IsNull(Ordering, -1) <> IsNull(OriginalOrdering, -1)
			union
			select CustomEntityId
			from @OrderingRemovedEntities
			) x
		for xml path ('')
	)

end