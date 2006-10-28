using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    internal class DNNTreeUpLevelWriter : WebControl, IDNNTreeWriter
    {
        private DnnTree _tree;

        public void RenderTree( HtmlTextWriter writer, DnnTree tree )
        {
            _tree = tree;
            RenderControl( writer );
        }

        
        protected override void RenderContents( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Width, "100%" );
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "DNNTree" );
            writer.AddAttribute( HtmlTextWriterAttribute.Name, _tree.UniqueID );
            writer.AddAttribute( HtmlTextWriterAttribute.Id, _tree.ClientID ); //_tree.UniqueID.Replace(":", "_"))

            writer.AddAttribute( "sysimgpath", _tree.SystemImagesPath );
            writer.AddAttribute( "indentw", _tree.IndentWidth.ToString() );
            if( _tree.ExpandCollapseImageWidth != 12 )
            {
                writer.AddAttribute( "expcolimgw", _tree.ExpandCollapseImageWidth.ToString() );
            }
            if( _tree.CheckBoxes )
            {
                writer.AddAttribute( "checkboxes", "1" );
            }
            if( _tree.Target.Length > 0 )
            {
                writer.AddAttribute( "target", _tree.Target );
            }

            if( _tree.ImageList.Count > 0 )
            {
                string strList = "";
                NodeImage objNodeImage;
                foreach( NodeImage tempLoopVar_objNodeImage in _tree.ImageList )
                {
                    objNodeImage = tempLoopVar_objNodeImage;
                    strList += ( ( strList.Length > 0 ) ? "," : "" ).ToString() + objNodeImage.ImageUrl;
                }
                writer.AddAttribute( "imagelist", strList );
            }

            //css attributes
            if( _tree.DefaultNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "css", _tree.DefaultNodeCssClass );
            }
            if( _tree.DefaultChildNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "csschild", _tree.DefaultChildNodeCssClass );
            }
            if( _tree.DefaultNodeCssClassOver.Length > 0 )
            {
                writer.AddAttribute( "csshover", _tree.DefaultNodeCssClassOver );
            }
            if( _tree.DefaultNodeCssClassSelected.Length > 0 )
            {
                writer.AddAttribute( "csssel", _tree.DefaultNodeCssClassSelected );
            }
            if( _tree.DefaultIconCssClass.Length > 0 )
            {
                writer.AddAttribute( "cssicon", _tree.DefaultIconCssClass );
            }

            if( _tree.JSFunction.Length > 0 )
            {
                writer.AddAttribute( "js", _tree.JSFunction );
            }

            if( _tree.WorkImage.Length > 0 )
            {
                writer.AddAttribute( "workimg", _tree.WorkImage );
            }
            if( _tree.AnimationFrames != 5 )
            {
                writer.AddAttribute( "animf", _tree.AnimationFrames.ToString() );
            }
            writer.AddAttribute( "expimg", _tree.ExpandedNodeImage );
            writer.AddAttribute( "colimg", _tree.CollapsedNodeImage );
            writer.AddAttribute( "postback", ClientAPI.GetPostBackEventReference( _tree, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "Click" ) );

            if( _tree.PopulateNodesFromClient )
            {
                if( ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XMLHTTP ) )
                {
                    writer.AddAttribute( "callback", ClientAPI.GetCallbackEventReference( _tree, "\'[NODEXML]\'", "this.callBackSuccess", "oTNode", "this.callBackFail", "this.callBackStatus" ) );
                }
                else
                {
                    writer.AddAttribute( "callback", ClientAPI.GetPostBackClientHyperlink( _tree, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "OnDemand" ) );
                }
                if( _tree.CallbackStatusFunction.Length > 0 )
                {
                    writer.AddAttribute( "callbackSF", _tree.CallbackStatusFunction );
                }
            }

            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            writer.RenderEndTag();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        protected override void RenderChildren( HtmlTextWriter writer )
        {
            TreeNode TempNode;
            foreach( TreeNode tempLoopVar_TempNode in _tree.TreeNodes )
            {
                TempNode = tempLoopVar_TempNode;
                TempNode.Render( writer );
            }
        }
    }
}