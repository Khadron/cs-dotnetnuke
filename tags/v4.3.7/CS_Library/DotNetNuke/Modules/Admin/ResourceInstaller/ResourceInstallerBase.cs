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
using System.Web;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <Summary>
    /// The ResourceInstallerBase class is a Base Class for all Resource Installer
    /// classes that need to use Localized Strings.  It provides these strings
    /// as localized Constants.
    /// </Summary>
    public class ResourceInstallerBase
    {
        protected string DNN_Reading;
        protected string DNN_Success;
        protected string DNN_Valid;
        protected string DNN_ValidSkinObject;
        protected string EXCEPTION;
        protected string EXCEPTION_DesktopSrc;
        protected string EXCEPTION_FileLoad;
        protected string EXCEPTION_FileName;
        protected string EXCEPTION_FileRead;
        protected string EXCEPTION_FolderDesc;
        protected string EXCEPTION_FolderName;
        protected string EXCEPTION_FolderProvider;
        protected string EXCEPTION_FolderVersion;
        protected string EXCEPTION_Format;
        protected string EXCEPTION_LoadFailed;
        protected string EXCEPTION_MissingDnn;
        protected string EXCEPTION_MissingResource;
        protected string EXCEPTION_MultipleDnn;
        protected string EXCEPTION_Src;
        protected string EXCEPTION_Type;
        protected string FILE_Created;
        protected string FILE_Found;
        protected string FILE_Loading;
        protected string FILE_NotFound;
        protected string FILE_ReadSuccess;
        protected string FILES_Created;
        protected string FILES_CreatedResources;
        protected string FILES_Creating;
        protected string FILES_Expanding;
        protected string FILES_Loading;
        protected string FILES_Reading;
        protected string FILES_ReadingEnd;
        protected string INSTALL_Compatibility;
        protected string INSTALL_Aborted;
        protected string INSTALL_Start;
        protected string INSTALL_Success;
        protected string MODULES_ControlInfo;
        protected string MODULES_Loading;
        protected string REGISTER_Controls;
        protected string REGISTER_Definition;
        protected string REGISTER_End;
        protected string REGISTER_Module;
        protected string SQL_Begin;
        protected string SQL_BeginFile;
        protected string SQL_End;
        protected string SQL_EndFile;
        protected string SQL_Exceptions;
        protected string SQL_Executing;
        protected string SQL_UnknownFile;
        protected string XML_Loaded;

        public ResourceInstallerBase()
        {
            this.DNN_Reading = this.GetLocalizedString( "DNN_Reading" );
            this.DNN_Success = this.GetLocalizedString( "DNN_Success" );
            this.DNN_Valid = this.GetLocalizedString( "DNN_Valid" );
            this.DNN_ValidSkinObject = this.GetLocalizedString( "DNN_ValidSkinObject" );
            this.EXCEPTION = this.GetLocalizedString( "EXCEPTION" );
            this.EXCEPTION_DesktopSrc = this.GetLocalizedString( "EXCEPTION_DesktopSrc" );
            this.EXCEPTION_FileLoad = this.GetLocalizedString( "EXCEPTION_FileLoad" );
            this.EXCEPTION_FileName = this.GetLocalizedString( "EXCEPTION_FileName" );
            this.EXCEPTION_FileRead = this.GetLocalizedString( "EXCEPTION_FileRead" );
            this.EXCEPTION_FolderDesc = this.GetLocalizedString( "EXCEPTION_FolderDesc" );
            this.EXCEPTION_FolderName = this.GetLocalizedString( "EXCEPTION_FolderName" );
            this.EXCEPTION_FolderProvider = this.GetLocalizedString( "EXCEPTION_FolderProvider" );
            this.EXCEPTION_FolderVersion = this.GetLocalizedString( "EXCEPTION_FolderVersion" );
            this.EXCEPTION_Format = this.GetLocalizedString( "EXCEPTION_LoadFailed" );
            this.EXCEPTION_LoadFailed = this.GetLocalizedString( "EXCEPTION_LoadFailed" );
            this.EXCEPTION_MissingDnn = this.GetLocalizedString( "EXCEPTION_MissingDnn" );
            this.EXCEPTION_MissingResource = this.GetLocalizedString( "EXCEPTION_MissingResource" );
            this.EXCEPTION_MultipleDnn = this.GetLocalizedString( "EXCEPTION_MultipleDnn" );
            this.EXCEPTION_Src = this.GetLocalizedString( "EXCEPTION_Src" );
            this.EXCEPTION_Type = this.GetLocalizedString( "EXCEPTION_Type" );
            this.FILE_Created = this.GetLocalizedString( "FILE_Created" );
            this.FILE_Found = this.GetLocalizedString( "FILE_Found" );
            this.FILE_Loading = this.GetLocalizedString( "FILE_Loading" );
            this.FILE_NotFound = this.GetLocalizedString( "FILE_NotFound" );
            this.FILE_ReadSuccess = this.GetLocalizedString( "FILE_ReadSuccess" );
            this.FILES_Created = this.GetLocalizedString( "FILES_Created" );
            this.FILES_CreatedResources = this.GetLocalizedString( "FILES_CreatedResources" );
            this.FILES_Creating = this.GetLocalizedString( "FILES_Creating" );
            this.FILES_Expanding = this.GetLocalizedString( "FILES_Expanding" );
            this.FILES_Loading = this.GetLocalizedString( "FILES_Loading" );
            this.FILES_Reading = this.GetLocalizedString( "FILES_Reading" );
            this.FILES_ReadingEnd = this.GetLocalizedString( "FILES_ReadingEnd" );
            this.INSTALL_Compatibility = GetLocalizedString("INSTALL_Compatibility");
            this.INSTALL_Aborted = this.GetLocalizedString( "INSTALL_Aborted" );
            this.INSTALL_Start = this.GetLocalizedString( "INSTALL_Start" );
            this.INSTALL_Success = this.GetLocalizedString( "INSTALL_Success" );
            this.MODULES_ControlInfo = this.GetLocalizedString( "MODULES_ControlInfo" );
            this.MODULES_Loading = this.GetLocalizedString( "MODULES_Loading" );
            this.REGISTER_Controls = this.GetLocalizedString( "REGISTER_Controls" );
            this.REGISTER_Definition = this.GetLocalizedString( "REGISTER_Definition" );
            this.REGISTER_End = this.GetLocalizedString( "REGISTER_End" );
            this.REGISTER_Module = this.GetLocalizedString( "REGISTER_Module" );
            this.SQL_Begin = this.GetLocalizedString( "SQL_Begin" );
            this.SQL_BeginFile = this.GetLocalizedString( "SQL_BeginFile" );
            this.SQL_End = this.GetLocalizedString( "SQL_End" );
            this.SQL_EndFile = this.GetLocalizedString( "SQL_EndFile" );
            this.SQL_Exceptions = this.GetLocalizedString( "SQL_Exceptions" );
            this.SQL_Executing = this.GetLocalizedString( "SQL_Executing" );
            this.SQL_UnknownFile = this.GetLocalizedString( "SQL_UnknownFile" );
            this.XML_Loaded = this.GetLocalizedString( "XML_Loaded" );
        }

        private string GetLocalizedString( string key )
        {
            PortalSettings portalSettings = ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            if( portalSettings != null )
            {
                return DotNetNuke.Services.Localization.Localization.GetString( key, portalSettings );
            }
            else
            {
                return key;
            }
        }
    }
}