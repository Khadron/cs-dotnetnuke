#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.VisualBasic;

namespace DotNetNuke.Data
{
    public class SqlDataProvider : DataProvider
    {
        private const string ProviderType = "data";

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration( ProviderType );
        private string connectionString;
        private string providerPath;
        private string objectQualifier;
        private string databaseOwner;
        private string upgradeConnectionString;

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider

            //Get Connection string from web.config
            connectionString = Config.GetConnectionString();

            if( connectionString == "" )
            {
                // Use connection string specified in provider
                connectionString = objProvider.Attributes["connectionString"];
            }

            providerPath = objProvider.Attributes["providerPath"];

            objectQualifier = objProvider.Attributes["objectQualifier"];
            if( !String.IsNullOrEmpty( objectQualifier ) && objectQualifier.EndsWith( "_" ) == false )
            {
                objectQualifier += "_";
            }

            databaseOwner = objProvider.Attributes["databaseOwner"];
            if( !String.IsNullOrEmpty( databaseOwner ) && databaseOwner.EndsWith( "." ) == false )
            {
                databaseOwner += ".";
            }

            if( Convert.ToString( objProvider.Attributes["upgradeConnectionString"] ) != "" )
            {
                upgradeConnectionString = objProvider.Attributes["upgradeConnectionString"];
            }
            else
            {
                upgradeConnectionString = connectionString;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return databaseOwner;
            }
        }

        public string UpgradeConnectionString
        {
            get
            {
                return upgradeConnectionString;
            }
        }

        private static void ExecuteADOScript( SqlTransaction trans, string SQL )
        {
            //Create a new command (with no timeout)
            SqlCommand command = new SqlCommand( SQL, trans.Connection );
            command.Transaction = trans;
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
        }

