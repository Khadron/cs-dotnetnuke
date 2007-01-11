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
using System.Collections;
using System.IO;
using System.Threading;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Common
{
    /// <summary>
    /// Global, everything begins from here.
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// CheckVersion determines whether the App is synchronized with the DB
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    2/17/2005   created
        /// </history>
        private void CheckVersion()
        {
            HttpServerUtility serverUtility = HttpContext.Current.Server;
            HttpRequest httpRequest = HttpContext.Current.Request;
            HttpResponse httpResponse = HttpContext.Current.Response;

            bool AutoUpgrade;
            if( Config.GetSetting( "AutoUpgrade" ) == null )
            {
                AutoUpgrade = true;
            }
            else
            {
                AutoUpgrade = bool.Parse( Config.GetSetting( "AutoUpgrade" ) );
            }

            //Determine the Upgrade status and redirect to Install.aspx
            switch( Globals.GetUpgradeStatus() )
            {
                case Globals.UpgradeStatus.Install:
                    if( AutoUpgrade )
                    {
                        httpResponse.Redirect( "~/Install/Install.aspx?mode=Install" );
                    }
                    else
                    {
                        CreateUnderConstructionPage();
                        httpResponse.Redirect( "~/Install/UnderConstruction.htm" );
                    }
                    break;

                case Globals.UpgradeStatus.Upgrade:

                    if( AutoUpgrade )
                    {
                        httpResponse.Redirect( "~/Install/Install.aspx?mode=Install" );
                    }
                    else
                    {
                        CreateUnderConstructionPage();
                        httpResponse.Redirect( "~/Install/UnderConstruction.htm" );
                    }
                    break;
                case Globals.UpgradeStatus.Error:

                    if( AutoUpgrade )
                    {
                        httpResponse.Redirect( "~/Install/Install.aspx?mode=none" );
                    }
                    else
                    {
                        CreateUnderConstructionPage();
                        httpResponse.Redirect( "~/Install/UnderConstruction.htm" );
                    }
                    break;
            }
        }

        private void CreateUnderConstructionPage()
        {
            // create an UnderConstruction page if it does not exist already
            if( ! File.Exists( HttpContext.Current.Server.MapPath( "~/Install/UnderConstruction.htm" ) ) )
            {
                if( File.Exists( HttpContext.Current.Server.MapPath( "~/Install/UnderConstruction.template.htm" ) ) )
                {
                    File.Copy( HttpContext.Current.Server.MapPath( "~/Install/UnderConstruction.template.htm" ), HttpContext.Current.Server.MapPath( "~/Install/UnderConstruction.htm" ) );
                }
            }
        }

        /// <summary>
        /// LogEnd logs the Application Start Event
        /// </summary>
        /// <history>
        ///     [cnurse]    1/28/2005   Moved back to App_End from Logging Module
        /// </history>
        private void LogEnd()
        {
            try
            {
                EventLogController objEv = new EventLogController();
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.BypassBuffering = true;
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.APPLICATION_SHUTTING_DOWN.ToString();
                objEv.AddLog( objEventLogInfo );
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }

            // purge log buffer
            LoggingProvider.Instance().PurgeLogBuffer();
        }

        /// <summary>
        /// CacheMappedDirectory caches the Portal Mapped Directory(s)
        /// </summary>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Caching Module
        /// </history>
        private void CacheMappedDirectory()
        {
            //Cache the mapped physical home directory for each portal
            //so the mapped directories are available outside
            //of httpcontext.   This is especially necessary
            //when the /Portals or portal home directory has been
            //mapped in IIS to another directory or server.
            FolderController objFolderController = new FolderController();
            PortalController objPortalController = new PortalController();
            ArrayList arrPortals = objPortalController.GetPortals();
            int i;
            for( i = 0; i <= arrPortals.Count - 1; i++ )
            {
                PortalInfo objPortalInfo = (PortalInfo)arrPortals[i];
                objFolderController.SetMappedDirectory( objPortalInfo, HttpContext.Current );
            }
        }

        /// <summary>
        /// StopScheduler stops the Scheduler
        /// </summary>
        /// <history>
        ///     [cnurse]    1/28/2005   Moved back to App_End from Scheduling Module
        /// </history>
        private void StopScheduler()
        {
            // stop scheduled jobs
            SchedulingProvider.Instance().Halt( "Stopped by Application_End" );
        }

        /// <summary>
        /// LogStart logs the Application Start Event
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Logging Module
        /// </history>
        public static void LogStart()
        {
            EventLogController objEv = new EventLogController();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.BypassBuffering = true;
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.APPLICATION_START.ToString();
            objEv.AddLog( objEventLogInfo );
        }

        /// <summary>
        /// StartScheduler starts the Scheduler
        /// </summary>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Scheduling Module
        /// </history>
        public static void StartScheduler()
        {
            // instantiate APPLICATION_START scheduled jobs
            if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
            {
                SchedulingProvider scheduler = SchedulingProvider.Instance();
                scheduler.RunEventSchedule( EventName.APPLICATION_START );
                Thread newThread = new Thread( new ThreadStart( SchedulingProvider.Instance().Start ) );
                newThread.IsBackground = true;
                newThread.Start();
            }
        }

        /// <summary>
        /// Application_Start
        /// Executes on the first web request into the portal application,
        /// when a new DLL is deployed, or when web.config is modified.
        /// </summary>
        /// <remarks>
        /// - global variable initialization
        /// </remarks>
        protected void Application_Start( object sender, EventArgs e )
        {
            HttpServerUtility httpServerUtility = HttpContext.Current.Server;

            //global variable initialization
            Globals.ServerName = httpServerUtility.MachineName;

            if( HttpContext.Current.Request.ApplicationPath == "/" )
            {
                Globals.ApplicationPath = "";
            }
            else
            {
                Globals.ApplicationPath = HttpContext.Current.Request.ApplicationPath;
            }
            Globals.ApplicationMapPath = AppDomain.CurrentDomain.BaseDirectory.Substring( 0, AppDomain.CurrentDomain.BaseDirectory.Length - 1 );
            Globals.ApplicationMapPath = Globals.ApplicationMapPath.Replace( "/", "\\" );

            Globals.HostPath = Globals.ApplicationPath + "/Portals/_default/";
            Globals.HostMapPath = httpServerUtility.MapPath( Globals.HostPath );

            Globals.AssemblyPath = Globals.ApplicationMapPath + "\\bin\\dotnetnuke.dll";

            //Check whether the current App Version is the same as the DB Version
            CheckVersion();

            //Cache Mapped Directory(s)
            CacheMappedDirectory();

            //log APPLICATION_START event
            LogStart();

            //Start Scheduler
            StartScheduler();

            //Process any messages in the EventQueue for the Application_Start event
            EventQueueController oEventController = new EventQueueController();
            oEventController.ProcessMessages( "Application_Start" );
        }

        /// <summary>
        /// Application_End
        /// Executes when the Application times out
        /// </summary>
        protected void Application_End( object sender, EventArgs e )
        {
            // log APPLICATION_END event
            LogEnd();

            // stop scheduled jobs
            StopScheduler();
        }

        /// <summary>
        /// Application_BeginRequest
        /// Executes when the request is initiated
        /// </summary>
        protected void Application_BeginRequest( object sender, EventArgs e )
        {
            //First check if we are upgrading/installing
            if( Request.Url.LocalPath.EndsWith( "Install.aspx" ) )
            {
                return;
            }

            try
            {
                if( SchedulingProvider.SchedulerMode == SchedulerMode.REQUEST_METHOD && SchedulingProvider.ReadyForPoll )
                {
                    SchedulingProvider scheduler = SchedulingProvider.Instance();
                    Thread RequestScheduleThread;
                    RequestScheduleThread = new Thread( new ThreadStart( scheduler.ExecuteTasks ) );
                    RequestScheduleThread.IsBackground = true;
                    RequestScheduleThread.Start();

                    SchedulingProvider.ScheduleLastPolled = DateTime.Now;
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
        }
    }
}