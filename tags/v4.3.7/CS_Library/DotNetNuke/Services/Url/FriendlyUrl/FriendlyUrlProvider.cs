#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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