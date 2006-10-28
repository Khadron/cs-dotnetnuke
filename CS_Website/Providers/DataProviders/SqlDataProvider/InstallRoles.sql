/**********************************************************************/
/* InstallRoles.SQL                                                   */
/*                                                                    */
/* Installs the tables, triggers and stored procedures necessary for  */
/* supporting the aspnet feature of ASP.Net                           */
/*                                                                    */
/* InstallCommon.sql must be run before running this file.            */
/*
** Copyright Microsoft, Inc. 2002
** All Rights Reserved.
*/
/**********************************************************************/

PRINT '--------------------------------------'
PRINT 'Starting execution of InstallRoles.SQL'
PRINT '--------------------------------------'
GO

-- In the area between the ASP.NET SPECIAL REGION "BEGIN" and "END" marker
-- comments, ASP.NET SQL Registration Tool will optionally:
-- 1. Replace the name of the database in all "USE" statements.
-- 2. Replace the value of the local variable @dbname
-- The replacement happens only in memory when the tool is running.

-- Inside such regions, user can only modify the name of the database.


-- Explicitly set the options that the server stores with the object in sysobjects.status
-- so that it doesn't matter IF the script is run using a DBLib or ODBC based client.
SET QUOTED_IDENTIFIER OFF -- We don't use quoted identifiers
SET ANSI_NULLS ON         -- We don't want (NULL = NULL) == TRUE
GO
SET ANSI_PADDING ON
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Applications')
                  AND (type = 'U')))
BEGIN
  RAISERROR('The table ''aspnet_Applications'' cannot be found. Please use aspnet_regsql.exe for installing ASP.NET application services.', 18, 1)
END

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Users')
                  AND (type = 'U')))
BEGIN
  RAISERROR('The table ''aspnet_Users'' cannot be found. Please use aspnet_regsql.exe for installing ASP.NET application services.', 18, 1)
END

IF (NOT EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Applications_CreateApplication')
               AND (type = 'P')))
BEGIN
  RAISERROR('The stored procedure ''aspnet_Applications_CreateApplication'' cannot be found. Please use aspnet_regsql.exe for installing ASP.NET application services.', 18, 1)
END

IF (NOT EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_CreateUser')
               AND (type = 'P')))
BEGIN
  RAISERROR('The stored procedure ''aspnet_Users_CreateUser'' cannot be found. Please use aspnet_regsql.exe for installing ASP.NET application services.', 18, 1)
END

IF (NOT EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_DeleteUser')
               AND (type = 'P')))
BEGIN
  RAISERROR('The stored procedure ''aspnet_Users_DeleteUser'' cannot be found. Please use aspnet_regsql.exe for installing ASP.NET application services.', 18, 1)
END

/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Roles')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_Roles table...'
  CREATE TABLE dbo.aspnet_Roles (
        ApplicationId    UNIQUEIDENTIFIER    NOT NULL FOREIGN KEY REFERENCES dbo.aspnet_Applications(ApplicationId),
        RoleId           UNIQUEIDENTIFIER    PRIMARY KEY  NONCLUSTERED DEFAULT NEWID(),
        RoleName         NVARCHAR(256)       NOT NULL,
        LoweredRoleName  NVARCHAR(256)       NOT NULL,
        Description      NVARCHAR(256)       )
 CREATE UNIQUE  CLUSTERED  INDEX aspnet_Roles_index1 ON  dbo.aspnet_Roles(ApplicationId, LoweredRoleName)
END
GO

/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_UsersInRoles')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_UsersInRoles table...'
  CREATE TABLE dbo.aspnet_UsersInRoles (
        UserId     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY(UserId, RoleId) FOREIGN KEY REFERENCES dbo.aspnet_Users (UserId),
        RoleId     UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES dbo.aspnet_Roles (RoleId))

  CREATE INDEX aspnet_UsersInRoles_index ON  dbo.aspnet_UsersInRoles(RoleId)
END


/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_IsUserInRole')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_IsUserInRole
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_IsUserInRole
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @RoleName         NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)
    DECLARE @UserId UNIQUEIDENTIFIER
    SELECT  @UserId = NULL
    DECLARE @RoleId UNIQUEIDENTIFIER
    SELECT  @RoleId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(2)

    SELECT  @RoleId = RoleId
    FROM    dbo.aspnet_Roles
    WHERE   LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
        RETURN(3)

    IF (EXISTS( SELECT * FROM dbo.aspnet_UsersInRoles WHERE  UserId = @UserId AND RoleId = @RoleId))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO

