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