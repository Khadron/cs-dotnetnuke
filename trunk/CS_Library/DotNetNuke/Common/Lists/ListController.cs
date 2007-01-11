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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Common.Lists
{

    public class ListController
    {
        public enum CacheScope
        {
            None,
            Lists,
            ListCollection
        }

        private const string ENTRY_LIST_CACHE_KEY = "Lists";

        public int AddListEntry(ListEntryInfo ListEntry)
        {
            bool EnableSortOrder = ListEntry.SortOrder > 0;
            return DataProvider.Instance().AddListEntry(ListEntry.ListName, ListEntry.Value, ListEntry.Text, ListEntry.ParentKey, EnableSortOrder, ListEntry.DefinitionID, ListEntry.Description);
        }

        public ListInfo FillListInfo(string ListName, string ParentKey, int DefinitionID) // Get single entry list by Name
        {
            IDataReader dr = DataProvider.Instance().GetList(ListName, ParentKey, DefinitionID);
            ListInfo objListInfo = null;

            try
            {
                if (dr.Read())
                {
                    objListInfo = new ListInfo(Convert.ToString(dr["ListName"]));
                    objListInfo.DisplayName = Convert.ToString(dr["DisplayName"]);
                    objListInfo.Level = Convert.ToInt32(dr["Level"]);
                    objListInfo.DefinitionID = Convert.ToInt32(dr["DefinitionID"]);
                    objListInfo.Key = Convert.ToString(dr["Key"]);
                    objListInfo.EntryCount = Convert.ToInt32(dr["EntryCount"]);
                    objListInfo.ParentID = Convert.ToInt32(dr["ParentID"]);
                    objListInfo.ParentKey = Convert.ToString(dr["ParentKey"]);
                    objListInfo.Parent = Convert.ToString(dr["Parent"]);
                    objListInfo.ParentList = Convert.ToString(dr["ParentList"]);
                    objListInfo.EnableSortOrder = Convert.ToInt32(dr["MaxSortOrder"]) > 0;
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if (dr != null)
                {
                    dr.Close();
                }
            }

            return objListInfo;
        }

        public ListEntryInfo GetListEntryInfo(int EntryID) // Get single entry by ID
        {
            return ((ListEntryInfo)CBO.FillObject(DataProvider.Instance().GetListEntries("", "", EntryID, -1), typeof(ListEntryInfo)));
        }

        public ListEntryInfo GetListEntryInfo(string ListName, string Value) // Get single entry by ListName/Value
        {
            return GetListEntryInfo(ListName, Value, "");
        }

        public ListEntryInfo GetListEntryInfo(string ListName, string Value, string ParentKey) // Get single entry by ListName/Value
        {
            return ((ListEntryInfo)CBO.FillObject(DataProvider.Instance().GetListEntriesByListName(ListName, Value, ParentKey), typeof(ListEntryInfo)));
        }

        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName) // Get collection of entry lists
        {
            return GetListEntryInfoCollection(ListName, "");
        }

        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName, string Value) // Get collection of entry lists
        {
            return GetListEntryInfoCollection(ListName, Value, "");
        }

        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName, string Value, string ParentKey) // Get collection of entry lists
        {
            ListEntryInfoCollection objListEntryInfoCollection = new ListEntryInfoCollection();
            ArrayList arrListEntries = CBO.FillCollection(DataProvider.Instance().GetListEntriesByListName(ListName, Value, ParentKey), typeof(ListEntryInfo));
            foreach (ListEntryInfo entry in arrListEntries)
            {
                objListEntryInfoCollection.Add(entry.Key, entry);
            }
            return objListEntryInfoCollection;
        }

        /// <summary>
        /// Get entry list(s)
        /// Optional parent key to specifed if result is all entries in that category or a subset
        /// </summary>
        /// <remarks>
        /// To be used with Method 2:
        /// This method query each array of entries when it needed
        /// An option to specified if arraylist object will be stored in cache or not
        /// pro: stored data in cache only if specified; don't have populate all entries into collection
        /// con: call database more than previous method; store to cache need to be considered each time using this method
        /// </remarks>
        public ListInfo GetListInfo(string ListName)
        {
            return GetListInfo(ListName, "");
        }

        public ListInfo GetListInfo(string ListName, string ParentKey)
        {
            return GetListInfo(ListName, ParentKey, -1);
        }

        public ListInfo GetListInfo(string ListName, string ParentKey, int DefinitionID)
        {
            return FillListInfo(ListName, ParentKey, DefinitionID);
        }

        public ListInfoCollection GetListInfoCollection()
        {
            return GetListInfoCollection("");
        }

        public ListInfoCollection GetListInfoCollection(string ListName)
        {
            return GetListInfoCollection(ListName, "");
        }

        public ListInfoCollection GetListInfoCollection(string ListName, string ParentKey)
        {
            return GetListInfoCollection(ListName, ParentKey, -1);
        }

        public ListInfoCollection GetListInfoCollection(string ListName, string ParentKey, int DefinitionID)
        {
            IList objListInfoCollection = new ListInfoCollection();
            ListInfo objListInfo = new ListInfo();

            return ((ListInfoCollection)CBO.FillCollection(DataProvider.Instance().GetList(ListName, ParentKey, DefinitionID), typeof(ListInfo), ref objListInfoCollection));
        }

        public void DeleteList(string ListName, string ParentKey)
        {
            DataProvider.Instance().DeleteList(ListName, ParentKey);
        }

        public void DeleteListEntryByID(int EntryID, bool DeleteChild)
        {
            DataProvider.Instance().DeleteListEntryByID(EntryID, DeleteChild);
        }

        public void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild)
        {
            DataProvider.Instance().DeleteListEntryByListName(ListName, Value, DeleteChild);
        }

        public void UpdateListEntry(ListEntryInfo ListEntry)
        {
            DataProvider.Instance().UpdateListEntry(ListEntry.EntryID, ListEntry.ListName, ListEntry.Value, ListEntry.Text, ListEntry.Description);
        }

        public void UpdateListSortOrder(int EntryID, bool MoveUp)
        {
            DataProvider.Instance().UpdateListSortOrder(EntryID, MoveUp);
        }
    }
}