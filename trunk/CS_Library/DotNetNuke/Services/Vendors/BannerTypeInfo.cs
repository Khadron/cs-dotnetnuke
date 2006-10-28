namespace DotNetNuke.Services.Vendors
{
    public class BannerTypeInfo
    {
        private int _BannerTypeId;
        private string _BannerTypeName;

        public BannerTypeInfo( int BannerTypeId, string BannerTypeName )
        {
            this._BannerTypeId = BannerTypeId;
            this._BannerTypeName = BannerTypeName;
        }

        public BannerTypeInfo()
        {
        }

        public int BannerTypeId
        {
            get
            {
                return this._BannerTypeId;
            }
            set
            {
                this._BannerTypeId = value;
            }
        }

        public string BannerTypeName
        {
            get
            {
                return this._BannerTypeName;
            }
            set
            {
                this._BannerTypeName = value;
            }
        }
    }
}