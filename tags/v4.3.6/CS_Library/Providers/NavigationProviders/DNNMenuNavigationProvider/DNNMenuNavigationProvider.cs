#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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

namespace DotNetNuke.NavigationControl
{
    public class DNNMenuNavigationProvider : NavigationProvider
    {
        private bool m_blnIndicateChildren;
        private DNNMenu m_objMenu;
        private string m_strControlID;
        private string m_strCSSBreadCrumbRoot;
        private string m_strCSSBreadCrumbSub;
        private string m_strCSSBreak;

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
        private string m_strPathImage;
        private string m_strSeparatorHTML = "";
        private string m_strSeparatorLeftHTML = "";
        private string m_strSeparatorLeftHTMLActive = "";
        private string m_strSeparatorLeftHTMLBreadCrumb = "";
        private string m_strSeparatorRightHTML = "";
        private string m_strSeparatorRightHTMLActive = "";
        private string m_strSeparatorRightHTMLBreadCrumb = "";

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
                Orientation orientation1 = Orientation.Horizontal;
                if( this.Menu.Orientation == UI.WebControls.Orientation.Horizontal )
                {
                    return Orientation.Horizontal;
                }
                else if( this.Menu.Orientation == UI.WebControls.Orientation.Vertical )
                {
                    return Orientation.Vertical;
                }
                return orientation1;
            }
            set
            {
                if( value == Orientation.Horizontal )
                {
                    this.Menu.Orientation = UI.WebControls.Orientation.Horizontal;
                    return;
                }
                else if( value == Orientation.Vertical )
                {
                    this.Menu.Orientation = UI.WebControls.Orientation.Vertical;
                    return;
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
                return m_strCSSBreak;
            }
            set
            {
                m_strCSSBreak = value;
            }
        }

        public override string CSSContainerSub
        {
            get
            {
                return Menu.MenuCssClass;
            }
            set
            {
                Menu.MenuCssClass = value;
            }
        }

        public override string CSSControl
        {
            get
            {
                return Menu.MenuBarCssClass;
            }
            set
            {
                Menu.MenuBarCssClass = value;
            }
        }

        public override string CSSIcon
        {
            get
            {
                return Menu.DefaultIconCssClass;
            }
            set
            {
                Menu.DefaultIconCssClass = value;
            }
        }

        public override string CSSIndicateChildRoot
        {
            get
            {
                //Return Menu.MenuCSS.RootMenuArrow
                return String.Empty;
            }
            set
            {
                //Menu.MenuCSS.RootMenuArrow = Value
            }
        }

        public override string CSSIndicateChildSub
        {
            get
            {
                //Return Menu.MenuCSS.MenuArrow
                return String.Empty;
            }
            set
            {
                //Menu.MenuCSS.MenuArrow = Value
            }
        }

        public override string CSSNode
        {
            get
            {
                return Menu.DefaultNodeCssClass;
            }
            set
            {
                Menu.DefaultNodeCssClass = value;
            }
        }

