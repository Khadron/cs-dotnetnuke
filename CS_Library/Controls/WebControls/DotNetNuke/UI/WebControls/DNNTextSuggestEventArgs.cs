using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNTextSuggestEventArgs : EventArgs
    {
        private DNNNodeCollection _nodes;
        private string _text;

        public DNNTextSuggestEventArgs( DNNNodeCollection nodes, string text )
        {
            this._nodes = nodes;
            this._text = text;
        }

        public DNNNodeCollection Nodes
        {
            get
            {
                return this._nodes;
            }
        }

        public string Text
        {
            get
            {
                return this._text;
            }
        }
    }
}