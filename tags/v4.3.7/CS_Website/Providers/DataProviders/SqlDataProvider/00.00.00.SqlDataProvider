/************************************************************/
/*****              Initialization Script               *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

if exists (select * from dbo.sysobjects where id = object_id(N'Version') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
  if '{objectQualifier}' <> ''
  begin
    EXECUTE sp_rename N'{databaseOwner}Version', N'{objectQualifier}Version', 'OBJECT'
    EXECUTE sp_rename N'PK_Version', N'PK_{objectQualifier}Version', 'OBJECT' 
    EXECUTE sp_rename N'IX_Version', N'IX_{objectQualifier}Version', 'INDEX' 

    ALTER TABLE {databaseOwner}{objectQualifier}Version ADD CONSTRAINT
	IX_{objectQualifier}Version UNIQUE NONCLUSTERED 
	(
	Major,
	Minor,
	Build
	) ON [PRIMARY]
  end
end
else
begin
  if not exists (select * from dbo.sysobjects where id = object_id(N'{objectQualifier}Version') and OBJECTPROPERTY(id, N'IsTable') = 1)
  begin
    CREATE TABLE {databaseOwner}{objectQualifier}Version (
	  [VersionId] [int] IDENTITY (1, 1) NOT NULL ,
  	  [Major] [int] NOT NULL ,
	  [Minor] [int] NOT NULL ,
	  [Build] [int] NOT NULL ,
	  [CreatedDate] [datetime] NOT NULL 
    ) ON [PRIMARY]

    ALTER TABLE {databaseOwner}{objectQualifier}Version WITH NOCHECK ADD 
	  CONSTRAINT [PK_{objectQualifier}Version] PRIMARY KEY  CLUSTERED 
	  (
		[VersionId]
	  )  ON [PRIMARY] 

    ALTER TABLE {databaseOwner}{objectQualifier}Version ADD CONSTRAINT
	IX_{objectQualifier}Version UNIQUE NONCLUSTERED 
	(
	Major,
	Minor,
	Build
	) ON [PRIMARY]

  end
end
GO

if exists (select * from dbo.sysobjects where id = object_id(N'{objectQualifier}GetDatabaseVersion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}GetDatabaseVersion
GO

create procedure {databaseOwner}{objectQualifier}GetDatabaseVersion

as

select Major,
       Minor,
       Build
from   {objectQualifier}Version 
where  VersionId = ( select max(VersionId) from {objectQualifier}Version )

GO

if exists (select * from dbo.sysobjects where id = object_id(N'{objectQualifier}FindDatabaseVersion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}FindDatabaseVersion
GO

create procedure {databaseOwner}{objectQualifier}FindDatabaseVersion

@Major  int,
@Minor  int,
@Build  int

as

select 1
from   {objectQualifier}Version
where  Major = @Major
and    Minor = @Minor
and    Build = @Build

GO

if exists (select * from dbo.sysobjects where id = object_id(N'{objectQualifier}UpdateDatabaseVersion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}UpdateDatabaseVersion
GO

create procedure {databaseOwner}{objectQualifier}UpdateDatabaseVersion

@Major  int,
@Minor  int,
@Build  int

as

insert into {objectQualifier}Version (
  Major,
  Minor,
  Build,
  CreatedDate
)
values (
  @Major,
  @Minor,
  @Build,
  getdate()
)

GO
