#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using TabInfo=DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.Services.Search
{
    /// <summary>
    /// The SearchDataStore is an implementation of the abstract SearchDataStoreProvider
    /// class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    public class SearchDataStore : SearchDataStoreProvider
    {
        private Hashtable _defaultSettings;
        private Hashtable _settings;
        private bool includeCommon = false;
        private bool includeNumbers = true;
        private int maxWordLength = 50;
        private int minWordLength = 4;

        /// <summary>
        /// CanIndexWord determines whether the Word should be indexed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strWord">The Word to validate</param>
        /// <returns>True if indexable, otherwise false</returns>
        /// <history>
        ///		[cnurse]	11/16/2004	created
        /// </history>
        private bool CanIndexWord( string strWord, string Locale )
        {
            //Create Boolean to hold return value
            bool retValue = true;

            // get common words for exclusion
            Hashtable CommonWords = GetCommonWords( Locale );

            //Determine if Word is actually a number
            int result;
            if( Int32.TryParse(strWord, out result) )
            {
                //Word is Numeric
                if( ! includeNumbers )
                {
                    retValue = false;
                }
            }
            else
            {
                //Word is Non-Numeric

                //Determine if Word satisfies Minimum/Maximum length
                if( strWord.Length < minWordLength || strWord.Length > maxWordLength )
                {
                    retValue = false;
                }
                else
                {
                    //Determine if Word is a Common Word (and should be excluded)
                    if( CommonWords.ContainsKey( strWord ) == true && ! includeCommon )
                    {
                        retValue = false;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetCommonWords gets a list of the Common Words for the locale
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="Locale">The locale string</param>
        /// <returns>A hashtable of common words</returns>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        private Hashtable GetCommonWords( string Locale )
        {
            string strCacheKey = "CommonWords" + Locale;

            Hashtable objWords = (Hashtable)DataCache.GetCache( strCacheKey );
            if( objWords == null )
            {
                objWords = new Hashtable();
                System.Data.IDataReader drWords = DataProvider.Instance().GetSearchCommonWordsByLocale( Locale );
                try
                {
                    while( drWords.Read() )
                    {
                        objWords.Add( drWords["CommonWord"].ToString(), drWords["CommonWord"].ToString() );
                    }
                }
                finally
                {
                    drWords.Close();
                    drWords.Dispose();
                }
                DataCache.SetCache( strCacheKey, objWords );
            }
            return objWords;
        }

        private SearchItemInfoCollection GetSearchItems(int ModuleId)
        {
            return new SearchItemInfoCollection( CBO.FillCollection( DataProvider.Instance().GetSearchItems( Null.NullInteger, Null.NullInteger, ModuleId ), typeof( SearchItemInfo ) ) );
        }

        /// <summary>
        /// GetSearchItems gets a collection of Search Items for a Module/Tab/Portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="PortalID">A Id of the Portal</param>
        /// <param name="TabID">A Id of the Tab</param>
        /// <param name="ModuleID">A Id of the Module</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        public override SearchResultsInfoCollection GetSearchItems(int PortalID, int TabID, int ModuleID)
        {
            return new SearchResultsInfoCollection( CBO.FillCollection( DataProvider.Instance().GetSearchItems( PortalID, TabID, ModuleID ), typeof( SearchResultsInfo ) ) );
        }

        /// <summary>
        /// GetSearchResults gets the search results for a passed in criteria string
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="PortalID">A Id of the Portal</param>
        /// <param name="Criteria">The criteria string</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        public override SearchResultsInfoCollection GetSearchResults(int PortalID, string Criteria)
        {
            //We will assume that the content is in the locale of the Portal
            PortalController objPortalController = new PortalController();
            PortalInfo objPortal = objPortalController.GetPortal( PortalID );
            string locale = objPortal.DefaultLanguage;
            Hashtable CommonWords = GetCommonWords( locale );

            // clean criteria
            Criteria = Criteria.ToLower();

            // split search criteria into words
            SearchCriteriaCollection SearchWords = new SearchCriteriaCollection( Criteria );
            Hashtable SearchResults = new Hashtable();

            // iterate through search criteria words
            SearchCriteria Criterion;
            foreach( SearchCriteria tempLoopVar_Criterion in SearchWords )
            {
                Criterion = tempLoopVar_Criterion;
                if( CommonWords.ContainsKey( Criterion.Criteria ) == false )
                {
                    SearchResultsInfoCollection ResultsCollection = SearchDataStoreController.GetSearchResults( PortalID, Criterion.Criteria );
                    if( Criterion.MustExclude == false )
                    {
                        // Add all these to the results
                        foreach( SearchResultsInfo Result in ResultsCollection )
                        {
                            if( SearchResults.ContainsKey( Result.SearchItemID ) )
                            {
                                ( (SearchResultsInfo)SearchResults[Result.SearchItemID] ).Relevance += Result.Relevance;
                            }
                            else
                            {
                                SearchResults.Add( Result.SearchItemID, Result );
                            }
                        }
                    }
                }
            }

            // Validate MustInclude and MustExclude
            foreach( SearchCriteria tempLoopVar_Criterion in SearchWords )
            {
                Criterion = tempLoopVar_Criterion;
                SearchResultsInfoCollection ResultsCollection = SearchDataStoreController.GetSearchResults( PortalID, Criterion.Criteria );
                if( Criterion.MustInclude )
                {
                    // We need to remove items which do not include this term
                    Hashtable MandatoryResults = new Hashtable();
                    foreach( SearchResultsInfo Result in ResultsCollection )
                    {
                        MandatoryResults.Add( Result.SearchItemID, 0 );
                    }
                    foreach( SearchResultsInfo Result in SearchResults.Values )
                    {
                        if( MandatoryResults.ContainsKey( Result.SearchItemID ) == false )
                        {
                            Result.Delete = true;
                        }
                    }
                }
                if( Criterion.MustExclude )
                {
                    // We need to remove items which do include this term
                    Hashtable ExcludedResults = new Hashtable();
                    foreach( SearchResultsInfo Result in ResultsCollection )
                    {
                        ExcludedResults.Add( Result.SearchItemID, 0 );
                    }
                    foreach( SearchResultsInfo Result in SearchResults.Values )
                    {
                        if( ExcludedResults.ContainsKey( Result.SearchItemID ) == true )
                        {
                            Result.Delete = true;
                        }
                    }
                }
            }

            //Only include results we have permission to see
            SearchResultsInfoCollection Results = new SearchResultsInfoCollection();
            TabController objTabController = new TabController();
            ModuleController objModuleController = new ModuleController();
            Hashtable hashTabsAllowed = new Hashtable();
            foreach( SearchResultsInfo SearchResult in SearchResults.Values )
            {
                if( ! SearchResult.Delete )
                {
                    //Check If authorised to View Tab
                    Hashtable hashModulesAllowed;
                    object tabAllowed = hashTabsAllowed[SearchResult.TabId];
                    if( tabAllowed == null )
                    {
                        TabInfo objTab = objTabController.GetTab( SearchResult.TabId );
                        if( PortalSecurity.IsInRoles( objTab.AuthorizedRoles ) )
                        {
                            hashModulesAllowed = new Hashtable();
                            tabAllowed = hashModulesAllowed;
                        }
                        else
                        {
                            tabAllowed = 0;
                            hashModulesAllowed = null;
                        }
                        hashTabsAllowed.Add( SearchResult.TabId, tabAllowed );
                    }
                    else
                    {
                        if( tabAllowed is Hashtable )
                        {
                            hashModulesAllowed = (Hashtable)tabAllowed;
                        }
                        else
                        {
                            hashModulesAllowed = null;
                        }
                    }

                    if( hashModulesAllowed != null )
                    {
                        bool addResult;
                        if( ! hashModulesAllowed.ContainsKey( SearchResult.ModuleId ) )
                        {
                            //Now check if authorized to view module
                            ModuleInfo objModule = objModuleController.GetModule( SearchResult.ModuleId, SearchResult.TabId );
                            addResult = objModule.IsDeleted == false && PortalSecurity.IsInRoles( objModule.AuthorizedViewRoles );
                            hashModulesAllowed.Add( SearchResult.ModuleId, addResult );
                        }
                        else
                        {
                            addResult = System.Convert.ToBoolean( hashModulesAllowed[SearchResult.ModuleId] );
                        }

                        if( addResult )
                        {
                            Results.Add( SearchResult );
                        }
                    }
                }
            }

            //Return Search Results Collection
            return Results;
        }

        /// <summary>
        /// GetSearchWords gets a list of the current Words in the Database's Index
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A hashtable of words</returns>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        private Hashtable GetSearchWords()
        {
            string strCacheKey = "SearchWords";

            Hashtable objWords = (Hashtable)DataCache.GetCache( strCacheKey );
            if( objWords == null )
            {
                objWords = new Hashtable();
                System.Data.IDataReader drWords = DataProvider.Instance().GetSearchWords();
                try
                {
                    while( drWords.Read() )
                    {
                        objWords.Add( drWords["Word"].ToString(), drWords["SearchWordsID"] );
                    }
                }
                finally
                {
                    drWords.Close();
                    drWords.Dispose();
                }
                DataCache.SetCache( strCacheKey, objWords, TimeSpan.FromMinutes( 2 ) );
            }
            return objWords;
        }

        /// <summary>
        /// GetSetting gets a Search Setting from the Portal Modules Settings table (or
        /// from the Host Settings)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/16/2004	created
        /// </history>
        private string GetSetting( string txtName )
        {
            string settingValue = "";

            //Try Portal setting first
            if( _settings[txtName] == null == false )
            {
                settingValue = System.Convert.ToString( _settings[txtName] );
            }
            else
            {
                //Get Default setting
                if( _defaultSettings[txtName] == null == false )
                {
                    settingValue = System.Convert.ToString( _defaultSettings[txtName] );
                }
            }

            return settingValue;
        }

        /// <summary>
        /// AddIndexWords adds the Index Words to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="IndexId">The Id of the SearchItem</param>
        /// <param name="SearchItem">The SearchItem</param>
        /// <param name="Language">The Language of the current Item</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        ///     [cnurse]    11/16/2004  replaced calls to separate content clean-up
        ///                             functions with new call to HtmlUtils.Clean().
        ///                             replaced logic to determine whether word should
        ///                             be indexed by call to CanIndexWord()
        /// </history>
        private void AddIndexWords(int IndexId, SearchItemInfo SearchItem, string Language)
        {
            Hashtable IndexWords = new Hashtable();
            Hashtable IndexPositions = new Hashtable();
            string setting;

            //Get the Settings for this Module
            _settings = SearchDataStoreController.GetSearchSettings( SearchItem.ModuleId );
            setting = GetSetting( "MaxSearchWordLength" );
            if( setting != "" )
            {
                maxWordLength = int.Parse( setting );
            }
            setting = GetSetting( "MinSearchWordLength" );
            if( setting != "" )
            {
                minWordLength = int.Parse( setting );
            }
            setting = GetSetting( "SearchIncludeCommon" );
            if( setting == "Y" )
            {
                includeCommon = true;
            }
            setting = GetSetting( "SearchIncludeNumeric" );
            if( setting == "N" )
            {
                includeNumbers = false;
            }

            string Content = SearchItem.Content;

            // clean content
            Content = HtmlUtils.Clean( Content, true );
            Content = Content.ToLower();

            //' split content into words
            string[] ContentWords = Content.Split( ' ' );

            // process each word
            int intWord = 0;
            string strWord;
            foreach( string tempLoopVar_strWord in ContentWords )
            {
                strWord = tempLoopVar_strWord;
                if( CanIndexWord( strWord, Language ) )
                {
                    intWord++;
                    if( IndexWords.ContainsKey( strWord ) == false )
                    {
                        IndexWords.Add( strWord, 0 );
                        IndexPositions.Add( strWord, 1 );
                    }
                    // track number of occurrences of word in content
                    IndexWords[strWord] = System.Convert.ToInt32( IndexWords[strWord] ) + 1;
                    // track positions of word in content
                    IndexPositions[strWord] = System.Convert.ToString( IndexPositions[strWord] ) + "," + intWord.ToString();
                }
            }

            // get list of words ( non-common )
            Hashtable Words = GetSearchWords(); // this could be cached
            int WordId;

            //' iterate through each indexed word
            object objWord;
            foreach( object tempLoopVar_objWord in IndexWords.Keys )
            {
                objWord = tempLoopVar_objWord;
                strWord = System.Convert.ToString( objWord );
                if( Words.ContainsKey( strWord ) )
                {
                    // word is in the DataStore
                    WordId = System.Convert.ToInt32( Words[strWord] );
                }
                else
                {
                    // add the word to the DataStore
                    WordId = DataProvider.Instance().AddSearchWord( strWord );
                    Words.Add( strWord, WordId );
                }
                // add the indexword
                int SearchItemWordID = DataProvider.Instance().AddSearchItemWord( IndexId, WordId, System.Convert.ToInt32( IndexWords[strWord] ) );
                DataProvider.Instance().AddSearchItemWordPosition( SearchItemWordID, System.Convert.ToString( IndexPositions[strWord] ) );
            }
        }

        /// <summary>
        /// StoreSearchItems adds the Search Item to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="SearchItems">A Collection of SearchItems</param>
        /// <history>
        ///		[cnurse]	11/15/2004	documented
        /// </history>
        public override void StoreSearchItems(SearchItemInfoCollection SearchItems)
        {
            int i;

            //Get the default Search Settings
            _defaultSettings = Common.Globals.HostSettings;

            //For now as we don't support Localized content - set the locale to the default locale. This
            //is to avoid the error in GetDefaultLanguageByModule which artificially limits the number
            //of modules that can be indexed.  This will need to be addressed when we support localized content.
            Hashtable Modules = new Hashtable();
            for( i = 0; i <= SearchItems.Count - 1; i++ )
            {
                if( ! Modules.ContainsKey( SearchItems[ i ].ModuleId.ToString() ) )
                {
                    Modules.Add( SearchItems[ i ].ModuleId.ToString(), "en-US" );
                }
            }

            SearchItemInfo SearchItem;
            SearchItemInfo IndexedItem;
            SearchItemInfoCollection IndexedItems;
            SearchItemInfoCollection ModuleItems;
            int IndexID;
            int iSearch;
            int ModuleId;
            string Language;
            bool ItemFound;

            //Process the SearchItems by Module to reduce Database hits
            IDictionaryEnumerator moduleEnumerator = Modules.GetEnumerator();
            while( moduleEnumerator.MoveNext() )
            {
                ModuleId = System.Convert.ToInt32( moduleEnumerator.Key );
                Language = System.Convert.ToString( moduleEnumerator.Value );

                //Get the Indexed Items that are in the Database for this Module
                IndexedItems = GetSearchItems( ModuleId );
                //Get the Module's SearchItems to compare
                ModuleItems = SearchItems.ModuleItems( ModuleId );

                //As we will be potentially removing items from the collection iterate backwards
                for( iSearch = ModuleItems.Count - 1; iSearch >= 0; iSearch-- )
                {
                    SearchItem = ModuleItems[ iSearch ];
                    ItemFound = false;

                    //Iterate through Indexed Items
                    foreach( SearchItemInfo tempLoopVar_IndexedItem in IndexedItems )
                    {
                        IndexedItem = tempLoopVar_IndexedItem;
                        //Compare the SearchKeys
                        if( SearchItem.SearchKey == IndexedItem.SearchKey )
                        {
                            //Item exists so compare Dates to see if modified
                            if( IndexedItem.PubDate < SearchItem.PubDate )
                            {
                                try
                                {
                                    //Content modified so update SearchItem and delete item's Words Collection
                                    SearchItem.SearchItemId = IndexedItem.SearchItemId;
                                    SearchDataStoreController.UpdateSearchItem( SearchItem );
                                    SearchDataStoreController.DeleteSearchItemWords( SearchItem.SearchItemId );

                                    // re-index the content
                                    AddIndexWords( SearchItem.SearchItemId, SearchItem, Language );
                                }
                                catch( Exception ex )
                                {
                                    //Log Exception
                                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                                }
                            }

                            //Remove Items from both collections
                            IndexedItems.Remove( IndexedItem );
                            ModuleItems.Remove( SearchItem );

                            //Exit the Iteration as Match found
                            ItemFound = true;
                            break;
                        }
                    }

                    if( ! ItemFound )
                    {
                        try
                        {
                            //Item doesn't exist so Add to Index
                            IndexID = SearchDataStoreController.AddSearchItem( SearchItem );
                            // index the content
                            AddIndexWords( IndexID, SearchItem, Language );
                        }
                        catch( Exception )
                        {
                            //Log Exception
                            //LogException(ex) ** this exception has been suppressed because it fills up the event log with duplicate key errors - we still need to understand what causes it though
                        }
                    }
                }

                //As we removed the IndexedItems as we matched them the remaining items are deleted Items
                //ie they have been indexed but are no longer present
                Hashtable ht = new Hashtable();
                foreach( SearchItemInfo tempLoopVar_IndexedItem in IndexedItems )
                {
                    IndexedItem = tempLoopVar_IndexedItem;
                    try
                    {
                        //dedupe
                        if( ht[IndexedItem.SearchItemId] == null )
                        {
                            SearchDataStoreController.DeleteSearchItem( IndexedItem.SearchItemId );
                            ht.Add( IndexedItem.SearchItemId, 0 );
                        }
                    }
                    catch( Exception ex )
                    {
                        //Log Exception
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    }
                }
            }
        }
    }
}