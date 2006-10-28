using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.UI.WebControls;
using Solpart.WebControls;

namespace DotNetNuke.NavigationControl
{
    public class SolpartMenuNavigationProvider : NavigationProvider
    {
        private bool m_blnIndicateChildren;
        private SolpartMenu m_objMenu;
        private string m_strControlID;
        private string m_strCSSBreadCrumbRoot;
        private string m_strCSSBreadCrumbSub;

        private string m_strCSSLeftSeparator;
        private string m_strCSSLeftSeparatorBreadCrumb;
        private string m_strCSSLeftSeparatorSelection;
        private string m_strCSSNodeHoverRoot;
        private string m_strCSSNodeHoverSub;
        private string m_strCSSNodeRoot;
        private string m_strCSSRightSeparator;
        private string m_strCSSRightSeparatorBreadCrumb;
        private string m_strCSSRightSeparatorSelection;
        private string m_strCSSSeparator;
        private string m_strIndicateChildImageBreadCrumbRoot;
        private string m_strIndicateChildImageBreadCrumbSub;
        private string m_strNodeLeftHTMLBreadCrumbRoot = "";
        private string m_strNodeLeftHTMLBreadCrumbSub = "";
        private string m_strNodeLeftHTMLRoot = "";
        private string m_strNodeLeftHTMLSub = "";
        private string m_strNodeRightHTMLBreadCrumbRoot = "";
        private string m_strNodeRightHTMLBreadCrumbSub = "";
        private string m_strNodeRightHTMLRoot = "";
        private string m_strNodeRightHTMLSub = "";
        private string m_strNodeSelectedRoot;
        private string m_strNodeSelectedSub;
        private string m_strSeparatorHTML = "";
        private string m_strSeparatorLeftHTML = "";
        private string m_strSeparatorLeftHTMLActive = "";
        private string m_strSeparatorLeftHTMLBreadCrumb = "";
        private string m_strSeparatorRightHTML = "";
        private string m_strSeparatorRightHTMLActive = "";
        private string m_strSeparatorRightHTMLBreadCrumb = "";
        private string m_strStyleRoot;

        public override Alignment ControlAlignment
        {
            get
            {
                if( Menu.MenuAlignment.ToLower() == "left" )
                {
                    return Alignment.Left;
                }
                else if( Menu.MenuAlignment.ToLower() == "right" )
                {
                    return Alignment.Right;
                }
                else if( Menu.MenuAlignment.ToLower() == "center" )
                {
                    return Alignment.Center;
                }
                else if( Menu.MenuAlignment.ToLower() == "justify" )
                {
                    return Alignment.Justify;
                }
                else
                {
                    return Alignment.Justify;
                }
            }
            set
            {
                switch( value )
                {
                    case Alignment.Left:

                        Menu.MenuAlignment = "Left";
                        break;
                    case Alignment.Right:

                        Menu.MenuAlignment = "Right";
                        break;
                    case Alignment.Center:

                        Menu.MenuAlignment = "Center";
                        break;
                    case Alignment.Justify:

                        Menu.MenuAlignment = "Justify";
                        break;
                }
            }
        }

        public override string ControlID
        {
            get
            {
                return m_strControlID;
            }
            set
            {
                m_strControlID = value;
            }
        }

        public override Orientation ControlOrientation
        {
            get
            {
                if( Menu.Display.ToLower() == "horizontal" )
                {
                    return Orientation.Horizontal;
                }
                else if( Menu.Display.ToLower() == "vertical" )
                {
                    return Orientation.Vertical;
                }
                return Orientation.Horizontal;
            }
            set
            {
                switch( value )
                {
                    case Orientation.Horizontal:

                        Menu.Display = "Horizontal";
                        break;
                    case Orientation.Vertical:

                        Menu.Display = "Vertical";
                        break;
                }
            }
        }

        public override string CSSBreadCrumbRoot
        {
            get
            {
                return m_strCSSBreadCrumbRoot;
            }
            set
            {
                m_strCSSBreadCrumbRoot = value;
            }
        }

        public override string CSSBreadCrumbSub
        {
            get
            {
                return m_strCSSBreadCrumbSub;
            }
            set
            {
                m_strCSSBreadCrumbSub = value;
            }
        }

        public override string CSSBreak
        {
            get
            {
                return Menu.MenuCSS.MenuBreak;
            }
            set
            {
                Menu.MenuCSS.MenuBreak = value;
            }
        }

