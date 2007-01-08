#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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