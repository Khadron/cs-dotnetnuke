namespace DotNetNuke.Common.Utilities
{
    public class UrlInfo
    {
        private int _PortalID;
        private string _Url;
        private int _UrlID;

        public int PortalID
        {
            get
            {
                return this._PortalID;
            }
            set
            {
                this._PortalID = value;
            }
        }

        public string Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }

        public int UrlID
        {
            get
            {
                return this._UrlID;
            }
            set
            {
                this._UrlID = value;
            }
        }
    }
}