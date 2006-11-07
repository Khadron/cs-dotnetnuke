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