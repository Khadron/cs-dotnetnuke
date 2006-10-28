using System;
using System.Web.UI;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.NavigationControl
{
    public class DNNTreeNavigationProvider : NavigationProvider
    {
        private DnnTree m_objTree;
        private string m_strControlID;
        private string m_strCSSBreadCrumbSub;
        private string m_strCSSBreadCrumbRoot;

        private string m_strNodeSelectedSub;
        private string m_strNodeSelectedRoot;
        private string m_strCSSNodeRoot;
        private string m_strCSSNodeHoverSub;
        private string m_strCSSNodeHoverRoot;
        private string m_strNodeLeftHTML;
        private string m_strNodeLeftHTMLBreadCrumb;
        private string m_strNodeLeftHTMLBreadCrumbRoot;
        private string m_strNodeLeftHTMLRoot;
        private string m_strNodeRightHTML;
        private string m_strNodeRightHTMLBreadCrumb;
        private string m_strNodeRightHTMLBreadCrumbRoot;
        private string m_strNodeRightHTMLRoot;
        private bool m_blnIndicateChildren;
        private string m_strPathImage;

        public DnnTree Tree
        {
            get
            {
                return m_objTree;
            }
        }

        public override Control NavigationControl
        {
            get
            {
                return Tree;
            }
        }

        public override bool SupportsPopulateOnDemand
        {
            get
            {
                return true;
            }
        }

        public override string IndicateChildImageSub
        {
            get
            {
                return Tree.CollapsedNodeImage;
            }
            set
            {
                Tree.CollapsedNodeImage = value;
            }
        }

        public override string IndicateChildImageRoot
        {
            get
            {
                return Tree.CollapsedNodeImage;
            }
            set
            {
                Tree.CollapsedNodeImage = value;
            }
        }

        public override string WorkImage
        {
            get
            {
                return Tree.WorkImage;
            }
            set
            {
                Tree.WorkImage = value;
            }
        }

        public override string IndicateChildImageExpandedRoot
        {
            get
            {
                return Tree.ExpandedNodeImage;
            }
            set
            {
                Tree.ExpandedNodeImage = value;
            }
        }

        public override string IndicateChildImageExpandedSub
        {
            get
            {
                return Tree.ExpandedNodeImage;
            }
            set
            {
                Tree.ExpandedNodeImage = value;
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

        public override string CSSControl
        {
            get
            {
                return Tree.CssClass; //???
            }
            set
            {
                Tree.CssClass = value;
            }
        }

        public override string CSSIcon
        {
            get
            {
                return Tree.DefaultIconCssClass;
            }
            set
            {
                Tree.DefaultIconCssClass = value;
            }
        }

        public override string CSSNode
        {
            get
            {
                return Tree.DefaultNodeCssClass;
            }
            set
            {
                Tree.DefaultNodeCssClass = value;
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

        public override string CSSNodeHover
        {
            get
            {
                return Tree.DefaultNodeCssClassOver;
            }
            set
            {
                Tree.DefaultNodeCssClassOver = value;
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

        public override string ForceDownLevel
        {
            get
            {
                return Tree.ForceDownLevel.ToString();
            }
            set
            {
                Tree.ForceDownLevel = Convert.ToBoolean( value );
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

        public override bool PopulateNodesFromClient
        {
            get
            {
                return Tree.PopulateNodesFromClient;
            }
            set
            {
                Tree.PopulateNodesFromClient = value;
            }
        }

        public override string PathSystemImage
        {
            get
            {
                return Tree.SystemImagesPath;
            }
            set
            {
                Tree.SystemImagesPath = value;
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

        public override string PathSystemScript
        {
            get
            {
                return Tree.TreeScriptPath;
            }
            set
            {
                //Tree.TreeScriptPath = Value	'Take out, use default CAPI
            }
        }

        public override void Initialize()
        {
            m_objTree = new DnnTree();
            Tree.ID = m_strControlID;
            Tree.NodeClick += new DnnTree.DNNTreeNodeClickHandler( DNNTree_NodeClick );
            Tree.PopulateOnDemand += new DnnTree.DNNTreeEventHandler( DNNTree_PopulateOnDemand );
        }

        public override void Bind( DNNNodeCollection objNodes )
        {
            DNNNode objNode;
            TreeNode objTreeItem;
            int intIndex;

            if( IndicateChildren == false )
            {
                IndicateChildImageSub = "";
                IndicateChildImageRoot = "";
                this.IndicateChildImageExpandedSub = "";
                this.IndicateChildImageExpandedRoot = "";
            }

            if( this.CSSNodeSelectedRoot.Length > 0 && this.CSSNodeSelectedRoot == this.CSSNodeSelectedSub )
            {
                Tree.DefaultNodeCssClassSelected = this.CSSNodeSelectedRoot; //set on parent, thus decreasing overall payload
            }

            //For i = 0 To objNodes.Count - 1			  'Each objNode In objNodes
            foreach( DNNNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                //objNode = objNodes(i)
                if( objNode.Level == 0 ) // root Tree
                {
                    intIndex = Tree.TreeNodes.Import( objNode, true );
                    objTreeItem = Tree.TreeNodes[intIndex];
                    //objTreeItem.ID = objNode.ID
                    if( objNode.Enabled == false )
                    {
                        objTreeItem.ClickAction = eClickAction.Expand;
                    }

                    if( this.CSSNodeRoot != "" )
                    {
                        objTreeItem.CssClass = this.CSSNodeRoot;
                    }
                    if( this.CSSNodeHoverRoot != "" )
                    {
                        objTreeItem.CSSClassHover = this.CSSNodeHoverRoot;
                    }

                    if( this.NodeLeftHTMLRoot != "" )
                    {
                        //objTreeItem.LeftHTML = Me.NodeLeftHTMLRoot
                    }

                    if( Tree.DefaultNodeCssClassSelected.Length == 0 && this.CSSNodeSelectedRoot.Length > 0 )
                    {
                        objTreeItem.CSSClassSelected = this.CSSNodeSelectedRoot;
                    }

                    objTreeItem.CSSIcon = " "; //< ignore for root...???
                    if( objNode.BreadCrumb )
                    {
                        objTreeItem.CssClass = this.CSSBreadCrumbRoot;
                        //If NodeLeftHTMLBreadCrumbRoot <> "" Then objTreeItem.LeftHTML = NodeLeftHTMLBreadCrumbRoot
                        //If NodeRightHTMLBreadCrumbRoot <> "" Then objTreeItem.RightHTML = NodeRightHTMLBreadCrumbRoot
                        //If objNode.Selected Then				   '<--- not necessary - control handles it
                        //	objTreeItem.CSSClassSelected = Me.CSSNodeSelectedRoot
                        //End If
                    }

                    if( this.NodeRightHTMLRoot != "" )
                    {
                        //objTreeItem.RightHTML = NodeRightHTMLRoot
                    }
                }
                else
                {
                    try
                    {
                        TreeNode objParent = Tree.TreeNodes.FindNode( objNode.ParentNode.ID );

                        if( objParent == null ) //POD
                        {
                            objParent = Tree.TreeNodes[Tree.TreeNodes.Import( objNode.ParentNode.Clone(), true )];
                        }
                        objTreeItem = objParent.TreeNodes.FindNode( objNode.ID );
                        if( objTreeItem == null ) //POD
                        {
                            objTreeItem = objParent.TreeNodes[objParent.TreeNodes.Import( objNode.Clone(), true )];
                        }

                        //objTreeItem.ID = objNode.ID
                        if( objNode.Enabled == false )
                        {
                            objTreeItem.ClickAction = eClickAction.Expand;
                        }
                        //objTreeItem.Selected = objNode.Selected

                        if( CSSNodeHover != "" )
                        {
                            objTreeItem.CSSClassHover = CSSNodeHover;
                        }
                        if( NodeLeftHTMLSub != "" )
                        {
                            //objTreeItem.LeftHTML = NodeLeftHTML
                        }

                        if( Tree.DefaultNodeCssClassSelected.Length == 0 && this.CSSNodeSelectedSub.Length > 0 )
                        {
                            objTreeItem.CSSClassSelected = this.CSSNodeSelectedSub;
                        }

                        if( objNode.BreadCrumb )
                        {
                            objTreeItem.CssClass = this.CSSBreadCrumbSub;
                            //If NodeLeftHTMLBreadCrumb <> "" Then objTreeItem.LeftHTML = NodeLeftHTMLBreadCrumb
                            //If NodeRightHTMLBreadCrumb <> "" Then objTreeItem.RightHTML = NodeRightHTMLBreadCrumb
                            //If objNode.Selected Then
                            //	objTreeItem.ItemCss = Me.CSSNodeActive
                            //End If
                        }

                        if( this.NodeRightHTMLSub != "" )
                        {
                            //objTreeItem.RightHTML = Me.NodeRightHTML
                        }
                    }
                    catch
                    {
                        // throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
                        objTreeItem = null;
                    }
                }

                if( objNode.Image.Length > 0 )
                {
                    if( objNode.Image.StartsWith( "/" ) == false && this.PathImage.Length > 0 )
                    {
                        objNode.Image = this.PathImage + objNode.Image;
                    }
                    objTreeItem.Image = objNode.Image;
                }
                objTreeItem.ToolTip = objNode.ToolTip;

                //End Select
                if( objNode.Selected )
                {
                    Tree.SelectNode( objNode.ID );
                }
                Bind( objNode.DNNNodes );
            }
        }

        private void DNNTree_NodeClick( object source, DNNTreeNodeClickEventArgs e )
        {
            base.RaiseEvent_NodeClick( e.Node );
        }

        private void DNNTree_PopulateOnDemand( object source, DNNTreeEventArgs e )
        {
            base.RaiseEvent_PopulateOnDemand( e.Node );
        }

        public override void ClearNodes()
        {
            Tree.TreeNodes.Clear();
        }
    }
}