        public override string CSSContainerRoot
        {
            get
            {
                return Menu.MenuCSS.MenuContainer;
            }
            set
            {
                Menu.MenuCSS.MenuContainer = value;
            }
        }

        public override string CSSContainerSub
        {
            get
            {
                return Menu.MenuCSS.SubMenu;
            }
            set
            {
                Menu.MenuCSS.SubMenu = value;
            }
        }

        public override string CSSControl
        {
            get
            {
                return Menu.MenuCSS.MenuBar;
            }
            set
            {
                Menu.MenuCSS.MenuBar = value;
            }
        }

        public override string CSSIcon
        {
            get
            {
                return Menu.MenuCSS.MenuIcon;
            }
            set
            {
                Menu.MenuCSS.MenuIcon = value;
            }
        }

        public override string CSSIndicateChildRoot
        {
            get
            {
                return Menu.MenuCSS.RootMenuArrow;
            }
            set
            {
                Menu.MenuCSS.RootMenuArrow = value;
            }
        }

        public override string CSSIndicateChildSub
        {
            get
            {
                return Menu.MenuCSS.MenuArrow;
            }
            set
            {
                Menu.MenuCSS.MenuArrow = value;
            }
        }

        public override string CSSLeftSeparator
        {
            get
            {
                return m_strCSSLeftSeparator;
            }
            set
            {
                m_strCSSLeftSeparator = value;
            }
        }

        public override string CSSLeftSeparatorBreadCrumb
        {
            get
            {
                return m_strCSSLeftSeparatorBreadCrumb;
            }
            set
            {
                m_strCSSLeftSeparatorBreadCrumb = value;
            }
        }

        public override string CSSLeftSeparatorSelection
        {
            get
            {
                return m_strCSSLeftSeparatorSelection;
            }
            set
            {
                m_strCSSLeftSeparatorSelection = value;
            }
        }

        public override string CSSNode
        {
            get
            {
                return Menu.MenuCSS.MenuItem;
            }
            set
            {
                Menu.MenuCSS.MenuItem = value;
            }
        }

        public override string CSSNodeHover
        {
            get
            {
                return Menu.MenuCSS.MenuItemSel;
            }
            set
            {
                Menu.MenuCSS.MenuItemSel = value;
            }
        }

        public override string CSSNodeHoverRoot
        {
            get
            {
                return m_strCSSNodeHoverRoot;
            }
            set
            {
                m_strCSSNodeHoverRoot = value;
            }
        }

        public override string CSSNodeHoverSub
        {
            get
            {
                return m_strCSSNodeHoverSub;
            }
            set
            {
                m_strCSSNodeHoverSub = value;
            }
        }

        public override string CSSNodeRoot
        {
            get
            {
                return m_strCSSNodeRoot;
            }
            set
            {
                m_strCSSNodeRoot = value;
            }
        }

        public override string CSSNodeSelectedRoot
        {
            get
            {
                return m_strNodeSelectedRoot;
            }
            set
            {
                m_strNodeSelectedRoot = value;
            }
        }

        public override string CSSNodeSelectedSub
        {
            get
            {
                return m_strNodeSelectedSub;
            }
            set
            {
                m_strNodeSelectedSub = value;
            }
        }

        public override string CSSRightSeparator
        {
            get
            {
                return m_strCSSRightSeparator;
            }
            set
            {
                m_strCSSRightSeparator = value;
            }
        }

        public override string CSSRightSeparatorBreadCrumb
        {
            get
            {
                return m_strCSSRightSeparatorBreadCrumb;
            }
            set
            {
                m_strCSSRightSeparatorBreadCrumb = value;
            }
        }

        public override string CSSRightSeparatorSelection
        {
            get
            {
                return m_strCSSRightSeparatorSelection;
            }
            set
            {
                m_strCSSRightSeparatorSelection = value;
            }
        }

        public override string CSSSeparator
        {
            get
            {
                return m_strCSSSeparator;
            }
            set
            {
                m_strCSSSeparator = value;
            }
        }

        public override double EffectsDuration
        {
            get
            {
                return Menu.MenuEffects.MenuTransitionLength;
            }
            set
            {
                Menu.MenuEffects.MenuTransitionLength = value;
            }
        }

