using System;
using System.Collections;
using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    public class DNNTreeBuilder : ControlBuilder
    {
        public override Type GetChildControlType( string tagName, IDictionary attribs )
        {
            if( tagName.ToUpper().EndsWith( "TreeNode" ) )
            {
                return typeof( TreeNode );
            }
            return null;
        }
    }
}