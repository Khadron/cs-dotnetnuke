using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Services.Vendors
{
    public class AffiliateController
    {
        public ArrayList GetAffiliates(int VendorId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetAffiliates(VendorId), typeof(AffiliateInfo));
        }

        public AffiliateInfo GetAffiliate(int AffiliateId, int VendorId, int PortalID)
        {
            return ((AffiliateInfo)CBO.FillObject(DataProvider.Instance().GetAffiliate(AffiliateId, VendorId, PortalID), typeof(AffiliateInfo)));
        }

        public void DeleteAffiliate(int AffiliateId)
        {
            DataProvider.Instance().DeleteAffiliate(AffiliateId);
        }

        public void AddAffiliate(AffiliateInfo objAffiliate)
        {
            DataProvider.Instance().AddAffiliate(objAffiliate.VendorId, objAffiliate.StartDate, objAffiliate.EndDate, objAffiliate.CPC, objAffiliate.CPA);
        }

        public void UpdateAffiliate(AffiliateInfo objAffiliate)
        {
            DataProvider.Instance().UpdateAffiliate(objAffiliate.AffiliateId, objAffiliate.StartDate, objAffiliate.EndDate, objAffiliate.CPC, objAffiliate.CPA);
        }

        public void UpdateAffiliateStats(int AffiliateId, int Clicks, int Acquisitions)
        {
            DataProvider.Instance().UpdateAffiliateStats(AffiliateId, Clicks, Acquisitions);
        }
    }
}