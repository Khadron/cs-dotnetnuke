using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;

namespace DotNetNuke.Security.Profile
{
    public abstract class ProfileProvider
    {
        private static ProfileProvider objProvider;

        static ProfileProvider()
        {
            objProvider = null;
            CreateProvider();
        }

        public abstract bool CanEditProviderProperties { get; }

        private static void CreateProvider()
        {
            objProvider = ( (ProfileProvider)Reflection.CreateObject( "profiles" ) );
        }

        public abstract void GetUserProfile( ref UserInfo user );

        public static ProfileProvider Instance()
        {
            return objProvider;
        }

        public abstract void UpdateUserProfile( UserInfo user );
    }
}