using System.Collections;
using System.Data;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Data;

namespace DotNetNuke.Services.Personalization
{
    public class PersonalizationController
    {
        // default implementation relies on HTTPContext
        public void LoadProfile(HttpContext objHTTPContext, int UserId, int PortalId)
        {
            objHTTPContext.Items.Add("Personalization", LoadProfile(UserId, PortalId));
        }

        // override allows for manipulation of PersonalizationInfo outside of HTTPContext
        public PersonalizationInfo LoadProfile(int UserId, int PortalId)
        {
            PersonalizationInfo objPersonalization = new PersonalizationInfo();

            objPersonalization.UserId = UserId;
            objPersonalization.PortalId = PortalId;
            objPersonalization.IsModified = false;

            IDataReader dr = DataProvider.Instance().GetProfile(UserId, PortalId);
            if (dr.Read())
            {
                objPersonalization.Profile = Globals.DeserializeHashTableXml(dr["ProfileData"].ToString());
            }
            else // does not exist
            {
                DataProvider.Instance().AddProfile(UserId, PortalId);
                objPersonalization.Profile = new Hashtable();
            }
            dr.Close();

            return objPersonalization;
        }

        public void SaveProfile(PersonalizationInfo objPersonalization)
        {
            SaveProfile(objPersonalization, objPersonalization.UserId, objPersonalization.PortalId);
        }

        // default implementation relies on HTTPContext
        public void SaveProfile(HttpContext objHTTPContext, int UserId, int PortalId)
        {
            PersonalizationInfo objPersonalization = (PersonalizationInfo)objHTTPContext.Items["Personalization"];
            SaveProfile(objPersonalization, UserId, PortalId);
        }

        // override allows for manipulation of PersonalizationInfo outside of HTTPContext
        public void SaveProfile(PersonalizationInfo objPersonalization, int UserId, int PortalId)
        {
            if (objPersonalization != null)
            {
                if (objPersonalization.IsModified)
                {
                    string ProfileData = Globals.SerializeHashTableXml(objPersonalization.Profile);
                    DataProvider.Instance().UpdateProfile(UserId, PortalId, ProfileData);
                }
            }
        }
    }
}