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
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Modules
{
    public class DesktopModuleController
    {
        public int AddDesktopModule(DesktopModuleInfo objDesktopModule)
        {
            return DataProvider.Instance().AddDesktopModule(objDesktopModule.ModuleName, objDesktopModule.FolderName, objDesktopModule.FriendlyName, objDesktopModule.Description, objDesktopModule.Version, objDesktopModule.IsPremium, objDesktopModule.IsAdmin, objDesktopModule.BusinessControllerClass, objDesktopModule.SupportedFeatures, objDesktopModule.CompatibleVersions);
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

        public void UpdateDesktopModule(DesktopModuleInfo objDesktopModule)
        {
            DataProvider.Instance().UpdateDesktopModule(objDesktopModule.DesktopModuleID, objDesktopModule.ModuleName, objDesktopModule.FolderName, objDesktopModule.FriendlyName, objDesktopModule.Description, objDesktopModule.Version, objDesktopModule.IsPremium, objDesktopModule.IsAdmin, objDesktopModule.BusinessControllerClass, objDesktopModule.SupportedFeatures, objDesktopModule.CompatibleVersions);
        }
    }
}