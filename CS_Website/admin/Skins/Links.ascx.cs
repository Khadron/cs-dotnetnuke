using System;
using System.Diagnostics;
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

        public string Level
        {
            get
            {
                return _level;
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
                return _alignment;
            }
            set
            {
                _alignment = value;
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
            string strCssClass;
            if( CssClass != "" )
            {
                strCssClass = CssClass;
            }
            else
            {
                strCssClass = "SkinObject";
            }

            string strSeparator;
            if( Separator != "" )
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
            string strLinks = "";

            strLinks = BuildLinks( Level, Alignment, strSeparator, strCssClass );

            if( strLinks == "" )
            {
                strLinks = BuildLinks( "", Alignment, strSeparator, strCssClass );
            }

            lblLinks.Text = strLinks;
        }

        private string BuildLinks( string Level, string Alignment, string strSeparator, string strCssClass )
        {
            string strLinks = "";
            string strLoop;
            int intIndex;

            for( intIndex = 0; intIndex <= PortalSettings.DesktopTabs.Count - 1; intIndex++ )
            {
                TabInfo objTab = (TabInfo)PortalSettings.DesktopTabs[intIndex];

                if( objTab.IsVisible == true && objTab.IsDeleted == false )
                {
                    if( ( objTab.StartDate < DateTime.Now && objTab.EndDate > DateTime.Now ) || AdminMode == true )
                    {
                        if( PortalSecurity.IsInRoles( objTab.AuthorizedRoles ) == true )
                        {
                            strLoop = "";
                            if( Alignment == "Vertical" )
                            {
                                if( strLinks != "" )
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
                                if( strLinks != "" )
                                {
                                    strLoop = strSeparator;
                                }
                            }

                            switch( Level )
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