/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_GetRolesForUser')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_GetRolesForUser
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_GetRolesForUser
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
    DECLARE @UserId UNIQUEIDENTIFIER
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(1)

    SELECT r.RoleName
    FROM   dbo.aspnet_Roles r, dbo.aspnet_UsersInRoles ur
    WHERE  r.RoleId = ur.RoleId AND r.ApplicationId = @ApplicationId AND ur.UserId = @UserId
    ORDER BY r.RoleName
    RETURN (0)
END
GO

/*************************************************************/
/*************************************************************/
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Roles_CreateRole')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Roles_CreateRole
GO
CREATE PROCEDURE dbo.aspnet_Roles_CreateRole
    @ApplicationName  NVARCHAR(256),
    @RoleName         NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     INT
    SET @ErrorCode = 0

    DECLARE @TranStarted   BIT
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS(SELECT RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId))
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    INSERT INTO dbo.aspnet_Roles
                (ApplicationId, RoleName, LoweredRoleName)
         VALUES (@ApplicationId, @RoleName, LOWER(@RoleName))

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

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

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Roles_DeleteRole')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Roles_DeleteRole
GO

CREATE PROCEDURE dbo.aspnet_Roles_DeleteRole
    @ApplicationName            NVARCHAR(256),
    @RoleName                   NVARCHAR(256),
    @DeleteOnlyIfRoleIsEmpty    BIT
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)

    DECLARE @ErrorCode     INT
    SET @ErrorCode = 0

    DECLARE @TranStarted   BIT
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    DECLARE @RoleId   UNIQUEIDENTIFIER
    SELECT  @RoleId = NULL
    SELECT  @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
    BEGIN
        SELECT @ErrorCode = 1
        GOTO Cleanup
    END
    IF (@DeleteOnlyIfRoleIsEmpty <> 0)
    BEGIN
        IF (EXISTS (SELECT RoleId FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId))
        BEGIN
            SELECT @ErrorCode = 2
            GOTO Cleanup
        END
    END


    DELETE FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DELETE FROM dbo.aspnet_Roles WHERE @RoleId = RoleId  AND ApplicationId = @ApplicationId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

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

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Roles_RoleExists')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Roles_RoleExists
GO

CREATE PROCEDURE dbo.aspnet_Roles_RoleExists
    @ApplicationName  NVARCHAR(256),
    @RoleName         NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(0)
    IF (EXISTS (SELECT RoleName FROM dbo.aspnet_Roles WHERE LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId ))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO

/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_AddUsersToRoles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_AddUsersToRoles
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_AddUsersToRoles
    @ApplicationName        NVARCHAR(256),
    @UserNames              NVARCHAR(4000),
    @RoleNames              NVARCHAR(4000),
    @TimeZoneAdjustment     INT
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)


    DECLARE @TranStarted   BIT
    DECLARE @ErrorCode INT
    SET @ErrorCode   = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    DECLARE @RoleId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @UserName     NVARCHAR(256)
    DECLARE @RoleName     NVARCHAR(256)

    DECLARE @CurrentPosU  INT
    DECLARE @NextPosU     INT
    DECLARE @CurrentPosR  INT
    DECLARE @NextPosR     INT

    SELECT  @CurrentPosU = 1

    WHILE(@CurrentPosU <= LEN(@UserNames))
    BEGIN
        SELECT @NextPosU = CHARINDEX(N',', @UserNames,  @CurrentPosU)
        IF (@NextPosU = 0 OR @NextPosU IS NULL)
            SELECT @NextPosU = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPosU, @NextPosU - @CurrentPosU)
        SELECT @CurrentPosU = @NextPosU+1

        SELECT @CurrentPosR = 1
        WHILE(@CurrentPosR <= LEN(@RoleNames))
        BEGIN
            SELECT @NextPosR = CHARINDEX(N',', @RoleNames,  @CurrentPosR)
            IF (@NextPosR = 0 OR @NextPosR IS NULL)
                SELECT @NextPosR = LEN(@RoleNames) + 1
            SELECT @RoleName = SUBSTRING(@RoleNames, @CurrentPosR, @NextPosR - @CurrentPosR)
            SELECT @CurrentPosR = @NextPosR+1


            SELECT @RoleId = NULL
            SELECT @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId
            IF (@RoleId IS NULL)
            BEGIN
                SELECT @RoleName
                SET @ErrorCode = 2
                GOTO Cleanup
            END

            SELECT @UserId = NULL
            SELECT @UserId = UserId FROM dbo.aspnet_Users WHERE LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId
            IF (@UserId IS NULL)
            BEGIN
                DECLARE @DateTimeNowUTC DATETIME
                EXEC dbo.aspnet_GetUtcDate @TimeZoneAdjustment, @DateTimeNowUTC OUTPUT
                EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @DateTimeNowUTC, @UserId OUTPUT
            END

            IF (EXISTS(SELECT * FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId AND @RoleId = RoleId))
            BEGIN
                SELECT @UserName, @RoleName
                SET @ErrorCode = 3
                GOTO Cleanup
            END
            INSERT INTO dbo.aspnet_UsersInRoles (UserId, RoleId) VALUES(@UserId, @RoleId)
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

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

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_RemoveUsersFromRoles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_RemoveUsersFromRoles
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_RemoveUsersFromRoles
    @ApplicationName  NVARCHAR(256),
    @UserNames        NVARCHAR(4000),
    @RoleNames        NVARCHAR(4000)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)


    DECLARE @TranStarted   BIT
    DECLARE @ErrorCode INT
    SET @ErrorCode   = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0


    DECLARE @RoleId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @UserName     NVARCHAR(256)
    DECLARE @RoleName     NVARCHAR(256)

    DECLARE @CurrentPosU  INT
    DECLARE @NextPosU     INT
    DECLARE @CurrentPosR  INT
    DECLARE @NextPosR     INT

    SELECT  @CurrentPosU = 1

    WHILE(@CurrentPosU <= LEN(@UserNames))
    BEGIN
        SELECT @NextPosU = CHARINDEX(N',', @UserNames,  @CurrentPosU)
        IF (@NextPosU = 0  OR @NextPosU IS NULL)
            SELECT @NextPosU = LEN(@UserNames)+1
        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPosU, @NextPosU - @CurrentPosU)
        SELECT @CurrentPosU = @NextPosU+1

        SELECT @CurrentPosR = 1
        WHILE(@CurrentPosR <= LEN(@RoleNames))
        BEGIN
            SELECT @NextPosR = CHARINDEX(N',', @RoleNames,  @CurrentPosR)
            IF (@NextPosR = 0 OR @NextPosR IS NULL)
                SELECT @NextPosR = LEN(@RoleNames)+1
            SELECT @RoleName = SUBSTRING(@RoleNames, @CurrentPosR, @NextPosR - @CurrentPosR)
            SELECT @CurrentPosR = @NextPosR+1

            SELECT @RoleId = NULL
            SELECT @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId
            IF (@RoleId IS NULL)
            BEGIN
                SELECT N'', @RoleName
                SET @ErrorCode = 2
                GOTO Cleanup
            END

            SELECT @UserId = NULL
            SELECT @UserId = UserId FROM dbo.aspnet_Users WHERE LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId
            IF (@UserId IS NULL)
            BEGIN
                SELECT @UserName, N''
                SET @ErrorCode = 1
                GOTO Cleanup
            END

            IF (NOT(EXISTS(SELECT * FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId AND @RoleId = RoleId)))
            BEGIN
                SELECT @UserName, @RoleName
                SET @ErrorCode = 3
                GOTO Cleanup
            END
            DELETE FROM dbo.aspnet_UsersInRoles WHERE (UserId = @UserId AND RoleId = @RoleId)
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

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

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_GetUsersInRoles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_GetUsersInRoles
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_GetUsersInRoles
    @ApplicationName  NVARCHAR(256),
    @RoleName         NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId UNIQUEIDENTIFIER
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId
    ORDER BY u.UserName
    RETURN(0)
END
GO

/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UsersInRoles_FindUsersInRole')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_UsersInRoles_FindUsersInRole
GO

CREATE PROCEDURE dbo.aspnet_UsersInRoles_FindUsersInRole
    @ApplicationName  NVARCHAR(256),
    @RoleName         NVARCHAR(256),
    @UserNameToMatch  NVARCHAR(256)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId UNIQUEIDENTIFIER
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId AND LoweredUserName LIKE LOWER(@UserNameToMatch)
    ORDER BY u.UserName
    RETURN(0)
END
GO

/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Roles_GetAllRoles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Roles_GetAllRoles
GO

CREATE PROCEDURE dbo.aspnet_Roles_GetAllRoles (
    @ApplicationName           NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN
    SELECT RoleName
    FROM   dbo.aspnet_Roles WHERE ApplicationId = @ApplicationId
    ORDER BY RoleName
END
GO

/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'vw_aspnet_Roles')
                  AND (type = 'V')))
BEGIN
  PRINT 'Creating the vw_aspnet_Roles view...'
  EXEC(N'
  CREATE VIEW [dbo].[vw_aspnet_Roles]
  AS SELECT [dbo].[aspnet_Roles].[ApplicationId], [dbo].[aspnet_Roles].[RoleId], [dbo].[aspnet_Roles].[RoleName], [dbo].[aspnet_Roles].[LoweredRoleName], [dbo].[aspnet_Roles].[Description]
  FROM [dbo].[aspnet_Roles]
  ')
END
GO

/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'vw_aspnet_UsersInRoles')
                  AND (type = 'V')))
