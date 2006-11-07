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
using System.Diagnostics;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class BreadCrumb : SkinObjectBase
    {
        // private members
        private string _separator;
        private string _cssClass;
        private string _rootLevel;

        private const string MyFileName = "Breadcrumb.ascx";

        // protected controls

        public string Separator
        {
            get
            {
                return _separator;
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
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

        public string RootLevel
        {
            get
            {
                return _rootLevel;
            }
            set
            {
                _rootLevel = value;
            }
        }

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        private void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
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
            string strSeparator;
            if( Separator != "" )
            {
                if( Separator.IndexOf( "src=" ) != - 1 )
                {
                    Separator = Separator.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                }
                strSeparator = Separator;
            }
            else
            {
                strSeparator = "&nbsp;<img alt=\"*\" src=\"" + Globals.ApplicationPath + "/images/breadcrumb.gif\">&nbsp;";
            }

            string strCssClass;
            if( CssClass != "" )
            {
                strCssClass = CssClass;
            }
            else
            {
                strCssClass = "SkinObject";
            }

            int intRootLevel;
            if( RootLevel != "" )
            {
                intRootLevel = int.Parse( RootLevel );
            }
            else
            {
                intRootLevel = 1;
            }

            string strBreadCrumbs = "";

            if( intRootLevel == - 1 )
            {
                strBreadCrumbs += string.Format(Localization.GetString("Root", Localization.GetResourceFile(this, MyFileName)), Globals.GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request, true), strCssClass);
                strBreadCrumbs += strSeparator;
                intRootLevel = 0;
            }

            // process bread crumbs
            int intTab;
            for( intTab = intRootLevel; intTab <= PortalSettings.ActiveTab.BreadCrumbs.Count - 1; intTab++ )
            {
                if( intTab != intRootLevel )
                {
                    strBreadCrumbs += strSeparator;
                }
                TabInfo objTab = (TabInfo)PortalSettings.ActiveTab.BreadCrumbs[ intTab ];
                if( objTab.DisableLink )
                {
                    strBreadCrumbs += "<span class=\"" + strCssClass + "\">" + objTab.TabName + "</span>";
                }
                else
                {
                    strBreadCrumbs += "<a href=\"" + objTab.FullUrl + "\" class=\"" + strCssClass + "\">" + objTab.TabName + "</a>";
                }
            }
            lblBreadCrumb.Text = Convert.ToString( strBreadCrumbs );
        }
    }
}