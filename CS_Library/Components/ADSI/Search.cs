#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
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
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetNuke.Security.Authentication.ADSI
{
    public class Search : DirectorySearcher
    {
        private ArrayList mSearchFilters = new ArrayList();
        private string mFilterString;

        public Search()
        {
        }

        public Search( DirectoryEntry rearchRoot ) : base( rearchRoot )
        {
            PopulateDefaultProperties();
        }

        public Search( DirectoryEntry rearchRoot, string Filter, string SortProperty ) : base( rearchRoot, Filter )
        {
            PopulateDefaultProperties();

            Sort.PropertyName = SortProperty;
        }

        private void PopulateDefaultProperties()
        {
            CacheResults = true; // default is True
            ReferralChasing = ReferralChasingOption.All; //default is External
            SearchScope = SearchScope.Subtree; //default is Subtree
            PageSize = 1000;
        }

        public DirectoryEntry GetEntry()
        {
            SearchResult result;

            try
            {
                Filter = FilterString;
                result = FindOne();

                if( result != null )
                {
                    return result.GetDirectoryEntry();
                }
                else
                {
                    return null;
                }
            }
            catch( COMException )
            {
                return null;
            }
        }

        public ArrayList GetEntries()
        {
            SearchResultCollection resultCollection;
            SearchResult result;
            ArrayList entries = new ArrayList();
            try
            {
                Filter = FilterString;
                resultCollection = FindAll();
                foreach( SearchResult tempLoopVar_result in resultCollection )
                {
                    result = tempLoopVar_result;
                    entries.Add( result.GetDirectoryEntry() );
                }
            }
            catch( COMException )
            {
            }

            return entries;
        }

        public void AddFilter( string Name, CompareOperator @Operator, string Value )
        {
            SearchFilter filter = new SearchFilter();

            filter.SetFilter( Name, @Operator, Value );
            mSearchFilters.Add( filter );
        }

        public ArrayList SearchFilters
        {
            get
            {
                return mSearchFilters;
            }
            set
            {
                mSearchFilters = value;
            }
        }

        public string FilterString
        {
            get
            {
                SearchFilter filter;
                StringBuilder sb = new StringBuilder();

                sb.Append( "(&" );
                foreach( SearchFilter tempLoopVar_filter in this.SearchFilters )
                {
                    filter = tempLoopVar_filter;
                    sb.Append( AppendFilter( filter ) );
                }
                sb.Append( ")" );
                return sb.ToString();
            }
        }

        private string AppendFilter( SearchFilter Filter )
        {
            StringBuilder sb = new StringBuilder();
            SearchFilter with_1 = Filter;
            switch( Filter.ADSICompareOperator )
            {
                case CompareOperator.Is:

                    sb.Append( "(" );
                    sb.Append( with_1.Name );
                    sb.Append( "=" );
                    sb.Append( with_1.Value );
                    sb.Append( ")" );
                    break;
                case CompareOperator.IsNot:

                    sb.Append( "(!" );
                    sb.Append( with_1.Name );
                    sb.Append( "=" );
                    sb.Append( with_1.Value );
                    sb.Append( ")" );
                    break;
                case CompareOperator.StartsWith:

                    sb.Append( "(" );
                    sb.Append( with_1.Name );
                    sb.Append( "=" );
                    sb.Append( with_1.Value );
                    sb.Append( "*)" );
                    break;
                case CompareOperator.EndsWith:

                    sb.Append( "(" );
                    sb.Append( with_1.Name );
                    sb.Append( "=*" );
                    sb.Append( with_1.Value );
                    sb.Append( ")" );
                    break;
                case CompareOperator.Present:

                    sb.Append( "(" );
                    sb.Append( with_1.Name );
                    sb.Append( "=" );
                    sb.Append( "*)" );
                    break;
                case CompareOperator.NotPresent:

                    sb.Append( "(!" );
                    sb.Append( with_1.Name );
                    sb.Append( "=" );
                    sb.Append( "*)" );
                    break;
            }

            return sb.ToString();
        }

        public struct SearchFilter
        {
            internal string mName;
            internal string mValue;
            internal CompareOperator mCompareOperator;

            internal void SetFilter( string Name, CompareOperator @Operator, string Value )
            {
                mName = Name;
                mValue = Value;
                mCompareOperator = @Operator;
            }

            public string Name
            {
                get
                {
                    return mName;
                }
            }

            public string Value
            {
                get
                {
                    return mValue;
                }
            }

            public CompareOperator ADSICompareOperator
            {
                get
                {
                    return mCompareOperator;
                }
            }
        }
    }
}