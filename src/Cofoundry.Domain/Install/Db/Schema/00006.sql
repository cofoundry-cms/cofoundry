/* 
	Refactoring of WorkFlowStatus to improve speed of content version lookups and open
	the door to future workflow features and improvements.
 */

 -- Old WorkFlowStatusIds:
 -- 1 Draft
 -- 2 approval
 -- 3 Rejected
 -- 4 Published
 -- 5 Approved


 -- ** ValidateData **

 if (exists (
	 select * from Cofoundry.[Page] p 
	 left outer join Cofoundry.PageVersion pv on pv.PageId = p.PageId
	 where pv.WorkFlowStatusId in (1, 4) and PageVersionId is null
	 )) throw 50000, 'Invalid page version data. Pages must have at least one draft or published version', 1

 if (exists (
	 select * from Cofoundry.CustomEntity e 
	 left outer join Cofoundry.CustomEntityVersion ev on ev.CustomEntityId = e.CustomEntityId
	 where ev.WorkFlowStatusId in (1, 4) and CustomEntityVersionId is null
	 )) throw 50000, 'Invalid custom entity version data. Custom entities must have at least one draft or published version', 1
	
	 
 -- ** PublishStatus **

create table Cofoundry.PublishStatus (
	PublishStatusCode char(1) not null,
	[Name] varchar(20) not null,

	constraint PK_PublishStatus primary key (PublishStatusCode)
)
go

create unique index UIX_PublishStatus_Name on Cofoundry.PublishStatus ([Name])

go

insert into Cofoundry.PublishStatus (PublishStatusCode, [Name]) values ('U', 'Unpublished')
insert into Cofoundry.PublishStatus (PublishStatusCode, [Name]) values ('P', 'Published')

go

alter table Cofoundry.[Page] add PublishStatusCode char(1) null
alter table Cofoundry.[Page] add PublishDate datetime2(7) null
alter table Cofoundry.[Page] add constraint FK_Page_PublishStatus foreign key (PublishStatusCode) references Cofoundry.PublishStatus (PublishStatusCode)

alter table Cofoundry.[CustomEntity] add PublishStatusCode char(1) null
alter table Cofoundry.[CustomEntity] add PublishDate datetime2(7) null
alter table Cofoundry.[CustomEntity] add constraint FK_CustomEntity_PublishStatus foreign key (PublishStatusCode) references Cofoundry.PublishStatus (PublishStatusCode)

go

-- ** Update PublishStatus: Pages **

;with CTE_LatestPageVersions as
(
   select PageId, WorkFlowStatusId, CreateDate,
         row_number() over (partition by PageId order by WorkFlowStatusId desc) as RowNumber
   from Cofoundry.PageVersion
   where WorkFlowStatusId in (1,4) and IsDeleted = 0
)
update Cofoundry.[Page] 
set PublishStatusCode = case when WorkFlowStatusId = 4 then 'P' else 'U' end,
	PublishDate = case when WorkFlowStatusId = 4 then pv.CreateDate else null end
from Cofoundry.[Page] p
left outer join CTE_LatestPageVersions pv on p.PageId = pv.PageId
where RowNumber = 1


-- ** Update PublishStatus: CustomEntities **

;with CTE_LatestCustomEntityVersions as
(
   select CustomEntityId, WorkFlowStatusId, CreateDate,
         row_number() over (partition by CustomEntityId order by WorkFlowStatusId desc) as RowNumber
   from Cofoundry.CustomEntityVersion
   where WorkFlowStatusId in (1,4)
)
update Cofoundry.CustomEntity 
set PublishStatusCode = case when WorkFlowStatusId = 4 then 'P' else 'U' end,
	PublishDate = case when WorkFlowStatusId = 4 then ev.CreateDate else null end
from Cofoundry.CustomEntity e
left outer join CTE_LatestCustomEntityVersions ev on e.CustomEntityId = ev.CustomEntityId
where RowNumber = 1

go

alter table Cofoundry.[Page] alter column PublishStatusCode char(1) not null
alter table Cofoundry.[CustomEntity] alter column PublishStatusCode char(1) not null
alter table Cofoundry.[Page] add constraint CK_Page_PublishDate_SetWhenPublished check ((PublishStatusCode = 'P' and PublishDate is not null) or PublishStatusCode = 'U') 
alter table Cofoundry.[CustomEntity] add constraint CK_CustomEntity_PublishDate_SetWhenPublished check ((PublishStatusCode = 'P' and PublishDate is not null) or PublishStatusCode = 'U') 
go

-- ** Prepare for WorkFlowStatus table changes **
-- Cutting down to only Draft/Publish status, which will be expanded upon in a future version
-- At some point you coud see other status in here like approved/rejected etc