BEGIN
  PRINT 'Creating the vw_aspnet_UsersInRoles view...'
  EXEC(N'
  CREATE VIEW [dbo].[vw_aspnet_UsersInRoles]
  AS SELECT [dbo].[aspnet_UsersInRoles].[UserId], [dbo].[aspnet_UsersInRoles].[RoleId]
  FROM [dbo].[aspnet_UsersInRoles]
  ')
END
GO

/*************************************************************/
/*************************************************************/

--
--Create Role Manager schema version
--

declare @command nvarchar(4000)
set @command = 'grant execute on [dbo].aspnet_RegisterSchemaVersion to ' + QUOTENAME(user)
exec (@command)
GO

EXEC [dbo].aspnet_RegisterSchemaVersion N'Role Manager', N'1', 1, 1
GO

/*************************************************************/
/*************************************************************/

--
--Create Role Manager roles
--

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Roles_FullAccess'  ) )
EXEC sp_addrole N'aspnet_Roles_FullAccess'

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Roles_BasicAccess'  ) )
EXEC sp_addrole N'aspnet_Roles_BasicAccess'

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Roles_ReportingAccess'  ) )
EXEC sp_addrole N'aspnet_Roles_ReportingAccess'
GO

EXEC sp_addrolemember N'aspnet_Roles_BasicAccess', N'aspnet_Roles_FullAccess'
EXEC sp_addrolemember N'aspnet_Roles_ReportingAccess', N'aspnet_Roles_FullAccess'
GO

--
--Stored Procedure rights for BasicAccess
--
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_IsUserInRole TO aspnet_Roles_BasicAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_GetRolesForUser TO aspnet_Roles_BasicAccess
GRANT EXECUTE ON dbo.aspnet_CheckSchemaVersion TO aspnet_Roles_BasicAccess
GRANT EXECUTE ON dbo.aspnet_RegisterSchemaVersion TO aspnet_Roles_BasicAccess
GRANT EXECUTE ON dbo.aspnet_UnRegisterSchemaVersion TO aspnet_Roles_BasicAccess

--
--Stored Procedure rights for ReportingAccess
--
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_IsUserInRole TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_GetRolesForUser TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_Roles_RoleExists TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_GetUsersInRoles TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_FindUsersInRole TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_Roles_GetAllRoles TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_CheckSchemaVersion TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_RegisterSchemaVersion TO aspnet_Roles_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_UnRegisterSchemaVersion TO aspnet_Roles_ReportingAccess

--
--Additional stored procedure rights for FullAccess
--

GRANT EXECUTE ON dbo.aspnet_Roles_CreateRole TO aspnet_Roles_FullAccess
GRANT EXECUTE ON dbo.aspnet_Roles_DeleteRole TO aspnet_Roles_FullAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_AddUsersToRoles TO aspnet_Roles_FullAccess
GRANT EXECUTE ON dbo.aspnet_UsersInRoles_RemoveUsersFromRoles TO aspnet_Roles_FullAccess

--
--View rights
--
GRANT SELECT ON dbo.vw_aspnet_Applications TO aspnet_Roles_ReportingAccess
GRANT SELECT ON dbo.vw_aspnet_Users TO aspnet_Roles_ReportingAccess

GRANT SELECT ON dbo.vw_aspnet_Roles TO aspnet_Roles_ReportingAccess
GRANT SELECT ON dbo.vw_aspnet_UsersInRoles TO aspnet_Roles_ReportingAccess

GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/

declare @command nvarchar(4000)
set @command = 'revoke execute on [dbo].aspnet_RegisterSchemaVersion from ' + QUOTENAME(user)
exec (@command)
GO

PRINT '---------------------------------------'
PRINT 'Completed execution of InstallRoles.SQL'
PRINT '---------------------------------------'

Drop TABLE dbo.temp_aspnet_Permissions
GO


GO

--WSHA FIX
declare @username sysname
set @username = user
if (@username <> 'dbo')
	begin
		declare @statement nvarchar(2000)
		set @statement = 'sp_addrolemember N''aspnet_Roles_FullAccess'', ''' + @username + ''''
		exec (@statement)
		set @statement = 'sp_addrolemember N''aspnet_Profile_FullAccess'', ''' + @username + ''''
		exec (@statement)
		set @statement = 'sp_addrolemember N''aspnet_Membership_FullAccess'', ''' + @username + ''''
		exec (@statement)
	end
GO
--WSHA FIX

/*************************************************************/
/*************************************************************/
