#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.Providers;

namespace DotNetNuke.Services.Exceptions
{
    [Serializable()]
    public class BasePortalException : Exception
    {
        private string m_AbsoluteURL;
        private string m_AbsoluteURLReferrer;
        private int m_ActiveTabID;
        private string m_ActiveTabName;
        private string m_AssemblyVersion;
        private string m_DefaultDataProvider;
        private string m_ExceptionGUID;
        private int m_FileColumnNumber;
        private int m_FileLineNumber;
        private string m_FileName;
        private string m_InnerExceptionString;
        private string m_Message;
        private string m_Method;
        private int m_PortalID;
        private string m_PortalName;
        private string m_RawURL;
        private string m_Source;
        private string m_StackTrace;
        private string m_UserAgent;
        private int m_UserID;
        private string m_UserName;

        public string AbsoluteURL
        {
            get
            {
                
                return this.m_AbsoluteURL;
            }
        }

        public string AbsoluteURLReferrer
        {
            get
            {
                return this.m_AbsoluteURLReferrer;
            }
        }

        public int ActiveTabID
        {
            get
            {
                return this.m_ActiveTabID;
            }
        }

        public string ActiveTabName
        {
            get
            {
                return this.m_ActiveTabName;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return this.m_AssemblyVersion;
            }
        }

        public string DefaultDataProvider
        {
            get
            {
                return this.m_DefaultDataProvider;
            }
        }

        public string ExceptionGUID
        {
            get
            {
                return this.m_ExceptionGUID;
            }
        }

        public int FileColumnNumber
        {
            get
            {
                return this.m_FileColumnNumber;
            }
        }

        public int FileLineNumber
        {
            get
            {
                return this.m_FileLineNumber;
            }
        }

        public string FileName
        {
            get
            {
                return this.m_FileName;
            }
        }

        public string Method
        {
            get
            {
                return this.m_Method;
            }
        }

        public int PortalID
        {
            get
            {
                return this.m_PortalID;
            }
        }

        public string PortalName
        {
            get
            {
                return this.m_PortalName;
            }
        }

        public string RawURL
        {
            get
            {
                return this.m_RawURL;
            }
        }

        [XmlIgnore()]
        public new MethodBase TargetSite
        {
            get
            {
                return base.TargetSite;
            }
        }

        public string UserAgent
        {
            get
            {
                return this.m_UserAgent;
            }
        }

        public int UserID
        {
            get
            {
                return this.m_UserID;
            }
        }

        public string UserName
        {
            get
            {
                return this.m_UserName;
            }
        }

        // default constructor
        public BasePortalException()
        {
        }

        //constructor with exception message
        public BasePortalException(string message)
            : base(message)
        {
            InitilizePrivateVariables();
        }

        // constructor with message and inner exception
        public BasePortalException(string message, Exception inner)
            : base(message, inner)
        {
            InitilizePrivateVariables();
        }

        protected BasePortalException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
            this.InitilizePrivateVariables();
            this.m_AssemblyVersion = info.GetString( "m_AssemblyVersion" );
            this.m_PortalID = info.GetInt32( "m_PortalID" );
            this.m_PortalName = info.GetString( "m_PortalName" );
            this.m_UserID = info.GetInt32( "m_UserID" );
            this.m_UserName = info.GetString( "m_Username" );
            this.m_ActiveTabID = info.GetInt32( "m_ActiveTabID" );
            this.m_ActiveTabName = info.GetString( "m_ActiveTabName" );
            this.m_RawURL = info.GetString( "m_RawURL" );
            this.m_AbsoluteURL = info.GetString( "m_AbsoluteURL" );
            this.m_AbsoluteURLReferrer = info.GetString( "m_AbsoluteURLReferrer" );
            this.m_UserAgent = info.GetString( "m_UserAgent" );
            this.m_DefaultDataProvider = info.GetString( "m_DefaultDataProvider" );
            this.m_ExceptionGUID = info.GetString( "m_ExceptionGUID" );
            this.m_InnerExceptionString = info.GetString( "m_InnerExceptionString" );
            this.m_FileName = info.GetString( "m_FileName" );
            this.m_FileLineNumber = info.GetInt32( "m_FileLineNumber" );
            this.m_FileColumnNumber = info.GetInt32( "m_FileColumnNumber" );
            this.m_Method = info.GetString( "m_Method" );
            this.m_StackTrace = info.GetString( "m_StackTrace" );
            this.m_Message = info.GetString( "m_Message" );
            this.m_Source = info.GetString( "m_Source" );
        }

