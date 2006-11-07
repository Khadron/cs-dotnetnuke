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
using System.IO;
using DotNetNuke.Services.Log.EventLog;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaInstaller : ResourceInstallerBase
    {
        private PaFolderCollection _dnnFolders;
        private PaInstallInfo _installInfo = new PaInstallInfo();
        private Stream _zipStream;

        public PaInstallInfo InstallerInfo
        {
            get
            {
                return _installInfo;
            }
        }

        public Stream ZipStream
        {
            get
            {
                return _zipStream;
            }
        }

        public PaInstaller( string Path, string SitePath )
        {
            InstallerInfo.SitePath = SitePath;
            _zipStream = new FileStream( Path, FileMode.Open, FileAccess.Read );
        }

        public PaInstaller( Stream inputStream, string SitePath )
        {
            InstallerInfo.SitePath = SitePath;
            _zipStream = inputStream;
        }

        public bool Install()
        {
            // -----------------------------------------------------------------------------
            // Step 1:  Expand ZipFile in memory - identify .dnn file
            // Step 2:  Identify .dnn version/type and translate to object model
            // Step 3:  Install objects
            // -----------------------------------------------------------------------------

            InstallerInfo.Log.StartJob( INSTALL_Start );
            try
            {
                // Step 1
                ReadZipStream();

                // Step 2
                PaDnnLoaderFactory Factory = new PaDnnLoaderFactory( InstallerInfo );
                _dnnFolders = Factory.GetDnnAdapter().ReadDnn();

                // Step 3
                Factory.GetDnnInstaller().Install( _dnnFolders );
            }
            catch( Exception ex )
            {
                InstallerInfo.Log.Add( ex );
                return false;
            }

            InstallerInfo.Log.EndJob( INSTALL_Success );

            // log installation event
            try
            {
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Install Module:", InstallerInfo.DnnFile.Name.Replace( ".dnn", "" ) ) );
                PaLogEntry objLogEntry;
                foreach( PaLogEntry tempLoopVar_objLogEntry in InstallerInfo.Log.Logs )
                {
                    objLogEntry = tempLoopVar_objLogEntry;
                    objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Info:", objLogEntry.Description ) );
                }
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog( objEventLogInfo );
            }
            catch( Exception )
            {
                // error
            }

            return true;
        }

        private void ReadZipStream()
        {
            InstallerInfo.Log.StartJob( FILES_Reading );

            ZipInputStream unzip = new ZipInputStream( ZipStream );

            ZipEntry entry = unzip.GetNextEntry();

            while( !( entry == null ) )
            {
                if( !entry.IsDirectory )
                {
                    InstallerInfo.Log.AddInfo( FILE_Loading + entry.Name );

                    // add file to the file list
                    PaFile file = new PaFile( unzip, entry );

                    InstallerInfo.FileTable[file.Name.ToLower()] = file;

                    if( file.Type == PaFileType.Dnn )
                    {
                        if( !( InstallerInfo.DnnFile == null ) )
                        {
                            InstallerInfo.Log.AddFailure( EXCEPTION_MultipleDnn + InstallerInfo.DnnFile.Name + " and " + file.Name );
                        }
                        else
                        {
                            InstallerInfo.DnnFile = file;
                        }
                    }
                    InstallerInfo.Log.AddInfo( string.Format( FILE_ReadSuccess, file.FullName ) );
                }
                entry = unzip.GetNextEntry();
            }

            if( InstallerInfo.DnnFile == null )
            {
                InstallerInfo.Log.AddFailure( EXCEPTION_MissingDnn );
            }

            if( InstallerInfo.Log.Valid )
            {
                InstallerInfo.Log.EndJob( FILES_ReadingEnd );
            }
            else
            {
                throw ( new Exception( EXCEPTION_FileLoad ) );
            }
        }
    }
}