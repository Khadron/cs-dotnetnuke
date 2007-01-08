/**********************************************************************/
/* InstallProfile.SQL                                         */
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

PRINT '------------------------------------------------'
PRINT 'Starting execution of InstallProfile.SQL'
PRINT '------------------------------------------------'
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
/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'aspnet_Profile')
                  AND (type = 'U')))
BEGIN
  PRINT 'Creating the aspnet_Profile table...'
  CREATE TABLE dbo.aspnet_Profile (
        UserId                   UNIQUEIDENTIFIER   PRIMARY KEY FOREIGN KEY REFERENCES dbo.aspnet_Users(UserId),
        PropertyNames            NTEXT NOT NULL,
        PropertyValuesString     NTEXT NOT NULL,
        PropertyValuesBinary     IMAGE NOT NULL,
        LastUpdatedDate          DATETIME NOT NULL)
END
GO

/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Profile_GetProperties')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_GetProperties
GO

CREATE PROCEDURE dbo.aspnet_Profile_GetProperties
    @ApplicationName      NVARCHAR(256),
    @UserName             NVARCHAR(256),
    @TimeZoneAdjustment   INT
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    DECLARE @UserId UNIQUEIDENTIFIER
    SELECT  @UserId = NULL
    DECLARE @DateTimeNowUTC DATETIME
    EXEC dbo.aspnet_GetUtcDate @TimeZoneAdjustment, @DateTimeNowUTC OUTPUT

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)

    IF (@UserId IS NULL)
        RETURN
    SELECT TOP 1 PropertyNames, PropertyValuesString, PropertyValuesBinary
    FROM         dbo.aspnet_Profile
    WHERE        UserId = @UserId

    IF (@@ROWCOUNT > 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate=@DateTimeNowUTC
        WHERE  UserId = @UserId
    END
END
GO

/*************************************************************/
/*************************************************************/

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Profile_SetProperties')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_SetProperties
GO

CREATE PROCEDURE dbo.aspnet_Profile_SetProperties
    @ApplicationName        NVARCHAR(256),
    @PropertyNames          NTEXT,
    @PropertyValuesString   NTEXT,
    @PropertyValuesBinary   IMAGE,
    @UserName               NVARCHAR(256),
    @IsUserAnonymous        BIT,
    @TimeZoneAdjustment     INT
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

    DECLARE @DateTimeNowUTC DATETIME
    EXEC dbo.aspnet_GetUtcDate @TimeZoneAdjustment, @DateTimeNowUTC OUTPUT
    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @LastActivityDate DATETIME
    SELECT  @UserId = NULL
    SELECT @LastActivityDate = @DateTimeNowUTC

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, @IsUserAnonymous, @LastActivityDate, @UserId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS( SELECT *
               FROM   dbo.aspnet_Profile
               WHERE  UserId = @UserId))
        UPDATE dbo.aspnet_Profile
        SET    PropertyNames=@PropertyNames, PropertyValuesString = @PropertyValuesString,
               PropertyValuesBinary = @PropertyValuesBinary, LastUpdatedDate=@DateTimeNowUTC
        WHERE  UserId = @UserId
    ELSE
        INSERT INTO dbo.aspnet_Profile(UserId, PropertyNames, PropertyValuesString, PropertyValuesBinary, LastUpdatedDate)
             VALUES (@UserId, @PropertyNames, @PropertyValuesString, @PropertyValuesBinary, @DateTimeNowUTC)

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Users
    SET    LastActivityDate=@DateTimeNowUTC
    WHERE  UserId = @UserId

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

    RETURN 0

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
             WHERE (name = N'aspnet_Profile_DeleteProfiles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_DeleteProfiles
GO

CREATE PROCEDURE dbo.aspnet_Profile_DeleteProfiles
    @ApplicationName        NVARCHAR(256),
    @UserNames              NVARCHAR(4000)
AS
BEGIN
    DECLARE @UserName     NVARCHAR(256)
    DECLARE @CurrentPos   INT
    DECLARE @NextPos      INT
    DECLARE @NumDeleted   INT
    DECLARE @DeletedUser  INT
    DECLARE @TranStarted  BIT
    DECLARE @ErrorCode    INT

    SET @ErrorCode = 0
    SET @CurrentPos = 1
    SET @NumDeleted = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    WHILE (@CurrentPos <= LEN(@UserNames))
    BEGIN
        SELECT @NextPos = CHARINDEX(N',', @UserNames,  @CurrentPos)
        IF (@NextPos = 0 OR @NextPos IS NULL)
            SELECT @NextPos = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPos, @NextPos - @CurrentPos)
        SELECT @CurrentPos = @NextPos+1

        IF (LEN(@UserName) > 0)
        BEGIN
            SELECT @DeletedUser = 0
            EXEC dbo.aspnet_Users_DeleteUser @ApplicationName, @UserName, 4, @DeletedUser OUTPUT
            IF( @@ERROR <> 0 )
            BEGIN
                SET @ErrorCode = -1
                GOTO Cleanup
            END
            IF (@DeletedUser <> 0)
                SELECT @NumDeleted = @NumDeleted + 1
        END
    END
    SELECT @NumDeleted
    IF (@TranStarted = 1)
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END
    SET @TranStarted = 0

    RETURN 0

