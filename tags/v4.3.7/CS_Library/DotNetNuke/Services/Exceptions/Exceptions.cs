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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Services.Exceptions
{
    [StandardModule()]
    public sealed class Exceptions
    {
        public static ExceptionInfo GetExceptionInfo( Exception e )
        {
            ExceptionInfo objExceptionInfo = new ExceptionInfo();

            while( !( e.InnerException == null ) )
            {
                e = e.InnerException;
            }

            StackTrace st = new StackTrace( e, true );
            StackFrame sf = st.GetFrame( 0 );

            try
            {
                //Get the corresponding method for that stack frame.
                MemberInfo mi = sf.GetMethod();
                // Get the namespace where that method is defined.
                string res = mi.DeclaringType.Namespace + ".";
                // Append the type name.
                res += mi.DeclaringType.Name + ".";
                // Append the name of the method.
                res += mi.Name;
                objExceptionInfo.Method = res;
            }
            catch( Exception )
            {
                objExceptionInfo.Method = "N/A - Reflection Permission required";
            }

            if( sf.GetFileName() != "" )
            {
                objExceptionInfo.FileName = sf.GetFileName();
                objExceptionInfo.FileColumnNumber = sf.GetFileColumnNumber();
                objExceptionInfo.FileLineNumber = sf.GetFileLineNumber();
            }

            return objExceptionInfo;
        }

        private static bool ThreadAbortCheck( Exception exc )
        {
            if( exc is ThreadAbortException )
            {
                Thread.ResetAbort();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LogException( ModuleLoadException exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.MODULE_LOAD_EXCEPTION );
        }

        public static void LogException( PageLoadException exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.PAGE_LOAD_EXCEPTION );
        }

        public static void LogException( SchedulerException exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.SCHEDULER_EXCEPTION );
        }

        public static void LogException( SecurityException exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.SECURITY_EXCEPTION );
        }

        public static void LogException( Exception exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.GENERAL_EXCEPTION );
        }

        public static void ProcessModuleLoadException( string FriendlyMessage, PortalModuleBase ctrlModule, Exception exc, bool DisplayErrorMessage )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            try
            {
                if( Convert.ToString( Globals.HostSettings["UseCustomErrorMessages"] ) == "N" )
                {
                    throw ( new ModuleLoadException( FriendlyMessage, exc ) );
                }
                else
                {
                    ModuleLoadException lex = new ModuleLoadException( exc.Message.ToString(), exc, ctrlModule.ModuleConfiguration );
                    //publish the exception

                    ExceptionLogController objExceptionLog = new ExceptionLogController();
                    objExceptionLog.AddLog( lex );

                    //Some modules may want to suppress an error message
                    //and just log the exception.
                    if( DisplayErrorMessage )
                    {
                        PlaceHolder ErrorPlaceholder = null;
                        if( ctrlModule.Parent != null )
                        {
                            ErrorPlaceholder = (PlaceHolder)ctrlModule.Parent.FindControl( "MessagePlaceHolder" );
                        }
                        if( ErrorPlaceholder != null )
                        {
                            //hide the module
                            ctrlModule.Visible = false;
                            ErrorPlaceholder.Visible = true;
                            ErrorPlaceholder.Controls.Add( new ErrorContainer( _portalSettings, FriendlyMessage, lex ).Container ); //add the error message to the error placeholder
                        }
                        else
                        {
                            //there's no ErrorPlaceholder, add it to the module's control collection
                            ctrlModule.Controls.Add( new ErrorContainer( _portalSettings, FriendlyMessage, lex ).Container );
                        }
                    }
                }
            }
            catch( Exception exc2 )
            {
                ProcessPageLoadException( exc2 );
            }
        }

        public static void ProcessModuleLoadException( PortalModuleBase ctrlModule, Exception exc )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            ProcessModuleLoadException( string.Format( Localization.Localization.GetString( "ModuleUnavailable" ), ctrlModule.ModuleConfiguration.ModuleTitle ), ctrlModule, exc, true );
        }

        public static void ProcessModuleLoadException( PortalModuleBase ctrlModule, Exception exc, bool DisplayErrorMessage )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            ProcessModuleLoadException( string.Format( Localization.Localization.GetString( "ModuleUnavailable" ), ctrlModule.ModuleConfiguration.ModuleTitle ), ctrlModule, exc, DisplayErrorMessage );
        }

        public static void ProcessModuleLoadException( string FriendlyMessage, Control UserCtrl, Exception exc )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }

            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            try
            {
                if( Convert.ToString( Globals.HostSettings["UseCustomErrorMessages"] ) == "N" )
                {
                    throw ( new ModuleLoadException( FriendlyMessage, exc ) );
                }
                else
                {
                    ModuleLoadException lex = new ModuleLoadException( exc.Message.ToString(), exc );
                    //publish the exception
                    ExceptionLogController objExceptionLog = new ExceptionLogController();
                    objExceptionLog.AddLog( lex );
                    //add the error message to the user control
                    UserCtrl.Controls.Add( new ErrorContainer( _portalSettings, FriendlyMessage, lex ).Container );
                }
            }
            catch
            {
                ProcessPageLoadException( exc );
            }
        }

        public static void ProcessModuleLoadException( string FriendlyMessage, Control ctrlModule, Exception exc, bool DisplayErrorMessage )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            try
            {
                if( Convert.ToString( Globals.HostSettings["UseCustomErrorMessages"] ) == "N" )
                {
                    throw ( new ModuleLoadException( FriendlyMessage, exc ) );
                }
                else
                {
                    ModuleLoadException lex = new ModuleLoadException( exc.Message.ToString(), exc );
                    //publish the exception
                    ExceptionLogController objExceptionLog = new ExceptionLogController();
                    objExceptionLog.AddLog( lex );
                    //Some modules may want to suppress an error message
                    //and just log the exception.
                    if( DisplayErrorMessage )
                    {
                        PlaceHolder ErrorPlaceholder = (PlaceHolder)ctrlModule.Parent.FindControl( "MessagePlaceHolder" );
                        if( ErrorPlaceholder != null )
                        {
                            //hide the module
                            ctrlModule.Visible = false;
                            ErrorPlaceholder.Visible = true;

                            ErrorPlaceholder.Controls.Add( new ErrorContainer( _portalSettings, FriendlyMessage, lex ).Container ); //add the error message to the error placeholder
                        }
                        else
                        {
                            //there's no ErrorPlaceholder, add it to the module's control collection
                            ctrlModule.Controls.Add( new ErrorContainer( _portalSettings, FriendlyMessage, lex ).Container );
                        }
                    }
                }
            }
            catch( Exception exc2 )
            {
                ProcessPageLoadException( exc2 );
            }
        }

        public static void ProcessModuleLoadException( Control UserCtrl, Exception exc )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            ProcessModuleLoadException( Localization.Localization.GetString( "ErrorOccurred" ), UserCtrl, exc );
        }

        public static void ProcessModuleLoadException( Control UserCtrl, Exception exc, bool DisplayErrorMessage )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }
            ProcessModuleLoadException( Localization.Localization.GetString( "ErrorOccurred" ), UserCtrl, exc, DisplayErrorMessage );
        }

        public static void ProcessPageLoadException( Exception exc )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string appURL = Globals.ApplicationURL();
            if( appURL.IndexOf( "?" ) == Null.NullInteger )
            {
                appURL += "?def=ErrorMessage";
            }
            else
            {
                appURL += "&def=ErrorMessage";
            }
            ProcessPageLoadException( exc, appURL );
        }

        public static void ProcessPageLoadException( Exception exc, string URL )
        {
            if( ThreadAbortCheck( exc ) )
            {
                return;
            }

            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            if( Convert.ToString( Globals.HostSettings["UseCustomErrorMessages"] ) == "N" )
            {
                throw ( new PageLoadException( exc.Message, exc ) );
            }
            else
            {
                PageLoadException lex = new PageLoadException( exc.Message.ToString(), exc );
                //publish the exception
                ExceptionLogController objExceptionLog = new ExceptionLogController();
                objExceptionLog.AddLog( lex );
                // redirect

                if( !String.IsNullOrEmpty(URL) )
                {
                    if( URL.IndexOf( "error=terminate" ) != -1 )
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Server.Transfer( "~/ErrorPage.aspx" );
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect( URL, true );
                    }
                }
            }
        }

        public static void ProcessSchedulerException( Exception exc )
        {
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( exc, ExceptionLogController.ExceptionLogType.SCHEDULER_EXCEPTION );
        }
    }
}