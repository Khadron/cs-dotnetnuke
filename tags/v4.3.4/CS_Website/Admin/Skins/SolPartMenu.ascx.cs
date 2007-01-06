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
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class SolPartMenu : NavObjectBase
    {
        private string _separateCss;
        private string _useSkinPathArrowImages;
        private string _useRootBreadCrumbArrow;
        private string _useSubMenuBreadCrumbArrow;
        private string _downArrow;
        private string _rightArrow;
        private string _level;
        private string _rootOnly;
        private string _toolTip;
        private string _clearDefaults;

        private string m_strRootBreadCrumbArrow;
        private string m_strSubMenuBreadCrumbArrow;
        private string m_strRootOnly;

        // protected controls

        public string SeparateCss
        {
            get
            {
                    return _separateCss;
                }
            set
            {
                _separateCss = value;
            }
        }

        public string MenuBarCssClass
        {
            get
            {
                return this.CSSControl;
            }
            set
            {
                this.CSSControl = value;
            }
        }

        public string MenuContainerCssClass
        {
            get
            {
                return this.CSSContainerRoot;
            }
            set
            {
                this.CSSContainerRoot = value;
            }
        }

        public string MenuItemCssClass
        {
            get
            {
                return this.CSSNode;
            }
            set
            {
                this.CSSNode = value;
            }
        }

        public string MenuIconCssClass
        {
            get
            {
                return this.CSSIcon;
            }
            set
            {
                this.CSSIcon = value;
            }
        }

        public string SubMenuCssClass
        {
            get
            {
                return this.CSSContainerSub;
            }
            set
            {
                this.CSSContainerSub = value;
            }
        }

        public string MenuItemSelCssClass
        {
            get
            {
                return this.CSSNodeHover;
            }
            set
            {
                this.CSSNodeHover = value;
            }
        }

        public string MenuBreakCssClass
        {
            get
            {
                return this.CSSBreak;
            }
            set
            {
                this.CSSBreak = value;
            }
        }

        public string MenuArrowCssClass
        {
            get
            {
                return this.CSSIndicateChildSub;
            }
            set
            {
                this.CSSIndicateChildSub = value;
            }
        }

        public string MenuRootArrowCssClass
        {
            get
            {
                return this.CSSIndicateChildRoot;
            }
            set
            {
                this.CSSIndicateChildRoot = value;
            }
        }

        public string BackColor
        {
            get
            {
                return this.StyleBackColor;
            }
            set
            {
                this.StyleBackColor = value;
            }
        }

        public string ForeColor
        {
            get
            {
                return this.StyleForeColor;
            }
            set
            {
                this.StyleForeColor = value;
            }
        }

        public string HighlightColor
        {
            get
            {
                return this.StyleHighlightColor;
            }
            set
            {
                this.StyleHighlightColor = value;
            }
        }

        public string IconBackgroundColor
        {
            get
            {
                return this.StyleIconBackColor;
            }
            set
            {
                this.StyleIconBackColor = value;
            }
        }

        public string SelectedBorderColor
        {
            get
            {
                return this.StyleSelectionBorderColor;
            }
            set
            {
                this.StyleSelectionBorderColor = value;
            }
        }

        public string SelectedColor
        {
            get
            {
                return this.StyleSelectionColor;
            }
            set
            {
                this.StyleSelectionColor = value;
            }
        }

        public string SelectedForeColor
        {
            get
            {
                return this.StyleSelectionForeColor;
            }
            set
            {
                this.StyleSelectionForeColor = value;
            }
        }

        public string Display
        {
            get
            {
                return this.ControlOrientation;
            }
            set
            {
                this.ControlOrientation = value;
            }
        }

        public string MenuBarHeight
        {
            get
            {
                return this.StyleControlHeight;
            }
            set
            {
                this.StyleControlHeight = value;
            }
        }

        public string MenuBorderWidth
        {
            get
            {
                return this.StyleBorderWidth;
            }
            set
            {
                this.StyleBorderWidth = value;
            }
        }

        public string MenuItemHeight
        {
            get
            {
                return this.StyleNodeHeight;
            }
            set
            {
                this.StyleNodeHeight = value;
            }
        }

        //Public Property ForceDownLevel() As String
        //	Get
        //		Return Me.ForceDownLevel
        //	End Get
        //	Set(ByVal Value As String)
        //		Me.ForceDownLevel = Value
        //	End Set
        //End Property

        public string Moveable
        {
            get
            {
                //Return _moveable
                return null;
            }
            set
            {
                //_moveable = Value
            }
        }

        public string IconWidth
        {
            get
            {
                return this.StyleIconWidth;
            }
            set
            {
                this.StyleIconWidth = value;
            }
        }

        public string MenuEffectsShadowColor
        {
            get
            {
                return this.EffectsShadowColor;
            }
            set
            {
                this.EffectsShadowColor = value;
            }
        }

        public string MenuEffectsMouseOutHideDelay
        {
            get
            {
                return this.MouseOutHideDelay;
            }
            set
            {
                this.MouseOutHideDelay = value;
            }
        }

        //Public Property MouseOutHideDelay() As String
        //	Get
        //		Return MenuEffectsMouseOutHideDelay
        //	End Get
        //	Set(ByVal Value As String)
        //		MenuEffectsMouseOutHideDelay = Value
        //	End Set
        //End Property

        public string MenuEffectsMouseOverDisplay
        {
            get
            {
                return this.MouseOverDisplay;
            }
            set
            {
                this.MouseOverDisplay = value;
            }
        }

        public string MenuEffectsMouseOverExpand
        {
            get
            {
                return this.MouseOverAction;
            }
            set
            {
                this.MouseOverAction = value;
            }
        }

        public string MenuEffectsStyle
        {
            get
            {
                return this.EffectsStyle;
            }
            set
            {
                this.EffectsStyle = value;
            }
        }

        public string FontNames
        {
            get
            {
                return this.StyleFontNames;
            }
            set
            {
                this.StyleFontNames = value;
            }
        }

        public string FontSize
        {
            get
            {
                return this.StyleFontSize;
            }
            set
            {
                this.StyleFontSize = value;
            }
        }

        public string FontBold
        {
            get
            {
                return this.StyleFontBold;
            }
            set
            {
                this.StyleFontBold = value;
            }
        }

        public string MenuEffectsShadowStrength
        {
            get
            {
                return this.EffectsShadowStrength;
            }
            set
            {
                this.EffectsShadowStrength = value;
            }
        }

        public string MenuEffectsMenuTransition
        {
            get
            {
                return this.EffectsTransition;
            }
            set
            {
                this.EffectsTransition = value;
            }
        }

        public string MenuEffectsMenuTransitionLength
        {
            get
            {
                return this.EffectsDuration;
            }
            set
            {
                this.EffectsDuration = value;
            }
        }

        public string MenuEffectsShadowDirection
        {
            get
            {
                return this.EffectsShadowDirection;
            }
            set
            {
                this.EffectsShadowDirection = value;
            }
        }

        public string ForceFullMenuList
        {
            get
            {
                return this.ForceCrawlerDisplay;
            }
            set
            {
                this.ForceCrawlerDisplay = value;
            }
        }

        public string UseSkinPathArrowImages
        {
            get
            {
                    return _useSkinPathArrowImages;
                }
            set
            {
                _useSkinPathArrowImages = value;
            }
        }

        public string UseRootBreadCrumbArrow
        {
            get
            {
                    return _useRootBreadCrumbArrow;
                }
            set
            {
                _useRootBreadCrumbArrow = value;
            }
        }

        public string UseSubMenuBreadCrumbArrow
        {
            get
            {
                    return _useSubMenuBreadCrumbArrow;
                }
            set
            {
                _useSubMenuBreadCrumbArrow = value;
            }
        }

        public string RootMenuItemBreadCrumbCssClass
        {
            get
            {
                return this.CSSBreadCrumbRoot;
            }
            set
            {
                this.CSSBreadCrumbRoot = value;
            }
        }

        public string SubMenuItemBreadCrumbCssClass
        {
            get
            {
                return this.CSSBreadCrumbSub;
            }
            set
            {
                this.CSSBreadCrumbSub = value;
            }
        }

        public string RootMenuItemCssClass
        {
            get
            {
                return this.CSSNodeRoot;
            }
            set
            {
                this.CSSNodeRoot = value;
            }
        }

        public string RootBreadCrumbArrow
        {
            get
            {
                return m_strRootBreadCrumbArrow; //Me.IndicateChildImageBreadCrumbRoot
            }
            set
            {
                m_strRootBreadCrumbArrow = value;
            }
        }

        public string SubMenuBreadCrumbArrow
        {
            get
            {
                return m_strSubMenuBreadCrumbArrow; //Me.IndicateChildImageBreadCrumbSub
            }
            set
            {
                m_strSubMenuBreadCrumbArrow = value;
            }
        }

        public string UseArrows
        {
            get
            {
                return this.IndicateChildren;
            }
            set
            {
                this.IndicateChildren = value;
            }
        }

        public string DownArrow
        {
            get
            {
                    return _downArrow;
                }
            set
            {
                _downArrow = value;
            }
        }

        public string RightArrow
        {
            get
            {
                    return _rightArrow;
                }
            set
            {
                _rightArrow = value;
            }
        }

        public string RootMenuItemActiveCssClass
        {
            get
            {
                return this.CSSNodeSelectedRoot;
            }
            set
            {
                this.CSSNodeSelectedRoot = value;
            }
        }

        public string SubMenuItemActiveCssClass
        {
            get
            {
                return this.CSSNodeSelectedSub;
            }
            set
            {
                this.CSSNodeSelectedSub = value;
            }
        }

        public string RootMenuItemSelectedCssClass
        {
            get
            {
                return this.CSSNodeHoverRoot;
            }
            set
            {
                this.CSSNodeHoverRoot = value;
            }
        }

        public string SubMenuItemSelectedCssClass
        {
            get
            {
                return this.CSSNodeHoverSub;
            }
            set
            {
                this.CSSNodeHoverSub = value;
            }
        }

        public string Separator
        {
            get
            {
                return this.SeparatorHTML;
            }
            set
            {
                this.SeparatorHTML = value;
            }
        }

        public string SeparatorCssClass
        {
            get
            {
                return this.CSSSeparator;
            }
            set
            {
                this.CSSSeparator = value;
            }
        }

        public string RootMenuItemLeftHtml
        {
            get
            {
                return this.NodeLeftHTMLRoot;
            }
            set
            {
                this.NodeLeftHTMLRoot = value;
            }
        }

        public string RootMenuItemRightHtml
        {
            get
            {
                return this.NodeRightHTMLRoot;
            }
            set
            {
                this.NodeRightHTMLRoot = value;
            }
        }

        public string SubMenuItemLeftHtml
        {
            get
            {
                return this.NodeLeftHTMLSub;
            }
            set
            {
                this.NodeLeftHTMLSub = value;
            }
        }

        public string SubMenuItemRightHtml
        {
            get
            {
                return this.NodeRightHTMLSub;
            }
            set
            {
                this.NodeRightHTMLSub = value;
            }
        }

        public string LeftSeparator
        {
            get
            {
                return this.SeparatorLeftHTML;
            }
            set
            {
                this.SeparatorLeftHTML = value;
            }
        }

        public string RightSeparator
        {
            get
            {
                return this.SeparatorRightHTML;
            }
            set
            {
                this.SeparatorRightHTML = value;
            }
        }

        public string LeftSeparatorActive
        {
            get
            {
                return this.SeparatorLeftHTMLActive;
            }
            set
            {
                this.SeparatorLeftHTMLActive = value;
            }
        }

        public string RightSeparatorActive
        {
            get
            {
                return this.SeparatorRightHTMLActive;
            }
            set
            {
                this.SeparatorRightHTMLActive = value;
            }
        }

        public string LeftSeparatorBreadCrumb
        {
            get
            {
                return this.SeparatorLeftHTMLBreadCrumb;
            }
            set
            {
                this.SeparatorLeftHTMLBreadCrumb = value;
            }
        }

        public string RightSeparatorBreadCrumb
        {
            get
            {
                return this.SeparatorRightHTMLBreadCrumb;
            }
            set
            {
                this.SeparatorRightHTMLBreadCrumb = value;
            }
        }

        public string LeftSeparatorCssClass
        {
            get
            {
                return this.CSSLeftSeparator;
            }
            set
            {
                this.CSSLeftSeparator = value;
            }
        }

        public string RightSeparatorCssClass
        {
            get
            {
                return this.CSSRightSeparator;
            }
            set
            {
                this.CSSRightSeparator = value;
            }
        }

        public string LeftSeparatorActiveCssClass
        {
            get
            {
                return this.CSSLeftSeparatorSelection;
            }
            set
            {
                this.CSSLeftSeparatorSelection = value;
            }
        }

        public string RightSeparatorActiveCssClass
        {
            get
            {
                return this.CSSRightSeparatorSelection;
            }
            set
            {
                this.CSSRightSeparatorSelection = value;
            }
        }

        public string LeftSeparatorBreadCrumbCssClass
        {
            get
            {
                return this.CSSLeftSeparatorBreadCrumb;
            }
            set
            {
                this.CSSLeftSeparatorBreadCrumb = value;
            }
        }

        public string RightSeparatorBreadCrumbCssClass
        {
            get
            {
                return this.CSSRightSeparatorBreadCrumb;
            }
            set
            {
                this.CSSRightSeparatorBreadCrumb = value;
            }
        }

        public string MenuAlignment
        {
            get
            {
                return this.ControlAlignment;
            }
            set
            {
                this.ControlAlignment = value;
            }
        }

        public string ClearDefaults
        {
            get
            {
                    return _clearDefaults;
                }
            set
            {
                _clearDefaults = value;
            }
        }

        public string DelaySubmenuLoad
        {
            get
            {
                //Return _delaySubmenuLoad
                return null;
            }
            set
            {
                //_delaySubmenuLoad = Value
            }
        }

        public string RootOnly
        {
            get
            {
                    return m_strRootOnly;
                }
            set
                {
                m_strRootOnly = value;
                }
            }
            
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                bool blnUseSkinPathArrowImages = bool.Parse( GetValue( UseSkinPathArrowImages, "False" ) ); //This setting allows for skin independant images without breaking legacy skins
                bool blnUseRootBreadcrumbArrow = bool.Parse( GetValue( UseRootBreadCrumbArrow, "True" ) ); //This setting determines if the DNN root menu will use an arrow to indicate it is the active root level tab
                bool blnUseSubMenuBreadcrumbArrow = bool.Parse( GetValue( UseSubMenuBreadCrumbArrow, "False" ) ); //This setting determines if the DNN sub-menus will use an arrow to indicate it is a member of the Breadcrumb tabs
                bool blnUseArrows = bool.Parse( GetValue( UseArrows, "True" ) ); //This setting determines if the submenu arrows will be used
                bool blnRootOnly = bool.Parse( GetValue( RootOnly, "False" ) ); //This setting determines if the submenu will be shown
                string strRootBreadcrumbArrow;
                string strSubMenuBreadcrumbArrow;
                string strRightArrow;
                string strDownArrow;

                if( blnRootOnly )
                {
                    blnUseArrows = false;
                    this.PopulateNodesFromClient = false;
                    this.StartTabId = - 1;
                    this.ExpandDepth = 1;
                }

//                SkinController objSkins = new SkinController();
                //image for root menu breadcrumb marking
                if( !String.IsNullOrEmpty(RootBreadCrumbArrow))
                {
                    strRootBreadcrumbArrow = PortalSettings.ActiveTab.SkinPath + RootBreadCrumbArrow;
                }
                else
                {
                    strRootBreadcrumbArrow = Globals.ApplicationPath + "/images/breadcrumb.gif";
                }

                //image for submenu breadcrumb marking
                if( !String.IsNullOrEmpty(SubMenuBreadCrumbArrow))
                {
                    strSubMenuBreadcrumbArrow = PortalSettings.ActiveTab.SkinPath + SubMenuBreadCrumbArrow;
                }

                if( blnUseSubMenuBreadcrumbArrow )
                {
                    strSubMenuBreadcrumbArrow = Globals.ApplicationPath + "/images/breadcrumb.gif";
                    this.NodeLeftHTMLBreadCrumbSub = "<img alt=\"*\" BORDER=\"0\" src=\"" + strSubMenuBreadcrumbArrow + "\">";
                }

                if( blnUseRootBreadcrumbArrow )
                {
                    this.NodeLeftHTMLBreadCrumbRoot = "<img alt=\"*\" BORDER=\"0\" src=\"" + strRootBreadcrumbArrow + "\">";
                }

                //image for right facing arrow
                if( !String.IsNullOrEmpty(RightArrow))
                {
                    strRightArrow = RightArrow;
                }
                else
                {
                    strRightArrow = "breadcrumb.gif";
                }
                //image for down facing arrow
                if( !String.IsNullOrEmpty(DownArrow))
                {
                    strDownArrow = DownArrow;
                }
                else
                {
                    strDownArrow = "menu_down.gif";
                }

                //Set correct image path for all separator images
                if( !String.IsNullOrEmpty(Separator))
                {
                    if( Separator.IndexOf( "src=" ) != - 1 )
                    {
                        Separator = Separator.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }

                if( !String.IsNullOrEmpty(LeftSeparator))
                {
                    if( LeftSeparator.IndexOf( "src=" ) != - 1 )
                    {
                        LeftSeparator = LeftSeparator.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                if( !String.IsNullOrEmpty(RightSeparator))
                {
                    if( RightSeparator.IndexOf( "src=" ) != - 1 )
                    {
                        RightSeparator = RightSeparator.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                if( !String.IsNullOrEmpty(LeftSeparatorBreadCrumb))
                {
                    if( LeftSeparatorBreadCrumb.IndexOf( "src=" ) != - 1 )
                    {
                        LeftSeparatorBreadCrumb = LeftSeparatorBreadCrumb.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                if( !String.IsNullOrEmpty(RightSeparatorBreadCrumb))
                {
                    if( RightSeparatorBreadCrumb.IndexOf( "src=" ) != - 1 )
                    {
                        RightSeparatorBreadCrumb = RightSeparatorBreadCrumb.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                if( !String.IsNullOrEmpty(LeftSeparatorActive))
                {
                    if( LeftSeparatorActive.IndexOf( "src=" ) != - 1 )
                    {
                        LeftSeparatorActive = LeftSeparatorActive.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                if( !String.IsNullOrEmpty(RightSeparatorActive))
                {
                    if( RightSeparatorActive.IndexOf( "src=" ) != - 1 )
                    {
                        RightSeparatorActive = RightSeparatorActive.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }

                // generate dynamic menu
                if( blnUseSkinPathArrowImages )
                {
                    this.PathSystemImage = PortalSettings.ActiveTab.SkinPath;
                }
                else
                {
                    this.PathSystemImage = Globals.ApplicationPath + "/images/";
                }
                this.PathImage = PortalSettings.HomeDirectory;
                if( blnUseArrows )
                {
                    this.IndicateChildImageSub = strRightArrow;
                    if( this.ControlOrientation.ToLower() == "vertical" ) //NavigationProvider.NavigationProvider.Orientation.Vertical Then
                    {
                        this.IndicateChildImageRoot = strRightArrow;
                    }
                    else
                    {
                        this.IndicateChildImageRoot = strDownArrow;
                    }
                }
                else
                {
                    this.PathSystemImage = Globals.ApplicationPath + "/images/";
                    this.IndicateChildImageSub = "spacer.gif";
                }
                if( String.IsNullOrEmpty( PathSystemScript ) )
                {
                    this.PathSystemScript = Globals.ApplicationPath + "/controls/SolpartMenu/";
                }

                BuildNodes( null );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private void BuildNodes( DNNNode objNode )
        {
            DNNNodeCollection objNodes;
            objNodes = GetNavigationNodes( objNode );
            this.Control.ClearNodes(); //since we always bind we need to clear the nodes for providers that maintain their state
            this.Bind( objNodes );
        }

        private void SetAttributes()
        {
            SeparateCss = "true"; // CStr(True)
            //Me.StyleSelectionBorderColor = Nothing

            if( bool.Parse( GetValue( ClearDefaults, "false" ) ) )
            {
                //Me.StyleSelectionBorderColor = Nothing
                //Me.StyleSelectionForeColor = Nothing
                //Me.StyleHighlightColor = Nothing
                //Me.StyleIconBackColor = Nothing
                //Me.EffectsShadowColor = Nothing
                //Me.StyleSelectionColor = Nothing
                //Me.StyleBackColor = Nothing
                //Me.StyleForeColor = Nothing
            }
            else //these defaults used to be on the page HTML
            {
                if( String.IsNullOrEmpty( MouseOutHideDelay ) )
                {
                    this.MouseOutHideDelay = "500";
                }
                if( String.IsNullOrEmpty( MouseOverAction ) )
                {
                    this.MouseOverAction = true.ToString(); //NavigationProvider.NavigationProvider.HoverAction.Expand
                }
                if( String.IsNullOrEmpty( StyleBorderWidth ) )
                {
                    this.StyleBorderWidth = "0";
                }
                if( String.IsNullOrEmpty( StyleControlHeight ) )
                {
                    this.StyleControlHeight = "16";
                }
                if( String.IsNullOrEmpty( StyleNodeHeight ) )
                {
                    this.StyleNodeHeight = "21";
                }
                if( String.IsNullOrEmpty( StyleIconWidth ) )
                {
                    this.StyleIconWidth = "0";
                }
                //Me.StyleSelectionBorderColor = "#333333" 'cleared above
                if( String.IsNullOrEmpty( StyleSelectionColor ) )
                {
                    this.StyleSelectionColor = "#CCCCCC";
                }
                if( String.IsNullOrEmpty( StyleSelectionForeColor ) )
                {
                    this.StyleSelectionForeColor = "White";
                }
                if( String.IsNullOrEmpty( StyleHighlightColor ) )
                {
                    this.StyleHighlightColor = "#FF8080";
                }
                if( String.IsNullOrEmpty( StyleIconBackColor ) )
                {
                    this.StyleIconBackColor = "#333333";
                }
                if( String.IsNullOrEmpty( EffectsShadowColor ) )
                {
                    this.EffectsShadowColor = "#404040";
                }
                if( String.IsNullOrEmpty( MouseOverDisplay ) )
                {
                    this.MouseOverDisplay = "highlight"; //NavigationProvider.NavigationProvider.HoverDisplay.Highlight
                }
                if( String.IsNullOrEmpty( EffectsStyle ) )
                {
                    this.EffectsStyle = "filter:progid:DXImageTransform.Microsoft.Shadow(color='DimGray', Direction=135, Strength=3);";
                }
                if( String.IsNullOrEmpty( StyleFontSize ) )
                {
                    this.StyleFontSize = "9";
                }
                if( String.IsNullOrEmpty( StyleFontBold ) )
                {
                    this.StyleFontBold = "True";
                }
                if( String.IsNullOrEmpty( StyleFontNames ) )
                {
                    this.StyleFontNames = "Tahoma,Arial,Helvetica";
                }
                if( String.IsNullOrEmpty( StyleForeColor ) )
                {
                    this.StyleForeColor = "White";
                }
                if( String.IsNullOrEmpty( StyleBackColor ) )
                {
                    this.StyleBackColor = "#333333";
                }
                if( String.IsNullOrEmpty( PathSystemImage ) )
                {
                    this.PathSystemImage = "/";
                }
            }

            if( SeparateCss != null && Boolean.Parse( SeparateCss  ) )
            {

                if(!String.IsNullOrEmpty(MenuBarCssClass))
                {
                    this.CSSControl = MenuBarCssClass;
                }
                else
                {
                    this.CSSControl = "MainMenu_MenuBar";
                }


                if (!String.IsNullOrEmpty(MenuContainerCssClass))
                {
                    this.CSSContainerRoot = MenuContainerCssClass;
                }
                else
                {
                    this.CSSContainerRoot = "MainMenu_MenuContainer";
                }

                if (!String.IsNullOrEmpty(MenuItemCssClass))
                {
                    this.CSSNode = MenuItemCssClass;
                }
                else
                {
                    this.CSSNode = "MainMenu_MenuItem";
                }

                if (!String.IsNullOrEmpty(MenuIconCssClass))
                {
                    this.CSSIcon = MenuIconCssClass;
                }
                else
                {
                    this.CSSIcon = "MainMenu_MenuIcon";
                }


                if (!String.IsNullOrEmpty(SubMenuCssClass))
                {
                    this.CSSContainerSub = SubMenuCssClass;
                }
                else
                {
                    this.CSSContainerSub = "MainMenu_SubMenu";
                }


                if (!String.IsNullOrEmpty(MenuBreakCssClass))
                {
                    this.CSSBreak = MenuBreakCssClass;
                }
                else
                {
                    this.CSSBreak = "MainMenu_MenuBreak";
                }

                if (!String.IsNullOrEmpty(MenuItemSelCssClass))
                {
                    this.CSSNodeHover = MenuItemSelCssClass;
                }
                else
                {
                    this.CSSNodeHover = "MainMenu_MenuItemSel";
                }

                if (!String.IsNullOrEmpty(MenuArrowCssClass))
                {
                    this.CSSIndicateChildSub = MenuArrowCssClass;
                }
                else
                {
                    this.CSSIndicateChildSub = "MainMenu_MenuArrow";
                }

                if (!String.IsNullOrEmpty(MenuRootArrowCssClass))
                {
                    this.CSSIndicateChildRoot = MenuRootArrowCssClass;
                }
                else
                {
                    this.CSSIndicateChildRoot = "MainMenu_RootMenuArrow";
                }
            }
        }

        protected override void OnInit( EventArgs e )
        {
            SetAttributes();
            InitializeNavControl( this, "SolpartMenuNavigationProvider" );
            base.OnInit( e );
        }
    }
}