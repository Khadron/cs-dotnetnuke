using System;

namespace DotNetNuke.UI.WebControls
{
    public class DNNLabelEditEventArgs : EventArgs
    {
        private string _text;


        public DNNLabelEditEventArgs( string text )
        {
            this._text = text;
        }

        public string Text
        {
            get
            {
                return _text;
            }
        }
    }
}