        public override string EffectsShadowColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.ShadowColor );
            }
            set
            {
                Menu.ShadowColor = Color.FromName( value );
            }
        }

        public override string EffectsShadowDirection
        {
            get
            {
                return Menu.MenuEffects.ShadowDirection;
            }
            set
            {
                Menu.MenuEffects.ShadowDirection = value;
            }
        }

        public override int EffectsShadowStrength
        {
            get
            {
                return Menu.MenuEffects.ShadowStrength;
            }
            set
            {
                Menu.MenuEffects.ShadowStrength = value;
            }
        }

        public override string EffectsStyle
        {
            get
            {
                return Menu.MenuEffects.get_Style( true ).ToString();
            }
            set
            {
                string.Concat( Menu.MenuEffects.get_Style( true ).ToString(), value );
            }
        }

        public override string EffectsTransition
        {
            get
            {
                return Menu.MenuEffects.MenuTransition;
            }
            set
            {
                Menu.MenuEffects.MenuTransition = value;
            }
        }

        public override string ForceCrawlerDisplay
        {
            get
            {
                return Menu.ForceFullMenuList.ToString();
            }
            set
            {
                Menu.ForceFullMenuList = Convert.ToBoolean( value );
            }
        }

        public override string ForceDownLevel
        {
            get
            {
                return Menu.ForceDownlevel.ToString();
            }
            set
            {
                Menu.ForceDownlevel = Convert.ToBoolean( value );
            }
        }

        public override string IndicateChildImageRoot
        {
            get
            {
                return Menu.RootArrowImage;
            }
            set
            {
                Menu.RootArrowImage = value;
            }
        }

        public override string IndicateChildImageSub
        {
            get
            {
                return Menu.ArrowImage;
            }
            set
            {
                Menu.ArrowImage = value;
            }
        }

        public override bool IndicateChildren
        {
            get
            {
                return m_blnIndicateChildren;
            }
            set
            {
                m_blnIndicateChildren = value;
            }
        }

        public SolpartMenu Menu
        {
            get
            {
                return m_objMenu;
            }
        }

        public override decimal MouseOutHideDelay
        {
            get
            {
                return Menu.MenuEffects.MouseOutHideDelay;
            }
            set
            {
                Menu.MenuEffects.MouseOutHideDelay = Convert.ToInt32( value );
            }
        }

        public override HoverAction MouseOverAction
        {
            get
            {
                if( Menu.MenuEffects.MouseOverExpand )
                {
                    return HoverAction.Expand;
                }
                else
                {
                    return HoverAction.None;
                }
            }
            set
            {
                if( value == HoverAction.Expand )
                {
                    Menu.MenuEffects.MouseOverExpand = true;
                }
                else
                {
                    Menu.MenuEffects.MouseOverExpand = false;
                }
            }
        }

        public override HoverDisplay MouseOverDisplay
        {
            get
            {
                if( Menu.MenuEffects.MouseOverDisplay == MenuEffectsMouseOverDisplay.Highlight )
                {
                    return HoverDisplay.Highlight;
                }
                else if( Menu.MenuEffects.MouseOverDisplay == MenuEffectsMouseOverDisplay.Outset )
                {
                    return HoverDisplay.Outset;
                }
                else if( Menu.MenuEffects.MouseOverDisplay == MenuEffectsMouseOverDisplay.None )
                {
                    return HoverDisplay.None;
                }
                return HoverDisplay.None;
            }
            set
            {
                switch( value )
                {
                    case HoverDisplay.Highlight:

                        Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.Highlight;
                        break;
                    case HoverDisplay.Outset:

                        Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.Outset;
                        break;
                    case HoverDisplay.None:

                        Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.None;
                        break;
                }
            }
        }

        public override Control NavigationControl
        {
            get
            {
                return Menu;
            }
        }

        public override string NodeLeftHTMLBreadCrumbRoot
        {
            get
            {
                return m_strNodeLeftHTMLBreadCrumbRoot;
            }
            set
            {
                m_strNodeLeftHTMLBreadCrumbRoot = value;
            }
        }

        public override string NodeLeftHTMLBreadCrumbSub
        {
            get
            {
                return m_strNodeLeftHTMLBreadCrumbSub;
            }
            set
            {
                m_strNodeLeftHTMLBreadCrumbSub = value;
            }
        }

        public override string NodeLeftHTMLRoot
        {
            get
            {
                return m_strNodeLeftHTMLRoot;
            }
            set
            {
                m_strNodeLeftHTMLRoot = value;
            }
        }

        public override string NodeLeftHTMLSub
        {
            get
            {
                return m_strNodeLeftHTMLSub;
            }
            set
            {
                m_strNodeLeftHTMLSub = value;
            }
        }

        public override string NodeRightHTMLBreadCrumbRoot
        {
            get
            {
                return m_strNodeRightHTMLBreadCrumbRoot;
            }
            set
            {
                m_strNodeRightHTMLBreadCrumbRoot = value;
            }
        }

        public override string NodeRightHTMLBreadCrumbSub
        {
            get
            {
                return m_strNodeRightHTMLBreadCrumbSub;
            }
            set
            {
                m_strNodeRightHTMLBreadCrumbSub = value;
            }
        }

        public override string NodeRightHTMLRoot
        {
            get
            {
                return m_strNodeRightHTMLRoot;
            }
            set
            {
                m_strNodeRightHTMLRoot = value;
            }
        }

        public override string NodeRightHTMLSub
        {
            get
            {
                return m_strNodeRightHTMLSub;
            }
            set
            {
                m_strNodeRightHTMLSub = value;
            }
        }

        public override string PathImage
        {
            get
            {
                return Menu.IconImagesPath;
            }
            set
            {
                Menu.IconImagesPath = value;
            }
        }

        public override string PathSystemImage
        {
            get
            {
                return Menu.SystemImagesPath;
            }
            set
            {
                Menu.SystemImagesPath = value;
            }
        }

        public override string PathSystemScript
        {
            get
            {
                return Menu.SystemScriptPath;
            }
            set
            {
                Menu.SystemScriptPath = value;
            }
        }

        public override string SeparatorHTML
        {
            get
            {
                return m_strSeparatorHTML;
            }
            set
            {
                m_strSeparatorHTML = value;
            }
        }

        public override string SeparatorLeftHTML
        {
            get
            {
                return m_strSeparatorLeftHTML;
            }
            set
            {
                m_strSeparatorLeftHTML = value;
            }
        }

        public override string SeparatorLeftHTMLActive
        {
            get
            {
                return m_strSeparatorLeftHTMLActive;
            }
            set
            {
                m_strSeparatorLeftHTMLActive = value;
            }
        }

        public override string SeparatorLeftHTMLBreadCrumb
        {
            get
            {
                return m_strSeparatorLeftHTMLBreadCrumb;
            }
            set
            {
                m_strSeparatorLeftHTMLBreadCrumb = value;
            }
        }

        public override string SeparatorRightHTML
        {
            get
            {
                return m_strSeparatorRightHTML;
            }
            set
            {
                m_strSeparatorRightHTML = value;
            }
        }

        public override string SeparatorRightHTMLActive
        {
            get
            {
                return m_strSeparatorRightHTMLActive;
            }
            set
            {
                m_strSeparatorRightHTMLActive = value;
            }
        }

        public override string SeparatorRightHTMLBreadCrumb
        {
            get
            {
                return m_strSeparatorRightHTMLBreadCrumb;
            }
            set
            {
                m_strSeparatorRightHTMLBreadCrumb = value;
            }
        }

        public override string StyleBackColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.BackColor );
            }
            set
            {
                Menu.BackColor = Color.FromName( value );
            }
        }

        public override decimal StyleBorderWidth
        {
            get
            {
                return Menu.MenuBorderWidth;
            }
            set
            {
                Menu.MenuBorderWidth = Convert.ToInt32( value );
            }
        }

        public override decimal StyleControlHeight
        {
            get
            {
                return Menu.MenuBarHeight;
            }
            set
            {
                Menu.MenuBarHeight = Convert.ToInt32( value );
            }
        }

        public override string StyleFontBold
        {
            get
            {
                return Menu.Font.Bold.ToString();
            }
            set
            {
                Menu.Font.Bold = Convert.ToBoolean( value );
            }
        }

        public override string StyleFontNames
        {
            get
            {
                return string.Join( ";", Menu.Font.Names );
            }
            set
            {
                Menu.Font.Names = value.Split( Convert.ToChar( ";" ) );
            }
        }

        public override decimal StyleFontSize
        {
            get
            {
                return Convert.ToDecimal( Menu.Font.Size.Unit.Value );
            }
            set
            {
                Menu.Font.Size = FontUnit.Parse( value.ToString() );
            }
        }

        public override string StyleForeColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.ForeColor );
            }
            set
            {
                Menu.ForeColor = Color.FromName( value );
            }
        }

        public override string StyleHighlightColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.HighlightColor );
            }
            set
            {
                Menu.HighlightColor = Color.FromName( value );
            }
        }

        public override string StyleIconBackColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.IconBackgroundColor );
            }
            set
            {
                Menu.IconBackgroundColor = Color.FromName( value );
            }
        }

        public override decimal StyleIconWidth
        {
            get
            {
                return Menu.IconWidth;
            }
            set
            {
                Menu.IconWidth = Convert.ToInt32( value );
            }
        }

        public override decimal StyleNodeHeight
        {
            get
            {
                return Menu.MenuItemHeight;
            }
            set
            {
                Menu.MenuItemHeight = Convert.ToInt32( value );
            }
        }

        public override string StyleRoot
        {
            get
            {
                return m_strStyleRoot;
            }
            set
            {
                m_strStyleRoot = value;
            }
        }

        public override string StyleSelectionBorderColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.SelectedBorderColor );
            }
            set
            {
                if( value != null )
                {
                    Menu.SelectedBorderColor = Color.FromName( value );
                }
                else
                {
                    Menu.SelectedBorderColor = Color.Empty;
                }
            }
        }

        public override string StyleSelectionColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.SelectedColor );
            }
            set
            {
                Menu.SelectedColor = Color.FromName( value );
            }
        }

        public override string StyleSelectionForeColor
        {
            get
            {
                return ColorTranslator.ToHtml( Menu.SelectedForeColor );
            }
            set
            {
                Menu.SelectedForeColor = Color.FromName( value );
            }
        }

        public override bool SupportsPopulateOnDemand
        {
            get
            {
                return false;
            }
        }

        private string GetClientScriptURL( string strScript, string strID )
        {
            if( strScript.ToLower().StartsWith( "javascript:" ) == false )
            {
                strScript = "javascript: " + strScript;
            }
            return strScript;
        }

        private string GetSeparatorTD( string strClass, string strHTML )
        {
            string strRet = "";
            if( strClass != "" )
            {
                strRet += "<td class = \"" + strClass + "\">";
            }
            else
            {
                strRet += "<td>";
            }
            strRet += strHTML + "</td>";

            return strRet;
        }

        private string GetSeparatorText( string strNormal, string strBreadCrumb, string strSelection, DNNNode objNode )
        {
            string strRet = "";
            if( strNormal != "" )
            {
                strRet = strNormal;
            }
            if( strBreadCrumb != "" && objNode != null && objNode.BreadCrumb )
            {
                strRet = strBreadCrumb;
            }
            if( strSelection != "" && objNode != null && objNode.Selected )
            {
                strRet = strSelection;
            }
            return strRet;
        }

        private void AddSeparator( string strType, DNNNode objPrevNode, DNNNode objNextNode )
        {
            string strLeftHTML = SeparatorLeftHTML + SeparatorLeftHTMLBreadCrumb + SeparatorLeftHTMLActive;
            string strRightHTML = SeparatorRightHTML + SeparatorRightHTMLBreadCrumb + SeparatorRightHTMLActive;
            string strHTML = this.SeparatorHTML + strLeftHTML + strRightHTML;
            XmlNode objBreak;
            if( strHTML.Length > 0 )
            {
                string strSeparatorTable = "";
                string strSeparator = "";
                string strSeparatorLeftHTML = "";
                string strSeparatorRightHTML = "";
                string strSeparatorClass = "";
                string strLeftSeparatorClass = "";
                string strRightSeparatorClass = "";

                if( strLeftHTML.Length > 0 )
                {
                    strLeftSeparatorClass = this.GetSeparatorText( CSSLeftSeparator, CSSLeftSeparatorBreadCrumb, CSSLeftSeparatorSelection, objNextNode );
                    strSeparatorLeftHTML = this.GetSeparatorText( SeparatorLeftHTML, SeparatorLeftHTMLBreadCrumb, SeparatorLeftHTMLActive, objNextNode );
                }
                if( SeparatorHTML != "" )
                {
                    if( CSSSeparator != "" )
                    {
                        strSeparatorClass = CSSSeparator;
                    }
                    strSeparator = SeparatorHTML;
                }
                if( strRightHTML.Length > 0 )
                {
                    strRightSeparatorClass = this.GetSeparatorText( CSSRightSeparator, CSSRightSeparatorBreadCrumb, CSSRightSeparatorSelection, objPrevNode );
                    strSeparatorRightHTML = this.GetSeparatorText( SeparatorRightHTML, SeparatorRightHTMLBreadCrumb, SeparatorRightHTMLActive, objPrevNode );
                }
                strSeparatorTable = "<table summary=\"Table for menu separator design\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr>";

                if( strSeparatorRightHTML != "" && strType != "Left" )
                {
                    strSeparatorTable += GetSeparatorTD( strRightSeparatorClass, strSeparatorRightHTML );
                }
                if( strSeparator != "" && strType != "Left" && strType != "Right" )
                {
                    strSeparatorTable += GetSeparatorTD( strSeparatorClass, strSeparator );
                }
                if( strSeparatorLeftHTML != "" && strType != "Right" )
                {
                    strSeparatorTable += GetSeparatorTD( strLeftSeparatorClass, strSeparatorLeftHTML );
                }
                strSeparatorTable += "</tr></table>";
                objBreak = Menu.AddBreak( "", strSeparatorTable );
            }
        }

        public override void Bind( DNNNodeCollection objNodes )
        {
            DNNNode objNode = null;
            SPMenuItemNode objMenuItem = null;
            DNNNode objPrevNode = null;
            bool RootFlag = false;
            if( IndicateChildren == false )
            {
                //should this be spacer.gif???
                //IndicateChildImageSub = ""
                //IndicateChildImageRoot = ""
            }
            else
            {
                if( this.IndicateChildImageRoot.Length > 0 )
                {
                    Menu.RootArrow = true;
                }
            }

            foreach( DNNNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                try
                {
                    if( objNode.Level == 0 ) // root menu
                    {
                        if( RootFlag == true ) //first root item has already been entered
                        {
                            AddSeparator( "All", objPrevNode, objNode );
                        }
                        else
                        {
                            if( SeparatorLeftHTML != "" || SeparatorLeftHTMLBreadCrumb != "" || this.SeparatorLeftHTMLActive != "" )
                            {
                                AddSeparator( "Left", objPrevNode, objNode );
                            }
                            RootFlag = true;
                        }

                        if( objNode.Enabled == false )
                        {
                            objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ID.ToString(), objNode.Text, "" ) );
                        }
                        else
                        {
                            if( objNode.JSFunction.Length > 0 )
                            {
                                objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ID.ToString(), objNode.Text, GetClientScriptURL( objNode.JSFunction, objNode.ID ) ) );
                            }
                            else
                            {
                                objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ID.ToString(), objNode.Text, objNode.NavigateURL ) );
                            }
                        }
                        if( this.StyleRoot.Length > 0 )
                        {
                            objMenuItem.ItemStyle = this.StyleRoot;
                        }
                        if( this.CSSNodeRoot != "" )
                        {
                            objMenuItem.ItemCss = this.CSSNodeRoot;
                        }
                        if( this.CSSNodeHoverRoot != "" )
                        {
                            objMenuItem.ItemSelectedCss = this.CSSNodeHoverRoot;
                        }

                        if( this.NodeLeftHTMLRoot != "" )
                        {
                            objMenuItem.LeftHTML = this.NodeLeftHTMLRoot;
                        }

                        if( objNode.BreadCrumb )
                        {
                            objMenuItem.ItemCss = objMenuItem.ItemCss + " " + this.CSSBreadCrumbRoot;
                            if( NodeLeftHTMLBreadCrumbRoot != "" )
                            {
                                objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbRoot;
                            }
                            if( NodeRightHTMLBreadCrumbRoot != "" )
                            {
                                objMenuItem.RightHTML = NodeRightHTMLBreadCrumbRoot;
                            }
                            if( objNode.Selected )
                            {
                                objMenuItem.ItemCss = objMenuItem.ItemCss + " " + this.CSSNodeSelectedRoot;
                            }
                        }

                        if( this.NodeRightHTMLRoot != "" )
                        {
                            objMenuItem.RightHTML = NodeRightHTMLRoot;
                        }
                    }
                    else if( objNode.IsBreak )
                    {
                        Menu.AddBreak( objNode.ParentNode.ID.ToString() );
                    }
                    else //If Not blnRootOnly Then
                    {
                        try
                        {
                            if( objNode.Enabled == false )
                            {
                                objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ParentNode.ID.ToString(), objNode.ID.ToString(), "&nbsp;" + objNode.Text, "" ) );
                            }
                            else
                            {
                                if( objNode.JSFunction.Length > 0 )
                                {
                                    objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ParentNode.ID.ToString(), objNode.ID.ToString(), "&nbsp;" + objNode.Text, GetClientScriptURL( objNode.JSFunction, objNode.ID ) ) );
                                }
                                else
                                {
                                    objMenuItem = new SPMenuItemNode( Menu.AddMenuItem( objNode.ParentNode.ID.ToString(), objNode.ID.ToString(), "&nbsp;" + objNode.Text, objNode.NavigateURL ) );
                                }
                            }

                            if( objNode.ClickAction == eClickAction.PostBack )
                            {
                                objMenuItem.RunAtServer = true;
                            }
                            if( CSSNodeHoverSub != "" )
                            {
                                objMenuItem.ItemSelectedCss = CSSNodeHoverSub;
                            }
                            if( NodeLeftHTMLSub != "" )
                            {
                                objMenuItem.LeftHTML = NodeLeftHTMLSub;
                            }

                            if( objNode.BreadCrumb )
                            {
                                objMenuItem.ItemCss = this.CSSBreadCrumbSub;
                                if( NodeLeftHTMLBreadCrumbSub != "" )
                                {
                                    objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbSub;
                                }
                                if( NodeRightHTMLBreadCrumbSub != "" )
                                {
                                    objMenuItem.RightHTML = NodeRightHTMLBreadCrumbSub;
                                }
                                if( objNode.Selected )
                                {
                                    objMenuItem.ItemCss = this.CSSNodeSelectedSub;
                                }
                            }

                            if( this.NodeRightHTMLSub != "" )
                            {
                                objMenuItem.RightHTML = this.NodeRightHTMLSub;
                            }
                        }
                        catch
                        {
                            // throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
                            objMenuItem = null;
                        }
                        //Else
                        //	objMenuItem = Nothing
                    }

                    if( objNode.Image.Length > 0 )
                    {
                        if( objNode.Image.IndexOf( "/" ) > - 1 ) //if image contains a path
                        {
                            string strImage = objNode.Image;
                            if( strImage.StartsWith( Menu.IconImagesPath ) ) //if path (or portion) is already set in header of menu truncate it off
                            {
                                strImage = strImage.Substring( Menu.IconImagesPath.Length );
                            }
                            if( strImage.IndexOf( "/" ) > - 1 ) //if the image still contains path info assign it
                            {
                                objMenuItem.Image = strImage.Substring( strImage.LastIndexOf( "/" ) + 1 );
                                if( strImage.StartsWith( "/" ) ) //is absolute path?
                                {
                                    objMenuItem.ImagePath = strImage.Substring( 0, strImage.LastIndexOf( "/" ) + 1 );
                                }
                                else
                                {
                                    objMenuItem.ImagePath = Menu.IconImagesPath + strImage.Substring( 0, strImage.LastIndexOf( "/" ) + 1 );
                                }
                            }
                            else
                            {
                                objMenuItem.Image = strImage;
                            }
                        }
                        else
                        {
                            objMenuItem.Image = objNode.Image;
                        }
                    }
                    if( objNode.ToolTip.Length > 0 )
                    {
                        objMenuItem.ToolTip = objNode.ToolTip;
                    }

                    Bind( objNode.DNNNodes );
                }
                catch( Exception ex )
                {
                    throw ( ex );
                }
                objPrevNode = objNode;
            }

            if( objNode != null && objNode.Level == 0 ) // root menu
            {
                if( SeparatorRightHTML != "" || SeparatorRightHTMLBreadCrumb != "" || this.SeparatorRightHTMLActive != "" )
                {
                    AddSeparator( "Right", objPrevNode, null );
                }
            }
        }

        private void ctlActions_MenuClick( string ID )
        {
            RaiseEvent_NodeClick( ID );
        }

        public override void Initialize()
        {
            m_objMenu = new SolpartMenu();
            Menu.ID = m_strControlID;
            Menu.SeparateCSS = true;
            StyleSelectionBorderColor = null; //used to be done in skin object...

            m_objMenu.MenuClick += new SolpartMenu.MenuClickEventHandler( ctlActions_MenuClick );
        }
    }
}