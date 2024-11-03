This cheat sheet gives examples on how to script common tasks in T-SQL. Useful for creating your auto-update scripts if you're not familiar with coding these by hand.

## Create Table

```sql
create table dbo.MyTable ( 
	MyTableId int identity (1, 1) not null,
	[Name] nvarchar(100) not null,
	Description nvarchar(max) null,
	IsSomething bit not null constraint DF_MyTable_IsSomething default(1),
	CreateDate datetime not null,
	CreatorId int not null,
	
	constraint PK_MyTable primary key (MyTableId),
	constraint FK_MyTable_User foreign key (CreatorId) references dbo.[User] (UserId)
)
```

## Columns

```sql
alter table dbo.MyTable alter column Name nvarchar(200) null
alter table dbo.MyTable drop column Name
```

## Constraints

```sql
alter table dbo.MyTable add constraint FK_MyTable_UpdaterUser 
      foreign key (UpdaterId) references dbo.[User] (UserId)
alter table dbo.MyTable add constraint CK_NameNotDescription check (Name <> Description)
alter table dbo.MyTable drop constraint CK_NameNotDescription
```

## Indexes

```sql
create index IX_MyTable_Name on dbo.MyTable (Name)
create unique index UIX_MyTable_Name on dbo.MyTable (Name) where IsSomething = 1
drop index IX_MyTable_Name on dbo.MyTable 
```

## Renaming
```sql
exec sp_rename 'dbo.MyTable', 'MyNewTable'
exec sp_rename 'dbo.MyNewTable.OldName' , 'NewName', 'column'
exec sp_rename 'dbo.FK_MyNewTable_User', 'FK_MyNewTable_CreatorUser', 'object'
exec sp_rename 'dbo.MyNEwTable.IX_MyTable_IndexName', 'IX_MyNewTable_IndexName', 'index'
```

## Change Schema
```sql
alter schema myschema transfer dbo.MyTable
```

## Drop auto-named constraint
```sql
declare @ObjectName nvarchar(100)
select @ObjectName = object_name([default_object_id]) from sys.columns
where [object_id] = object_id('dbo.MyTable') and [name] = 'IsSomething';
exec('alter table dbo.MyTable drop constraint ' + @ObjectName)
```

## Example Cascade Delete Trigger
```sql
create trigger dbo.MyTable_CascadeDelete
   on  dbo.MyTable
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
        delete dbo.OtherTable
	from dbo.OtherTable e
	inner join deleted d on e.OtherTableId = d.MyTableId

	-- Main Table
        delete dbo.MyTable
	from dbo.MyTable e
	inner join deleted d on e.MyTableId = d.MyTableId

end
```

## Object Exists
```sql
exists(select * from information_schema.tables 
       where table_name = 'MyTable'
       and table_schema = 'dbo')

exists(select * from information_schema.columns 
       where table_name = 'MyTable' and column_name = 'CreatorUserId')

exists (select * from sysobjects where id = object_id(N'dbo.FN_MyFunction') and type in (N'FN', N'IF', N'TF', N'FS', N'FT'))

object_id('dbo.PK_MyPrimaryKey', 'PK') is not null
object_id('dbo.FK_MyForeignKey', 'F') is not null
object_id('dbo.CK_MyCheckConstraint', 'C') is not null
object_id('dbo.DF_MyDefaultConstraint', 'D') is not null

exists (select * from sys.indexes 
		 where name='IX_MyTableName' AND object_id = object_id('dbo.MyTable'))
```

## Throw Error

```sql
throw error_number, msg, state;
```
State must be over 50000 (but does not need to be any particular value - its just for your reference). Always use 1 for the state, e.g.

```sql
if (exists(select * from MyTable)) throw 50000, 'Table isnt empty!', 1;
```