using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Modules.Admin.ResourceInstaller;
using DotNetNuke.Services.Upgrade;

namespace DotNetNuke.Framework
{
    public partial class Install : Page
    {
        private void ExecuteScripts()
        {
            //Start Timer
            Upgrade.StartTimer();

            //Write out Header
            HtmlUtils.WriteHeader( Response, "executeScripts" );

            Response.Write( "<h2>Execute Scripts Status Report</h2>" );
            Response.Flush();

            string strProviderPath = PortalSettings.GetProviderPath();
            if( ! strProviderPath.StartsWith( "ERROR:" ) )
            {
                Upgrade.ExecuteScripts( strProviderPath );
            }

            Response.Write( "<h2>Execution Complete</h2>" );
            Response.Flush();

            //Write out Footer
            HtmlUtils.WriteFooter( Response );
        }

        private void InstallApplication()
        {
            // the application uses a two step installation process. The first step is used to update
            // the Web.config with any configuration settings - which forces an application restart.
            // The second step finishes the installation process and provisions the site.
            string installationDate = Config.GetSetting( "InstallationDate" );
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";

            if( installationDate == null || installationDate == "" )
            {
                XmlDocument xmlConfig = new XmlDocument();
                string strError = "";

                //save the current config files
                try
                {
                    if( !Directory.Exists( Globals.ApplicationMapPath + backupFolder ) )
                    {
                        Directory.CreateDirectory( Globals.ApplicationMapPath + backupFolder );
                    }

                    if( File.Exists( Globals.ApplicationMapPath + "\\web.config" ) )
                    {
                        File.Copy( Globals.ApplicationMapPath + "\\web.config", Globals.ApplicationMapPath + backupFolder + "web_old.config", true );
                    }
                }
                catch( Exception )
                {
                    //Error backing up old web.config
                    //This error is not critical, so can be ignored
                }

                try
                {
                    // open the web.config
                    xmlConfig = Config.Load();

                    // create random keys for the Membership machine keys
                    xmlConfig = Config.UpdateMachineKey( xmlConfig );
                }
                catch( Exception ex )
                {
                    strError += ex.Message;
                }

                // save a copy of the web.config
                strError += Config.Save( xmlConfig, backupFolder + "web_.config" );

                // save the web.config
                strError += Config.Save( xmlConfig );

                if( strError == "" )
                {
                    // send a new request to the application to initiate step 2
                    Response.Redirect( HttpContext.Current.Request.RawUrl, true );
                }
                else
                {
                    // error saving web.config
                    StreamReader objStreamReader;
                    objStreamReader = File.OpenText( HttpContext.Current.Server.MapPath( "~/403-3.htm" ) );
                    string strHTML = objStreamReader.ReadToEnd();
                    objStreamReader.Close();
                    strHTML = strHTML.Replace( "[MESSAGE]", strError );
                    HttpContext.Current.Response.Write( strHTML );
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                //Start Timer
                Upgrade.StartTimer();

                //Write out Header
                HtmlUtils.WriteHeader( Response, "install" );

                // get path to script files
                string strProviderPath = PortalSettings.GetProviderPath();
                if( ! strProviderPath.StartsWith( "ERROR:" ) )
                {
                    Response.Write( "<h2>Version: " + Globals.glbAppVersion + "</h2>" );
                    Response.Flush();

                    Response.Write( "<br><br>" );
                    Response.Write( "<h2>Installation Status Report</h2>" );
                    Response.Flush();
                    Upgrade.InstallDNN( strProviderPath );

                    Response.Write( "<h2>Installation Complete</h2>" );
                    Response.Write( "<br><br><h2><a href=\'../Default.aspx\'>Click Here To Access Your Portal</a></h2><br><br>" );
                    Response.Flush();
                }
                else
                {
                    // upgrade error
                    Response.Write( "<h2>Upgrade Error: " + strProviderPath + "</h2>" );
                    Response.Flush();
                }

                //Write out Footer
                HtmlUtils.WriteFooter( Response );

                //log APPLICATION_START event
                Global.LogStart();

                //Start Scheduler
                Global.StartScheduler();
            }
        }

        private void UpgradeApplication()
        {
            //Start Timer
            Upgrade.StartTimer();

            //Write out Header
            HtmlUtils.WriteHeader( Response, "upgrade" );

            Response.Write( "<h2>Current Assembly Version: " + Globals.glbAppVersion + "</h2>" );
            Response.Flush();

            // get path to script files
            string strProviderPath = PortalSettings.GetProviderPath();
            if( ! strProviderPath.StartsWith( "ERROR:" ) )
            {
                string strDatabaseVersion;

                // get current database version
                IDataReader dr = PortalSettings.GetDatabaseVersion();
                if( dr.Read() )
                {
                    //Call Upgrade with the current DB Version to upgrade an
                    //existing DNN installation
                    int majVersion = Convert.ToInt32( dr["Major"] );
                    int minVersion = Convert.ToInt32( dr["Minor"] );
                    int buildVersion = Convert.ToInt32( dr["Build"] );
                    strDatabaseVersion = String.Format( majVersion.ToString(), "00" ) + "." + String.Format( minVersion.ToString(), "00" ) + "." + String.Format( buildVersion.ToString(), "00" );

                    Response.Write( "<h2>Current Database Version: " + strDatabaseVersion + "</h2>" );
                    Response.Flush();

                    string ignoreWarning = Null.NullString;
                    string strWarning = Null.NullString;
                    if( ( majVersion == 3 && minVersion < 3 ) || ( majVersion == 4 && minVersion < 3 ) )
                    {
                        //Users and profile have not been transferred

                        // Get the name of the data provider
                        ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration( "data" );

                        //Execute Special Script
                        Upgrade.ExecuteScript( strProviderPath + "Upgrade." + objProviderConfiguration.DefaultProvider );

                        if( ( Request.QueryString["ignoreWarning"] != null ) )
                        {
                            ignoreWarning = Request.QueryString["ignoreWarning"].ToLower();
                        }

                        strWarning = Upgrade.CheckUpgrade();
                    }
                    else
                    {
                        ignoreWarning = "true";
                    }

                    //Check whether Upgrade is ok
                    if( strWarning == Null.NullString || ignoreWarning == "true" )
                    {
                        Response.Write( "<br><br>" );
                        Response.Write( "<h2>Upgrade Status Report</h2>" );
                        Response.Flush();
                        Upgrade.UpgradeDNN( strProviderPath, strDatabaseVersion.Replace( ".", "" ) );

                        //Install Resources
                        ResourceInstaller objResourceInstaller = new ResourceInstaller();
                        objResourceInstaller.Install( true, 0 );

                        Response.Write( "<h2>Upgrade Complete</h2>" );
                        Response.Write( "<br><br><h2><a href=\'../Default.aspx\'>Click Here To Access Your Portal</a></h2><br><br>" );
                    }
                    else
                    {
                        Response.Write( "<h2>Warning:</h2>" + strWarning.Replace( "\r\n", "<br />" ) );

                        Response.Write( "<br><br><a href=\'Install.aspx?mode=Install&ignoreWarning=true\'>Click Here To Proceed With The Upgrade.</a>" );
                    }
                    Response.Flush();
                }
                dr.Close();
            }
            else
            {
                Response.Write( "<h2>Upgrade Error: " + strProviderPath + "</h2>" );
                Response.Flush();
            }

            //Write out Footer
            HtmlUtils.WriteFooter( Response );
        }

        private void AddPortal()
        {
            //Start Timer
            Upgrade.StartTimer();

            //Write out Header
            HtmlUtils.WriteHeader( Response, "addPortal" );

            Response.Write( "<h2>Add Portal Status Report</h2>" );
            Response.Flush();

            // install new portal(s)
            string strNewFile = Globals.ApplicationMapPath + "\\Install\\Portal\\Portals.resources";
            if( File.Exists( strNewFile ) )
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node;
                XmlNodeList nodes;
                int intPortalId;
                xmlDoc.Load( strNewFile );

                // parse portal(s) if available
                nodes = xmlDoc.SelectNodes( "//dotnetnuke/portals/portal" );
                foreach( XmlNode tempLoopVar_node in nodes )
                {
                    node = tempLoopVar_node;
                    if( node != null )
                    {
                        intPortalId = Upgrade.AddPortal( node, true, 0 );
                    }
                }

                // delete the file
                try
                {
                    File.SetAttributes( strNewFile, FileAttributes.Normal );
                    File.Delete( strNewFile );
                }
                catch
                {
                    // error removing the file
                }

                Response.Write( "<h2>Installation Complete</h2>" );
                Response.Write( "<br><br><h2><a href=\'../Default.aspx\'>Click Here To Access Your Portal</a></h2><br><br>" );
                Response.Flush();
            }

            //Write out Footer
            HtmlUtils.WriteFooter( Response );
        }

