using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNMenuEventArgs : EventArgs
    {
        private MenuNode _node;

        public DNNMenuEventArgs( MenuNode node )
        {
            this._node = node;
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