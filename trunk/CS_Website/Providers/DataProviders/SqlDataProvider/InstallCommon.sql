/**********************************************************************/
/* InstallCommon.SQL                                                  */
/*                                                                    */
/* Installs the tables, triggers and stored procedures necessary for  */
/* supporting the aspnet feature of ASP.Net                           */
/*
** Copyright Microsoft, Inc. 2003
** All Rights Reserved.
*/
/**********************************************************************/

PRINT '---------------------------------------'
PRINT 'Starting execution of InstallCommon.SQL'
PRINT '---------------------------------------'
GO

-- In the area between the ASP.NET SPECIAL REGION "BEGIN" and "END" marker
-- comments, ASP.NET SQL Registration Tool will optionally:
-- 1. Replace the name of the database in all "USE" statements.
-- 2. Replace the value of the local variable @dbname
-- The replacement happens only in memory when the tool is running.

-- Inside such regions, user can only modify the name of the database.


-- Explicitly set the options that the server stores with the object in sysobjects.status
-- so that it doesn't matter IF the script is run using a DBLib or ODBC based client.
SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON         -- We don't want (NULL = NULL) == TRUE
GO
SET ANSI_PADDING ON
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
-- Create the temporary permission tables and stored procedures
-- TO preserve the permissions of an object.
--
-- We use this method instead of using CREATE (if the object
-- doesn't exist) and ALTER (if the object exists) because the
-- latter one either requires the use of dynamic SQL (which we want to
-- avoid) or writing the body of the object (e.g. an SP or view) twice,
-- once use CREATE and again using ALTER.


IF (OBJECT_ID('dbo.temp_aspnet_Permissions') IS NOT NULL)
BEGIN
    DROP TABLE dbo.temp_aspnet_Permissions
END
GO

CREATE TABLE dbo.temp_aspnet_Permissions
(
    Owner     sysname,
    Object    sysname,
    Grantee   sysname,
    Grantor   sysname,
    ProtectType char(10),
    [Action]    varchar(20),
    [Column]    sysname
)

INSERT INTO dbo.temp_aspnet_Permissions
EXEC sp_helprotect

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Setup_RestorePermissions')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Setup_RestorePermissions
GO

CREATE PROCEDURE [dbo].aspnet_Setup_RestorePermissions
    @name   sysname
AS
BEGIN
    DECLARE @object sysname
    DECLARE @protectType char(10)
    DECLARE @action varchar(20)
    DECLARE @grantee sysname
    DECLARE @cmd nvarchar(500)
    DECLARE c1 CURSOR FORWARD_ONLY FOR
        SELECT Object, ProtectType, [Action], Grantee FROM dbo.temp_aspnet_Permissions where Object = @name

    OPEN c1

    FETCH c1 INTO @object, @protectType, @action, @grantee
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = @protectType + ' ' + @action + ' on ' + @object + ' TO [' + @grantee + ']'
        EXEC (@cmd)
        FETCH c1 INTO @object, @protectType, @action, @grantee
    END

    close c1
    deallocate c1
END
GO


/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Setup_RemoveAllRoleMembers')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Setup_RemoveAllRoleMembers
GO

CREATE PROCEDURE [dbo].aspnet_Setup_RemoveAllRoleMembers
    @name   sysname
AS
BEGIN
    CREATE TABLE #aspnet_RoleMembers
    (
        Group_name      sysname,
        Group_id        smallint,
        Users_in_group  sysname,
        User_id         smallint
    )

    INSERT INTO #aspnet_RoleMembers
    EXEC sp_helpuser @name

    DECLARE @user_id smallint
    DECLARE @cmd nvarchar(500)
    DECLARE c1 CURSOR FORWARD_ONLY FOR
        SELECT User_id FROM #aspnet_RoleMembers

    OPEN c1

    FETCH c1 INTO @user_id
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = 'EXEC sp_droprolemember ' + '''' + @name + ''', ''' + USER_NAME(@user_id) + ''''
        EXEC (@cmd)
        FETCH c1 INTO @user_id
    END

    close c1
    deallocate c1
END
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
-- Create the aspnet_Applications table.

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Applications')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_Applications table...'
  CREATE TABLE [dbo].aspnet_Applications (
    ApplicationName         NVARCHAR(256)               NOT NULL UNIQUE,
    LoweredApplicationName  NVARCHAR(256)               NOT NULL UNIQUE,
    ApplicationId           UNIQUEIDENTIFIER            PRIMARY KEY NONCLUSTERED DEFAULT NEWID(),
    Description             NVARCHAR(256)       )
  CREATE CLUSTERED INDEX aspnet_Applications_Index ON [dbo].aspnet_Applications(LoweredApplicationName)
END
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
-- Create the aspnet_Users table
IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Users')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_Users table...'
  CREATE TABLE [dbo].aspnet_Users (
    ApplicationId    UNIQUEIDENTIFIER    NOT NULL FOREIGN KEY REFERENCES [dbo].aspnet_Applications(ApplicationId),
    UserId           UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWID(),
    UserName         NVARCHAR(256)       NOT NULL,
    LoweredUserName  NVARCHAR(256)	     NOT NULL,
    MobileAlias      NVARCHAR(16)        DEFAULT NULL,
    IsAnonymous      BIT                 NOT NULL DEFAULT 0,
    LastActivityDate DATETIME            NOT NULL)

   CREATE UNIQUE CLUSTERED INDEX aspnet_Users_Index ON [dbo].aspnet_Users(ApplicationId, LoweredUserName)
   CREATE NONCLUSTERED INDEX aspnet_Users_Index2 ON [dbo].aspnet_Users(ApplicationId, LastActivityDate)
END
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
-- Create the aspnet_SchemaVersions table
IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_SchemaVersions')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_SchemaVersions table...'
  CREATE TABLE [dbo].aspnet_SchemaVersions (
    Feature                  NVARCHAR(128)  NOT NULL PRIMARY KEY CLUSTERED( Feature, CompatibleSchemaVersion ),
    CompatibleSchemaVersion  NVARCHAR(128)	NOT NULL,
    IsCurrentVersion         BIT            NOT NULL )
END
GO

/*************************************************************/
/*************************************************************/
------------- Create Stored Procedures
/*************************************************************/
/*************************************************************/
-- RegisterSchemaVersion SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_RegisterSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_RegisterSchemaVersion
GO

CREATE PROCEDURE [dbo].aspnet_RegisterSchemaVersion
    @Feature                   NVARCHAR(128),
    @CompatibleSchemaVersion   NVARCHAR(128),
    @IsCurrentVersion          BIT,
    @RemoveIncompatibleSchema  BIT
AS
BEGIN
    IF( @RemoveIncompatibleSchema = 1 )
    BEGIN
        DELETE FROM dbo.aspnet_SchemaVersions WHERE Feature = LOWER( @Feature )
    END
    ELSE
    BEGIN
        IF( @IsCurrentVersion = 1 )
        BEGIN
            UPDATE dbo.aspnet_SchemaVersions
            SET IsCurrentVersion = 0
            WHERE Feature = LOWER( @Feature )
        END
    END

    INSERT  dbo.aspnet_SchemaVersions( Feature, CompatibleSchemaVersion, IsCurrentVersion )
    VALUES( LOWER( @Feature ), @CompatibleSchemaVersion, @IsCurrentVersion )
END
GO

declare @command nvarchar(4000)

set @command = 'grant execute on [dbo].aspnet_Setup_RestorePermissions to ' + QUOTENAME(user)
exec (@command)
set @command = 'grant execute on [dbo].aspnet_RegisterSchemaVersion to ' + QUOTENAME(user)
exec (@command)
GO

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_RegisterSchemaVersion'
GO

-- Create common schema version
EXEC [dbo].aspnet_RegisterSchemaVersion N'Common', N'1', 1, 1
GO

/*************************************************************/
/*************************************************************/
-- CheckSchemaVersion SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_CheckSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_CheckSchemaVersion
GO

CREATE PROCEDURE [dbo].aspnet_CheckSchemaVersion
    @Feature                   NVARCHAR(128),
    @CompatibleSchemaVersion   NVARCHAR(128)
AS
BEGIN
    IF (EXISTS( SELECT  *
                FROM    dbo.aspnet_SchemaVersions
                WHERE   Feature = LOWER( @Feature ) AND
                        CompatibleSchemaVersion = @CompatibleSchemaVersion ))
        RETURN 0

    RETURN 1
END
GO

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_CheckSchemaVersion'
GO

/*************************************************************/
/*************************************************************/
-- CreateApplication SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Applications_CreateApplication')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Applications_CreateApplication
GO

CREATE PROCEDURE [dbo].aspnet_Applications_CreateApplication
    @ApplicationName      NVARCHAR(256),
    @ApplicationId        UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

    IF(@ApplicationId IS NULL)
    BEGIN
        DECLARE @TranStarted   BIT
        SET @TranStarted = 0
        
        IF( @@TRANCOUNT = 0 )
        BEGIN
	        BEGIN TRANSACTION
	        SET @TranStarted = 1
        END
        ELSE
    	    SET @TranStarted = 0
        
        SELECT  @ApplicationId = ApplicationId 
        FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK) 
        WHERE LOWER(@ApplicationName) = LoweredApplicationName

        IF(@ApplicationId IS NULL)
        BEGIN
            SELECT  @ApplicationId = NEWID()
            INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
        END
        
        
        IF( @TranStarted = 1 )
        BEGIN
            IF(@@ERROR = 0)
            BEGIN
	        SET @TranStarted = 0
	        COMMIT TRANSACTION
            END
            ELSE
            BEGIN 
                SET @TranStarted = 0
                ROLLBACK TRANSACTION
            END
        END
    END
