/****** CofoundryPlugin.Error ******/

create table CofoundryPlugin.Error (

	ErrorId int identity(1,1) not null,
	ExceptionType varchar(max) not null,
	Url varchar(255) null,
	[Source] varchar(100) not null,
	[Target] varchar(255) not null,
	StackTrace varchar(max) not null,
	QueryString varchar(255) null,
	[Session] varchar(max) null,
	Form varchar(max) null,
	[Data] varchar(max) null,
	UserAgent varchar(255) null,
	EmailSent bit not null,
	CreateDate datetime not null,

	constraint PK_Error primary key (ErrorId)
)
