using System.Collections;

namespace DotNetNuke.Security.Authentication
{
    public class GroupController
    {
        private string mProviderTypeName;

        public GroupController()
        {
            this.mProviderTypeName = "";
            Configuration configuration1 = Configuration.GetConfig();
            this.mProviderTypeName = configuration1.ProviderTypeName;
        }

        public ArrayList GetGroups()
        {
            return AuthenticationProvider.Instance( this.mProviderTypeName ).GetGroups();
        }

        public bool IsAuthenticationMember( GroupInfo AuthenticationGroup, UserInfo AuthenticationUser )
        {
            return AuthenticationProvider.Instance( this.mProviderTypeName ).IsAuthenticationMember( AuthenticationGroup, AuthenticationUser );
        }
    }
}