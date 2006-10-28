using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Modules.Definitions
{
    public class ModuleDefinitionController
    {
        public ArrayList GetModuleDefinitions( int DesktopModuleId )
        {
            return CBO.FillCollection( DataProvider.Instance().GetModuleDefinitions( DesktopModuleId ), typeof( ModuleDefinitionInfo ) );
        }

        public ModuleDefinitionInfo GetModuleDefinition( int ModuleDefId )
        {
            return ( (ModuleDefinitionInfo)CBO.FillObject( DataProvider.Instance().GetModuleDefinition( ModuleDefId ), typeof( ModuleDefinitionInfo ) ) );
        }

        public ModuleDefinitionInfo GetModuleDefinitionByName( int DesktopModuleId, string FriendlyName )
        {
            return ( (ModuleDefinitionInfo)CBO.FillObject( DataProvider.Instance().GetModuleDefinitionByName( DesktopModuleId, FriendlyName ), typeof( ModuleDefinitionInfo ) ) );
        }

        public int AddModuleDefinition( ModuleDefinitionInfo objModuleDefinition )
        {
            return DataProvider.Instance().AddModuleDefinition( objModuleDefinition.DesktopModuleID, objModuleDefinition.FriendlyName, objModuleDefinition.DefaultCacheTime );
        }

        public void DeleteModuleDefinition( int ModuleDefinitionId )
        {
            DataProvider.Instance().DeleteModuleDefinition( ModuleDefinitionId );
        }

        public void UpdateModuleDefinition( ModuleDefinitionInfo objModuleDefinition )
        {
            DataProvider.Instance().UpdateModuleDefinition( objModuleDefinition.ModuleDefID, objModuleDefinition.FriendlyName, objModuleDefinition.DefaultCacheTime );
        }
    }
}