namespace DotNetNuke.UI.Skins
{
    /// <Summary>Handles the Business Object for Skins</Summary>
    public class SkinInfo
    {
        private int _PortalId;
        private int _SkinId;
        private string _SkinRoot;
        private string _SkinSrc;
        private SkinType _SkinType;

        public int PortalId
        {
            get
            {
                return this._PortalId;
            }
            set
            {
                this._PortalId = value;
            }
        }

        public static string RootContainer
        {
            get
            {
                return "Containers";
            }
        }

        public static string RootSkin
        {
            get
            {
                return "Skins";
            }
        }

        public int SkinId
        {
            get
            {
                return this._SkinId;
            }
            set
            {
                this._SkinId = value;
            }
        }

        public string SkinRoot
        {
            get
            {
                return this._SkinRoot;
            }
            set
            {
                this._SkinRoot = value;
            }
        }

        public string SkinSrc
        {
            get
            {
                return this._SkinSrc;
            }
            set
            {
                this._SkinSrc = value;
            }
        }

        public SkinType SkinType
        {
            get
            {
                return this._SkinType;
            }
            set
            {
                this._SkinType = value;
            }
        }
    }
}