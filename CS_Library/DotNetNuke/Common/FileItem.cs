namespace DotNetNuke.Common
{
    public class FileItem
    {
        private string _Text;
        private string _Value;

        public FileItem( string Value, string Text )
        {
            this._Value = Value;
            this._Text = Text;
        }

        public string Text
        {
            get
            {
                return this._Text;
            }
        }

        public string Value
        {
            get
            {
                return this._Value;
            }
        }
    }
}