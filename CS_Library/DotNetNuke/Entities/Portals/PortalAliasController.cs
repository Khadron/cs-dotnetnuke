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
using System;
using System.Collections;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Portals
{
    public class PortalAliasController
    {
        public int AddPortalAlias(PortalAliasInfo objPortalAliasInfo)
        {
            DotNetNuke.Common.Utilities.DataCache.ClearHostCache(false);

            return DotNetNuke.Data.DataProvider.Instance().AddPortalAlias(objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias);
        }

        public PortalAliasInfo GetPortalAlias(string PortalAlias, int PortalID)
        {
            return ((PortalAliasInfo)DotNetNuke.Common.Utilities.CBO.FillObject(DotNetNuke.Data.DataProvider.Instance().GetPortalAlias(PortalAlias, PortalID), typeof(PortalAliasInfo)));
        }

        public ArrayList GetPortalAliasArrayByPortalID(int PortalID)
        {
            IDataReader dr = DotNetNuke.Data.DataProvider.Instance().GetPortalAliasByPortalID(PortalID);
            try
            {
                ArrayList arr = new ArrayList();
                while (dr.Read())
                {
                    PortalAliasInfo objPortalAliasInfo = new PortalAliasInfo();
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr["PortalAliasID"]);
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr["PortalID"]);
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr["HTTPAlias"]).ToLower();
                    arr.Add(objPortalAliasInfo);
                }
                return arr;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
        }

        public PortalAliasInfo GetPortalAliasByPortalAliasID(int PortalAliasID)
        {
            return ((PortalAliasInfo)DotNetNuke.Common.Utilities.CBO.FillObject((DotNetNuke.Data.DataProvider.Instance().GetPortalAliasByPortalAliasID(PortalAliasID)), typeof(PortalAliasInfo)));
        }

        public PortalAliasCollection GetPortalAliasByPortalID(int PortalID)
        {
            IDataReader dr = DotNetNuke.Data.DataProvider.Instance().GetPortalAliasByPortalID(PortalID);
            try
            {
                PortalAliasCollection objPortalAliasCollection = new PortalAliasCollection();
                while (dr.Read())
                {
                    PortalAliasInfo objPortalAliasInfo = new PortalAliasInfo();
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr["PortalAliasID"]);
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr["PortalID"]);
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr["HTTPAlias"]);
                    objPortalAliasCollection.Add(Convert.ToString(dr["HTTPAlias"]).ToLower(), objPortalAliasInfo);
                }
                return objPortalAliasCollection;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
        }

        public PortalAliasCollection GetPortalAliases()
        {
            return GetPortalAliasByPortalID(-1);
        }

        public PortalInfo GetPortalByPortalAliasID(int PortalAliasId)
        {
            return ((PortalInfo)DotNetNuke.Common.Utilities.CBO.FillObject(DotNetNuke.Data.DataProvider.Instance().GetPortalByPortalAliasID(PortalAliasId), typeof(PortalInfo)));
        }

        public void DeletePortalAlias(int PortalAliasID)
        {
            DotNetNuke.Common.Utilities.DataCache.ClearHostCache(false);

            DotNetNuke.Data.DataProvider.Instance().DeletePortalAlias(PortalAliasID);
        }

        public void UpdatePortalAliasInfo(PortalAliasInfo objPortalAliasInfo)
        {
            DotNetNuke.Common.Utilities.DataCache.ClearHostCache(false);

            DotNetNuke.Data.DataProvider.Instance().UpdatePortalAliasInfo(objPortalAliasInfo.PortalAliasID, objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias);
        }
    }
}