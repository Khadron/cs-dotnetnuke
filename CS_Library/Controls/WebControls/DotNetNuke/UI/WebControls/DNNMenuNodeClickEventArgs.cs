using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNMenuNodeClickEventArgs : EventArgs
    {
        private MenuNode _node;

        public DNNMenuNodeClickEventArgs( MenuNode Node )
        {
            this._node = Node;
        }

        public MenuNode Node
        {
            get
            {
                return this._node;
            }
        }
    }
}