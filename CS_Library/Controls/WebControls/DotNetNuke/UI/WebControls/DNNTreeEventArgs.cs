using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNTreeEventArgs : EventArgs
    {
        private TreeNode _node;

        public DNNTreeEventArgs( TreeNode node )
        {
            this._node = node;
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