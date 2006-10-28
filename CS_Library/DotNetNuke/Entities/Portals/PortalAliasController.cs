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