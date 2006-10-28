using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNTreeNodeClickEventArgs : EventArgs
    {
        private TreeNode _node;

        public DNNTreeNodeClickEventArgs( TreeNode Node )
        {
            this._node = Node;
        }

        public TreeNode Node
        {
            get
            {
                return this._node;
            }
        }
    }
}