using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Services.Vendors
{
    public class ClassificationController
    {
        public ArrayList GetVendorClassifications(int VendorId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetVendorClassifications(VendorId), typeof(ClassificationInfo));
        }

        public void DeleteVendorClassifications(int VendorId)
        {
            DataProvider.Instance().DeleteVendorClassifications(VendorId);
        }

        public void AddVendorClassification(int VendorId, int ClassificationId)
        {
            DataProvider.Instance().AddVendorClassification(VendorId, ClassificationId);
        }
    }
}