        public override string CSSNodeHover
        {
            get
            {
                return Menu.DefaultNodeCssClassOver;
            }
            set
            {
                Menu.DefaultNodeCssClassOver = value;
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

        public override string ForceDownLevel
        {
            get
            {
                return Menu.ForceDownLevel.ToString();
            }
            set
            {
                Menu.ForceDownLevel = Convert.ToBoolean( value );
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
                return Menu.ChildArrowImage;
            }
            set
            {
                Menu.ChildArrowImage = value;
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

        public DNNMenu Menu
        {
            get
            {
                return m_objMenu;
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
                return m_strPathImage;
            }
            set
            {
                m_strPathImage = value;
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
                return Menu.MenuScriptPath;
            }
            set
            {
                //Menu.MenuScriptPath = Value	'Take out, use default CAPI
            }
        }

        public override bool PopulateNodesFromClient
        {
            get
            {
                return Menu.PopulateNodesFromClient;
            }
            set
            {
                Menu.PopulateNodesFromClient = value;
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

        public override bool SupportsPopulateOnDemand
        {
            get
            {
                return true;
            }
        }

        public override string WorkImage
        {
            get
            {
                return Menu.WorkImage;
            }
            set
            {
                Menu.WorkImage = value;
            }
        }

        private string GetSeparatorTD( string strClass, string strHTML )
        {
            string strRet = "";
            if( !String.IsNullOrEmpty(strClass) )
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
            if( !String.IsNullOrEmpty(strNormal) )
            {
                strRet = strNormal;
            }
            if( !String.IsNullOrEmpty(strBreadCrumb) && objNode != null && objNode.BreadCrumb )
            {
                strRet = strBreadCrumb;
            }
            if( !String.IsNullOrEmpty(strSelection) && objNode != null && objNode.Selected )
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
            if( strHTML.Length > 0 )
            {
                string strSeparatorTable;
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
                if( !String.IsNullOrEmpty(SeparatorHTML) )
                {
                    if( !String.IsNullOrEmpty(CSSSeparator) )
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

                if( !String.IsNullOrEmpty(strSeparatorRightHTML) && strType != "Left" )
                {
                    strSeparatorTable += GetSeparatorTD( strRightSeparatorClass, strSeparatorRightHTML );
                }
                if( !String.IsNullOrEmpty(strSeparator) && strType != "Left" && strType != "Right" )
                {
                    strSeparatorTable += GetSeparatorTD( strSeparatorClass, strSeparator );
                }
                if( !String.IsNullOrEmpty(strSeparatorLeftHTML) && strType != "Right" )
                {
                    strSeparatorTable += GetSeparatorTD( strLeftSeparatorClass, strSeparatorLeftHTML );
                }
                strSeparatorTable += "</tr></table>";
                //objBreak = Menu.AddBreak("", strSeparatorTable)
            }
        }

        public override void Bind( DNNNodeCollection objNodes )
        {
            DNNNode objNode = null;
            DNNNode objPrevNode = null;
            bool isRoot = false;

            if( IndicateChildren == false )
            {
                IndicateChildImageSub = "";
                IndicateChildImageRoot = "";
            }

            if( this.CSSNodeSelectedRoot.Length > 0 && this.CSSNodeSelectedRoot == this.CSSNodeSelectedSub )
            {
                Menu.DefaultNodeCssClassSelected = this.CSSNodeSelectedRoot; //set on parent, thus decreasing overall payload
            }

            //For i = 0 To objNodes.Count - 1
            foreach( DNNNode dnnNode in objNodes )
            {
                objNode = dnnNode;
                //objNode = objNodes(i)
                MenuNode objMenuItem;
                if( objNode.Level == 0 ) // root menu
                {
                    if( isRoot ) //first root item has already been entered
                    {
                        AddSeparator( "All", objPrevNode, objNode );
                    }
                    else
                    {
                        if( !String.IsNullOrEmpty(SeparatorLeftHTML) || !String.IsNullOrEmpty(SeparatorLeftHTMLBreadCrumb) || !String.IsNullOrEmpty(this.SeparatorLeftHTMLActive) )
                        {
                            AddSeparator( "Left", objPrevNode, objNode );
                        }
                        isRoot = true;
                    }

                    //If objNode.Enabled = False Then
                    //	intIndex = Menu.MenuNodes.Add(objNode.ID.ToString, objNode.Key, objNode.Text & "&nbsp;", "")						 'TODO:  Do we want to space &nbsp; here???
                    //Else
                    //	intIndex = Menu.MenuNodes.Add(objNode.ID.ToString, objNode.Key, objNode.Text & "&nbsp;", objNode.NavigateURL)
                    //End If
                    int intIndex = Menu.MenuNodes.Import( objNode, false );
                    objMenuItem = Menu.MenuNodes[intIndex];
                    if( !String.IsNullOrEmpty(this.CSSNodeRoot) )
                    {
                        objMenuItem.CSSClass = this.CSSNodeRoot;
                    }
                    if( !String.IsNullOrEmpty(this.CSSNodeHoverRoot) && this.CSSNodeHoverRoot != this.CSSNodeHoverSub )
                    {
                        objMenuItem.CSSClassHover = this.CSSNodeHoverRoot;
                    }

                    if( !String.IsNullOrEmpty(this.NodeLeftHTMLRoot) )
                    {
                        objMenuItem.LeftHTML = this.NodeLeftHTMLRoot;
                    }

                    objMenuItem.CSSIcon = " "; //< ignore for root...???
                    if( objNode.BreadCrumb )
                    {
                        if( !String.IsNullOrEmpty(CSSBreadCrumbRoot) )
                        {
                            objMenuItem.CSSClass = CSSBreadCrumbRoot;
                        }
                        if( !String.IsNullOrEmpty(NodeLeftHTMLBreadCrumbRoot) )
                        {
                            objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbRoot;
                        }
                        if( !String.IsNullOrEmpty(NodeRightHTMLBreadCrumbRoot) )
                        {
                            objMenuItem.RightHTML = NodeRightHTMLBreadCrumbRoot;
                        }
                        if (objNode.Selected && String.IsNullOrEmpty(Menu.DefaultNodeCssClassSelected)) //<--- not necessary when both are the same
                        {
                            objMenuItem.CSSClassSelected = this.CSSNodeSelectedRoot;
                        }
                    }

                    if( !String.IsNullOrEmpty(this.NodeRightHTMLRoot) )
                    {
                        objMenuItem.RightHTML = NodeRightHTMLRoot;
                    }
                }
                else //If Not blnRootOnly Then
                {
                    try
                    {
                        MenuNode objParent = Menu.MenuNodes.FindNode( objNode.ParentNode.ID );

                        if( objParent == null ) //POD
                        {
                            objParent = Menu.MenuNodes[Menu.MenuNodes.Import( objNode.ParentNode.Clone(), true )];
                        }
                        objMenuItem = objParent.MenuNodes.FindNode( objNode.ID );
                        if( objMenuItem == null ) //POD
                        {
                            objMenuItem = objParent.MenuNodes[objParent.MenuNodes.Import( objNode.Clone(), true )];
                        }

                        //If objNode.Enabled = False Then
                        //	intIndex = objParent.MenuNodes.Add(objNode.ID.ToString, objNode.Key, objNode.Text, "")
                        //Else
                        //	intIndex = objParent.MenuNodes.Add(objNode.ID.ToString, objNode.Key, objNode.Text, objNode.NavigateURL)
                        //End If

                        if( !String.IsNullOrEmpty(CSSNodeHoverSub) && this.CSSNodeHoverRoot != this.CSSNodeHoverSub )
                        {
                            objMenuItem.CSSClassHover = CSSNodeHoverSub;
                        }
                        if( !String.IsNullOrEmpty(NodeLeftHTMLSub) )
                        {
                            objMenuItem.LeftHTML = NodeLeftHTMLSub;
                        }

                        if( objNode.BreadCrumb )
                        {
                            if( !String.IsNullOrEmpty(CSSBreadCrumbSub) )
                            {
                                objMenuItem.CSSClass = this.CSSBreadCrumbSub;
                            }
                            if( !String.IsNullOrEmpty(NodeLeftHTMLBreadCrumbSub) )
                            {
                                objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbSub;
                            }
                            if( !String.IsNullOrEmpty(NodeRightHTMLBreadCrumbSub) )
                            {
                                objMenuItem.RightHTML = NodeRightHTMLBreadCrumbSub;
                            }
                            if (objNode.Selected && String.IsNullOrEmpty(Menu.DefaultNodeCssClassSelected))
                            {
                                objMenuItem.CSSClass = this.CSSNodeSelectedSub;
                            }
                        }

                        if( !String.IsNullOrEmpty(this.NodeRightHTMLSub) )
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
                if(objMenuItem != null)
                {
                    if( objNode.Image.Length > 0 )
                    {
                        if( objNode.Image.StartsWith( "/" ) == false && this.PathImage.Length > 0 )
                        {
                            objNode.Image = this.PathImage + objNode.Image;
                        }
                        objMenuItem.Image = objNode.Image;
                    }

                    if( objMenuItem.IsBreak )
                    {
                        objMenuItem.CSSClass = this.CSSBreak;
                    }

                    objMenuItem.ToolTip = objNode.ToolTip;
                    
                }
                Bind( objNode.DNNNodes );
                objPrevNode = objNode;
            }

            if( objNode != null && objNode.Level == 0 ) // root menu
            {
                if( !String.IsNullOrEmpty(SeparatorRightHTML) || !String.IsNullOrEmpty(SeparatorRightHTMLBreadCrumb) || !String.IsNullOrEmpty(this.SeparatorRightHTMLActive) )
                {
                    AddSeparator( "Right", objPrevNode, null );
                }
                //solpartactions has a hardcoded image with no path information.  Assume if value is present and no path we need to add one.
                if( IndicateChildImageSub.Length > 0 && IndicateChildImageSub.IndexOf( "/" ) == - 1 )
                {
                    IndicateChildImageSub = this.PathSystemImage + IndicateChildImageSub;
                }
                if( IndicateChildImageRoot.Length > 0 && IndicateChildImageRoot.IndexOf( "/" ) == - 1 )
                {
                    IndicateChildImageRoot = this.PathSystemImage + IndicateChildImageRoot;
                }
            }
        }

        public override void ClearNodes()
        {
            Menu.MenuNodes.Clear();
        }

        private void DNNMenu_NodeClick( object source, DNNMenuNodeClickEventArgs e )
        {
            RaiseEvent_NodeClick( e.Node );
        }

        private void DNNMenu_PopulateOnDemand( object source, DNNMenuEventArgs e )
        {
            RaiseEvent_PopulateOnDemand( e.Node );
        }

        public override void Initialize()
        {
            m_objMenu = new DNNMenu();
            Menu.ID = m_strControlID;
            Menu.EnableViewState = false;
            Menu.NodeClick += new DNNMenu.DNNMenuNodeClickHandler( DNNMenu_NodeClick );
            Menu.PopulateOnDemand += new DNNMenu.DNNMenuEventHandler( DNNMenu_PopulateOnDemand );
        }
    }
}