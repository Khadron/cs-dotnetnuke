using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.Personalization
{
    public class Personalization
    {
        private static PersonalizationInfo LoadProfile()
        {
            HttpContext context = HttpContext.Current;

            //First try and load Personalization object from the Context
            PersonalizationInfo objPersonalization = (PersonalizationInfo)context.Items["Personalization"];

            //If the Personalization object is nothing load it and store it in the context for future calls
            if (objPersonalization == null)
            {
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = (PortalSettings)context.Items["PortalSettings"];

                // load the user info object
                UserInfo UserInfo = UserController.GetCurrentUserInfo();

                // get the personalization object
                PersonalizationController objPersonalizationController = new PersonalizationController();
                objPersonalization = objPersonalizationController.LoadProfile(UserInfo.UserID, _portalSettings.PortalId);

                //store it in the context
                context.Items.Add("Personalization", objPersonalization);
            }

            return objPersonalization;
        }

        public static void SetProfile(string NamingContainer, string Key, object Value)
        {
            SetProfile(LoadProfile(), NamingContainer, Key, Value);
        }

        public static void SetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key, object Value)
        {
            if (objPersonalization != null)
            {
                objPersonalization.Profile[NamingContainer + ":" + Key] = Value;
                objPersonalization.IsModified = true;
            }
        }

        public static object GetProfile(string NamingContainer, string Key)
        {
            return GetProfile(LoadProfile(), NamingContainer, Key);
        }

        public static object GetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                return objPersonalization.Profile[NamingContainer + ":" + Key];
            }
            else
            {
                return "";
            }
        }

        public static void RemoveProfile(string NamingContainer, string Key)
        {
            RemoveProfile(LoadProfile(), NamingContainer, Key);
        }

        public static void RemoveProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                objPersonalization.Profile.Remove(NamingContainer + ":" + Key);
                objPersonalization.IsModified = true;
            }
        }
    }
}