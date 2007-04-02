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

using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace DotNetNuke.UI.Utilities
{

    [Serializable(), XmlRoot("capabilities")]
    public class BrowserCaps
    {


        private FunctionalityCollection m_objFunctionality;
        private Hashtable m_objFunctionalityDict;
        private const string CLIENTAPI_CACHE_KEY = "ClientAPICaps";



        [XmlElement("functionality")]
        public FunctionalityCollection Functionality
        {
            get
            {
                return m_objFunctionality;
            }
            set
            {
                m_objFunctionality = value;
            }
        }

        public Hashtable FunctionalityDictionary
        {
            get
            {
                if (m_objFunctionalityDict == null)
                {
                    m_objFunctionalityDict = new Hashtable();
                }
                return m_objFunctionalityDict;
            }
        }


        private static Browser GetBrowser(XPathNavigator objNav)
        {
            Browser objBrowser = new Browser();
            objBrowser.Contains = objNav.GetAttribute("contains", "");
            objBrowser.Name = objNav.GetAttribute("nm", "");
            string strMinVersion = objNav.GetAttribute("minversion", "");
            //If Not String.IsNullOrEmpty(strMinVersion) Then    '.NET 2.0 specific
            // HACK : Modified to not error if object is null.
            //if (strMinVersion.Length > 0)
            if (!String.IsNullOrEmpty(strMinVersion))
            {
                objBrowser.MinVersion = double.Parse(strMinVersion);
            }
            return objBrowser;
        }



        public static BrowserCaps GetBrowserCaps()
        {

            BrowserCaps objCaps = (BrowserCaps)(DataCache.GetCache(CLIENTAPI_CACHE_KEY));
            FunctionalityInfo objFunc = null;

            if (objCaps == null)
            {
                string strPath = string.Empty;

                try
                {
                    strPath = HttpContext.Current.Server.MapPath(ClientAPI.ScriptPath + "/ClientAPICaps.config");
                }
                catch (Exception ex)
                {
                    //ignore error - worried about people with reverse proxies and such...
                }

                if (File.Exists(strPath))
                {
                    objCaps = new BrowserCaps();
                    objCaps.Functionality = new FunctionalityCollection();

                    //Create a FileStream for the Config file
                    FileStream objReader = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    XPathDocument objDoc = new XPathDocument(objReader);

                    foreach (XPathNavigator objNavFunc in objDoc.CreateNavigator().Select("//functionality"))
                    {
                        objFunc = new FunctionalityInfo();
                        objFunc.Functionality = (ClientAPI.ClientFunctionality)(Enum.Parse(typeof(ClientAPI.ClientFunctionality), objNavFunc.GetAttribute("nm", "")));
                        objFunc.Desc = objNavFunc.GetAttribute("desc", "");

                        objFunc.Supports = new BrowserCollection();
                        foreach (XPathNavigator objNavSupports in objNavFunc.CreateNavigator().Select("supports/browser"))
                        {
                            objFunc.Supports.Add(GetBrowser(objNavSupports));
                        }

                        objFunc.Excludes = new BrowserCollection();
                        foreach (XPathNavigator objNavExcludes in objNavFunc.CreateNavigator().Select("excludes/browser"))
                        {
                            objFunc.Excludes.Add(GetBrowser(objNavExcludes));
                        }

                        objCaps.Functionality.Add(objFunc);
                    }

                    //Close the Reader
                    objReader.Close();

                    // Set back into Cache
                    DataCache.SetCache(CLIENTAPI_CACHE_KEY, objCaps, new CacheDependency(strPath));
                }

            }

            return objCaps;

        }


    }

}
