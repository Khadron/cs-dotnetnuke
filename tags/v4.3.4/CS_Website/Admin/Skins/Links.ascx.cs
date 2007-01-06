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
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Links : SkinObjectBase
    {
        // private members
        private string _separator;
        private string _cssClass;
        private string _level;
        private string _alignment;

        // protected controls

        public string Separator
        {
            get
            {
                if( _separator != null )
                {
                    return _separator;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _separator = value;
            }
        }

        public string CssClass
        {
            get
            {
                if( _cssClass != null )
                {
                    return _cssClass;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _cssClass = value;
            }
        }

        public string Level
        {
            get
            {
                if( _level != null )
                {
                    return _level;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _level = value;
            }
        }

        public string Alignment
        {
            get
            {
                if( _alignment != null )
                {
                    return _alignment;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _alignment = value;
            }
        }

        private void Page_Init( Object sender, EventArgs e )
        {
        }

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            // public attributes
            string strCssClass;
            if( !String.IsNullOrEmpty(CssClass) )
            {
                strCssClass = CssClass;
            }
            else
            {
                strCssClass = "SkinObject";
            }

            string strSeparator;
            if( !String.IsNullOrEmpty(Separator) )
            {
                if( Separator.IndexOf( "src=" ) != - 1 )
                {
                    strSeparator = Separator.Replace( "src=", "src=" + PortalSettings.ActiveTab.SkinPath );
                }
                else
                {
                    strSeparator = "<span class=\"" + strCssClass + "\">" + Separator.Replace( " ", "&nbsp;" ) + "</span>";
                }
            }
            else
            {
                strSeparator = "  ";
            }

            // build links

            string strLinks = BuildLinks( Level, Alignment, strSeparator, strCssClass );

            if( strLinks == "" )
            {
                strLinks = BuildLinks( "", Alignment, strSeparator, strCssClass );
            }

            lblLinks.Text = strLinks;
        }

        private string BuildLinks( string level, string alignment, string strSeparator, string strCssClass )
        {
            string strLinks = "";
            int intIndex;

            for( intIndex = 0; intIndex <= PortalSettings.DesktopTabs.Count - 1; intIndex++ )
            {
                TabInfo objTab = (TabInfo)PortalSettings.DesktopTabs[intIndex];

                if( objTab.IsVisible && objTab.IsDeleted == false )
                {
                    if( ( objTab.StartDate < DateTime.Now && objTab.EndDate > DateTime.Now ) || AdminMode )
                    {
                        if( PortalSecurity.IsInRoles( objTab.AuthorizedRoles ) )
                        {
                            string strLoop = "";
                            if( alignment == "Vertical" )
                            {
                                if( !String.IsNullOrEmpty(strLinks) )
                                {
                                    strLoop = "<br>" + strSeparator;
                                }
                                else
                                {
                                    strLoop = strSeparator;
                                }
                            }
                            else
                            {
                                if( !String.IsNullOrEmpty(strLinks) )
                                {
                                    strLoop = strSeparator;
                                }
                            }

                            switch( level )
                            {
                                case "Same":
                                    if( objTab.ParentId == PortalSettings.ActiveTab.ParentId )
                                    {
                                        strLinks += strLoop + AddLink( objTab.TabName, objTab.FullUrl, strCssClass );
                                    }
                                    break;

                                case "":

                                    if( objTab.ParentId == PortalSettings.ActiveTab.ParentId )
                                    {
                                        strLinks += strLoop + AddLink( objTab.TabName, objTab.FullUrl, strCssClass );
                                    }
                                    break;
                                case "Child":

                                    if( objTab.ParentId == PortalSettings.ActiveTab.TabID )
                                    {
                                        strLinks += strLoop + AddLink( objTab.TabName, objTab.FullUrl, strCssClass );
                                    }
                                    break;
                                case "Parent":

                                    if( objTab.TabID == PortalSettings.ActiveTab.ParentId )
                                    {
                                        strLinks += strLoop + AddLink( objTab.TabName, objTab.FullUrl, strCssClass );
                                    }
                                    break;
                                case "Root":

                                    if( objTab.Level == 0 )
                                    {
                                        strLinks += strLoop + AddLink( objTab.TabName, objTab.FullUrl, strCssClass );
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return strLinks;
        }

        private string AddLink( string strTabName, string strURL, string strCssClass )
        {
            return "<a class=\"" + strCssClass + "\" href=\"" + strURL + "\">" + strTabName + "</a>";
        }
    }
}