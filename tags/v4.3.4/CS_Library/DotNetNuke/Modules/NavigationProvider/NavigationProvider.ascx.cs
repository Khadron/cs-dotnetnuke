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

        /// <summary>
        /// MenuAlignment
        /// </summary>
        public virtual Alignment ControlAlignment 
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

        /// <summary>
        /// Display
        /// </summary>
        public virtual Orientation ControlOrientation
        {
            get
            {
                return Orientation.Horizontal;
            }
            set
            {
            }
        }
       
        /// <summary>
        /// RootMenuItemBreadCrumbCssClass
        /// </summary>
        public virtual string CSSBreadCrumbRoot
        {
            get
            {
                return String.Empty;
            }
            set
            {
                
            }
        }

        /// <summary>
        /// SubMenuItemBreadCrumbCssClass
        /// </summary>
        public virtual string CSSBreadCrumbSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuBreakCssClass
        /// </summary>
        public virtual string CSSBreak 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuContainerCssClass
        /// </summary>
        public virtual string CSSContainerRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SubMenuCssClass
        /// </summary>
        public virtual string CSSContainerSub 
        {
            get
            {
                return String.Empty;
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

        /// <summary>
        /// MenuBarCSSClass
        /// </summary>
        public abstract string CSSControl 
        { 
            get; set; 
        }

        /// <summary>
        /// MenuIconCssClass
        /// </summary>
        public virtual string CSSIcon 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuRootArrowCssClass
        /// </summary>
        public virtual string CSSIndicateChildRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuArrowCssClass
        /// </summary>
        public virtual string CSSIndicateChildSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparatorCssClass
        /// </summary>
        public virtual string CSSLeftSeparator 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparatorCssClassBreadCrumb
        /// </summary>
        public virtual string CSSLeftSeparatorBreadCrumb 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparatorActiveCssClass
        /// </summary>
        public virtual string CSSLeftSeparatorSelection 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuItemCssClass
        /// </summary>
        public virtual string CSSNode 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuItemSelCssClass
        /// </summary>
        public virtual string CSSNodeHover 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RootMenuItemSelectedCssClass
        /// </summary>
        public virtual string CSSNodeHoverRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SubMenuItemSelectedCssClass
        /// </summary>
        public virtual string CSSNodeHoverSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RootMenuItemCssClass
        /// </summary>
        public virtual string CSSNodeRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RootMenuItemActiveCssClass
        /// </summary>
        public virtual string CSSNodeSelectedRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SubMenuItemActiveCssClass
        /// </summary>
        public virtual string CSSNodeSelectedSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparatorCssClass
        /// </summary>
        public virtual string CSSRightSeparator 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparatorCssClassBreadCrumb
        /// </summary>
        public virtual string CSSRightSeparatorBreadCrumb 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparatorActiveCssClass
        /// </summary>
        public virtual string CSSRightSeparatorSelection 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SeparatorCssClass
        /// </summary>
        public virtual string CSSSeparator 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsMenuTransitionLength
        /// </summary>
        public virtual double EffectsDuration 
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsShadowColor
        /// </summary>
        public virtual string EffectsShadowColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsShadowDirection
        /// </summary>
        public virtual string EffectsShadowDirection 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsShadowStrength
        /// </summary>
        public virtual int EffectsShadowStrength 
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsStyle
        /// </summary>
        public virtual string EffectsStyle 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsTransition
        /// </summary>
        public virtual string EffectsTransition 
        {
            get
            {
                return "'";
            }
            set
            {
            }
        }

        /// <summary>
        /// ForceFullMenuList
        /// </summary>
        public virtual string ForceCrawlerDisplay 
        {
            get
            {
                return "false";
            }
            set
            {
            }
        }

        /// <summary>
        /// ForceDownLevel
        /// </summary>
        public virtual string ForceDownLevel 
        {
            get
            {
                return false.ToString();
            }
            set
            {
            }
        }

        /// <summary>
        /// for tree
        /// </summary>
        public virtual string IndicateChildImageExpandedRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// for tree
        /// </summary>
        public virtual string IndicateChildImageExpandedSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageRoot
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string IndicateChildImageSub
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// UseIndicateChilds
        /// </summary>
        public virtual bool IndicateChildren 
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// MouseOutHideDelay
        /// </summary>
        public virtual decimal MouseOutHideDelay //MenuEffectsMouseOutHideDelay, 
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsMouseOverExpand
        /// </summary>
        public virtual HoverAction MouseOverAction 
        {
            get
            {
                return HoverAction.Expand;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuEffectsMouseOverDisplay
        /// </summary>
        public virtual HoverDisplay MouseOverDisplay 
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

        public virtual string NodeLeftHTMLBreadCrumbRoot
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string NodeLeftHTMLBreadCrumbSub
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RootMenuItemLeftHtml
        /// </summary>
        public virtual string NodeLeftHTMLRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SubMenuItemLeftHtml
        /// </summary>
        public virtual string NodeLeftHTMLSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RootMenuItemRightHtml
        /// </summary>
        public virtual string NodeRightHTMLRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SubMenuItemRightHtml
        /// </summary>
        public virtual string NodeRightHTMLSub 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string PathImage
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string PathSystemImage
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        public virtual string PathSystemScript
        {
            get
            {
                return String.Empty;
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

        /// <summary>
        /// Separator
        /// </summary>
        public virtual string SeparatorHTML 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparator
        /// </summary>
        public virtual string SeparatorLeftHTML 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparatorActive
        /// </summary>
        public virtual string SeparatorLeftHTMLActive 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// LeftSeparatorBreadCrumb
        /// </summary>
        public virtual string SeparatorLeftHTMLBreadCrumb 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparator
        /// </summary>
        public virtual string SeparatorRightHTML 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparatorActive
        /// </summary>
        public virtual string SeparatorRightHTMLActive 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// RightSeparatorBreadCrumb
        /// </summary>
        public virtual string SeparatorRightHTMLBreadCrumb 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// BackColor
        /// </summary>
        public virtual string StyleBackColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuBorderWidth
        /// </summary>
        public virtual decimal StyleBorderWidth 
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuBarHeight
        /// </summary>
        public virtual decimal StyleControlHeight 
        {
            get
            {
                return 25;
            }
            set
            {
            }
        }

        /// <summary>
        /// FontBold
        /// </summary>
        public virtual string StyleFontBold 
        {
            get
            {
                return "false";
            }
            set
            {
            }
        }

        /// <summary>
        /// FontNames
        /// </summary>
        public virtual string StyleFontNames 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// FontSize
        /// </summary>
        public virtual decimal StyleFontSize 
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// ForeColor
        /// </summary>
        public virtual string StyleForeColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// HighlightColor
        /// </summary>
        public virtual string StyleHighlightColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// IconBackgroundColor
        /// </summary>
        public virtual string StyleIconBackColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// IconWidth
        /// </summary>
        public virtual decimal StyleIconWidth 
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// MenuItemHeight
        /// </summary>
        public virtual decimal StyleNodeHeight 
        {
            get
            {
                return 25;
            }
            set
            {
            }
        }

        /// <summary>
        /// For action menu backwards compatibility
        /// </summary>
        public virtual string StyleRoot 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SelectedBorderColor
        /// </summary>
        public virtual string StyleSelectionBorderColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SelectedColor
        /// </summary>
        public virtual string StyleSelectionColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// SelectedForeColor
        /// </summary>
        public virtual string StyleSelectionForeColor 
        {
            get
            {
                return String.Empty;
            }
            set
            {
            }
        }

        /// <summary>
        /// For action menu backwards compatibility (actually this is new, but since we needed the root...)
        /// </summary>
        public virtual string StyleSub 
        {
            get
            {
                return String.Empty;
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
                return String.Empty;
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