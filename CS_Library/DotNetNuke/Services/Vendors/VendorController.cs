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
using Microsoft.VisualBasic;

namespace DotNetNuke.Services.Vendors
{
    public class VendorController
    {
        public int AddVendor( VendorInfo objVendor )
        {
            return DataProvider.Instance().AddVendor( objVendor.PortalId, objVendor.VendorName, objVendor.Unit, objVendor.Street, objVendor.City, objVendor.Region, objVendor.Country, objVendor.PostalCode, objVendor.Telephone, objVendor.Fax, objVendor.Cell, objVendor.Email, objVendor.Website, objVendor.FirstName, objVendor.LastName, objVendor.UserName, objVendor.LogoFile, objVendor.KeyWords, objVendor.Authorized.ToString() );
        }

        public VendorInfo GetVendor( int VendorID, int PortalId )
        {
            return ( (VendorInfo)CBO.FillObject( DataProvider.Instance().GetVendor( VendorID, PortalId ), typeof( VendorInfo ) ) );
        }

        public ArrayList GetVendors( int PortalId, string Filter )
        {
            int TotalRecords = 0;
            return GetVendorsByName( Filter, PortalId, 0, 100000, ref TotalRecords );
        }

        public ArrayList GetVendors( int PortalId, bool UnAuthorized, int PageIndex, int PageSize, ref int TotalRecords )
        {
            IDataReader dr = DataProvider.Instance().GetVendors( PortalId, UnAuthorized, PageIndex, PageSize );
            ArrayList retValue = null;
            try
            {
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
                dr.NextResult();
                retValue = CBO.FillCollection( dr, typeof( VendorInfo ) );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return retValue;
        }

        public ArrayList GetVendorsByEmail( string Filter, int PortalId, int Page, int PageSize, ref int TotalRecords )
        {
            IDataReader dr = DataProvider.Instance().GetVendorsByEmail( Filter, PortalId, Page, PageSize );
            try
            {
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
                dr.NextResult();
                return CBO.FillCollection( dr, typeof( VendorInfo ) );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        public ArrayList GetVendorsByName( string Filter, int PortalId, int Page, int PageSize, ref int TotalRecords )
        {
            IDataReader dr = DataProvider.Instance().GetVendorsByName( Filter, PortalId, Page, PageSize );
            try
            {
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
                dr.NextResult();
                return CBO.FillCollection( dr, typeof( VendorInfo ) );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        public void DeleteVendor( int VendorID )
        {
            DataProvider.Instance().DeleteVendor( VendorID );
        }

        public void DeleteVendors()
        {
            DeleteVendors( -1 );
        }

        public void DeleteVendors( int PortalID )
        {
            IDataReader dr = DataProvider.Instance().GetVendors( PortalID, true, Null.NullInteger, Null.NullInteger );
            if( dr != null )
            {
                dr.NextResult();
            }
            while( dr.Read() )
            {
                if( DateAndTime.DateDiff( DateInterval.Day, Convert.ToDateTime( dr["CreatedDate"] ), DateAndTime.Now, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1 ) > ( (long)7 ) )
                {
                    DataProvider.Instance().DeleteVendor( Convert.ToInt32( dr["VendorID"] ) );
                }
            }
            dr.Close();
        }

        public void UpdateVendor( VendorInfo objVendor )
        {
            DataProvider.Instance().UpdateVendor( objVendor.VendorId, objVendor.VendorName, objVendor.Unit, objVendor.Street, objVendor.City, objVendor.Region, objVendor.Country, objVendor.PostalCode, objVendor.Telephone, objVendor.Fax, objVendor.Cell, objVendor.Email, objVendor.Website, objVendor.FirstName, objVendor.LastName, objVendor.UserName, objVendor.LogoFile, objVendor.KeyWords, objVendor.Authorized.ToString() );
        }
    }
}