-- drop this index, we're only ever concerned with the latest published version so Published/approved status is the same
drop index UIX_PageVersion_PublishedVersion on Cofoundry.PageVersion
drop index UIX_CustomEntityVersion_PublishedVersion on Cofoundry.CustomEntityVersion

go

update Cofoundry.PageVersion set WorkFlowStatusId = 4 where WorkFlowStatusId = 5
update Cofoundry.PageVersion set WorkFlowStatusId = 1 where WorkFlowStatusId in (2, 3) -- unused, but just to be sure
update Cofoundry.CustomEntityVersion set WorkFlowStatusId = 4 where WorkFlowStatusId = 5
update Cofoundry.CustomEntityVersion set WorkFlowStatusId = 1 where WorkFlowStatusId in (2, 3) -- unused, but just to be sure

delete from Cofoundry.WorkFlowStatus where WorkFlowStatusId not in (1, 4)

go

-- ** WorkFlowStatus changes **
-- WorkFlowStatus like Approved, Rejected etc are ultimately still considered unpublished/draft so we link the workflow status back to the base PublishStatus

alter table Cofoundry.WorkFlowStatus add PublishStatusCode char(1) null
alter table Cofoundry.WorkFlowStatus add constraint FK_WorkFlowStatus_PublishStatus foreign key (PublishStatusCode) references Cofoundry.PublishStatus (PublishStatusCode)

go

update Cofoundry.WorkFlowStatus set PublishStatusCode = 'U' where WorkFlowStatusId = 1
update Cofoundry.WorkFlowStatus set PublishStatusCode = 'P' where WorkFlowStatusId = 4

go

alter table Cofoundry.WorkFlowStatus alter column PublishStatusCode char(1) not null

go

-- ** PublishStatusQuery **

create table Cofoundry.PublishStatusQuery (
	PublishStatusQueryId smallint not null,
	[Name] varchar(20) not null,

	constraint PK_PublishStatusQuery primary key (PublishStatusQueryId)
)
go

create unique index UIX_PublishStatusQuery_Name on Cofoundry.PublishStatusQuery ([Name])
go

-- ** EntityPublishStatusQuery **

create table Cofoundry.PagePublishStatusQuery (
	PageId int not null,
	PublishStatusQueryId smallint not null,
	PageVersionId int not null,

	constraint PK_PagePublishStatusQuery primary key (PageId, PublishStatusQueryId),
	constraint FK_PagePublishStatusQuery_Page foreign key (PageId) references Cofoundry.[Page] (PageId),
	constraint FK_PagePublishStatusQuery_PageVersion foreign key (PageVersionId) references Cofoundry.PageVersion (PageVersionId),
	constraint FK_PagePublishStatusQuery_PublishStatusQuery foreign key (PublishStatusQueryId) references Cofoundry.PublishStatusQuery (PublishStatusQueryId)
)

create table Cofoundry.CustomEntityPublishStatusQuery (
	CustomEntityId int not null,
	PublishStatusQueryId smallint not null,
	CustomEntityVersionId int not null,

	constraint PK_CustomEntityPublishStatusQuery primary key (CustomEntityId, PublishStatusQueryId),
	constraint FK_CustomEntityPublishStatusQuery_Page foreign key (CustomEntityId) references Cofoundry.CustomEntity (CustomEntityId),
	constraint FK_CustomEntityPublishStatusQuery_PageVersion foreign key (CustomEntityVersionId) references Cofoundry.CustomEntityVersion (CustomEntityVersionId),
	constraint FK_CustomEntityPublishStatusQuery_PublishStatusQuery foreign key (PublishStatusQueryId) references Cofoundry.PublishStatusQuery (PublishStatusQueryId)
)

go

insert into Cofoundry.PublishStatusQuery (PublishStatusQueryId, [Name]) values (0, 'Published') 
insert into Cofoundry.PublishStatusQuery (PublishStatusQueryId, [Name]) values (1, 'Latest') 
insert into Cofoundry.PublishStatusQuery (PublishStatusQueryId, [Name]) values (2, 'Draft') 
insert into Cofoundry.PublishStatusQuery (PublishStatusQueryId, [Name]) values (3, 'PreferPublished') 


-- PublishStatusQuery.Published
;with CTE_LatestPublishedPageVersions as
(
	select v.PageId, v.PageVersionId,
		row_number() over (partition by v.PageId order by v.CreateDate desc) as RowNumber
	from Cofoundry.PageVersion v
	inner join Cofoundry.[Page] p on p.PageId = v.PageId
	where WorkFlowStatusId = 4 and p.PublishStatusCode = 'P' and v.IsDeleted = 0 and p.IsDeleted = 0
)
insert into Cofoundry.PagePublishStatusQuery (PageId, PublishStatusQueryId, PageVersionId)
select PageId, 0, PageVersionId
from CTE_LatestPublishedPageVersions
where RowNumber = 1

