namespace DotNetNuke.Entities.Portals
{
    public class PortalAliasInfo
    {
        private string _HTTPAlias;
        private int _PortalAliasID;
        private int _PortalID;

        public string HTTPAlias
        {
            get
            {
                return this._HTTPAlias;
            }
            set
            {
                this._HTTPAlias = value;
            }
        }

        public int PortalAliasID
        {
            get
            {
                return this._PortalAliasID;
            }
            set
            {
                this._PortalAliasID = value;
            }
        }

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
    }
}