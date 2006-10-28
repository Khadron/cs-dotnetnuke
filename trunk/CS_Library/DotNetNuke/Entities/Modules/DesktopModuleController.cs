using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Modules
{
    public class DesktopModuleController
    {

        public int AddDesktopModule( DesktopModuleInfo objDesktopModule )
        {
            return DataProvider.Instance().AddDesktopModule( objDesktopModule.ModuleName, objDesktopModule.FolderName, objDesktopModule.FriendlyName, objDesktopModule.Description, objDesktopModule.Version, objDesktopModule.IsPremium, objDesktopModule.IsAdmin, objDesktopModule.BusinessControllerClass, objDesktopModule.SupportedFeatures );
        }
        public DesktopModuleInfo GetDesktopModule( int DesktopModuleId )
        {
            return ( (DesktopModuleInfo)CBO.FillObject( DataProvider.Instance().GetDesktopModule( DesktopModuleId ), typeof( DesktopModuleInfo ) ) );
        }

        public DesktopModuleInfo GetDesktopModuleByFriendlyName( string FriendlyName )
        {
            return ( (DesktopModuleInfo)CBO.FillObject( DataProvider.Instance().GetDesktopModuleByFriendlyName( FriendlyName ), typeof( DesktopModuleInfo ) ) );
        }

        public DesktopModuleInfo GetDesktopModuleByModuleName( string ModuleName )
        {
            return ( (DesktopModuleInfo)CBO.FillObject( DataProvider.Instance().GetDesktopModuleByModuleName( ModuleName ), typeof( DesktopModuleInfo ) ) );
        }

        public DesktopModuleInfo GetDesktopModuleByName( string FriendlyName )
        {
            return ( (DesktopModuleInfo)CBO.FillObject( DataProvider.Instance().GetDesktopModuleByFriendlyName( FriendlyName ), typeof( DesktopModuleInfo ) ) );
        }

        public ArrayList GetDesktopModules()
        {
            return CBO.FillCollection( DataProvider.Instance().GetDesktopModules(), typeof( DesktopModuleInfo ) );
        }

        public ArrayList GetDesktopModulesByPortal( int PortalID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetDesktopModulesByPortal( PortalID ), typeof( DesktopModuleInfo ) );
        }

        public ArrayList GetPortalDesktopModules( int PortalID, int DesktopModuleID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPortalDesktopModules( PortalID, DesktopModuleID ), typeof( PortalDesktopModuleInfo ) );
        }

        public void AddPortalDesktopModule( int PortalID, int DesktopModuleID )
        {
            DataProvider.Instance().AddPortalDesktopModule( PortalID, DesktopModuleID );
        }

        public void DeleteDesktopModule( int DesktopModuleId )
        {
            DataProvider.Instance().DeleteDesktopModule( DesktopModuleId );
        }

        public void DeletePortalDesktopModules( int PortalID, int DesktopModuleID )
        {
            DataProvider.Instance().DeletePortalDesktopModules( PortalID, DesktopModuleID );
        }

        public void UpdateDesktopModule( DesktopModuleInfo objDesktopModule )
        {
            DataProvider.Instance().UpdateDesktopModule( objDesktopModule.DesktopModuleID, objDesktopModule.ModuleName, objDesktopModule.FolderName, objDesktopModule.FriendlyName, objDesktopModule.Description, objDesktopModule.Version, objDesktopModule.IsPremium, objDesktopModule.IsAdmin, objDesktopModule.BusinessControllerClass, objDesktopModule.SupportedFeatures );
        }
    }
}