;with CTE_LatestPublishedCustomEntityVersions as
(
	select v.CustomEntityId, v.CustomEntityVersionId,
		row_number() over (partition by v.CustomEntityId order by v.CreateDate desc) as RowNumber
	from Cofoundry.CustomEntityVersion v
	inner join Cofoundry.CustomEntity e on e.CustomEntityId = v.CustomEntityId and e.PublishStatusCode = 'P'
	where WorkFlowStatusId = 4
)
insert into Cofoundry.CustomEntityPublishStatusQuery (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
select CustomEntityId, 0, CustomEntityVersionId
from CTE_LatestPublishedCustomEntityVersions
where RowNumber = 1


-- PublishStatusQuery.Latest
;with CTE_LatestPageVersions as
(
	select v.PageId, v.PageVersionId,
			row_number() over (partition by v.PageId order by v.WorkFlowStatusId, v.CreateDate desc) as RowNumber
	from Cofoundry.PageVersion v
	inner join Cofoundry.[Page] p on p.PageId = v.PageId
	where v.IsDeleted = 0 and p.IsDeleted = 0
)
insert into Cofoundry.PagePublishStatusQuery (PageId, PublishStatusQueryId, PageVersionId)
select PageId, 1, PageVersionId
from CTE_LatestPageVersions
where RowNumber = 1

;with CTE_LatestCustomEntityVersions as
(
   select CustomEntityId, CustomEntityVersionId,
         row_number() over (partition by CustomEntityId order by WorkFlowStatusId, CreateDate desc) as RowNumber
   from Cofoundry.CustomEntityVersion
)
insert into Cofoundry.CustomEntityPublishStatusQuery (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
select CustomEntityId, 1, CustomEntityVersionId
from CTE_LatestCustomEntityVersions
where RowNumber = 1


-- PublishStatusQuery.Draft
;with CTE_LatestDraftPageVersions as
(
	select v.PageId, v.PageVersionId,
			row_number() over (partition by v.PageId order by v.CreateDate desc) as RowNumber
	from Cofoundry.PageVersion v
	inner join Cofoundry.[Page] p on p.PageId = v.PageId
	where WorkFlowStatusId = 1 and v.IsDeleted = 0 and p.IsDeleted = 0
)
insert into Cofoundry.PagePublishStatusQuery (PageId, PublishStatusQueryId, PageVersionId)
select PageId, 2, PageVersionId
from CTE_LatestDraftPageVersions
where RowNumber = 1

;with CTE_LatestDraftCustomEntityVersions as
(
   select CustomEntityId, CustomEntityVersionId,
         row_number() over (partition by CustomEntityId order by CreateDate desc) as RowNumber
   from Cofoundry.CustomEntityVersion
   where WorkFlowStatusId = 1
)
insert into Cofoundry.CustomEntityPublishStatusQuery (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
select CustomEntityId, 2, CustomEntityVersionId
from CTE_LatestDraftCustomEntityVersions
where RowNumber = 1


-- PublishStatusQuery.PreferPublished

;with CTE_PreferPublishedPageVersions as
(
	select v.PageId, v.PageVersionId,
			row_number() over (partition by v.PageId order by v.WorkFlowStatusId desc, v.CreateDate desc) as RowNumber
	from Cofoundry.PageVersion v
	inner join Cofoundry.[Page] p on p.PageId = v.PageId
	where WorkFlowStatusId in (1, 4) and v.IsDeleted = 0 and p.IsDeleted = 0
)
insert into Cofoundry.PagePublishStatusQuery (PageId, PublishStatusQueryId, PageVersionId)
select PageId, 3, PageVersionId
from CTE_PreferPublishedPageVersions
where RowNumber = 1

;with CTE_PreferPublishedCustomEntityVersions as
(
   select CustomEntityId, CustomEntityVersionId,
         row_number() over (partition by CustomEntityId order by WorkFlowStatusId desc, CreateDate desc) as RowNumber
   from Cofoundry.CustomEntityVersion
   where WorkFlowStatusId in (1, 4)
)
insert into Cofoundry.CustomEntityPublishStatusQuery (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
select CustomEntityId, 3, CustomEntityVersionId
from CTE_PreferPublishedCustomEntityVersions
where RowNumber = 1

update Cofoundry.[Page] 
set PublishDate = pv.CreateDate
from Cofoundry.[Page] p
inner join Cofoundry.PagePublishStatusQuery q on p.PageId = q.PageId and q.PublishStatusQueryId = 4
inner join Cofoundry.PageVersion pv on pv.PageVersionId = q.PageVersionId
