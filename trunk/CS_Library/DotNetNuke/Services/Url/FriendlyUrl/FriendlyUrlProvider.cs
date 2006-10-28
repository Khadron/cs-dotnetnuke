using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;

namespace DotNetNuke.Services.Url.FriendlyUrl
{
    public abstract class FriendlyUrlProvider
    {
        // singleton reference to the instantiated object
        private static FriendlyUrlProvider objProvider = null;

        // constructor
        static FriendlyUrlProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (FriendlyUrlProvider)Reflection.CreateObject("friendlyUrl");
        }

        // return the provider
        public new static FriendlyUrlProvider Instance()
        {
            return objProvider;
        }

        public abstract string FriendlyUrl(TabInfo tab, string path);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias);
    }
}