        private void InstallResources()
        {
            //Start Timer
            Upgrade.StartTimer();

            //Write out Header
            HtmlUtils.WriteHeader( Response, "installResources" );

            Response.Write( "<h2>Install Resources Status Report</h2>" );
            Response.Flush();

            // install new resources(s)
            ResourceInstaller objResourceInstaller = new ResourceInstaller();
            objResourceInstaller.Install( true, 0 );

            Response.Write( "<h2>Installation Complete</h2>" );
            Response.Flush();

            //Write out Footer
            HtmlUtils.WriteFooter( Response );
        }

        private void NoUpgrade()
        {
            // get path to script files
            string strProviderPath = PortalSettings.GetProviderPath();
            if( ! strProviderPath.StartsWith( "ERROR:" ) )
            {
                string strDatabaseVersion;
                // get current database version
                try
                {
                    IDataReader dr = PortalSettings.GetDatabaseVersion();
                    if( dr.Read() )
                    {
                        //Write out Header
                        HtmlUtils.WriteHeader( Response, "none" );
                        Response.Write( "<h2>Current Assembly Version: " + Globals.glbAppVersion + "</h2>" );

                        //Call Upgrade with the current DB Version to upgrade an
                        //existing DNN installation
                        strDatabaseVersion = String.Format( (string)dr["Major"], "00" ) + "." + String.Format( (string)dr["Minor"], "00" ) + "." + String.Format( (string)dr["Build"], "00" );

                        Response.Write( "<h2>Current Database Version: " + strDatabaseVersion + "</h2>" );

                        Response.Write( "<br><br><a href=\'Install.aspx?mode=Install\'>Click Here To Upgrade DotNetNuke</a>" );
                        Response.Flush();
                    }
                    else
                    {
                        //Write out Header
                        HtmlUtils.WriteHeader( Response, "noDBVersion" );
                        Response.Write( "<h2>Current Assembly Version: " + Globals.glbAppVersion + "</h2>" );

                        Response.Write( "<h2>Current Database Version: N/A</h2>" );
                        Response.Write( "<br><br><h2><a href=\'Install.aspx?mode=Install\'>Click Here To Install DotNetNuke</a></h2>" );
                        Response.Flush();
                    }
                    dr.Close();
                }
                catch( Exception ex )
                {
                    //Write out Header
                    HtmlUtils.WriteHeader( Response, "error" );
                    Response.Write( "<h2>Current Assembly Version: " + Globals.glbAppVersion + "</h2>" );

                    Response.Write( "<h2>" + ex.Message + "</h2>" );
                    Response.Flush();
                }
            }
            else
            {
                //Write out Header
                HtmlUtils.WriteHeader( Response, "error" );
                Response.Write( "<h2>Current Assembly Version: " + Globals.glbAppVersion + "</h2>" );

                Response.Write( "<h2>" + strProviderPath + "</h2>" );
                Response.Flush();
            }
            //Write out Footer
            HtmlUtils.WriteFooter( Response );
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            //Get current Script time-out
            int scriptTimeOut = Server.ScriptTimeout;

            string mode = "";
            if( ( Request.QueryString["mode"] != null ) )
            {
                mode = Request.QueryString["mode"].ToLower();
            }

            //Disable Client side caching
            Response.Cache.SetCacheability( HttpCacheability.ServerAndNoCache );

            //Check mode is not Nothing
            if( mode == "none" )
            {
                NoUpgrade();
            }
            else
            {
                //Set Script timeout to MAX value
                Server.ScriptTimeout = int.MaxValue;

                switch( Globals.GetUpgradeStatus() )
                {
                    case Globals.UpgradeStatus.Install:

                        InstallApplication();
                        break;
                    case Globals.UpgradeStatus.Upgrade:

                        UpgradeApplication();
                        break;
                    case Globals.UpgradeStatus.None:

                        //Check mode
                        switch( mode )
                        {
                            case "addportal":

                                AddPortal();
                                break;
                            case "installresources":

                                InstallResources();
                                break;
                            case "executescripts":

                                ExecuteScripts();
                                break;
                        }
                        break;
                    case Globals.UpgradeStatus.Error:

                        NoUpgrade();
                        break;
                }

                //restore Script timeout
                Server.ScriptTimeout = scriptTimeOut;
            }
        }

        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}