END
GO

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_Applications_CreateApplication'
GO

/*************************************************************/
/*************************************************************/
-- UnRegisterSchemaVersion SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UnRegisterSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_UnRegisterSchemaVersion
GO

CREATE PROCEDURE [dbo].aspnet_UnRegisterSchemaVersion
    @Feature                   NVARCHAR(128),
    @CompatibleSchemaVersion   NVARCHAR(128)
AS
BEGIN
    DELETE FROM dbo.aspnet_SchemaVersions
        WHERE   Feature = LOWER(@Feature) AND @CompatibleSchemaVersion = CompatibleSchemaVersion
END
GO

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_UnRegisterSchemaVersion'
GO

/*************************************************************/
/*************************************************************/
-- CreateUser SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_CreateUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Users_CreateUser
GO

CREATE PROCEDURE [dbo].aspnet_Users_CreateUser
    @ApplicationId    UNIQUEIDENTIFIER,
    @UserName         NVARCHAR(256),
    @IsUserAnonymous  BIT,
    @LastActivityDate DATETIME,
    @UserId           UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    IF( @UserId IS NULL )
        SELECT @UserId = NEWID()
    ELSE
    BEGIN
        IF( EXISTS( SELECT UserId FROM dbo.aspnet_Users
                    WHERE @UserId = UserId ) )
            RETURN -1
    END

    INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
    VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)

    RETURN 0
