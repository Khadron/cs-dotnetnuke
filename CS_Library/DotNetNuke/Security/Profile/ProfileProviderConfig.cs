using System.ComponentModel;

namespace DotNetNuke.Security.Profile
{
    /// <Summary>
    /// The ProfileProviderConfig class provides a wrapper to the Profile providers
    /// configuration
    /// </Summary>
    public class ProfileProviderConfig
    {
        private static ProfileProvider profileProvider;

        static ProfileProviderConfig()
        {
            profileProvider = ProfileProvider.Instance();
        }

        /// <Summary>Gets whether the Provider Properties can be edited</Summary>
        /// <Returns>A Boolean</Returns>
        [Browsable( false )]
        public static bool CanEditProviderProperties
        {
            get
            {
                return profileProvider.CanEditProviderProperties;
            }
        }
    }
}