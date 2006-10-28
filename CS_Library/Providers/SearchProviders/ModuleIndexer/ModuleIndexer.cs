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