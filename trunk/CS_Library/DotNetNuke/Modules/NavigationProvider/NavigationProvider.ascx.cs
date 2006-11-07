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
using System.Web.UI;
using DotNetNuke.Framework;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.NavigationProvider
{
    public abstract class NavigationProvider : UserControlBase
    {
        public delegate void NodeClickEventHandler( NavigationEventArgs args );

        public delegate void PopulateOnDemandEventHandler( NavigationEventArgs args );

        public enum Alignment
        {
            Left,
            Right,
            Center,
            Justify
        }

        public enum HoverAction
        {
            Expand,
            None
        }

        public enum HoverDisplay
        {
            Highlight,
            Outset,
            None
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public event NodeClickEventHandler NodeClick
        {
            add
            {
                NodeClickEvent += value;
            }
            remove
            {
                NodeClickEvent -= value;
            }
        }

        public event PopulateOnDemandEventHandler PopulateOnDemand
        {
            add
            {
                PopulateOnDemandEvent += value;
            }
            remove
            {
                PopulateOnDemandEvent -= value;
            }
        }

        private NodeClickEventHandler NodeClickEvent;

        private PopulateOnDemandEventHandler PopulateOnDemandEvent;

        public virtual Alignment ControlAlignment //MenuAlignment
        {
            get
            {
                return Alignment.Left;
            }
            set
            {
            }
        }

        public abstract string ControlID { get; set; }

        public virtual Orientation ControlOrientation //Display
        {
            get
            {
                return Orientation.Horizontal;
            }
            set
            {
            }
        }

        public virtual string CSSBreadCrumbRoot //RootMenuItemBreadCrumbCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSBreadCrumbSub //SubMenuItemBreadCrumbCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSBreak //MenuBreakCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSContainerRoot //MenuContainerCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSContainerSub //SubMenuCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        //NodeLeftHTMLBreadCrumbSub
        //Public Overridable Property IndicateChildImageBreadCrumbSub() As String		  'SubMenuBreadCrumbArrow
        //	Get
        //		Return ""
        //	End Get
        //	Set(ByVal Value As String)

        //	End Set
        //End Property
        //NodeLeftHTMLBreadCrumbRoot
        //Public Overridable Property IndicateChildImageBreadCrumbRoot() As String		  'RootBreadCrumbArrow
        //	Get
        //		Return ""
        //	End Get
        //	Set(ByVal Value As String)

        //	End Set
        //End Property

        public abstract string CSSControl { //MenuBarCSSClass
            get; set; }

        public virtual string CSSIcon //MenuIconCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSIndicateChildRoot //MenuRootArrowCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSIndicateChildSub //MenuArrowCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSLeftSeparator //LeftSeparatorCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSLeftSeparatorBreadCrumb //LeftSeparatorCssClassBreadCrumb
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSLeftSeparatorSelection //LeftSeparatorActiveCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNode //MenuItemCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeHover //MenuItemSelCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeHoverRoot //RootMenuItemSelectedCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeHoverSub //SubMenuItemSelectedCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeRoot //RootMenuItemCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeSelectedRoot //RootMenuItemActiveCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSNodeSelectedSub //SubMenuItemActiveCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSRightSeparator //RightSeparatorCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSRightSeparatorBreadCrumb //RightSeparatorCssClassBreadCrumb
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSRightSeparatorSelection //RightSeparatorActiveCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string CSSSeparator //SeparatorCssClass
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual double EffectsDuration //MenuEffectsMenuTransitionLength
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        public virtual string EffectsShadowColor //MenuEffectsShadowColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string EffectsShadowDirection //MenuEffectsShadowDirection
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual int EffectsShadowStrength //MenuEffectsShadowStrength
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        public virtual string EffectsStyle //MenuEffectsStyle
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string EffectsTransition //MenuEffectsTransition
        {
            get
            {
                return "\'";
            }
            set
            {
            }
        }

        public virtual string ForceCrawlerDisplay //ForceFullMenuList
        {
            get
            {
                return "False";
            }
            set
            {
            }
        }

        public virtual string ForceDownLevel //ForceDownLevel
        {
            get
            {
                return false.ToString();
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageExpandedRoot //for tree
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageExpandedSub //for tree
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageRoot
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageSub
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual bool IndicateChildren //UseIndicateChilds
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public virtual decimal MouseOutHideDelay //MenuEffectsMouseOutHideDelay, MouseOutHideDelay
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        public virtual HoverAction MouseOverAction //MenuEffectsMouseOverExpand
        {
            get
            {
                return HoverAction.Expand;
            }
            set
            {
            }
        }

        public virtual HoverDisplay MouseOverDisplay //MenuEffectsMouseOverDisplay
        {
            get
            {
                return HoverDisplay.Highlight;
            }
            set
            {
            }
        }

        // Properties
        public abstract Control NavigationControl { get; }

        public virtual string NodeLeftHTMLBreadCrumbRoot //New
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeLeftHTMLBreadCrumbSub //New
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeLeftHTMLRoot //RootMenuItemLeftHtml
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeLeftHTMLSub //SubMenuItemLeftHtml
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbRoot //New
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbSub //New
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLRoot //RootMenuItemRightHtml
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLSub //SubMenuItemRightHtml
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string PathImage
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string PathSystemImage
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string PathSystemScript
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual bool PopulateNodesFromClient
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        //Public MustOverride Property Moveable() As String		  'Moveable

        public virtual string SeparatorHTML //Separator
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorLeftHTML //LeftSeparator
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorLeftHTMLActive //LeftSeparatorActive
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorLeftHTMLBreadCrumb //LeftSeparatorBreadCrumb
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorRightHTML //RightSeparator
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorRightHTMLActive //RightSeparatorActive
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string SeparatorRightHTMLBreadCrumb //RightSeparatorBreadCrumb
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleBackColor //BackColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual decimal StyleBorderWidth //MenuBorderWidth
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public virtual decimal StyleControlHeight //MenuBarHeight
        {
            get
            {
                return 25;
            }
            set
            {
            }
        }

        public virtual string StyleFontBold //FontBold
        {
            get
            {
                return "False";
            }
            set
            {
            }
        }

        public virtual string StyleFontNames //FontNames
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual decimal StyleFontSize //FontSize
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public virtual string StyleForeColor //ForeColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleHighlightColor //HighlightColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleIconBackColor //IconBackgroundColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual decimal StyleIconWidth //IconWidth
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public virtual decimal StyleNodeHeight //MenuItemHeight
        {
            get
            {
                return 25;
            }
            set
            {
            }
        }

        public virtual string StyleRoot //For action menu backwards compatibility
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleSelectionBorderColor //SelectedBorderColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleSelectionColor //SelectedColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleSelectionForeColor //SelectedForeColor
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public virtual string StyleSub //For action menu backwards compatibility (actually this is new, but since we needed the root...)
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public abstract bool SupportsPopulateOnDemand { get; }

        public virtual string WorkImage
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public static NavigationProvider Instance( string FriendlyName )
        {
            return ( (NavigationProvider)Reflection.CreateObject( "navigationControl", FriendlyName, "", "" ) );
        }

        public abstract void Bind( DNNNodeCollection objNodes );

        public virtual void ClearNodes()
        {
        }

        //UseSkinPathArrowImages		'skin object will toggle ImageDirectory based on value
        //UseRootBreadCrumbArrow		'skin object will assign NodeLeftHTMLBreadCrumbRoot
        //UseSubMenuBreadCrumbArrow	'skin object will assign NodeLeftHTMLBreadCrumb
        //DownArrow					'skin object will assign IndicateChildImage/IndicateChildImageRoot
        //RightArrow					'skin object will assign IndicateChildImage/IndicateChildImageRoot
        //Tooltip					'skin object decides whether to populate tooltips...  maybe need to fix in navigation.vb class...
        //ClearDefaults				'skin object decides if defaults should be populated
        //DelaySubmenuLoad			'should no longer be necessary (was for Operation Aborted error)

        // Methods
        public abstract void Initialize();

        protected void RaiseEvent_NodeClick( DNNNode objNode )
        {
            if( NodeClickEvent != null )
            {
                NodeClickEvent( new NavigationEventArgs( objNode.ID, objNode ) );
            }
        }

        protected void RaiseEvent_NodeClick( string strID )
        {
            if( NodeClickEvent != null ) //DotNetNuke.UI.Navigation.GetNavigationNode(strID, Me.ControlID))
            {
                NodeClickEvent( new NavigationEventArgs( strID, null ) );
            }
        }

        protected void RaiseEvent_PopulateOnDemand( DNNNode objNode )
        {
            if( PopulateOnDemandEvent != null )
            {
                PopulateOnDemandEvent( new NavigationEventArgs( objNode.ID, objNode ) );
            }
        }

        protected void RaiseEvent_PopulateOnDemand( string strID )
        {
            //RaiseEvent_PopulateOnDemand(DotNetNuke.UI.Navigation.GetNavigationNode(strID, Me.ControlID))
            if( PopulateOnDemandEvent != null )
            {
                PopulateOnDemandEvent( new NavigationEventArgs( strID, null ) );
            }
        }
    }
}