        private void ExecuteADOScript( string SQL )
        {
            //Create a new connection
            SqlConnection connection = new SqlConnection( UpgradeConnectionString );

            //Create a new command (with no timeout)
            SqlCommand command = new SqlCommand( SQL, connection );
            command.CommandTimeout = 0;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        //Generic Methods
        //===============
        //

        /// <summary>
        /// ExecuteReader executes a stored procedure or "dynamic sql" statement, against
        /// the database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ProcedureName">The name of the Stored Procedure to Execute</param>
        /// <param name="commandParameters">An array of parameters to pass to the Database</param>
        /// <history>
        /// 	[cnurse]	12/11/2005	created
        /// </history>
        public override void ExecuteNonQuery( string ProcedureName, params object[] commandParameters )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters );
        }

        public override IDataReader ExecuteReader( string ProcedureName, params object[] commandParameters )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters );
        }

        public override object ExecuteScalar( string ProcedureName, params object[] commandParameters )
        {
            return SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters );
        }

        // general
        public override object GetNull( object Field )
        {
            return Null.GetNull( Field, DBNull.Value );
        }

        // upgrade
        public override string GetProviderPath()
        {
            string returnValue;
            HttpContext objHttpContext = HttpContext.Current;

            returnValue = ProviderPath;

            if( !String.IsNullOrEmpty( returnValue ) )
            {
                returnValue = objHttpContext.Server.MapPath( returnValue );

                if( Directory.Exists( returnValue ) )
                {
                    try
                    {
                        // check if database is initialized
                        IDataReader dr = GetDatabaseVersion();
                        dr.Close();
                    }
                    catch
                    {
                        // initialize the database
                        StreamReader objStreamReader;
                        objStreamReader = File.OpenText( returnValue + "00.00.00." + _providerConfiguration.DefaultProvider );
                        string strScript = objStreamReader.ReadToEnd();
                        objStreamReader.Close();

                        if( ExecuteScript( strScript ) != "" )
                        {
                            returnValue = "ERROR: Could not connect to database specified in connectionString for SqlDataProvider";
                        }
                    }
                }
                else
                {
                    returnValue = "ERROR: providerPath folder " + returnValue + " specified for SqlDataProvider does not exist on web server";
                }
            }
            else
            {
                returnValue = "ERROR: providerPath folder value not specified in web.config for SqlDataProvider";
            }
            return returnValue;
        }

        public override string ExecuteScript( string Script )
        {
            return ExecuteScript( Script, false );
        }

        public override string ExecuteScript( string Script, bool UseTransactions )
        {
            string SQL = "";
            string Exceptions = "";
            string Delimiter = "GO" + "\r\n";

            string[] arrSQL = Strings.Split( Script, Delimiter, -1, CompareMethod.Text );

            if( UseTransactions )
            {
                SqlConnection Conn = new SqlConnection( UpgradeConnectionString );
                Conn.Open();
                try
                {
                    SqlTransaction trans = Conn.BeginTransaction();

                    foreach( string sql in arrSQL )
                    {
                        SQL = sql;
                        if( SQL.Trim() != "" )
                        {
                            // script dynamic substitution
                            SQL = SQL.Replace( "{databaseOwner}", DatabaseOwner );
                            SQL = SQL.Replace( "{objectQualifier}", ObjectQualifier );

                            bool ignoreErrors = false;

                            if( SQL.Trim().StartsWith( "{IgnoreError}" ) )
                            {
                                ignoreErrors = true;
                                SQL = SQL.Replace( "{IgnoreError}", "" );
                            }

                            try
                            {
                                ExecuteADOScript( trans, SQL );
                            }
                            catch( SqlException objException )
                            {
                                if( ! ignoreErrors )
                                {
                                    Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
                                }
                            }
                        }
                    }
                    if( Exceptions.Length == 0 )
                    {
                        //No exceptions so go ahead and commit
                        trans.Commit();
                    }
                    else
                    {
                        //Found exceptions, so rollback db
                        trans.Rollback();
                        Exceptions += "SQL Execution failed.  Database was rolled back" + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
                    }
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                foreach( string tempLoopVar_SQL in arrSQL )
                {
                    SQL = tempLoopVar_SQL;
                    if( SQL.Trim() != "" )
                    {
                        // script dynamic substitution
                        SQL = SQL.Replace( "{databaseOwner}", DatabaseOwner );
                        SQL = SQL.Replace( "{objectQualifier}", ObjectQualifier );
                        try
                        {
                            ExecuteADOScript( SQL );
                        }
                        catch( SqlException objException )
                        {
                            Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
                        }
                    }
                }
            }

            // if the upgrade connection string is specified
            if( UpgradeConnectionString != ConnectionString )
            {
                try
                {
                    // grant execute rights to the public role for all stored procedures. This is
                    // necesary because the UpgradeConnectionString will create stored procedures
                    // which restrict execute permissions for the ConnectionString user account.
                    Exceptions += GrantStoredProceduresPermission( "EXECUTE", "public" );
                }
                catch( SqlException objException )
                {
                    Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
                }
                try
                {
                    // grant execute or select rights to the public role for all user defined functions based
                    // on what type of function it is (scalar function or table function). This is
                    // necesary because the UpgradeConnectionString will create user defined functions
                    // which restrict execute permissions for the ConnectionString user account.
                    Exceptions += GrantUserDefinedFunctionsPermission( "EXECUTE", "SELECT", "public" );
                }
                catch( SqlException objException )
                {
                    Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
                }
            }

            return Exceptions;
        }

        private string GrantStoredProceduresPermission( string Permission, string LoginOrRole )
        {
            string SQL = "";
            string Exceptions = "";
            try
            {
                // grant rights to a login or role for all stored procedures
                SQL += "declare @exec nvarchar(2000) ";
                SQL += "declare @name varchar(150) ";
                SQL += "declare sp_cursor cursor for select o.name as name ";
                SQL += "from dbo.sysobjects o ";
                SQL += "where ( OBJECTPROPERTY(o.id, N'IsProcedure') = 1 or OBJECTPROPERTY(o.id, N'IsExtendedProc') = 1 or OBJECTPROPERTY(o.id, N'IsReplProc') = 1 ) ";
                SQL += "and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "and o.name not like N'#%%' ";
                SQL += "and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "open sp_cursor ";
                SQL += "fetch sp_cursor into @name ";
                SQL += "while @@fetch_status >= 0 ";
                SQL += "begin";
                SQL += "  select @exec = 'grant " + Permission + " on ' +  @name  + ' to " + LoginOrRole + "'";
                SQL += "  execute (@exec)";
                SQL += "  fetch sp_cursor into @name ";
                SQL += "end ";
                SQL += "deallocate sp_cursor";
                SqlHelper.ExecuteNonQuery( UpgradeConnectionString, CommandType.Text, SQL );
            }
            catch( SqlException objException )
            {
                Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
            }
            return Exceptions;
        }

        private string GrantUserDefinedFunctionsPermission( string ScalarPermission, string TablePermission, string LoginOrRole )
        {
            string SQL = "";
            string Exceptions = "";
            try
            {
                // grant EXECUTE rights to a login or role for all functions
                SQL += "declare @exec nvarchar(2000) ";
                SQL += "declare @name varchar(150) ";
                SQL += "declare @isscalarfunction int ";
                SQL += "declare @istablefunction int ";
                SQL += "declare sp_cursor cursor for select o.name as name, OBJECTPROPERTY(o.id, N'IsScalarFunction') as IsScalarFunction ";
                SQL += "from dbo.sysobjects o ";
                SQL += "where ( OBJECTPROPERTY(o.id, N'IsScalarFunction') = 1 OR OBJECTPROPERTY(o.id, N'IsTableFunction') = 1 ) ";
                SQL += "and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "and o.name not like N'#%%' ";
                SQL += "and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "open sp_cursor ";
                SQL += "fetch sp_cursor into @name, @isscalarfunction ";
                SQL += "while @@fetch_status >= 0 ";
                SQL += "begin ";
                SQL += "if @IsScalarFunction = 1 ";
                SQL += "begin";
                SQL += "  select @exec = 'grant " + ScalarPermission + " on ' +  @name  + ' to " + LoginOrRole + "'";
                SQL += "  execute (@exec)";
                SQL += "  fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "end ";
                SQL += "else ";
                SQL += "begin";
                SQL += "  select @exec = 'grant " + TablePermission + " on ' +  @name  + ' to " + LoginOrRole + "'";
                SQL += "  execute (@exec)";
                SQL += "  fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "end ";
                SQL += "end ";
                SQL += "deallocate sp_cursor";
                SqlHelper.ExecuteNonQuery( UpgradeConnectionString, CommandType.Text, SQL );
            }
            catch( SqlException objException )
            {
                Exceptions += objException + "\r\n" + "\r\n" + SQL + "\r\n" + "\r\n";
            }
            return Exceptions;
        }

        public override IDataReader GetDatabaseVersion()
        {
            return SqlHelper.ExecuteReader( UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseVersion", null );
        }

        public override void UpdateDatabaseVersion( int Major, int Minor, int Build )
        {
            SqlHelper.ExecuteNonQuery( UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDatabaseVersion", Major, Minor, Build );
        }

        public override IDataReader FindDatabaseVersion( int Major, int Minor, int Build )
        {
            return SqlHelper.ExecuteReader( UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "FindDatabaseVersion", Major, Minor, Build );
        }

        public override void UpgradeDatabaseSchema( int Major, int Minor, int Build )
        {
            // not necessary for SQL Server - use Transact-SQL scripts
        }

        // host
        public override IDataReader GetHostSettings()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSettings", null );
        }

        public override IDataReader GetHostSetting( string SettingName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSetting", SettingName );
        }

        public override void AddHostSetting( string SettingName, string SettingValue, bool SettingIsSecure )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddHostSetting", SettingName, SettingValue, SettingIsSecure );
        }

        public override void UpdateHostSetting( string SettingName, string SettingValue, bool SettingIsSecure )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateHostSetting", SettingName, SettingValue, SettingIsSecure );
        }

        // portal
        public override int AddPortalInfo( string PortalName, string Currency, string FirstName, string LastName, string Username, string Password, string Email, DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalInfo", PortalName, Currency, GetNull( ExpiryDate ), HostFee, HostSpace, PageQuota, UserQuota, GetNull( SiteLogHistory ), HomeDirectory ) );
        }

        public override int CreatePortal( string PortalName, string Currency, DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalInfo", PortalName, Currency, GetNull( ExpiryDate ), HostFee, HostSpace, PageQuota, UserQuota, GetNull( SiteLogHistory ), HomeDirectory ) );
        }

        public override void DeletePortalInfo( int PortalId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalInfo", PortalId );
        }

        public override IDataReader GetExpiredPortals()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetExpiredPortals" );
        }

        public override IDataReader GetPortal( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortal", PortalId );
        }

        public override IDataReader GetPortalByAlias( string PortalAlias )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByAlias", PortalAlias );
        }

        public override IDataReader GetPortalByTab( int TabId, string PortalAlias )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByTab", TabId, PortalAlias );
        }

        public override int GetPortalCount()
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalCount" ) );
        }

        public override IDataReader GetPortals()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortals" );
        }

        public override IDataReader GetPortalsByName( string nameToMatch, int pageIndex, int pageSize )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalsByName", nameToMatch, pageIndex, pageSize );
        }

        public override IDataReader GetPortalSpaceUsed( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalSpaceUsed", GetNull( PortalId ) );
        }

        public override void UpdatePortalInfo( int PortalId, string PortalName, string LogoFile, string FooterText, DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee, double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalInfo", PortalId, PortalName, GetNull( LogoFile ), GetNull( FooterText ), GetNull( ExpiryDate ), UserRegistration, BannerAdvertising, Currency, GetNull( AdministratorId ), HostFee, HostSpace, PageQuota, UserQuota, GetNull( PaymentProcessor ), GetNull( ProcessorUserId ), GetNull( ProcessorPassword ), GetNull( Description ), GetNull( KeyWords ), GetNull( BackgroundFile ), GetNull( SiteLogHistory ), GetNull( SplashTabId ), GetNull( HomeTabId ), GetNull( LoginTabId ), GetNull( UserTabId ), GetNull( DefaultLanguage ), GetNull( TimeZoneOffset ), HomeDirectory );
        }

        public override void UpdatePortalSetup( int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, int AdminTabId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalSetup", PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, SplashTabId, HomeTabId, LoginTabId, UserTabId, AdminTabId );
        }

        public override IDataReader VerifyPortal( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortal", PortalId );
        }

        public override IDataReader VerifyPortalTab( int PortalId, int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortalTab", PortalId, TabId );
        }

        // tab
        public override int AddTab( int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddTab", GetNull( PortalId ), TabName, IsVisible, DisableLink, GetNull( ParentId ), IconFile, Title, Description, KeyWords, Url, GetNull( SkinSrc ), GetNull( ContainerSrc ), TabPath, GetNull( StartDate ), GetNull( EndDate ), GetNull( RefreshInterval ), GetNull( PageHeadText ) ) );
        }

        [Obsolete( "This method is used for legacy support during the upgrade process (pre v3.1.1). It has been replaced by one that adds the RefreshInterval and PageHeadText variables." )]
        public override void UpdateTab( int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTab", TabId, TabName, IsVisible, DisableLink, GetNull( ParentId ), IconFile, Title, Description, KeyWords, IsDeleted, Url, GetNull( SkinSrc ), GetNull( ContainerSrc ), TabPath, GetNull( StartDate ), GetNull( EndDate ) );
        }

        public override void UpdateTab( int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTab", TabId, TabName, IsVisible, DisableLink, GetNull( ParentId ), IconFile, Title, Description, KeyWords, IsDeleted, Url, GetNull( SkinSrc ), GetNull( ContainerSrc ), TabPath, GetNull( StartDate ), GetNull( EndDate ), GetNull( RefreshInterval ), GetNull( PageHeadText ) );
        }

        public override void UpdateTabOrder( int TabId, int TabOrder, int Level, int ParentId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabOrder", TabId, TabOrder, Level, GetNull( ParentId ) );
        }

        public override void DeleteTab( int TabId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTab", TabId );
        }

        public override IDataReader GetTabs( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabs", GetNull( PortalId ) );
        }

        public override IDataReader GetAllTabs()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabs" );
        }

        public override IDataReader GetTab( int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTab", TabId );
        }

        public override IDataReader GetTabByName( string TabName, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabByName", TabName, GetNull( PortalId ) );
        }

        public override int GetTabCount( int PortalId )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabCount", PortalId ) );
        }

        public override IDataReader GetTabsByParentId( int ParentId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByParentId", ParentId );
        }

        public override IDataReader GetTabPanes( int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPanes", TabId );
        }

        public override IDataReader GetPortalTabModules( int PortalId, int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId );
        }

        public override IDataReader GetTabModules( int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId );
        }

        // module
        public override IDataReader GetAllModules()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllModules", null );
        }

        public override IDataReader GetModules( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModules", PortalId );
        }

        public override IDataReader GetAllTabsModules( int PortalId, bool AllTabs )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabsModules", PortalId, AllTabs );
        }

        public override IDataReader GetModule( int ModuleId, int TabId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModule", ModuleId, GetNull( TabId ) );
        }

        public override IDataReader GetModuleByDefinition( int PortalId, string FriendlyName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleByDefinition", GetNull( PortalId ), FriendlyName );
        }

        public override int AddModule( int PortalID, int ModuleDefID, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddModule", GetNull( PortalID ), ModuleDefID, ModuleTitle, AllTabs, GetNull( Header ), GetNull( Footer ), GetNull( StartDate ), GetNull( EndDate ), InheritViewPermissions, IsDeleted ) );
        }

        public override void UpdateModule( int ModuleId, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModule", ModuleId, ModuleTitle, AllTabs, GetNull( Header ), GetNull( Footer ), GetNull( StartDate ), GetNull( EndDate ), InheritViewPermissions, IsDeleted );
        }

        public override void DeleteModule( int ModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModule", ModuleId );
        }

        public override IDataReader GetTabModuleOrder( int TabId, string PaneName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleOrder", TabId, PaneName );
        }

        public override void UpdateModuleOrder( int TabId, int ModuleId, int ModuleOrder, string PaneName )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleOrder", TabId, ModuleId, ModuleOrder, PaneName );
        }

        public override void AddTabModule( int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabModule", TabId, ModuleId, ModuleOrder, PaneName, CacheTime, GetNull( Alignment ), GetNull( Color ), GetNull( Border ), GetNull( IconFile ), Visibility, GetNull( ContainerSrc ), DisplayTitle, DisplayPrint, DisplaySyndicate );
        }

        public override void UpdateTabModule( int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModule", TabId, ModuleId, ModuleOrder, PaneName, CacheTime, GetNull( Alignment ), GetNull( Color ), GetNull( Border ), GetNull( IconFile ), Visibility, GetNull( ContainerSrc ), DisplayTitle, DisplayPrint, DisplaySyndicate );
        }

        public override void DeleteTabModule( int TabId, int ModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModule", TabId, ModuleId );
        }

        public override IDataReader GetSearchModules( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchModules", PortalId );
        }

        public override IDataReader GetModuleSettings( int ModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSettings", ModuleId );
        }

        public override IDataReader GetModuleSetting( int ModuleId, string SettingName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSetting", ModuleId, SettingName );
        }

        public override void AddModuleSetting( int ModuleId, string SettingName, string SettingValue )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleSetting", ModuleId, SettingName, SettingValue );
        }

        public override void UpdateModuleSetting( int ModuleId, string SettingName, string SettingValue )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleSetting", ModuleId, SettingName, SettingValue );
        }

        public override void DeleteModuleSetting( int ModuleId, string SettingName )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSetting", ModuleId, SettingName );
        }

        public override void DeleteModuleSettings( int ModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSettings", ModuleId );
        }

        public override IDataReader GetTabModuleSettings( int TabModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSettings", TabModuleId );
        }

        public override IDataReader GetTabModuleSetting( int TabModuleId, string SettingName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSetting", TabModuleId, SettingName );
        }

        public override void AddTabModuleSetting( int TabModuleId, string SettingName, string SettingValue )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabModuleSetting", TabModuleId, SettingName, SettingValue );
        }

        public override void UpdateTabModuleSetting( int TabModuleId, string SettingName, string SettingValue )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleSetting", TabModuleId, SettingName, SettingValue );
        }

        public override void DeleteTabModuleSetting( int TabModuleId, string SettingName )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSetting", TabModuleId, SettingName );
        }

        public override void DeleteTabModuleSettings( int TabModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSettings", TabModuleId );
        }

        // module definition
        public override IDataReader GetDesktopModule( int DesktopModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModule", DesktopModuleId );
        }

        public override IDataReader GetDesktopModuleByFriendlyName( string FriendlyName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModuleByFriendlyName", FriendlyName );
        }

        public override IDataReader GetDesktopModuleByModuleName( string ModuleName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModuleByModuleName", ModuleName );
        }

        public override IDataReader GetDesktopModules()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModules", null );
        }

        public override IDataReader GetDesktopModulesByPortal( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulesByPortal", PortalId );
        }

        public override int AddDesktopModule( string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures, string CompatibleVersions )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddDesktopModule", ModuleName, FolderName, FriendlyName, GetNull( Description ), GetNull( Version ), IsPremium, IsAdmin, BusinessControllerClass, SupportedFeatures, GetNull( CompatibleVersions ) ) );
        }

        public override void UpdateDesktopModule( int DesktopModuleId, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures, string CompatibleVersions )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDesktopModule", DesktopModuleId, ModuleName, FolderName, FriendlyName, GetNull( Description ), GetNull( Version ), IsPremium, IsAdmin, BusinessControllerClass, SupportedFeatures, GetNull( CompatibleVersions ) );
        }

        public override void DeleteDesktopModule( int DesktopModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModule", DesktopModuleId );
        }

        public override IDataReader GetPortalDesktopModules( int PortalId, int DesktopModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalDesktopModules", GetNull( PortalId ), GetNull( DesktopModuleId ) );
        }

        public override int AddPortalDesktopModule( int PortalId, int DesktopModuleId )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalDesktopModule", PortalId, DesktopModuleId ) );
        }

        public override void DeletePortalDesktopModules( int PortalId, int DesktopModuleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalDesktopModules", GetNull( PortalId ), GetNull( DesktopModuleId ) );
        }

        public override IDataReader GetModuleDefinitions( int DesktopModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinitions", DesktopModuleId );
        }

        public override IDataReader GetModuleDefinition( int ModuleDefId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinition", ModuleDefId );
        }

        public override IDataReader GetModuleDefinitionByName( int DesktopModuleId, string FriendlyName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinitionByName", DesktopModuleId, FriendlyName );
        }

        public override int AddModuleDefinition( int DesktopModuleId, string FriendlyName, int DefaultCacheTime )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleDefinition", DesktopModuleId, FriendlyName, DefaultCacheTime ) );
        }

        public override void DeleteModuleDefinition( int ModuleDefId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleDefinition", ModuleDefId );
        }

        public override void UpdateModuleDefinition( int ModuleDefId, string FriendlyName, int DefaultCacheTime )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleDefinition", ModuleDefId, FriendlyName, DefaultCacheTime );
        }

        public override IDataReader GetModuleControl( int ModuleControlId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControl", ModuleControlId );
        }

        public override IDataReader GetModuleControls( int ModuleDefId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControls", GetNull( ModuleDefId ) );
        }

        public override IDataReader GetModuleControlsByKey( string ControlKey, int ModuleDefId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlsByKey", GetNull( ControlKey ), GetNull( ModuleDefId ) );
        }

        public override IDataReader GetModuleControlByKeyAndSrc( int ModuleDefID, string ControlKey, string ControlSrc )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlByKeyAndSrc", GetNull( ModuleDefID ), GetNull( ControlKey ), GetNull( ControlSrc ) );
        }

        public override int AddModuleControl( int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleControl", GetNull( ModuleDefId ), GetNull( ControlKey ), GetNull( ControlTitle ), ControlSrc, GetNull( IconFile ), ControlType, GetNull( ViewOrder ), GetNull( HelpUrl ) ) );
        }

        public override void UpdateModuleControl( int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleControl", ModuleControlId, GetNull( ModuleDefId ), GetNull( ControlKey ), GetNull( ControlTitle ), ControlSrc, GetNull( IconFile ), ControlType, GetNull( ViewOrder ), GetNull( HelpUrl ) );
        }

        public override void DeleteModuleControl( int ModuleControlId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleControl", ModuleControlId );
        }

        // files
        public override IDataReader GetFiles( int PortalId, int FolderID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFiles", GetNull( PortalId ), FolderID );
        }

        public override IDataReader GetFile( string FileName, int PortalId, int FolderID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFile", FileName, GetNull( PortalId ), FolderID );
        }

        public override IDataReader GetFileById( int FileId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileById", FileId, GetNull( PortalId ) );
        }

        public override void DeleteFile( int PortalId, string FileName, int FolderID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFile", GetNull( PortalId ), FileName, FolderID );
        }

        public override void DeleteFiles( int PortalId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFiles", GetNull( PortalId ) );
        }

        public override int AddFile( int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddFile", GetNull( PortalId ), FileName, Extension, Size, GetNull( Width ), GetNull( Height ), ContentType, Folder, FolderID ) );
        }

        public override void UpdateFile( int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFile", FileId, FileName, Extension, Size, GetNull( Width ), GetNull( Height ), ContentType, Folder, FolderID );
        }

        public override DataTable GetAllFiles()
        {
            return SqlHelper.ExecuteDataset( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllFiles", null ).Tables[0];
        }

        public override IDataReader GetFileContent( int FileId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileContent", FileId, GetNull( PortalId ) );
        }

        public override void UpdateFileContent( int FileId, byte[] Content )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFileContent", FileId, GetNull( Content ) );
        }

        // site log
        public override void AddSiteLog( DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSiteLog", DateTime, PortalId, GetNull( UserId ), GetNull( Referrer ), GetNull( URL ), GetNull( UserAgent ), GetNull( UserHostAddress ), GetNull( UserHostName ), GetNull( TabId ), GetNull( AffiliateId ) );
        }

        public override IDataReader GetSiteLog( int PortalId, string PortalAlias, string ReportName, DateTime StartDate, DateTime EndDate )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + ReportName, PortalId, PortalAlias, StartDate, EndDate );
        }

        public override IDataReader GetSiteLogReports()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSiteLogReports", null );
        }

        public override void DeleteSiteLog( DateTime DateTime, int PortalId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSiteLog", DateTime, PortalId );
        }

        public override IDataReader ExecuteSQL( string SQL )
        {
            return ExecuteSQL( SQL, null );
        }

        public override IDataReader ExecuteSQL( string SQL, params IDataParameter[] commandParameters )
        {
            SqlParameter[] sqlCommandParameters = null;
            if( commandParameters != null )
            {
                sqlCommandParameters = new SqlParameter[commandParameters.Length];
                for( int intIndex = 0; intIndex < commandParameters.Length; intIndex++ )
                {
                    sqlCommandParameters[intIndex] = (SqlParameter)( commandParameters[intIndex] );
                }
            }

            SQL = SQL.Replace( "{databaseOwner}", DatabaseOwner );
            SQL = SQL.Replace( "{objectQualifier}", ObjectQualifier );

            try
            {
                return SqlHelper.ExecuteReader( ConnectionString, CommandType.Text, SQL, sqlCommandParameters );
            }
            catch
            {
                // error in SQL query
                return null;
            }
        }

        public override IDataReader GetTables()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTables", null );
        }

        public override IDataReader GetFields( string TableName )
        {
            string SQL = "SELECT * FROM {objectQualifier}" + TableName + " WHERE 1 = 0";
            return ExecuteSQL( SQL );
        }

        // vendors
        public override IDataReader GetVendors( int PortalId, bool UnAuthorized, int PageIndex, int PageSize )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendors", GetNull( PortalId ), UnAuthorized, GetNull( PageSize ), GetNull( PageIndex ) );
        }

        public override IDataReader GetVendorsByEmail( string Filter, int PortalId, int PageIndex, int PageSize )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByEmail", Filter, GetNull( PortalId ), GetNull( PageSize ), GetNull( PageIndex ) );
        }

        public override IDataReader GetVendorsByName( string Filter, int PortalId, int PageIndex, int PageSize )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByName", Filter, GetNull( PortalId ), GetNull( PageSize ), GetNull( PageIndex ) );
        }

        public override IDataReader GetVendor( int VendorId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendor", VendorId, GetNull( PortalId ) );
        }

        public override void DeleteVendor( int VendorId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendor", VendorId );
        }

        public override int AddVendor( int PortalId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax, string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddVendor", GetNull( PortalId ), VendorName, Unit, Street, City, Region, Country, PostalCode, Telephone, Fax, Cell, Email, Website, FirstName, LastName, UserName, LogoFile, KeyWords, bool.Parse( Authorized ) ) );
        }

        public override void UpdateVendor( int VendorId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax, string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateVendor", VendorId, VendorName, Unit, Street, City, Region, Country, PostalCode, Telephone, Fax, Cell, Email, Website, FirstName, LastName, UserName, LogoFile, KeyWords, bool.Parse( Authorized ) );
        }

        public override IDataReader GetVendorClassifications( int VendorId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorClassifications", GetNull( VendorId ) );
        }

        public override void DeleteVendorClassifications( int VendorId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendorClassifications", VendorId );
        }

        public override int AddVendorClassification( int VendorId, int ClassificationId )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddVendorClassification", VendorId, ClassificationId ) );
        }

        // banners
        public override IDataReader GetBanners( int VendorId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanners", VendorId );
        }

        public override IDataReader GetBanner( int BannerId, int VendorId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanner", BannerId, VendorId, GetNull( PortalId ) );
        }

        public override DataTable GetBannerGroups( int PortalId )
        {
            return SqlHelper.ExecuteDataset( ConnectionString, DatabaseOwner + ObjectQualifier + "GetBannerGroups", GetNull( PortalId ) ).Tables[0];
        }

        public override void DeleteBanner( int BannerId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteBanner", BannerId );
        }

        public override int AddBanner( string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName, int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddBanner", BannerName, VendorId, GetNull( ImageFile ), GetNull( URL ), Impressions, CPM, GetNull( StartDate ), GetNull( EndDate ), UserName, BannerTypeId, GetNull( Description ), GetNull( GroupName ), Criteria, Width, Height ) );
        }

        public override void UpdateBanner( int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName, int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBanner", BannerId, BannerName, GetNull( ImageFile ), GetNull( URL ), Impressions, CPM, GetNull( StartDate ), GetNull( EndDate ), UserName, BannerTypeId, GetNull( Description ), GetNull( GroupName ), Criteria, Width, Height );
        }

        public override IDataReader FindBanners( int PortalId, int BannerTypeId, string GroupName )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "FindBanners", GetNull( PortalId ), GetNull( BannerTypeId ), GetNull( GroupName ) );
        }

        public override void UpdateBannerViews( int BannerId, DateTime StartDate, DateTime EndDate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerViews", BannerId, GetNull( StartDate ), GetNull( EndDate ) );
        }

        public override void UpdateBannerClickThrough( int BannerId, int VendorId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerClickThrough", BannerId, VendorId );
        }

        // affiliates
        public override IDataReader GetAffiliates( int VendorId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliates", VendorId );
        }

        public override IDataReader GetAffiliate( int AffiliateId, int VendorId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliate", AffiliateId, VendorId, GetNull( PortalId ) );
        }

        public override void DeleteAffiliate( int AffiliateId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteAffiliate", AffiliateId );
        }

        public override int AddAffiliate( int VendorId, DateTime StartDate, DateTime EndDate, double CPC, double CPA )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddAffiliate", VendorId, GetNull( StartDate ), GetNull( EndDate ), CPC, CPA ) );
        }

        public override void UpdateAffiliate( int AffiliateId, DateTime StartDate, DateTime EndDate, double CPC, double CPA )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliate", AffiliateId, GetNull( StartDate ), GetNull( EndDate ), CPC, CPA );
        }

        public override void UpdateAffiliateStats( int AffiliateId, int Clicks, int Acquisitions )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliateStats", AffiliateId, Clicks, Acquisitions );
        }

        // skins/containers
        public override IDataReader GetSkin( string SkinRoot, int PortalId, int SkinType )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkin", SkinRoot, GetNull( PortalId ), SkinType );
        }

        public override IDataReader GetSkins( int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkins", GetNull( PortalId ) );
        }

        public override void DeleteSkin( string SkinRoot, int PortalId, int SkinType )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkin", SkinRoot, GetNull( PortalId ), SkinType );
        }

        public override int AddSkin( string SkinRoot, int PortalId, int SkinType, string SkinSrc )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkin", SkinRoot, GetNull( PortalId ), SkinType, SkinSrc ) );
        }

        // personalization
        public override IDataReader GetAllProfiles()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllProfiles", null );
        }

        public override IDataReader GetProfile( int UserId, int PortalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetProfile", UserId, PortalId );
        }

        public override void AddProfile( int UserId, int PortalId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddProfile", UserId, PortalId );
        }

        public override void UpdateProfile( int UserId, int PortalId, string ProfileData )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateProfile", UserId, PortalId, ProfileData );
        }

        //profile property definitions
        public override int AddPropertyDefinition( int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length )
        {
            int retValue;
            try
            {
                retValue = Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPropertyDefinition", GetNull( PortalId ), ModuleDefId, DataType, DefaultValue, PropertyCategory, PropertyName, Required, ValidationExpression, ViewOrder, Visible, Length ) );
            }
            catch( SqlException ex )
            {
                //If not a duplicate (throw an Exception)
                retValue = - ex.Number;
                if( ex.Number != 2601 )
                {
                    throw ( ex );
                }
            }
            return retValue;
        }

        public override void DeletePropertyDefinition( int definitionId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePropertyDefinition", definitionId );
        }

        public override IDataReader GetPropertyDefinition( int definitionId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinition", definitionId );
        }

        public override IDataReader GetPropertyDefinitionByName( int portalId, string name )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionByName", GetNull( portalId ), name );
        }

        public override IDataReader GetPropertyDefinitionsByCategory( int portalId, string category )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionsByCategory", GetNull( portalId ), category );
        }

        public override IDataReader GetPropertyDefinitionsByPortal( int portalId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionsByPortal", GetNull( portalId ) );
        }

        public override void UpdatePropertyDefinition( int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePropertyDefinition", PropertyDefinitionId, DataType, DefaultValue, PropertyCategory, PropertyName, Required, ValidationExpression, ViewOrder, Visible, Length );
        }

        // urls
        public override IDataReader GetUrls( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrls", PortalID );
        }

        public override IDataReader GetUrl( int PortalID, string Url )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrl", PortalID, Url );
        }

        public override void AddUrl( int PortalID, string Url )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrl", PortalID, Url );
        }

        public override void DeleteUrl( int PortalID, string Url )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrl", PortalID, Url );
        }

        public override IDataReader GetUrlTracking( int PortalID, string Url, int ModuleID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlTracking", PortalID, Url, GetNull( ModuleID ) );
        }

        public override void AddUrlTracking( int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlTracking", PortalID, Url, UrlType, LogActivity, TrackClicks, GetNull( ModuleID ), NewWindow );
        }

        public override void UpdateUrlTracking( int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTracking", PortalID, Url, LogActivity, TrackClicks, GetNull( ModuleID ), NewWindow );
        }

        public override void DeleteUrlTracking( int PortalID, string Url, int ModuleID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrlTracking", PortalID, Url, GetNull( ModuleID ) );
        }

        public override void UpdateUrlTrackingStats( int PortalID, string Url, int ModuleID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTrackingStats", PortalID, Url, GetNull( ModuleID ) );
        }

        public override IDataReader GetUrlLog( int UrlTrackingID, DateTime StartDate, DateTime EndDate )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlLog", UrlTrackingID, GetNull( StartDate ), GetNull( EndDate ) );
        }

        public override void AddUrlLog( int UrlTrackingID, int UserID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlLog", UrlTrackingID, GetNull( UserID ) );
        }

        //Permission
        public override IDataReader GetPermissionsByModuleDefID( int ModuleDefID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleDefID", ModuleDefID );
        }

        public override IDataReader GetPermissionsByModuleID( int ModuleID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleID", ModuleID );
        }

        public override IDataReader GetPermissionsByFolderPath( int PortalID, string Folder )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByFolderPath", GetNull( PortalID ), Folder );
        }

        public override IDataReader GetPermissionByCodeAndKey( string PermissionCode, string PermissionKey )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionByCodeAndKey", GetNull( PermissionCode ), GetNull( PermissionKey ) );
        }

        public override IDataReader GetPermissionsByTabID( int TabID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByTabID", TabID );
        }

        public override IDataReader GetPermission( int permissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermission", permissionID );
        }

        public override void DeletePermission( int permissionID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePermission", permissionID );
        }

        public override int AddPermission( string permissionCode, int moduleDefID, string permissionKey, string permissionName )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPermission", moduleDefID, permissionCode, permissionKey, permissionName ) );
        }

        public override void UpdatePermission( int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePermission", permissionID, permissionCode, moduleDefID, permissionKey, permissionName );
        }

        //ModulePermission
        public override IDataReader GetModulePermission( int modulePermissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermission", modulePermissionID );
        }

        public override IDataReader GetModulePermissionsByModuleID( int moduleID, int PermissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByModuleID", moduleID, PermissionID );
        }

        public override IDataReader GetModulePermissionsByPortal( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByPortal", PortalID );
        }

        public override IDataReader GetModulePermissionsByTabID( int TabID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByTabID", TabID );
        }

        public override void DeleteModulePermissionsByModuleID( int ModuleID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermissionsByModuleID", ModuleID );
        }

        public override void DeleteModulePermission( int modulePermissionID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermission", modulePermissionID );
        }

        public override int AddModulePermission( int moduleID, int PermissionID, int roleID, bool AllowAccess )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddModulePermission", moduleID, PermissionID, roleID, AllowAccess ) );
        }

        public override void UpdateModulePermission( int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModulePermission", modulePermissionID, moduleID, PermissionID, roleID, AllowAccess );
        }

        //TabPermission
        public override IDataReader GetTabPermissionsByPortal( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByPortal", GetNull( PortalID ) );
        }

        public override IDataReader GetTabPermissionsByTabID( int TabID, int PermissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByTabID", TabID, PermissionID );
        }

        public override void DeleteTabPermissionsByTabID( int TabID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermissionsByTabID", TabID );
        }

        public override void DeleteTabPermission( int TabPermissionID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermission", TabPermissionID );
        }

        public override int AddTabPermission( int TabID, int PermissionID, int roleID, bool AllowAccess )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabPermission", TabID, PermissionID, roleID, AllowAccess ) );
        }

        public override void UpdateTabPermission( int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabPermission", TabPermissionID, TabID, PermissionID, roleID, AllowAccess );
        }

        //Folders
        public override IDataReader GetFoldersByPortal( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolders", GetNull( PortalID ), - 1, "" );
        }

        public override IDataReader GetFoldersByUser( int PortalID, int UserID, bool IncludeSecure, bool IncludeDatabase, bool AllowAccess, string Permissions )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFoldersByUser", GetNull( PortalID ), GetNull( UserID ), IncludeSecure, IncludeDatabase, AllowAccess, Permissions );
        }

        public override IDataReader GetFolder( int PortalID, int FolderID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderID", GetNull( PortalID ), FolderID );
        }

        public override IDataReader GetFolder( int PortalID, string FolderPath )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderPath", GetNull( PortalID ), FolderPath );
        }

        public override int AddFolder( int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolder", GetNull( PortalID ), FolderPath, StorageLocation, IsProtected, IsCached, GetNull( LastUpdated ) ) );
        }

        public override void UpdateFolder( int PortalID, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated )
        {
            SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolder", GetNull( PortalID ), FolderID, FolderPath, StorageLocation, IsProtected, IsCached, GetNull( LastUpdated ) );
        }

        public override void DeleteFolder( int PortalID, string FolderPath )
        {
            SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolder", GetNull( PortalID ), FolderPath );
        }

        //FolderPermission
        public override IDataReader GetFolderPermission( int FolderPermissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermission", FolderPermissionID );
        }

        public override IDataReader GetFolderPermissionsByFolderPath( int PortalID, string FolderPath, int PermissionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermissionsByFolderPath", GetNull( PortalID ), FolderPath, PermissionID );
        }

        public override void DeleteFolderPermissionsByFolderPath( int PortalID, string FolderPath )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermissionsByFolderPath", GetNull( PortalID ), FolderPath );
        }

        public override void DeleteFolderPermission( int FolderPermissionID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermission", FolderPermissionID );
        }

        public override int AddFolderPermission( int FolderID, int PermissionID, int roleID, bool AllowAccess )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolderPermission", FolderID, PermissionID, roleID, AllowAccess ) );
        }

        public override void UpdateFolderPermission( int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolderPermission", FolderPermissionID, FolderID, PermissionID, roleID, AllowAccess );
        }

        // search engine
        public override IDataReader GetSearchIndexers()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchIndexers", null );
        }

        public override IDataReader GetSearchResultModules( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResultModules", PortalID );
        }

        // content search datastore
        public override void DeleteSearchItems( int ModuleID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItems", ModuleID );
        }

        public override void DeleteSearchItem( int SearchItemId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItem", SearchItemId );
        }

        public override void DeleteSearchItemWords( int SearchItemId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItemWords", SearchItemId );
        }

        public override int AddSearchItem( string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItem", Title, Description, GetNull( Author ), GetNull( PubDate ), ModuleId, Key, Guid, ImageFileId ) );
        }

        public override IDataReader GetSearchCommonWordsByLocale( string Locale )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchCommonWordsByLocale", Locale );
        }

        public override IDataReader GetDefaultLanguageByModule( string ModuleList )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetDefaultLanguageByModule", ModuleList );
        }

        public override IDataReader GetSearchSettings( int ModuleId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchSettings", ModuleId );
        }

        public override IDataReader GetSearchWords()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchWords", null );
        }

        public override int AddSearchWord( string Word )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchWord", Word ) );
        }

        public override int AddSearchItemWord( int SearchItemId, int SearchWordsID, int Occurrences )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWord", SearchItemId, SearchWordsID, Occurrences ) );
        }

        public override void AddSearchItemWordPosition( int SearchItemWordID, string ContentPositions )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWordPosition", SearchItemWordID, ContentPositions );
        }

        public override IDataReader GetSearchResults( int PortalID, string Word )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResults", PortalID, Word );
        }

        public override IDataReader GetSearchItems( int PortalID, int TabID, int ModuleID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItems", GetNull( PortalID ), GetNull( TabID ), GetNull( ModuleID ) );
        }

        public override IDataReader GetSearchItem( int ModuleID, string SearchKey )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItem", GetNull( ModuleID ), SearchKey );
        }

        public override void UpdateSearchItem( int SearchItemId, string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId )
        {
            SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSearchItem", SearchItemId, Title, Description, GetNull( Author ), GetNull( PubDate ), ModuleId, Key, Guid, HitCount, ImageFileId );
        }

        //Lists
        public override IDataReader GetListGroup()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetListGroup", null );
        }

        public override IDataReader GetList( string ListName, string ParentKey, int DefinitionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetList", ListName, ParentKey, DefinitionID );
        }

        public override IDataReader GetListEntries( string ListName, string ParentKey, int EntryID, int DefinitionID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntries", ListName, ParentKey, EntryID, DefinitionID, "" );
        }

        public override IDataReader GetListEntriesByListName( string ListName, string Value, string ParentKey )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntries", ListName, ParentKey, - 1, - 1, Value );
        }

        public override int AddListEntry( string ListName, string Value, string Text, string ParentKey, bool EnableSortOrder, int DefinitionID, string Description )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddListEntry", ListName, Value, Text, ParentKey, EnableSortOrder, DefinitionID, Description ) );
        }

        public override void UpdateListEntry( int EntryID, string ListName, string Value, string Text, string Description )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListEntry", EntryID, ListName, Value, Text, Description );
        }

        public override void DeleteListEntryByID( int EntryID, bool DeleteChild )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByID", EntryID, DeleteChild );
        }

        public override void DeleteList( string ListName, string ParentKey )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteList", ListName, ParentKey );
        }

        public override void DeleteListEntryByListName( string ListName, string Value, bool DeleteChild )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByListName", ListName, Value, DeleteChild );
        }

        public override void UpdateListSortOrder( int EntryID, bool MoveUp )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListSortOrder", EntryID, MoveUp );
        }

        //portal alias
        public override IDataReader GetPortalAlias( string PortalAlias, int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAlias", PortalAlias, PortalID );
        }

        public override IDataReader GetPortalByPortalAliasID( int PortalAliasId )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByPortalAliasID", PortalAliasId );
        }

        public override void UpdatePortalAlias( string PortalAlias )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAliasOnInstall", PortalAlias );
        }

        public override void UpdatePortalAliasInfo( int PortalAliasID, int PortalID, string HTTPAlias )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAlias", PortalAliasID, PortalID, HTTPAlias );
        }

        public override int AddPortalAlias( int PortalID, string HTTPAlias )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalAlias", PortalID, HTTPAlias ) );
        }

        public override void DeletePortalAlias( int PortalAliasID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalAlias", PortalAliasID );
        }

        public override IDataReader GetPortalAliasByPortalID( int PortalID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalID", PortalID );
        }

        public override IDataReader GetPortalAliasByPortalAliasID( int PortalAliasID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalAliasID", PortalAliasID );
        }
    }
}