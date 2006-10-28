using System.IO;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnInstaller_V2Provider extends PaDnnInstallerBase to support V2 Providers
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnInstaller_V2Provider : PaDnnInstallerBase
    {
        public PaDnnInstaller_V2Provider( PaInstallInfo InstallerInfo ) : base( InstallerInfo )
        {
        }

        protected override void CreateFiles( PaFolder Folder )
        {
            InstallerInfo.Log.StartJob( FILES_Creating );
            // define bin folder location
            string binFolder = Path.Combine( InstallerInfo.SitePath, "bin" );

            // create the root folder
            string rootFolder = Path.Combine( InstallerInfo.SitePath, Path.Combine( "Providers", Path.Combine( Folder.ProviderType, Folder.Name ) ) );
            if( !Directory.Exists( rootFolder ) )
            {
                Directory.CreateDirectory( rootFolder );
            }

            // create the files
            PaFile file;
            foreach( PaFile tempLoopVar_file in Folder.Files )
            {
                file = tempLoopVar_file;
                if( file.Type == PaFileType.DataProvider || file.Type == PaFileType.Other || file.Type == PaFileType.Sql )
                {
                    // create the directory for this file
                    string fileFolder = Path.Combine( rootFolder, file.Path );
                    if( !Directory.Exists( fileFolder ) )
                    {
                        Directory.CreateDirectory( fileFolder );
                    }

                    // save file
                    string FullFileName = Path.Combine( fileFolder, file.Name );
                    CreateFile( FullFileName, file.Buffer );
                    InstallerInfo.Log.AddInfo( FILE_Created + FullFileName );
                }
                else if( file.Type == PaFileType.Dll )
                {
                    // all dlls go to the bin folder
                    string binFullFileName = Path.Combine( binFolder, file.Name );
                    CreateFile( binFullFileName, file.Buffer );
                    InstallerInfo.Log.AddInfo( FILE_Created + binFullFileName );
                }
            }
            InstallerInfo.Log.EndJob( FILES_Created );
        }

        public override void Install( PaFolderCollection folders )
        {
            PaFolder folder;
            foreach( PaFolder tempLoopVar_folder in folders )
            {
                folder = tempLoopVar_folder;
                CreateFiles( folder );
            }
        }
    }
}