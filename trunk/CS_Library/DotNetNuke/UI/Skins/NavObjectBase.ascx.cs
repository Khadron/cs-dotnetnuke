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
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.UI.WebControls;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins
{
    public class NavObjectBase : SkinObjectBase
    {
        private bool m_blnPopulateNodesFromClient = true; //JH - POD
        private int m_intExpandDepth = -1; //JH - POD
        private int m_intStartTabId = -1;
        private NavigationProvider m_objControl;
        private string m_strControlAlignment;
        private string m_strControlOrientation;
        private string m_strCSSBreadCrumbRoot;
        private string m_strCSSBreadCrumbSub;
        private string m_strCSSBreak;
        private string m_strCSSContainerRoot;
        private string m_strCSSContainerSub;
        private string m_strCSSControl;
        private string m_strCSSIcon;
        private string m_strCSSIndicateChildRoot;
        private string m_strCSSIndicateChildSub;
        private string m_strCSSLeftSeparator;
        private string m_strCSSLeftSeparatorBreadCrumb;
        private string m_strCSSLeftSeparatorSelection;
        private string m_strCSSNode;
        private string m_strCSSNodeHover;
        private string m_strCSSNodeHoverRoot;
        private string m_strCSSNodeHoverSub;
        private string m_strCSSNodeRoot;
        private string m_strCSSNodeSelectedRoot;
        private string m_strCSSNodeSelectedSub;
        private string m_strCSSRightSeparator;
        private string m_strCSSRightSeparatorBreadCrumb;
        private string m_strCSSRightSeparatorSelection;
        private string m_strCSSSeparator;
        private string m_strEffectsDuration;
        private string m_strEffectsShadowColor;
        private string m_strEffectsShadowDirection;
        private string m_strEffectsShadowStrength;
        private string m_strEffectsStyle;
        private string m_strEffectsTransition;
        private string m_strForceCrawlerDisplay;
        private string m_strForceDownLevel;
        private string m_strIndicateChildImageExpandedRoot;
        private string m_strIndicateChildImageExpandedSub;
        private string m_strIndicateChildImageRoot;
        private string m_strIndicateChildImageSub;
        private string m_strIndicateChildren;
        private string m_strLevel;
        private string m_strMouseOutHideDelay;
        private string m_strMouseOverAction;
        private string m_strMouseOverDisplay;
        private string m_strNodeLeftHTMLBreadCrumbRoot;
        private string m_strNodeLeftHTMLBreadCrumbSub;
        private string m_strNodeLeftHTMLRoot;
        private string m_strNodeLeftHTMLSub;
        private string m_strNodeRightHTMLBreadCrumbRoot;
        private string m_strNodeRightHTMLBreadCrumbSub;
        private string m_strNodeRightHTMLRoot;
        private string m_strNodeRightHTMLSub;
        private string m_strPathImage;
        private string m_strPathSystemImage;
        private string m_strPathSystemScript;
        private string m_strProviderName;
        private string m_strSeparatorHTML;
        private string m_strSeparatorLeftHTML;
        private string m_strSeparatorLeftHTMLActive;
        private string m_strSeparatorLeftHTMLBreadCrumb;
        private string m_strSeparatorRightHTML;
        private string m_strSeparatorRightHTMLActive;
        private string m_strSeparatorRightHTMLBreadCrumb;
        private string m_strStyleBackColor;
        private string m_strStyleBorderWidth;
        private string m_strStyleControlHeight;
        private string m_strStyleFontBold;
        private string m_strStyleFontNames;
        private string m_strStyleFontSize;
        private string m_strStyleForeColor;
        private string m_strStyleHighlightColor;
        private string m_strStyleIconBackColor;
        private string m_strStyleIconWidth;
        private string m_strStyleNodeHeight;
        private string m_strStyleSelectionBorderColor;
        private string m_strStyleSelectionColor;
        private string m_strStyleSelectionForeColor;
        private string m_strToolTip;
        private string m_strWorkImage;

        protected NavigationProvider Control
        {
            get
            {
                return this.m_objControl;
            }
        }

        public string ControlAlignment
        {
            get
            {
                string string2 = "";
                if( this.Control == null )
                {
                    return this.m_strControlAlignment;
                }
                switch( this.Control.ControlAlignment )
                {
                    case NavigationProvider.Alignment.Left:
                        {
                            return "Left";
                        }
                    case NavigationProvider.Alignment.Right:
                        {
                            return "Right";
                        }
                    case NavigationProvider.Alignment.Center:
                        {
                            return "Center";
                        }
                    case NavigationProvider.Alignment.Justify:
                        {
                            return "Justify";
                        }
                }
                return string2;
            }
            set
            {
                if( Control == null )
                {
                    m_strControlAlignment = value;
                }
                else
                {
                    switch( value.ToLower() )
                    {
                        case "left":

                            Control.ControlAlignment = NavigationProvider.Alignment.Left;
                            break;
                        case "right":

                            Control.ControlAlignment = NavigationProvider.Alignment.Right;
                            break;
                        case "center":

                            Control.ControlAlignment = NavigationProvider.Alignment.Center;
                            break;
                        case "justify":

                            Control.ControlAlignment = NavigationProvider.Alignment.Justify;
                            break;
                    }
                }
            }
        }

        public string ControlOrientation
        {
            get
            {
                string retvalue = "";
                if( Control == null )
                {
                    retvalue = m_strControlOrientation;
                }
                else
                {
                    switch( Control.ControlOrientation )
                    {
                        case NavigationProvider.Orientation.Horizontal:

                            retvalue = "Horizontal";
                            break;
                        case NavigationProvider.Orientation.Vertical:

                            retvalue = "Vertical";
                            break;
                    }
                }
                return retvalue;
            }
            set
            {
                if( Control == null )
                {
                    m_strControlOrientation = value;
                }
                else
                {
                    switch( value.ToLower() )
                    {
                        case "horizontal":

                            Control.ControlOrientation = NavigationProvider.Orientation.Horizontal;
                            break;
                        case "vertical":

                            Control.ControlOrientation = NavigationProvider.Orientation.Vertical;
                            break;
                    }
                }
            }
        }

        public string CSSBreadCrumbRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSBreadCrumbRoot;
                }
                else
                {
                    return Control.CSSBreadCrumbRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSBreadCrumbRoot = value;
                }
                else
                {
                    Control.CSSBreadCrumbRoot = value;
                }
            }
        }

        public string CSSBreadCrumbSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSBreadCrumbSub;
                }
                else
                {
                    return Control.CSSBreadCrumbSub;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSBreadCrumbSub = value;
                }
                else
                {
                    Control.CSSBreadCrumbSub = value;
                }
            }
        }

        public string CSSBreak
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSBreak;
                }
                else
                {
                    return Control.CSSBreak;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSBreak = value;
                }
                else
                {
                    Control.CSSBreak = value;
                }
            }
        }

        public string CSSContainerRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSContainerRoot;
                }
                else
                {
                    return Control.CSSContainerRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSContainerRoot = value;
                }
                else
                {
                    Control.CSSContainerRoot = value;
                }
            }
        }

        public string CSSContainerSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSContainerSub;
                }
                else
                {
                    return Control.CSSContainerSub;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSContainerSub = value;
                }
                else
                {
                    Control.CSSContainerSub = value;
                }
            }
        }

        public string CSSControl
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSControl;
                }
                else
                {
                    return Control.CSSControl;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSControl = value;
                }
                else
                {
                    Control.CSSControl = value;
                }
            }
        }

        public string CSSIcon
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSIcon;
                }
                else
                {
                    return Control.CSSIcon;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSIcon = value;
                }
                else
                {
                    Control.CSSIcon = value;
                }
            }
        }

        public string CSSIndicateChildRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSIndicateChildRoot;
                }
                else
                {
                    return Control.CSSIndicateChildRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSIndicateChildRoot = value;
                }
                else
                {
                    Control.CSSIndicateChildRoot = value;
                }
            }
        }

        public string CSSIndicateChildSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSIndicateChildSub;
                }
                else
                {
                    return Control.CSSIndicateChildSub;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSIndicateChildSub = value;
                }
                else
                {
                    Control.CSSIndicateChildSub = value;
                }
            }
        }

        public string CSSLeftSeparator
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSLeftSeparator;
                }
                else
                {
                    return Control.CSSLeftSeparator;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSLeftSeparator = value;
                }
                else
                {
                    Control.CSSLeftSeparator = value;
                }
            }
        }

        public string CSSLeftSeparatorBreadCrumb
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSLeftSeparatorBreadCrumb;
                }
                else
                {
                    return Control.CSSLeftSeparatorBreadCrumb;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSLeftSeparatorBreadCrumb = value;
                }
                else
                {
                    Control.CSSLeftSeparatorBreadCrumb = value;
                }
            }
        }

        public string CSSLeftSeparatorSelection
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSLeftSeparatorSelection;
                }
                else
                {
                    return Control.CSSLeftSeparatorSelection;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSLeftSeparatorSelection = value;
                }
                else
                {
                    Control.CSSLeftSeparatorSelection = value;
                }
            }
        }

        public string CSSNode
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNode;
                }
                else
                {
                    return Control.CSSNode;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNode = value;
                }
                else
                {
                    Control.CSSNode = value;
                }
            }
        }

        public string CSSNodeHover
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeHover;
                }
                else
                {
                    return Control.CSSNodeHover;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeHover = value;
                }
                else
                {
                    Control.CSSNodeHover = value;
                }
            }
        }

        public string CSSNodeHoverRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeHoverRoot;
                }
                else
                {
                    return Control.CSSNodeHoverRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeHoverRoot = value;
                }
                else
                {
                    Control.CSSNodeHoverRoot = value;
                }
            }
        }

        public string CSSNodeHoverSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeHoverSub;
                }
                else
                {
                    return Control.CSSNodeHoverSub;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeHoverSub = value;
                }
                else
                {
                    Control.CSSNodeHoverSub = value;
                }
            }
        }

        public string CSSNodeRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeRoot;
                }
                else
                {
                    return Control.CSSNodeRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeRoot = value;
                }
                else
                {
                    Control.CSSNodeRoot = value;
                }
            }
        }

        public string CSSNodeSelectedRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeSelectedRoot;
                }
                else
                {
                    return Control.CSSNodeSelectedRoot;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeSelectedRoot = value;
                }
                else
                {
                    Control.CSSNodeSelectedRoot = value;
                }
            }
        }

        public string CSSNodeSelectedSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSNodeSelectedSub;
                }
                else
                {
                    return Control.CSSNodeSelectedSub;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSNodeSelectedSub = value;
                }
                else
                {
                    Control.CSSNodeSelectedSub = value;
                }
            }
        }

        public string CSSRightSeparator
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSRightSeparator;
                }
                else
                {
                    return Control.CSSRightSeparator;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSRightSeparator = value;
                }
                else
                {
                    Control.CSSRightSeparator = value;
                }
            }
        }

        public string CSSRightSeparatorBreadCrumb
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSRightSeparatorBreadCrumb;
                }
                else
                {
                    return Control.CSSRightSeparatorBreadCrumb;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSRightSeparatorBreadCrumb = value;
                }
                else
                {
                    Control.CSSRightSeparatorBreadCrumb = value;
                }
            }
        }

        public string CSSRightSeparatorSelection
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSRightSeparatorSelection;
                }
                else
                {
                    return Control.CSSRightSeparatorSelection;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSRightSeparatorSelection = value;
                }
                else
                {
                    Control.CSSRightSeparatorSelection = value;
                }
            }
        }

        public string CSSSeparator
        {
            get
            {
                if( Control == null )
                {
                    return m_strCSSSeparator;
                }
                else
                {
                    return Control.CSSSeparator;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strCSSSeparator = value;
                }
                else
                {
                    Control.CSSSeparator = value;
                }
            }
        }

        public string EffectsDuration
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsDuration;
                }
                else
                {
                    return Control.EffectsDuration.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsDuration = value;
                }
                else
                {
                    Control.EffectsDuration = Convert.ToDouble( value );
                }
            }
        }

        public string EffectsShadowColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsShadowColor;
                }
                else
                {
                    return Control.EffectsShadowColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsShadowColor = value;
                }
                else
                {
                    Control.EffectsShadowColor = value;
                }
            }
        }

        public string EffectsShadowDirection
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsShadowDirection;
                }
                else
                {
                    return Control.EffectsShadowDirection;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsShadowDirection = value;
                }
                else
                {
                    Control.EffectsShadowDirection = value;
                }
            }
        }

        public string EffectsShadowStrength
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsShadowStrength;
                }
                else
                {
                    return Control.EffectsShadowStrength.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsShadowStrength = value;
                }
                else
                {
                    Control.EffectsShadowStrength = Convert.ToInt32( value );
                }
            }
        }

        public string EffectsStyle
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsStyle;
                }
                else
                {
                    return Control.EffectsStyle;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsStyle = value;
                }
                else
                {
                    Control.EffectsStyle = value;
                }
            }
        }

        public string EffectsTransition
        {
            get
            {
                if( Control == null )
                {
                    return m_strEffectsTransition;
                }
                else
                {
                    return Control.EffectsTransition;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strEffectsTransition = value;
                }
                else
                {
                    Control.EffectsTransition = value;
                }
            }
        }

        public int ExpandDepth //JH - POD
        {
            get
            {
                return m_intExpandDepth;
            }
            set
            {
                m_intExpandDepth = value;
            }
        }

        public string ForceCrawlerDisplay
        {
            get
            {
                if( Control == null )
                {
                    return m_strForceCrawlerDisplay;
                }
                else
                {
                    return Control.ForceCrawlerDisplay;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strForceCrawlerDisplay = value;
                }
                else
                {
                    Control.ForceCrawlerDisplay = value;
                }
            }
        }

        public string ForceDownLevel
        {
            get
            {
                if( Control == null )
                {
                    return m_strForceDownLevel;
                }
                else
                {
                    return Control.ForceDownLevel;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strForceDownLevel = value;
                }
                else
                {
                    Control.ForceDownLevel = value;
                }
            }
        }

        public string IndicateChildImageExpandedRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strIndicateChildImageExpandedRoot;
                }
                else
                {
                    return Control.IndicateChildImageExpandedRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strIndicateChildImageExpandedRoot = value;
                }
                else
                {
                    Control.IndicateChildImageExpandedRoot = value;
                }
            }
        }

        public string IndicateChildImageExpandedSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strIndicateChildImageExpandedSub;
                }
                else
                {
                    return Control.IndicateChildImageExpandedSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strIndicateChildImageExpandedSub = value;
                }
                else
                {
                    Control.IndicateChildImageExpandedSub = value;
                }
            }
        }

        public string IndicateChildImageRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strIndicateChildImageRoot;
                }
                else
                {
                    return Control.IndicateChildImageRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strIndicateChildImageRoot = value;
                }
                else
                {
                    Control.IndicateChildImageRoot = value;
                }
            }
        }

        public string IndicateChildImageSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strIndicateChildImageSub;
                }
                else
                {
                    return Control.IndicateChildImageSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strIndicateChildImageSub = value;
                }
                else
                {
                    Control.IndicateChildImageSub = value;
                }
            }
        }

        public string IndicateChildren
        {
            get
            {
                if( Control == null )
                {
                    return m_strIndicateChildren;
                }
                else
                {
                    return Control.IndicateChildren.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strIndicateChildren = value;
                }
                else
                {
                    Control.IndicateChildren = Convert.ToBoolean( value );
                }
            }
        }

        public string Level
        {
            get
            {
                if (m_strLevel != null) return m_strLevel; else return String.Empty;
            }
            set
            {
                m_strLevel = value;
            }
        }

        public string MouseOutHideDelay
        {
            get
            {
                if( Control == null )
                {
                    return m_strMouseOutHideDelay;
                }
                else
                {
                    return Control.MouseOutHideDelay.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strMouseOutHideDelay = value;
                }
                else
                {
                    Control.MouseOutHideDelay = Convert.ToDecimal( value );
                }
            }
        }

        public string MouseOverAction
        {
            get
            {
                string retvalue = "";
                if( Control == null )
                {
                    retvalue = m_strMouseOverAction;
                }
                else
                {
                    switch( Control.MouseOverAction )
                    {
                        case NavigationProvider.HoverAction.Expand:

                            retvalue = "True";
                            break;
                        case NavigationProvider.HoverAction.None:

                            retvalue = "False";
                            break;
                    }
                }
                return retvalue;
            }
            set
            {
                if( Control == null )
                {
                    m_strMouseOverAction = value;
                }
                else
                {
                    if( Convert.ToBoolean( value ) == true )
                    {
                        Control.MouseOverAction = NavigationProvider.HoverAction.Expand;
                    }
                    else
                    {
                        Control.MouseOverAction = NavigationProvider.HoverAction.None;
                    }
                }
            }
        }

        public string MouseOverDisplay
        {
            get
            {
                string retvalue = "";
                if( Control == null )
                {
                    retvalue = m_strMouseOverDisplay;
                }
                else
                {
                    switch( Control.MouseOverDisplay )
                    {
                        case NavigationProvider.HoverDisplay.Highlight:

                            retvalue = "Highlight";
                            break;
                        case NavigationProvider.HoverDisplay.None:

                            retvalue = "None";
                            break;
                        case NavigationProvider.HoverDisplay.Outset:

                            retvalue = "Outset";
                            break;
                    }
                }
                return retvalue;
            }
            set
            {
                if( Control == null )
                {
                    m_strMouseOverDisplay = value;
                }
                else
                {
                    switch( value.ToLower() )
                    {
                        case "highlight":

                            Control.MouseOverDisplay = NavigationProvider.HoverDisplay.Highlight;
                            break;
                        case "outset":

                            Control.MouseOverDisplay = NavigationProvider.HoverDisplay.Outset;
                            break;
                        case "none":

                            Control.MouseOverDisplay = NavigationProvider.HoverDisplay.None;
                            break;
                    }
                }
            }
        }

        public string NodeLeftHTMLBreadCrumbRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeLeftHTMLBreadCrumbRoot;
                }
                else
                {
                    return Control.NodeLeftHTMLBreadCrumbRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeLeftHTMLBreadCrumbRoot = value;
                }
                else
                {
                    Control.NodeLeftHTMLBreadCrumbRoot = value;
                }
            }
        }

        public string NodeLeftHTMLBreadCrumbSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeLeftHTMLBreadCrumbSub;
                }
                else
                {
                    return Control.NodeLeftHTMLBreadCrumbSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeLeftHTMLBreadCrumbSub = value;
                }
                else
                {
                    Control.NodeLeftHTMLBreadCrumbSub = value;
                }
            }
        }

        public string NodeLeftHTMLRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeLeftHTMLRoot;
                }
                else
                {
                    return Control.NodeLeftHTMLRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeLeftHTMLRoot = value;
                }
                else
                {
                    Control.NodeLeftHTMLRoot = value;
                }
            }
        }

        public string NodeLeftHTMLSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeLeftHTMLSub;
                }
                else
                {
                    return Control.NodeLeftHTMLSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeLeftHTMLSub = value;
                }
                else
                {
                    Control.NodeLeftHTMLSub = value;
                }
            }
        }

        public string NodeRightHTMLBreadCrumbRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeRightHTMLBreadCrumbRoot;
                }
                else
                {
                    return Control.NodeRightHTMLBreadCrumbRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeRightHTMLBreadCrumbRoot = value;
                }
                else
                {
                    Control.NodeRightHTMLBreadCrumbRoot = value;
                }
            }
        }

        public string NodeRightHTMLBreadCrumbSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeRightHTMLBreadCrumbSub;
                }
                else
                {
                    return Control.NodeRightHTMLBreadCrumbSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeRightHTMLBreadCrumbSub = value;
                }
                else
                {
                    Control.NodeRightHTMLBreadCrumbSub = value;
                }
            }
        }

        public string NodeRightHTMLRoot
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeRightHTMLRoot;
                }
                else
                {
                    return Control.NodeRightHTMLRoot;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeRightHTMLRoot = value;
                }
                else
                {
                    Control.NodeRightHTMLRoot = value;
                }
            }
        }

        public string NodeRightHTMLSub
        {
            get
            {
                if( Control == null )
                {
                    return m_strNodeRightHTMLSub;
                }
                else
                {
                    return Control.NodeRightHTMLSub;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strNodeRightHTMLSub = value;
                }
                else
                {
                    Control.NodeRightHTMLSub = value;
                }
            }
        }

        public string PathImage
        {
            get
            {
                if( Control == null )
                {
                    return m_strPathImage;
                }
                else
                {
                    return Control.PathImage;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strPathImage = value;
                }
                else
                {
                    Control.PathImage = value;
                }
            }
        }

        public string PathSystemImage
        {
            get
            {
                if( Control == null )
                {
                    return m_strPathSystemImage;
                }
                else
                {
                    return Control.PathSystemImage;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strPathSystemImage = value;
                }
                else
                {
                    Control.PathSystemImage = value;
                }
            }
        }

        public string PathSystemScript
        {
            get
            {
                if( Control == null )
                {
                    return m_strPathSystemScript;
                }
                else
                {
                    return Control.PathSystemScript;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strPathSystemScript = value;
                }
                else
                {
                    Control.PathSystemScript = value;
                }
            }
        }

        public bool PopulateNodesFromClient
        {
            get
            {
                return m_blnPopulateNodesFromClient;
            }
            set
            {
                m_blnPopulateNodesFromClient = value;
            }
        }

        public string ProviderName
        {
            get
            {
                return m_strProviderName;
            }
            set
            {
                m_strProviderName = value;
            }
        }

        public string SeparatorHTML
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorHTML;
                }
                else
                {
                    return Control.SeparatorHTML;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorHTML = value;
                }
                else
                {
                    Control.SeparatorHTML = value;
                }
            }
        }

        public string SeparatorLeftHTML
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorLeftHTML;
                }
                else
                {
                    return Control.SeparatorLeftHTML;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorLeftHTML = value;
                }
                else
                {
                    Control.SeparatorLeftHTML = value;
                }
            }
        }

        public string SeparatorLeftHTMLActive
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorLeftHTMLActive;
                }
                else
                {
                    return Control.SeparatorLeftHTMLActive;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorLeftHTMLActive = value;
                }
                else
                {
                    Control.SeparatorLeftHTMLActive = value;
                }
            }
        }

        public string SeparatorLeftHTMLBreadCrumb
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorLeftHTMLBreadCrumb;
                }
                else
                {
                    return Control.SeparatorLeftHTMLBreadCrumb;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorLeftHTMLBreadCrumb = value;
                }
                else
                {
                    Control.SeparatorLeftHTMLBreadCrumb = value;
                }
            }
        }

        public string SeparatorRightHTML
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorRightHTML;
                }
                else
                {
                    return Control.SeparatorRightHTML;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorRightHTML = value;
                }
                else
                {
                    Control.SeparatorRightHTML = value;
                }
            }
        }

        public string SeparatorRightHTMLActive
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorRightHTMLActive;
                }
                else
                {
                    return Control.SeparatorRightHTMLActive;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorRightHTMLActive = value;
                }
                else
                {
                    Control.SeparatorRightHTMLActive = value;
                }
            }
        }

        public string SeparatorRightHTMLBreadCrumb
        {
            get
            {
                if( Control == null )
                {
                    return m_strSeparatorRightHTMLBreadCrumb;
                }
                else
                {
                    return Control.SeparatorRightHTMLBreadCrumb;
                }
            }
            set
            {
                value = GetPath( value );
                if( Control == null )
                {
                    m_strSeparatorRightHTMLBreadCrumb = value;
                }
                else
                {
                    Control.SeparatorRightHTMLBreadCrumb = value;
                }
            }
        }

        public int StartTabId
        {
            get
            {
                return m_intStartTabId;
            }
            set
            {
                m_intStartTabId = value;
            }
        }

        public string StyleBackColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleBackColor;
                }
                else
                {
                    return Control.StyleBackColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleBackColor = value;
                }
                else
                {
                    Control.StyleBackColor = value;
                }
            }
        }

        public string StyleBorderWidth
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleBorderWidth;
                }
                else
                {
                    return Control.StyleBorderWidth.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleBorderWidth = value;
                }
                else
                {
                    Control.StyleBorderWidth = Convert.ToDecimal( value );
                }
            }
        }

        public string StyleControlHeight
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleControlHeight;
                }
                else
                {
                    return Control.StyleControlHeight.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleControlHeight = value;
                }
                else
                {
                    Control.StyleControlHeight = Convert.ToDecimal( value );
                }
            }
        }

        public string StyleFontBold
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleFontBold;
                }
                else
                {
                    return Control.StyleFontBold;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleFontBold = value;
                }
                else
                {
                    Control.StyleFontBold = value;
                }
            }
        }

        public string StyleFontNames
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleFontNames;
                }
                else
                {
                    return Control.StyleFontNames;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleFontNames = value;
                }
                else
                {
                    Control.StyleFontNames = value;
                }
            }
        }

        public string StyleFontSize
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleFontSize;
                }
                else
                {
                    return Control.StyleFontSize.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleFontSize = value;
                }
                else
                {
                    Control.StyleFontSize = Convert.ToDecimal( value );
                }
            }
        }

        public string StyleForeColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleForeColor;
                }
                else
                {
                    return Control.StyleForeColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleForeColor = value;
                }
                else
                {
                    Control.StyleForeColor = value;
                }
            }
        }

        public string StyleHighlightColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleHighlightColor;
                }
                else
                {
                    return Control.StyleHighlightColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleHighlightColor = value;
                }
                else
                {
                    Control.StyleHighlightColor = value;
                }
            }
        }

        public string StyleIconBackColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleIconBackColor;
                }
                else
                {
                    return Control.StyleIconBackColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleIconBackColor = value;
                }
                else
                {
                    Control.StyleIconBackColor = value;
                }
            }
        }

        public string StyleIconWidth
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleIconWidth;
                }
                else
                {
                    return Control.StyleIconWidth.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleIconWidth = value;
                }
                else
                {
                    Control.StyleIconWidth = Convert.ToDecimal( value );
                }
            }
        }

        public string StyleNodeHeight
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleNodeHeight;
                }
                else
                {
                    return Control.StyleNodeHeight.ToString();
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleNodeHeight = value;
                }
                else
                {
                    Control.StyleNodeHeight = Convert.ToDecimal( value );
                }
            }
        }

        public string StyleSelectionBorderColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleSelectionBorderColor;
                }
                else
                {
                    return Control.StyleSelectionBorderColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleSelectionBorderColor = value;
                }
                else
                {
                    Control.StyleSelectionBorderColor = value;
                }
            }
        }

        public string StyleSelectionColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleSelectionColor;
                }
                else
                {
                    return Control.StyleSelectionColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleSelectionColor = value;
                }
                else
                {
                    Control.StyleSelectionColor = value;
                }
            }
        }

        public string StyleSelectionForeColor
        {
            get
            {
                if( Control == null )
                {
                    return m_strStyleSelectionForeColor;
                }
                else
                {
                    return Control.StyleSelectionForeColor;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strStyleSelectionForeColor = value;
                }
                else
                {
                    Control.StyleSelectionForeColor = value;
                }
            }
        }

        //Public Property RootOnly() As String
        //	Get
        //		Return m_strRootOnly
        //	End Get
        //	Set(ByVal value As String)
        //		m_strRootOnly = value
        //	End Set
        //End Property

        public string ToolTip
        {
            get
            {
                if (m_strToolTip != null) return m_strToolTip; else return String.Empty;
            }
            set
            {
                m_strToolTip = value;
            }
        }

        public string WorkImage
        {
            get
            {
                if( Control == null )
                {
                    return m_strWorkImage;
                }
                else
                {
                    return Control.WorkImage;
                }
            }
            set
            {
                if( Control == null )
                {
                    m_strWorkImage = value;
                }
                else
                {
                    Control.WorkImage = value;
                }
            }
        }

        public NavObjectBase()
        {
            this.m_blnPopulateNodesFromClient = true;
            this.m_intExpandDepth = -1;
            this.m_intStartTabId = -1;
            this.m_strProviderName = "";
        }

        public DNNNodeCollection GetNavigationNodes( DNNNode objNode )
        {
            int intRootParent = PortalSettings.ActiveTab.TabID;
            DNNNodeCollection objNodes;
            Navigation.ToolTipSource eToolTips;
            int intNavNodeOptions = 0;
            //Dim blnRootOnly As Boolean = Boolean.Parse(Getvalue(RootOnly, "False"))
            int intDepth = ExpandDepth;

            //This setting indicates the root level for the menu
            if( Level.ToLower() == "child" )
            {
            }
            else if( Level.ToLower() == "parent" )
            {
                intNavNodeOptions = (int)Navigation.NavNodeOptions.IncludeParent + (int)Navigation.NavNodeOptions.IncludeSelf;
            }
            else if( Level.ToLower() == "same" )
            {
                intNavNodeOptions = (int)Navigation.NavNodeOptions.IncludeSiblings + (int)Navigation.NavNodeOptions.IncludeSelf;
            }
            else
            {
                //root
                intRootParent = -1;
                intNavNodeOptions = (int)Navigation.NavNodeOptions.IncludeSiblings + (int)Navigation.NavNodeOptions.IncludeSelf;
            }

            if( ToolTip.ToLower() == "name" )
            {
                eToolTips = Navigation.ToolTipSource.TabName;
            }
            else if( ToolTip.ToLower() == "title" )
            {
                eToolTips = Navigation.ToolTipSource.Title;
            }
            else if( ToolTip.ToLower() == "description" )
            {
                eToolTips = Navigation.ToolTipSource.Description;
            }
            else
            {
                eToolTips = Navigation.ToolTipSource.None;
            }

            if( this.PopulateNodesFromClient && Control.SupportsPopulateOnDemand )
            {
                intNavNodeOptions += (int)Navigation.NavNodeOptions.MarkPendingNodes;
            }
            if( this.PopulateNodesFromClient && Control.SupportsPopulateOnDemand == false )
            {
                ExpandDepth = -1;
            }

            if( StartTabId != -1 )
            {
                intRootParent = StartTabId;
            }

            if( objNode != null )
            {
                //we are in a POD request
                intRootParent = Convert.ToInt32( objNode.ID );
                intNavNodeOptions = (int)Navigation.NavNodeOptions.MarkPendingNodes; //other options for this don't apply, but we do know we want to mark pending nodes
                objNodes = Navigation.GetNavigationNodes( objNode, eToolTips, intRootParent, intDepth, intNavNodeOptions );
            }
            else
            {
                objNodes = Navigation.GetNavigationNodes( Control.ClientID, eToolTips, intRootParent, intDepth, intNavNodeOptions );
            }

            return objNodes;
        }

        private string GetPath( string strPath )
        {
            if( ( strPath.IndexOf( "[SKINPATH]" ) > -1 ) )
            {
                return Strings.Replace( strPath, "[SKINPATH]", this.PortalSettings.ActiveTab.SkinPath, 1, -1, CompareMethod.Binary );
            }
            if( ( strPath.IndexOf( "[APPIMAGEPATH]" ) > -1 ) )
            {
                return Strings.Replace( strPath, "[APPIMAGEPATH]", ( Globals.ApplicationPath + "/images/" ), 1, -1, CompareMethod.Binary );
            }
            if( ( strPath.IndexOf( "[HOMEDIRECTORY]" ) > -1 ) )
            {
                return Strings.Replace( strPath, "[HOMEDIRECTORY]", this.PortalSettings.HomeDirectory, 1, -1, CompareMethod.Binary );
            }
            if( ! strPath.StartsWith( "~" ) )
            {
                return strPath;
            }
            else
            {
                return this.ResolveUrl( strPath );
            }
        }

        protected string GetValue( string strVal, string strDefault )
        {
            if( string.IsNullOrEmpty(strVal) )
            {
                return strDefault;
            }
            else
            {
                return strVal;
            }
        }

        /// <summary>
        /// since page needs to assign attributes before the init method we need this class to act
        /// as place holders. Then once we are ready to bind the control (thus it is instantiated)
        /// we will assign the placeholder properties to the control  
        /// </summary>
        private void AssignControlProperties()
        {
            if( m_strPathSystemImage != null ) if( m_strPathSystemImage.Length > 0 )
            {
                Control.PathSystemImage = m_strPathSystemImage;
            }
            if (m_strPathImage != null && m_strPathImage.Length > 0)
            {
                Control.PathImage = m_strPathImage;
            }
            if (m_strPathSystemScript != null && m_strPathSystemScript.Length > 0)
            {
                Control.PathSystemScript = m_strPathSystemScript;
            }
            if (m_strWorkImage!= null && m_strWorkImage.Length > 0)
            {
                Control.WorkImage = m_strWorkImage;
            }
            if (m_strControlOrientation!= null && m_strControlOrientation.Length > 0)
            {
                switch( m_strControlOrientation.ToLower() )
                {
                    case "horizontal":

                        Control.ControlOrientation = NavigationProvider.Orientation.Horizontal;
                        break;
                    case "vertical":

                        Control.ControlOrientation = NavigationProvider.Orientation.Vertical;
                        break;
                }
            }
            if( m_strControlAlignment != null ) if( m_strControlAlignment.Length > 0 )
            {
                switch( m_strControlAlignment.ToLower() )
                {
                    case "left":

                        Control.ControlAlignment = NavigationProvider.Alignment.Left;
                        break;
                    case "right":

                        Control.ControlAlignment = NavigationProvider.Alignment.Right;
                        break;
                    case "center":

                        Control.ControlAlignment = NavigationProvider.Alignment.Center;
                        break;
                    case "justify":

                        Control.ControlAlignment = NavigationProvider.Alignment.Justify;
                        break;
                }
            }
            Control.ForceCrawlerDisplay = GetValue( m_strForceCrawlerDisplay, "False" );
            Control.ForceDownLevel = GetValue( m_strForceDownLevel, "False" );
            if( m_strMouseOutHideDelay != null ) if( m_strMouseOutHideDelay.Length > 0 )
            {
                Control.MouseOutHideDelay = Convert.ToDecimal( m_strMouseOutHideDelay );
            }
            if( m_strMouseOverDisplay != null ) if( m_strMouseOverDisplay.Length > 0 )
            {
                switch( m_strMouseOverDisplay.ToLower() )
                {
                    case "highlight":

                        Control.MouseOverDisplay = NavigationProvider.HoverDisplay.Highlight;
                        break;
                    case "outset":

                        Control.MouseOverDisplay = NavigationProvider.HoverDisplay.Outset;
                        break;
                    case "none":

                        Control.MouseOverDisplay = NavigationProvider.HoverDisplay.None;
                        break;
                }
            }
            if( Convert.ToBoolean( GetValue( m_strMouseOverAction, "True" ) ) )
            {
                Control.MouseOverAction = NavigationProvider.HoverAction.Expand;
            }
            else
            {
                Control.MouseOverAction = NavigationProvider.HoverAction.None;
            }
            Control.IndicateChildren = Convert.ToBoolean( GetValue( m_strIndicateChildren, "True" ) );
            if( m_strIndicateChildImageRoot != null ) if( m_strIndicateChildImageRoot.Length > 0 )
            {
                Control.IndicateChildImageRoot = m_strIndicateChildImageRoot;
            }
            if( m_strIndicateChildImageSub != null ) if( m_strIndicateChildImageSub.Length > 0 )
            {
                Control.IndicateChildImageSub = m_strIndicateChildImageSub;
            }
            if( m_strIndicateChildImageExpandedRoot != null ) if( m_strIndicateChildImageExpandedRoot.Length > 0 )
            {
                Control.IndicateChildImageExpandedRoot = m_strIndicateChildImageExpandedRoot;
            }
            if( m_strIndicateChildImageExpandedSub != null ) if( m_strIndicateChildImageExpandedSub.Length > 0 )
            {
                Control.IndicateChildImageExpandedSub = m_strIndicateChildImageExpandedSub;
            }
            if( m_strNodeLeftHTMLRoot != null ) if( m_strNodeLeftHTMLRoot.Length > 0 )
            {
                Control.NodeLeftHTMLRoot = m_strNodeLeftHTMLRoot;
            }
            if( m_strNodeRightHTMLRoot != null ) if( m_strNodeRightHTMLRoot.Length > 0 )
            {
                Control.NodeRightHTMLRoot = m_strNodeRightHTMLRoot;
            }
            if( m_strNodeLeftHTMLSub != null ) if( m_strNodeLeftHTMLSub.Length > 0 )
            {
                Control.NodeLeftHTMLSub = m_strNodeLeftHTMLSub;
            }
            if( m_strNodeRightHTMLSub != null ) if( m_strNodeRightHTMLSub.Length > 0 )
            {
                Control.NodeRightHTMLSub = m_strNodeRightHTMLSub;
            }
            if( m_strNodeLeftHTMLBreadCrumbRoot != null ) if( m_strNodeLeftHTMLBreadCrumbRoot.Length > 0 )
            {
                Control.NodeLeftHTMLBreadCrumbRoot = m_strNodeLeftHTMLBreadCrumbRoot;
            }
            if( m_strNodeLeftHTMLBreadCrumbSub != null ) if( m_strNodeLeftHTMLBreadCrumbSub.Length > 0 )
            {
                Control.NodeLeftHTMLBreadCrumbSub = m_strNodeLeftHTMLBreadCrumbSub;
            }
            if( m_strNodeRightHTMLBreadCrumbRoot != null ) if( m_strNodeRightHTMLBreadCrumbRoot.Length > 0 )
            {
                Control.NodeRightHTMLBreadCrumbRoot = m_strNodeRightHTMLBreadCrumbRoot;
            }
            if( m_strNodeRightHTMLBreadCrumbSub != null ) if( m_strNodeRightHTMLBreadCrumbSub.Length > 0 )
            {
                Control.NodeRightHTMLBreadCrumbSub = m_strNodeRightHTMLBreadCrumbSub;
            }
            if( m_strSeparatorHTML != null ) if( m_strSeparatorHTML.Length > 0 )
            {
                Control.SeparatorHTML = m_strSeparatorHTML;
            }
            if( m_strSeparatorLeftHTML != null ) if( m_strSeparatorLeftHTML.Length > 0 )
            {
                Control.SeparatorLeftHTML = m_strSeparatorLeftHTML;
            }
            if( m_strSeparatorRightHTML != null ) if( m_strSeparatorRightHTML.Length > 0 )
            {
                Control.SeparatorRightHTML = m_strSeparatorRightHTML;
            }
            if( m_strSeparatorLeftHTMLActive != null ) if( m_strSeparatorLeftHTMLActive.Length > 0 )
            {
                Control.SeparatorLeftHTMLActive = m_strSeparatorLeftHTMLActive;
            }
            if( m_strSeparatorRightHTMLActive != null ) if( m_strSeparatorRightHTMLActive.Length > 0 )
            {
                Control.SeparatorRightHTMLActive = m_strSeparatorRightHTMLActive;
            }
            if( m_strSeparatorLeftHTMLBreadCrumb != null ) if( m_strSeparatorLeftHTMLBreadCrumb.Length > 0 )
            {
                Control.SeparatorLeftHTMLBreadCrumb = m_strSeparatorLeftHTMLBreadCrumb;
            }
            if( m_strSeparatorRightHTMLBreadCrumb != null ) if( m_strSeparatorRightHTMLBreadCrumb.Length > 0 )
            {
                Control.SeparatorRightHTMLBreadCrumb = m_strSeparatorRightHTMLBreadCrumb;
            }
            if( m_strCSSControl != null ) if( m_strCSSControl.Length > 0 )
            {
                Control.CSSControl = m_strCSSControl;
            }
            if( m_strCSSContainerRoot != null ) if( m_strCSSContainerRoot.Length > 0 )
            {
                Control.CSSContainerRoot = m_strCSSContainerRoot;
            }
            if( m_strCSSNode != null ) if( m_strCSSNode.Length > 0 )
            {
                Control.CSSNode = m_strCSSNode;
            }
            if( m_strCSSIcon != null ) if( m_strCSSIcon.Length > 0 )
            {
                Control.CSSIcon = m_strCSSIcon;
            }
            if( m_strCSSContainerSub != null ) if( m_strCSSContainerSub.Length > 0 )
            {
                Control.CSSContainerSub = m_strCSSContainerSub;
            }
            if( m_strCSSNodeHover != null ) if( m_strCSSNodeHover.Length > 0 )
            {
                Control.CSSNodeHover = m_strCSSNodeHover;
            }
            if( m_strCSSBreak != null ) if( m_strCSSBreak.Length > 0 )
            {
                Control.CSSBreak = m_strCSSBreak;
            }
            if( m_strCSSIndicateChildSub != null ) if( m_strCSSIndicateChildSub.Length > 0 )
            {
                Control.CSSIndicateChildSub = m_strCSSIndicateChildSub;
            }
            if( m_strCSSIndicateChildRoot != null ) if( m_strCSSIndicateChildRoot.Length > 0 )
            {
                Control.CSSIndicateChildRoot = m_strCSSIndicateChildRoot;
            }
            if( m_strCSSBreadCrumbRoot != null ) if( m_strCSSBreadCrumbRoot.Length > 0 )
            {
                Control.CSSBreadCrumbRoot = m_strCSSBreadCrumbRoot;
            }
            if( m_strCSSBreadCrumbSub != null ) if( m_strCSSBreadCrumbSub.Length > 0 )
            {
                Control.CSSBreadCrumbSub = m_strCSSBreadCrumbSub;
            }
            if( m_strCSSNodeRoot != null ) if( m_strCSSNodeRoot.Length > 0 )
            {
                Control.CSSNodeRoot = m_strCSSNodeRoot;
            }
            if( m_strCSSNodeSelectedRoot != null ) if( m_strCSSNodeSelectedRoot.Length > 0 )
            {
                Control.CSSNodeSelectedRoot = m_strCSSNodeSelectedRoot;
            }
            if( m_strCSSNodeSelectedSub != null ) if( m_strCSSNodeSelectedSub.Length > 0 )
            {
                Control.CSSNodeSelectedSub = m_strCSSNodeSelectedSub;
            }
            if( m_strCSSNodeHoverRoot != null ) if( m_strCSSNodeHoverRoot.Length > 0 )
            {
                Control.CSSNodeHoverRoot = m_strCSSNodeHoverRoot;
            }
            if( m_strCSSNodeHoverSub != null ) if( m_strCSSNodeHoverSub.Length > 0 )
            {
                Control.CSSNodeHoverSub = m_strCSSNodeHoverSub;
            }
            if( m_strCSSSeparator != null ) if( m_strCSSSeparator.Length > 0 )
            {
                Control.CSSSeparator = m_strCSSSeparator;
            }
            if( m_strCSSLeftSeparator != null ) if( m_strCSSLeftSeparator.Length > 0 )
            {
                Control.CSSLeftSeparator = m_strCSSLeftSeparator;
            }
            if( m_strCSSRightSeparator != null ) if( m_strCSSRightSeparator.Length > 0 )
            {
                Control.CSSRightSeparator = m_strCSSRightSeparator;
            }
            if( m_strCSSLeftSeparatorSelection != null ) if( m_strCSSLeftSeparatorSelection.Length > 0 )
            {
                Control.CSSLeftSeparatorSelection = m_strCSSLeftSeparatorSelection;
            }
            if( m_strCSSRightSeparatorSelection != null ) if( m_strCSSRightSeparatorSelection.Length > 0 )
            {
                Control.CSSRightSeparatorSelection = m_strCSSRightSeparatorSelection;
            }
            if( m_strCSSLeftSeparatorBreadCrumb != null ) if( m_strCSSLeftSeparatorBreadCrumb.Length > 0 )
            {
                Control.CSSLeftSeparatorBreadCrumb = m_strCSSLeftSeparatorBreadCrumb;
            }
            if( m_strCSSRightSeparatorBreadCrumb != null ) if( m_strCSSRightSeparatorBreadCrumb.Length > 0 )
            {
                Control.CSSRightSeparatorBreadCrumb = m_strCSSRightSeparatorBreadCrumb;
            }
            if( m_strStyleBackColor != null ) if( m_strStyleBackColor.Length > 0 )
            {
                Control.StyleBackColor = m_strStyleBackColor;
            }
            if( m_strStyleForeColor != null ) if( m_strStyleForeColor.Length > 0 )
            {
                Control.StyleForeColor = m_strStyleForeColor;
            }
            if( m_strStyleHighlightColor != null ) if( m_strStyleHighlightColor.Length > 0 )
            {
                Control.StyleHighlightColor = m_strStyleHighlightColor;
            }
            if( m_strStyleIconBackColor != null ) if( m_strStyleIconBackColor.Length > 0 )
            {
                Control.StyleIconBackColor = m_strStyleIconBackColor;
            }
            if( m_strStyleSelectionBorderColor != null ) if( m_strStyleSelectionBorderColor.Length > 0 )
            {
                Control.StyleSelectionBorderColor = m_strStyleSelectionBorderColor;
            }
            if( m_strStyleSelectionColor != null ) if( m_strStyleSelectionColor.Length > 0 )
            {
                Control.StyleSelectionColor = m_strStyleSelectionColor;
            }
            if( m_strStyleSelectionForeColor != null ) if( m_strStyleSelectionForeColor.Length > 0 )
            {
                Control.StyleSelectionForeColor = m_strStyleSelectionForeColor;
            }
            if( m_strStyleControlHeight != null ) if( m_strStyleControlHeight.Length > 0 )
            {
                Control.StyleControlHeight = Convert.ToDecimal( m_strStyleControlHeight );
            }
            if( m_strStyleBorderWidth != null ) if( m_strStyleBorderWidth.Length > 0 )
            {
                Control.StyleBorderWidth = Convert.ToDecimal( m_strStyleBorderWidth );
            }
            if( m_strStyleNodeHeight != null ) if( m_strStyleNodeHeight.Length > 0 )
            {
                Control.StyleNodeHeight = Convert.ToDecimal( m_strStyleNodeHeight );
            }
            if( m_strStyleIconWidth != null ) if( m_strStyleIconWidth.Length > 0 )
            {
                Control.StyleIconWidth = Convert.ToDecimal( m_strStyleIconWidth );
            }
            if( m_strStyleFontNames != null ) if( m_strStyleFontNames.Length > 0 )
            {
                Control.StyleFontNames = m_strStyleFontNames;
            }
            if( m_strStyleFontSize != null ) if( m_strStyleFontSize.Length > 0 )
            {
                Control.StyleFontSize = Convert.ToDecimal( m_strStyleFontSize );
            }
            if( m_strStyleFontBold != null ) if( m_strStyleFontBold.Length > 0 )
            {
                Control.StyleFontBold = m_strStyleFontBold;
            }
            if( m_strEffectsShadowColor != null ) if( m_strEffectsShadowColor.Length > 0 )
            {
                Control.EffectsShadowColor = m_strEffectsShadowColor;
            }
            if( m_strEffectsStyle != null ) if( m_strEffectsStyle.Length > 0 )
            {
                Control.EffectsStyle = m_strEffectsStyle;
            }
            if( m_strEffectsShadowStrength != null ) if( m_strEffectsShadowStrength.Length > 0 )
            {
                Control.EffectsShadowStrength = Convert.ToInt32( m_strEffectsShadowStrength );
            }
            if( m_strEffectsTransition != null ) if( m_strEffectsTransition.Length > 0 )
            {
                Control.EffectsTransition = m_strEffectsTransition;
            }
            if( m_strEffectsDuration != null ) if( m_strEffectsDuration.Length > 0 )
            {
                Control.EffectsDuration = Convert.ToDouble( m_strEffectsDuration );
            }
            if( m_strEffectsShadowDirection != null ) if( m_strEffectsShadowDirection.Length > 0 )
            {
                Control.EffectsShadowDirection = m_strEffectsShadowDirection;
            }
        }

        protected void Bind( DNNNodeCollection objNodes )
        {
            this.Control.Bind( objNodes );
        }

        protected void InitializeNavControl( Control objParent, string strDefaultProvider )
        {
            if( this.ProviderName.Length == 0 )
            {
                ProviderName = strDefaultProvider;
            }
            m_objControl = NavigationProvider.Instance( this.ProviderName );
            Control.ControlID = "ctl" + this.ID;
            Control.Initialize();
            AssignControlProperties();
            objParent.Controls.Add( Control.NavigationControl );
        }
    }
}