END
GO

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_Users_CreateUser'
GO

/*************************************************************/
/*************************************************************/
--- DeleteUser SP

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_DeleteUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Users_DeleteUser
GO
CREATE PROCEDURE [dbo].aspnet_Users_DeleteUser
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @TablesToDeleteFrom INT,
    @NumTablesDeletedFrom INT OUTPUT
AS
BEGIN
    DECLARE @UserId               UNIQUEIDENTIFIER
    SELECT  @UserId               = NULL
    SELECT  @NumTablesDeletedFrom = 0

    DECLARE @TranStarted   BIT
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    DECLARE @ErrorCode   INT
    DECLARE @RowCount    INT

    SET @ErrorCode = 0
    SET @RowCount  = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   u.LoweredUserName       = LOWER(@UserName)
        AND u.ApplicationId         = a.ApplicationId
        AND LOWER(@ApplicationName) = a.LoweredApplicationName

    IF (@UserId IS NULL)
    BEGIN
        GOTO Cleanup
    END

    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_Membership') AND (type = 'U'))))
    BEGIN
        DELETE FROM dbo.aspnet_Membership WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
               @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
    IF ((@TablesToDeleteFrom & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_UsersInRoles') AND (type = 'U'))) )
    BEGIN
        DELETE FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
    IF ((@TablesToDeleteFrom & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_Profile') AND (type = 'U'))) )
    BEGIN
        DELETE FROM dbo.aspnet_Profile WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
    IF ((@TablesToDeleteFrom & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_PersonalizationPerUser') AND (type = 'U'))) )
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (@TablesToDeleteFrom & 2) <> 0 AND
        (@TablesToDeleteFrom & 4) <> 0 AND
        (@TablesToDeleteFrom & 8) <> 0 AND
        (EXISTS (SELECT UserId FROM dbo.aspnet_Users WHERE @UserId = UserId)))
    BEGIN
        DELETE FROM dbo.aspnet_Users WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:
    SET @NumTablesDeletedFrom = 0

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
	    ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_GetUtcDate')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_GetUtcDate
GO

/*************************************************************/

DECLARE @ver INT
DECLARE @version NCHAR(100)
DECLARE @dot INT
DECLARE @hyphen INT
DECLARE @SqlToExec NCHAR(400)

SELECT @ver = 8
SELECT @version = @@Version
SELECT @hyphen  = CHARINDEX(N' - ', @version)
IF (NOT(@hyphen IS NULL) AND @hyphen > 0)
BEGIN
    SELECT @hyphen = @hyphen + 3
    SELECT @dot    = CHARINDEX(N'.', @version, @hyphen)
    IF (NOT(@dot IS NULL) AND @dot > @hyphen)
    BEGIN
        SELECT @version = SUBSTRING(@version, @hyphen, @dot - @hyphen)
        SELECT @ver     = CONVERT(INT, @version)
    END
END


/*************************************************************/

IF (@ver < 8)
    SET @SqlToExec = N'
CREATE PROCEDURE [dbo].aspnet_GetUtcDate
    @TimeZoneAdjustment INT,
    @DateNow            DATETIME OUTPUT
AS
BEGIN
    SELECT @DateNow = DATEADD(n, -@TimeZoneAdjustment, GETDATE())
END
'
ELSE
    SET @SqlToExec = N'
CREATE PROCEDURE [dbo].aspnet_GetUtcDate
    @TimeZoneAdjustment INT,
    @DateNow            DATETIME OUTPUT
AS
BEGIN
    SELECT @DateNow = GETUTCDATE()
END
'

EXEC (@SqlToExec)
GO

IF (OBJECT_ID('tempdb.#AspstateVer') IS NOT NULL)
BEGIN
    DROP TABLE #AspstateVer
END
GO

/*************************************************************/
/*************************************************************/

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_Users_DeleteUser'
GO

EXEC [dbo].aspnet_Setup_RestorePermissions N'aspnet_GetUtcDate'
GO
/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'vw_aspnet_Applications')
                  AND (type = 'V')))
BEGIN
  PRINT 'Creating the vw_aspnet_Applications view...'
  EXEC('
  CREATE VIEW [dbo].[vw_aspnet_Applications]
  AS SELECT [dbo].[aspnet_Applications].[ApplicationName], [dbo].[aspnet_Applications].[ApplicationId], [dbo].[aspnet_Applications].[Description]
  FROM [dbo].[aspnet_Applications]
  ')
END

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'vw_aspnet_Applications'
GO

/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'vw_aspnet_Users')
                  AND (type = 'V')))
BEGIN
  PRINT 'Creating the vw_aspnet_Users view...'
  EXEC('
  CREATE VIEW [dbo].[vw_aspnet_Users]
  AS SELECT [dbo].[aspnet_Users].[ApplicationId], [dbo].[aspnet_Users].[UserId], [dbo].[aspnet_Users].[UserName], [dbo].[aspnet_Users].[LoweredUserName], [dbo].[aspnet_Users].[MobileAlias], [dbo].[aspnet_Users].[IsAnonymous], [dbo].[aspnet_Users].[LastActivityDate]
  FROM [dbo].[aspnet_Users]
  ')
END

-- Restore the permissions
EXEC [dbo].aspnet_Setup_RestorePermissions N'vw_aspnet_Users'
GO

/*************************************************************/
/*************************************************************/
declare @command nvarchar(4000)

set @command = 'revoke execute on [dbo].aspnet_Setup_RestorePermissions from ' + QUOTENAME(user)
exec (@command)
set @command = 'revoke execute on [dbo].aspnet_RegisterSchemaVersion from ' + QUOTENAME(user)
exec (@command)
GO

PRINT '----------------------------------------'
PRINT 'Completed execution of InstallCommon.SQL'
PRINT '----------------------------------------'

