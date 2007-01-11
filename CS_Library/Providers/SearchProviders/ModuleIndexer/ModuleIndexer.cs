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
using System.Collections;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Search
{
    /// <summary>
    /// The ModuleIndexer is an implementation of the abstract IndexingProvider
    /// class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    public class ModuleIndexer : IndexingProvider
    {
        /// <summary>
        /// GetSearchIndexItems gets the SearchInfo Items for the Portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        public override SearchItemInfoCollection GetSearchIndexItems(int PortalID)
        {
            SearchItemInfoCollection SearchItems = new SearchItemInfoCollection();
            SearchContentModuleInfoCollection SearchCollection = GetModuleList( PortalID );

            foreach( SearchContentModuleInfo ScModInfo in SearchCollection )
            {
                try
                {
                    SearchItemInfoCollection myCollection;
                    myCollection = ScModInfo.ModControllerType.GetSearchItems( ScModInfo.ModInfo );
                    if( myCollection != null )
                    {
                        SearchItems.AddRange( myCollection );
                    }
                }
                catch( Exception ex )
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }
            }

            return SearchItems;
        }

        /// <summary>
        /// GetModuleList gets a collection of SearchContentModuleInfo Items for the Portal
        /// </summary>
        /// <remarks>
        /// Parses the Modules of the Portal, determining whetehr they are searchable.
        /// </remarks>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        protected SearchContentModuleInfoCollection GetModuleList( int PortalID )
        {
            SearchContentModuleInfoCollection Results = new SearchContentModuleInfoCollection();

            ModuleController objModules = new ModuleController();
            ArrayList arrModules = objModules.GetSearchModules( PortalID );
            Hashtable businessControllers = new Hashtable();
            Hashtable htModules = new Hashtable();

            ModuleInfo objModule;
            foreach( ModuleInfo tempLoopVar_objModule in arrModules )
            {
                objModule = tempLoopVar_objModule;
                if( ! htModules.ContainsKey( objModule.ModuleID ) )
                {
                    try
                    {
                        //Check if the business controller is in the Hashtable
                        object objController = businessControllers[objModule.BusinessControllerClass];

                        //If nothing create a new instance
                        if( objController == null )
                        {
                            objController = Framework.Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );

                            //Add to hashtable
                            businessControllers.Add( objModule.BusinessControllerClass, objController );
                        }

                        //Double-Check that module supports ISearchable
                        if( objController is ISearchable )
                        {
                            SearchContentModuleInfo ContentInfo = new SearchContentModuleInfo();
                            ContentInfo.ModControllerType = (ISearchable)objController;
                            ContentInfo.ModInfo = objModule;
                            Results.Add( ContentInfo );
                        }
                    }
                    catch( Exception ex )
                    {
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    }
                    finally
                    {
                        htModules.Add( objModule.ModuleID, objModule.ModuleID );
                    }
                }
            }

            return Results;
        }
    }
}