Cleanup:
    IF (@TranStarted = 1 )
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
             WHERE (name = N'aspnet_Profile_DeleteInactiveProfiles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_DeleteInactiveProfiles
GO

CREATE PROCEDURE dbo.aspnet_Profile_DeleteInactiveProfiles
    @ApplicationName        NVARCHAR(256),
    @ProfileAuthOptions     INT,
    @InactiveSinceDate      DATETIME,
    @TimeZoneAdjustment     INT
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT  0
        RETURN
    END

    IF (@InactiveSinceDate > CONVERT(DATETIME, '17540101', 112) AND  @InactiveSinceDate < CONVERT(DATETIME, '99980101', 112))
        SELECT @InactiveSinceDate = DATEADD(n, -@TimeZoneAdjustment, @InactiveSinceDate)

    DELETE
    FROM    dbo.aspnet_Profile
    WHERE   UserId IN
            (   SELECT  UserId
                FROM    dbo.aspnet_Users u
                WHERE   ApplicationId = @ApplicationId
                        AND (LastActivityDate <= @InactiveSinceDate)
                        AND (
                                (@ProfileAuthOptions = 2)
                             OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                             OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                            )
            )

    SELECT  @@ROWCOUNT
END
GO

/*************************************************************/
/*************************************************************/
 IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Profile_GetNumberOfInactiveProfiles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_GetNumberOfInactiveProfiles
GO

CREATE PROCEDURE dbo.aspnet_Profile_GetNumberOfInactiveProfiles
    @ApplicationName        NVARCHAR(256),
    @ProfileAuthOptions     INT,
    @InactiveSinceDate      DATETIME,
    @TimeZoneAdjustment     INT
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT 0
        RETURN
    END

    IF (@InactiveSinceDate > CONVERT(DATETIME, '17540101', 112) AND  @InactiveSinceDate < CONVERT(DATETIME,'99980101', 112))
        SELECT @InactiveSinceDate = DATEADD(n, -@TimeZoneAdjustment, @InactiveSinceDate)

    SELECT  COUNT(*)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
    WHERE   ApplicationId = @ApplicationId
        AND u.UserId = p.UserId
        AND (LastActivityDate <= @InactiveSinceDate)
        AND (
                (@ProfileAuthOptions = 2)
                OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
            )
END
GO


/*************************************************************/
/*************************************************************/
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Profile_GetProfiles')
               AND (type = 'P')))
DROP PROCEDURE dbo.aspnet_Profile_GetProfiles
GO

CREATE PROCEDURE dbo.aspnet_Profile_GetProfiles
    @ApplicationName        NVARCHAR(256),
    @ProfileAuthOptions     INT,
    @PageIndex              INT,
    @PageSize               INT,
    @TimeZoneAdjustment     INT,
    @UserNameToMatch        NVARCHAR(256) = NULL,
    @InactiveSinceDate      DATETIME      = NULL
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    IF ((NOT(@InactiveSinceDate IS NULL)) AND (@InactiveSinceDate > CONVERT(DATETIME, '17540101', 112)) AND  (@InactiveSinceDate < CONVERT(DATETIME, '99980101', 112)))
        SELECT @InactiveSinceDate = DATEADD(n, -@TimeZoneAdjustment, @InactiveSinceDate)

    -- Set the page bounds
    DECLARE @PageLowerBound INT
    DECLARE @PageUpperBound INT
    DECLARE @TotalRecords   INT
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId UNIQUEIDENTIFIER
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT  u.UserId
        FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
        WHERE   ApplicationId = @ApplicationId
            AND u.UserId = p.UserId
            AND (@InactiveSinceDate IS NULL OR LastActivityDate <= @InactiveSinceDate)
            AND (     (@ProfileAuthOptions = 2)
                   OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                   OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                 )
            AND (@UserNameToMatch IS NULL OR LoweredUserName LIKE LOWER(@UserNameToMatch))
        ORDER BY UserName

    SELECT  u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate,
            DATALENGTH(p.PropertyNames) + DATALENGTH(p.PropertyValuesString) + DATALENGTH(p.PropertyValuesBinary)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p, #PageIndexForUsers i
    WHERE   u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= @PageLowerBound AND i.IndexId <= @PageUpperBound

    SELECT COUNT(*)
    FROM   #PageIndexForUsers

    DROP TABLE #PageIndexForUsers