        [SecurityPermission( SecurityAction.Demand )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            info.AddValue( "m_AssemblyVersion", this.m_AssemblyVersion, typeof( string ) );
            info.AddValue( "m_PortalID", this.m_PortalID, typeof( int ) );
            info.AddValue( "m_PortalName", this.m_PortalName, typeof( string ) );
            info.AddValue( "m_UserID", this.m_UserID, typeof( int ) );
            info.AddValue( "m_UserName", this.m_UserName, typeof( string ) );
            info.AddValue( "m_ActiveTabID", this.m_ActiveTabID, typeof( int ) );
            info.AddValue( "m_ActiveTabName", this.m_ActiveTabName, typeof( string ) );
            info.AddValue( "m_RawURL", this.m_RawURL, typeof( string ) );
            info.AddValue( "m_AbsoluteURL", this.m_AbsoluteURL, typeof( string ) );
            info.AddValue( "m_AbsoluteURLReferrer", this.m_AbsoluteURLReferrer, typeof( string ) );
            info.AddValue( "m_UserAgent", this.m_UserAgent, typeof( string ) );
            info.AddValue( "m_DefaultDataProvider", this.m_DefaultDataProvider, typeof( string ) );
            info.AddValue( "m_ExceptionGUID", this.m_ExceptionGUID, typeof( string ) );
            info.AddValue( "m_FileName", this.m_FileName, typeof( string ) );
            info.AddValue( "m_FileLineNumber", this.m_FileLineNumber, typeof( int ) );
            info.AddValue( "m_FileColumnNumber", this.m_FileColumnNumber, typeof( int ) );
            info.AddValue( "m_Method", this.m_Method, typeof( string ) );
            info.AddValue( "m_StackTrace", this.m_StackTrace, typeof( string ) );
            info.AddValue( "m_Message", this.m_Message, typeof( string ) );
            info.AddValue( "m_Source", this.m_Source, typeof( string ) );
            base.GetObjectData( info, context );
        }

        private void InitilizePrivateVariables()
        {
            //Try and get the Portal settings from context
            //If an error occurs getting the context then set the variables to -1
            try
            {
                HttpContext _context = HttpContext.Current;
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                Exception _objInnermostException;
                _objInnermostException = new Exception(this.Message, this);
                while (!(_objInnermostException.InnerException == null))
                {
                    _objInnermostException = _objInnermostException.InnerException;
                }
                ExceptionInfo _exceptionInfo = Exceptions.GetExceptionInfo(_objInnermostException);

                try
                {
                    m_AssemblyVersion = Globals.glbAppVersion;
                }
                catch
                {
                    m_AssemblyVersion = "-1";
                }

                try
                {
                    m_PortalID = _portalSettings.PortalId;
                    m_PortalName = _portalSettings.PortalName;
                }
                catch
                {
                    m_PortalID = -1;
                    m_PortalName = "";
                }

                try
                {
                    UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                    m_UserID = objUserInfo.UserID;
                }
                catch
                {
                    m_UserID = -1;
                }

                try
                {
                    if (m_UserID != -1)
                    {
                        UserInfo objUserInfo = UserController.GetUser(m_PortalID, m_UserID, false);
                        if (objUserInfo != null)
                        {
                            m_UserName = objUserInfo.Username;
                        }
                        else
                        {
                            m_UserName = "";
                        }
                    }
                    else
                    {
                        m_UserName = "";
                    }
                }
                catch
                {
                    m_UserName = "";
                }

                try
                {
                    m_ActiveTabID = _portalSettings.ActiveTab.TabID;
                }
                catch (Exception)
                {
                    m_ActiveTabID = -1;
                }

                try
                {
                    m_ActiveTabName = _portalSettings.ActiveTab.TabName;
                }
                catch (Exception)
                {
                    m_ActiveTabName = "";
                }

                try
                {
                    m_RawURL = _context.Request.RawUrl;
                }
                catch
                {
                    m_RawURL = "";
                }

                try
                {
                    m_AbsoluteURL = _context.Request.Url.AbsolutePath;
                }
                catch
                {
                    m_AbsoluteURL = "";
                }

                try
                {
                    m_AbsoluteURLReferrer = _context.Request.UrlReferrer.AbsoluteUri;
                }
                catch
                {
                    m_AbsoluteURLReferrer = "";
                }

                try
                {
                    m_UserAgent = _context.Request.UserAgent;
                }
                catch (Exception)
                {
                    m_UserAgent = "";
                }

                try
                {
                    ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data");
                    string strTypeName = ((Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
                    m_DefaultDataProvider = strTypeName;
                }
                catch (Exception)
                {
                    m_DefaultDataProvider = "";
                }

                try
                {
                    m_ExceptionGUID = Guid.NewGuid().ToString();
                }
                catch (Exception)
                {
                    m_ExceptionGUID = "";
                }

                try
                {
                    m_FileName = _exceptionInfo.FileName;
                }
                catch
                {
                    m_FileName = "";
                }

                try
                {
                    m_FileLineNumber = _exceptionInfo.FileLineNumber;
                }
                catch
                {
                    m_FileLineNumber = -1;
                }

                try
                {
                    m_FileColumnNumber = _exceptionInfo.FileColumnNumber;
                }
                catch
                {
                    m_FileColumnNumber = -1;
                }

                try
                {
                    m_Method = _exceptionInfo.Method;
                }
                catch
                {
                    m_Method = "";
                }

                try
                {
                    m_StackTrace = this.StackTrace;
                }
                catch (Exception)
                {
                    m_StackTrace = "";
                }

                try
                {
                    m_Message = this.Message;
                }
                catch (Exception)
                {
                    m_Message = "";
                }

                try
                {
                    m_Source = this.Source;
                }
                catch (Exception)
                {
                    m_Source = "";
                }
            }
            catch (Exception)
            {
                m_PortalID = -1;
                m_UserID = -1;
                m_AssemblyVersion = "-1";
                m_ActiveTabID = -1;
                m_ActiveTabName = "";
                m_RawURL = "";
                m_AbsoluteURL = "";
                m_AbsoluteURLReferrer = "";
                m_UserAgent = "";
                m_DefaultDataProvider = "";
                m_ExceptionGUID = "";
                m_FileName = "";
                m_FileLineNumber = -1;
                m_FileColumnNumber = -1;
                m_Method = "";
                m_StackTrace = "";
                m_Message = "";
                m_Source = "";
            }
        }
    }
}