/****************************************************************************
	We can use the Cofoundry auto-update framework to upgrade our
	database using custom sql scripts. Cofoundry objects are namespaced
	under the Cofoundry schema, and we create an "app" schema that you can 
	use for your own database objects but you're free to use your own schema
	or the default dbo schema if you prefer.
*****************************************************************************/

-- Table to keep track of user likes

create table app.CatLike (
	CatCustomEntityId int not null,
	UserId int not null,
	CreateDate datetime2(7) not null,

	constraint PK_CatLike primary key (CatCustomEntityId, UserId),
	constraint FK_CatLike_CatCustomEntity foreign key (CatCustomEntityId) references Cofoundry.CustomEntity (CustomEntityId) on delete cascade,
	constraint FK_CatLike_User foreign key (UserId) references Cofoundry.[User] (UserId) on delete cascade
)

-- Table to cache the total number of likes
create table app.CatLikeCount (
	CatCustomEntityId int not null,
	TotalLikes int not null,

	constraint PK_CatLikeCount primary key (CatCustomEntityId),
	constraint FK_CatLikeCount_CatCustomEntity foreign key (CatCustomEntityId) references Cofoundry.CustomEntity (CustomEntityId) on delete cascade
)