END
GO

/*************************************************************/
/*************************************************************/
IF (NOT EXISTS (SELECT name
                FROM sysobjects
                WHERE (name = N'vw_aspnet_Profiles')
                  AND (type = 'V')))
BEGIN
  PRINT 'Creating the vw_aspnet_Profiles view...'
  EXEC(N'
  CREATE VIEW [dbo].[vw_aspnet_Profiles]
  AS SELECT [dbo].[aspnet_Profile].[UserId], [dbo].[aspnet_Profile].[LastUpdatedDate],
      [DataSize]=  DATALENGTH([dbo].[aspnet_Profile].[PropertyNames])
                 + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesString])
                 + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesBinary])
  FROM [dbo].[aspnet_Profile]
  ')
END
GO

/*************************************************************/
/*************************************************************/

--
--Create Profile schema version
--

declare @command nvarchar(4000)
set @command = 'grant execute on [dbo].aspnet_RegisterSchemaVersion to ' + QUOTENAME(user)
exec (@command)
GO

EXEC [dbo].aspnet_RegisterSchemaVersion N'Profile', N'1', 1, 1
GO

/*************************************************************/
/*************************************************************/

--
--Create Profile roles
--

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Profile_FullAccess' ) )
EXEC sp_addrole N'aspnet_Profile_FullAccess'

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Profile_BasicAccess' ) )
EXEC sp_addrole N'aspnet_Profile_BasicAccess'

IF ( NOT EXISTS ( SELECT name
                  FROM sysusers
                  WHERE issqlrole = 1
                  AND name = N'aspnet_Profile_ReportingAccess' ) )
EXEC sp_addrole N'aspnet_Profile_ReportingAccess'
GO

EXEC sp_addrolemember N'aspnet_Profile_BasicAccess', N'aspnet_Profile_FullAccess'
EXEC sp_addrolemember N'aspnet_Profile_ReportingAccess', N'aspnet_Profile_FullAccess'
GO

--
--Stored Procedure rights for BasicAccess
--
GRANT EXECUTE ON dbo.aspnet_Profile_GetProperties TO aspnet_Profile_BasicAccess
GRANT EXECUTE ON dbo.aspnet_Profile_SetProperties TO aspnet_Profile_BasicAccess
GRANT EXECUTE ON dbo.aspnet_CheckSchemaVersion TO aspnet_Profile_BasicAccess
GRANT EXECUTE ON dbo.aspnet_RegisterSchemaVersion TO aspnet_Profile_BasicAccess
GRANT EXECUTE ON dbo.aspnet_UnRegisterSchemaVersion TO aspnet_Profile_BasicAccess

--
--Stored Procedure rights for ReportingAccess
--
GRANT EXECUTE ON dbo.aspnet_Profile_GetNumberOfInactiveProfiles TO aspnet_Profile_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_Profile_GetProfiles TO aspnet_Profile_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_CheckSchemaVersion TO aspnet_Profile_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_RegisterSchemaVersion TO aspnet_Profile_ReportingAccess
GRANT EXECUTE ON dbo.aspnet_UnRegisterSchemaVersion TO aspnet_Profile_ReportingAccess

--
--Additional stored procedure rights for FullAccess
--
GRANT EXECUTE ON dbo.aspnet_Profile_DeleteProfiles TO aspnet_Profile_FullAccess
GRANT EXECUTE ON dbo.aspnet_Profile_DeleteInactiveProfiles TO aspnet_Profile_FullAccess

--
--View rights
--
GRANT SELECT ON dbo.vw_aspnet_Applications TO aspnet_Profile_ReportingAccess
GRANT SELECT ON dbo.vw_aspnet_Users TO aspnet_Profile_ReportingAccess

GRANT SELECT ON dbo.vw_aspnet_Profiles TO aspnet_Profile_ReportingAccess
GO

-------------------------------------------------------------------------
--- Version specific install
-------------------------------------------------------------------------

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

IF (@ver >= 8)
BEGIN
    EXEC sp_tableoption N'aspnet_Profile', 'text in row', 6000
END
GO
/*************************************************************/
/*************************************************************/
/*************************************************************/
/*************************************************************/


declare @command nvarchar(4000)
set @command = 'revoke execute on [dbo].aspnet_RegisterSchemaVersion from ' + QUOTENAME(user)
exec (@command)
GO

PRINT '-------------------------------------------------'
PRINT 'Completed execution of InstallProfile.SQL'
PRINT '-------------------------------------------------'
