using System.Collections;

namespace DotNetNuke.Services.Vendors
{

    public class BannerTypeController
    {
        public ArrayList GetBannerTypes()
        {
            ArrayList arrBannerTypes = new ArrayList();

            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Banner, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Banner.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.MicroButton, DotNetNuke.Services.Localization.Localization.GetString("BannerType.MicroButton.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Button, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Button.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Block, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Block.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Skyscraper, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Skyscraper.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Text, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Text.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));
            arrBannerTypes.Add(new BannerTypeInfo((int)BannerType.Script, DotNetNuke.Services.Localization.Localization.GetString("BannerType.Script.String", DotNetNuke.Services.Localization.Localization.GlobalResourceFile)));

            return arrBannerTypes;
        }
    }
}