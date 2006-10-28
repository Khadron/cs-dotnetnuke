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