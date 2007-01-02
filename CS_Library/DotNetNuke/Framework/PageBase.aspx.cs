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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using AttributeCollection=System.Web.UI.AttributeCollection;

namespace DotNetNuke.Framework
{
    public abstract class PageBase : Page
    {
        private ArrayList _localizedControls;
        private string _localResourceFile;

        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                string[] page = Request.ServerVariables["SCRIPT_NAME"].Split('/');

                if (String.IsNullOrEmpty( _localResourceFile ))
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" + page[page.GetUpperBound(0)] + ".resx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                _localResourceFile = value;
            }
        }

        public CultureInfo PageCulture
        {
            get
            {
                LocaleCollection enabledLocales = Localization.GetEnabledLocales();
                CultureInfo ci = null;
                //used as temporary variable to get info about the preferred locale
                string preferredLocale = "";
                //used as temporary variable where the language part of the preferred locale will be saved
                string preferredLanguage = "";

                //first try if a specific language is requested by cookie, querystring, or form
                if (HttpContext.Current != null)
                {
                    try
                    {
                        preferredLocale = HttpContext.Current.Request["language"];
                        if (!String.IsNullOrEmpty( preferredLocale))
                        {
                            if (Localization.LocaleIsEnabled(ref preferredLocale))
                            {
                                ci = new CultureInfo(preferredLocale);
                            }
                            else
                            {
                                preferredLanguage = preferredLocale.Split('-')[0];
                            }
                        }
                    }
                    catch(Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                }

                if (ci == null)
                {
                    // next try to get the preferred language of the logged on user
                    UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                    if (objUserInfo.UserID != -1)
                    {
                        if (!String.IsNullOrEmpty(objUserInfo.Profile.PreferredLocale))
                        {
                            if (Localization.LocaleIsEnabled(ref preferredLocale))
                            {
                                ci = new CultureInfo(objUserInfo.Profile.PreferredLocale);
                            }
                            else
                            {
                                if (preferredLanguage == "")
                                {
                                    preferredLanguage = objUserInfo.Profile.PreferredLocale.Split('-')[0];
                                }
                            }
                        }
                    }
                }

                if (ci == null)
                {
                    // use Request.UserLanguages to get the preferred language
                    if (HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Request.UserLanguages != null)
                        {
                            try
                            {
                                for( int i = 0; i < HttpContext.Current.Request.UserLanguages.Length; i++ )
                                {
                                    string userLang = HttpContext.Current.Request.UserLanguages[i];
                                    //split userlanguage by ";"... all but the first language will contain a preferrence index eg. ;q=.5
                                    string userlanguage = userLang.Split( ';' )[0];
                                    if( Localization.LocaleIsEnabled( ref userlanguage ) )
                                    {
                                        ci = new CultureInfo( userlanguage );
                                    }
                                    else if( userLang.Split( ';' )[0].IndexOf( "-" ) != -1 )
                                    {
                                        //if userLang is neutral we don't need to do this part since
                                        //it has already been done in LocaleIsEnabled( )
                                        string templang = userLang.Split( ';' )[0];
                                        for( int i1 = 0; i1 < enabledLocales.AllKeys.Length; i1++ )
                                        {
                                            string _localeCode = enabledLocales.AllKeys[i1];
                                            if( _localeCode.Split( '-' )[0] == templang.Split( '-' )[0] )
                                            {
                                                //the preferredLanguage was found in the enabled locales collection, so we are going to use this one
                                                //eg, requested locale is en-GB, requested language is en, enabled locale is en-US, so en is a match for en-US
                                                ci = new CultureInfo( _localeCode );
                                                break;
                                            }
                                        }
                                    }
                                    if( ci != null )
                                    {
                                        break;
                                    }
                                }
                            }
                            catch(Exception exc)
                            {
                                Exceptions.ProcessModuleLoadException( this, exc );
                            }
                        }
                    }
                }

                if (ci == null && !String.IsNullOrEmpty(preferredLanguage))
                {
                    //we still don't have a good culture, so we are going to try to get a culture with the preferredlanguage instead
                    foreach (string code in enabledLocales.AllKeys)
                    {
                        if (code.Split('-')[0] == preferredLanguage)
                        {
                            //the preferredLanguage was found in the enabled locales collection, so we are going to use this one
                            //eg, requested locale is en-GB, requested language is en, enabled locale is en-US, so en is a match for en-US
                            ci = new CultureInfo(code);
                            
                            break;
                        }
                    }
                }

                //we still have no culture info set, so we are going to use the fallback method
                if (ci == null)
                {
                    if (PortalSettings.DefaultLanguage == "")
                    {
                        // this is a last resort, as the portal default language should always be set
                        // however if its not set, return the first enabled locale
                        // if there are no enabled locales, return the systemlocale
                        if (enabledLocales.Count > 0)
                        {
                            ci = new CultureInfo(Convert.ToString(enabledLocales[0].Key));
                        }
                        else
                        {
                            ci = new CultureInfo(Localization.SystemLocale);
                        }
                    }
                    else
                    {
                        // as the portal default language can never be disabled, we know this language is available and enabled
                        ci = new CultureInfo(PortalSettings.DefaultLanguage);
                    }
                }

                if (ci == null)
                {
                    //just a safeguard, to make sure we return something
                    ci = new CultureInfo(Localization.SystemLocale);
                }

                //finally set the cookie
                Localization.SetLanguage(ci.Name);
                return ci;
            }
            set
            {
                Thread.CurrentThread.CurrentUICulture = PageCulture;
                Thread.CurrentThread.CurrentCulture = PageCulture;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalSettings PortalSettings
        {
            get
            {                
                return PortalController.GetCurrentPortalSettings();                
            }
        }

        /// <summary>
        /// <para>Constructor</para>
        /// </summary>
        public PageBase()
        {
            _localizedControls = new ArrayList();
        }

        /// <summary>
        /// <para>GetControlAttribute looks a the type of control and does it's best to find an AttributeCollection.</para>
        /// </summary>
        /// <param name="c">Control to find the AttributeCollection on</param>
        /// <param name="affectedControls">ArrayList that hold the controls that have been localized. This is later used for the removal of the key attribute.</param>
        /// <returns>A string containing the key for the specified control or null if a key attribute wasn't found</returns>
        internal static string GetControlAttribute(Control c, ArrayList affectedControls)
        {
            AttributeCollection ac = null;
            string key = null;

            if (c is LiteralControl) // LiteralControls don't have an attribute collection
            {
                key = null;
                ac = null;
            }
            else
            {
                if (c is WebControl)
                {
                    WebControl w = (WebControl)c;
                    ac = w.Attributes;
                    key = ac[Localization.KeyName];
                }
                else
                {
                    if (c is HtmlControl)
                    {
                        HtmlControl h = (HtmlControl)c;
                        ac = h.Attributes;
                        key = ac[Localization.KeyName];
                    }
                    else
                    {
                        if (c is UserControl)
                        {
                            UserControl u = (UserControl)c;
                            ac = u.Attributes;
                            key = ac[Localization.KeyName];
                            // Use reflection to check for attribute key. This is a last ditch option
                        }
                        else
                        {
                            Type controlType = c.GetType();
                            PropertyInfo attributeProperty = controlType.GetProperty("Attributes", typeof(AttributeCollection));
                            if (attributeProperty != null)
                            {
                                ac = (AttributeCollection)attributeProperty.GetValue(c, null);
                                key = ac[Localization.KeyName];
                            }
                        }
                    }
                } // If the key was found add this AttributeCollection to the list that should have the key removed during Render
            }
            if (key != null && affectedControls != null)
            {
                affectedControls.Add(ac);
            }
            return key;
        } 

        public bool HasTabPermission(string PermissionKey)
        {
            return TabPermissionController.HasTabPermission(PortalSettings.ActiveTab.TabPermissions, PermissionKey);
        }

        /// <summary>
        /// <para>IterateControls performs the high level localization for each control on the page.</para>
        /// </summary>
        private void IterateControls(ArrayList affectedControls)
        {
            IterateControls(Controls, affectedControls, null);
        } 

        private void IterateControls(ControlCollection controls, ArrayList affectedControls, string ResourceFileRoot)
        {
            foreach (Control c in controls)
            {
                ProcessControl(c, affectedControls, true, ResourceFileRoot);
            }
        } 

        protected override void OnInit(EventArgs e)
        {
            // Set the current culture
            Thread.CurrentThread.CurrentUICulture = PageCulture;
            Thread.CurrentThread.CurrentCulture = PageCulture;
            // Localize portalsettings
            Localization.LocalizePortalSettings();
            base.OnInit(e);
        }

        protected void Page_Error(object Source, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            string strURL = Globals.ApplicationURL();
            if (Request.QueryString["error"] != null)
            {
                strURL += ((strURL.IndexOf("?") == -1) ? "?" : "&") + "error=terminate";
            }
            else
            {
//                strURL += ((strURL.IndexOf("?") == -1) ? "?" : "&") + "error=" + Server.UrlEncode(exc.Message);
                strURL += ((strURL.IndexOf("?") == -1) ? "?" : "&") + "error=" + Server.UrlEncode("Message: " + exc.Message + "---" + "StackTrace: " + exc.StackTrace);
                if (!Globals.IsAdminControl())
                {
                    strURL += "&content=0";
                }
            }
            Exceptions.ProcessPageLoadException(exc, strURL);
        }

        /// <summary>
        /// <para>ProcessControl peforms the high level localization for a single control and optionally it's children.</para>
        /// </summary>
        /// <param name="c">Control to find the AttributeCollection on</param>
        /// <param name="affectedControls">ArrayList that hold the controls that have been localized. This is later used for the removal of the key attribute.</param>
        /// <param name="includeChildren">If true, causes this method to process children of this controls.</param>
        /// <param name="resourceFileRoot"></param>
        internal void ProcessControl(Control c, ArrayList affectedControls, bool includeChildren, string resourceFileRoot)
        {
            // Perform the substitution if a key was found
            string key = GetControlAttribute(c, affectedControls);
            if (key != null)
            {
                //Translation starts here ....
                string value;
                value = Localization.GetString(key, resourceFileRoot);

                if (c is Label)
                {
                    Label ctrl;
                    ctrl = (Label)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is LinkButton)
                {
                    LinkButton ctrl;
                    ctrl = (LinkButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        MatchCollection imgMatches = Regex.Matches(value, "<(a|link|img|script|input|form).[^>]*(href|src|action)=(\\\"|\'|)(.[^\\\"\']*)(\\\"|\'|)[^>]*>", RegexOptions.IgnoreCase);

                        foreach (Match _match in imgMatches)
                        {
                            if (_match.Groups[_match.Groups.Count - 2].Value.IndexOf("~") != -1)
                            {
                                string resolvedUrl = Page.ResolveUrl(_match.Groups[_match.Groups.Count - 2].Value);

                                value = value.Replace(_match.Groups[_match.Groups.Count - 2].Value, resolvedUrl);
                            }
                        }

                        ctrl.Text = value;
                    }
                }
                if (c is HyperLink)
                {
                    HyperLink ctrl;
                    ctrl = (HyperLink)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is ImageButton)
                {
                    ImageButton ctrl;
                    ctrl = (ImageButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                    }
                }
                if (c is Button)
                {
                    Button ctrl;
                    ctrl = (Button)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is HtmlImage)
                {
                    HtmlImage ctrl;
                    ctrl = (HtmlImage)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Alt = value;
                    }
                }
                if (c is CheckBox)
                {
                    CheckBox ctrl;
                    ctrl = (CheckBox)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is BaseValidator)
                {
                    BaseValidator ctrl;
                    ctrl = (BaseValidator)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.ErrorMessage = value;
                    }
                }
                if (c is Image)
                {
                    Image ctrl;
                    ctrl = (Image)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                        ctrl.ToolTip = value;
                    }
                }
            }

            // Translate radiobuttonlist items here
            if (c is RadioButtonList)
            {
                RadioButtonList ctrl;
                ctrl = (RadioButtonList)c;                
                for (int i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    AttributeCollection ac = ctrl.Items[i].Attributes;
                    key = ac[Localization.KeyName];
                    if (key != null)
                    {
                        string value = Localization.GetString(key, resourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                }
            }

            // Translate dropdownlist items here
            if (c is DropDownList)
            {
                DropDownList ctrl;
                ctrl = (DropDownList)c;                
                for (int i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    AttributeCollection ac = ctrl.Items[i].Attributes;
                    key = ac[Localization.KeyName];
                    if (key != null)
                    {
                        string value = Localization.GetString(key, resourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                }
            }

            //' UrlRewriting Issue - ResolveClientUrl gets called instead of ResolveUrl
            //' Manual Override to ResolveUrl
            if (c is Image)
            {
                Image ctrl;
                ctrl = (Image)c;
                if (ctrl.ImageUrl.IndexOf("~") != -1)
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }

            // UrlRewriting Issue - ResolveClientUrl gets called instead of ResolveUrl
            // Manual Override to ResolveUrl
            if (c is HtmlImage)
            {
                HtmlImage ctrl;
                ctrl = (HtmlImage)c;
                if (ctrl.Src.IndexOf("~") != -1)
                {
                    ctrl.Src = Page.ResolveUrl(ctrl.Src);
                }
            }

            // UrlRewriting Issue - ResolveClientUrl gets called instead of ResolveUrl
            // Manual Override to ResolveUrl
            if (c is HyperLink)
            {
                HyperLink ctrl;
                ctrl = (HyperLink)c;
                if (ctrl.NavigateUrl.IndexOf("~") != -1)
                {
                    ctrl.NavigateUrl = Page.ResolveUrl(ctrl.NavigateUrl);
                }
                if (ctrl.ImageUrl.IndexOf("~") != -1)
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }

            // Process child controls
            if (includeChildren && c.HasControls())
            {
                if (c is PortalModuleBase)
                {
                    //Get Resource File Root from Controls LocalResourceFile Property
                    PortalModuleBase ctrl;
                    ctrl = (PortalModuleBase)c;
                    IterateControls(c.Controls, affectedControls, ctrl.LocalResourceFile);
                }
                else
                {
                    PropertyInfo pi = c.GetType().GetProperty("LocalResourceFile");
                    if (pi != null && pi.GetValue(c, null) != null)
                    {
                        //If controls has a LocalResourceFile property use this
                        IterateControls(c.Controls, affectedControls, pi.GetValue(c, null).ToString());
                    }
                    else
                    {
                        //Pass Resource File Root through
                        IterateControls(c.Controls, affectedControls, resourceFileRoot);
                    }
                }
            }
        } 

        /// <summary>
        /// <para>RemoveKeyAttribute remove the key attribute from the control. If this isn't done, then the HTML output will have
        /// a bad attribute on it which could cause some older browsers problems.</para>
        /// </summary>
        /// <param name="affectedControls">ArrayList that hold the controls that have been localized. This is later used for the removal of the key attribute.</param>
        public static void RemoveKeyAttribute(ArrayList affectedControls)
        {
            if (affectedControls == null)
            {
                return;
            }
            int i;
            for (i = 0; i <= affectedControls.Count - 1; i++)
            {
                AttributeCollection ac = (AttributeCollection)affectedControls[i];
                ac.Remove(Localization.KeyName);
            }
        } 

        // This method overrides the Render() method for the page and moves the ViewState
        // from its default location at the top of the page to the bottom of the page. This
        // results in better search engine spidering.
        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

            //Localize controls
            Trace.Write("LocalizedPage::PreRender", string.Format("Performing substitutions for {0}", Thread.CurrentThread.CurrentUICulture.Name));
            IterateControls(Controls, _localizedControls, LocalResourceFile);
            Trace.Write("LocalizedPage::Render", "Removing resourcekey attribute from controls");
            RemoveKeyAttribute(_localizedControls);

            base.Render(htmlWriter);

            string html = stringWriter.ToString();
            int StartPoint = html.IndexOf("<input type=\"hidden\" name=\"__VIEWSTATE\"");
            if (StartPoint >= 0) //does __VIEWSTATE exist?
            {
                int EndPoint = html.IndexOf("/>", StartPoint) + 2;
                string ViewStateInput = html.Substring(StartPoint, EndPoint - StartPoint);
                html = html.Remove(StartPoint, EndPoint - StartPoint);
                int FormEndStart = html.IndexOf("</form>");
                if (FormEndStart >= 0)
                {
                    html = html.Insert(FormEndStart, ViewStateInput);
                }
            }
            writer.Write(html);
        }

        [Obsolete("This function has been replaced by DotNetNuke.Services.Localization.Localization.SetLanguage")]
        public void SetLanguage(string value)
        {
            Localization.SetLanguage(value);
        }
    } 
}