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
using System;
using System.Collections;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Tabs;

namespace DotNetNuke.Services.Vendors
{
    public class BannerController
    {
        public ArrayList GetBanners(int VendorId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetBanners(VendorId), typeof(BannerInfo));
        }

        public BannerInfo GetBanner(int BannerId, int VendorId, int PortalId)
        {
            return ((BannerInfo)CBO.FillObject(DataProvider.Instance().GetBanner(BannerId, VendorId, PortalId), typeof(BannerInfo)));
        }

        public void DeleteBanner(int BannerId)
        {
            DataProvider.Instance().DeleteBanner(BannerId);

            ClearBannerCache();
        }

        public void AddBanner(BannerInfo objBannerInfo)
        {
            DataProvider.Instance().AddBanner(objBannerInfo.BannerName, objBannerInfo.VendorId, objBannerInfo.ImageFile, objBannerInfo.URL, objBannerInfo.Impressions, objBannerInfo.CPM, objBannerInfo.StartDate, objBannerInfo.EndDate, objBannerInfo.CreatedByUser, objBannerInfo.BannerTypeId, objBannerInfo.Description, objBannerInfo.GroupName, objBannerInfo.Criteria, objBannerInfo.Width, objBannerInfo.Height);

            ClearBannerCache();
        }

        public void UpdateBanner(BannerInfo objBannerInfo)
        {
            DataProvider.Instance().UpdateBanner(objBannerInfo.BannerId, objBannerInfo.BannerName, objBannerInfo.ImageFile, objBannerInfo.URL, objBannerInfo.Impressions, objBannerInfo.CPM, objBannerInfo.StartDate, objBannerInfo.EndDate, objBannerInfo.CreatedByUser, objBannerInfo.BannerTypeId, objBannerInfo.Description, objBannerInfo.GroupName, objBannerInfo.Criteria, objBannerInfo.Width, objBannerInfo.Height);

            ClearBannerCache();
        }

        private void ClearBannerCache()
        {
            DictionaryEntry objDictionaryEntry;
            foreach (DictionaryEntry tempLoopVar_objDictionaryEntry in HttpRuntime.Cache)
            {
                objDictionaryEntry = tempLoopVar_objDictionaryEntry;
                if (Convert.ToString(objDictionaryEntry.Key).StartsWith("Banners:"))
                {
                    DataCache.RemoveCache(Convert.ToString(objDictionaryEntry.Key));
                }
            }
        }

        public ArrayList LoadBanners(int PortalId, int ModuleId, int BannerTypeId, string GroupName, int Banners)
        {
            ArrayList returnValue;

            ArrayList arrBanners;
            BannerInfo objBanner;
            int intCounter;
            bool blnValid;

            if (GroupName == null)
            {
                GroupName = Null.NullString;
            }

            // cache key
            string strCacheKey = "Banners:" + PortalId.ToString() + ":" + BannerTypeId.ToString() + ":" + GroupName.ToString() + ":";

            // get banners
            arrBanners = (ArrayList)DataCache.GetCache(strCacheKey + "ArrayList");
            if (arrBanners == null)
            {
                arrBanners = CBO.FillCollection(DataProvider.Instance().FindBanners(PortalId, BannerTypeId, GroupName), typeof(BannerInfo));
                DataCache.SetCache(strCacheKey + "ArrayList", arrBanners);
            }

            // create return collection
            returnValue = new ArrayList(Banners);

            if (arrBanners.Count > 0)
            {
                if (Banners > arrBanners.Count)
                {
                    Banners = arrBanners.Count;
                }

                // get last index for rotation
                int intLastBannerIndex = 0;
                object objLastBannerIndex = DataCache.GetCache(strCacheKey + "LastIndex");
                if (objLastBannerIndex != null)
                {
                    intLastBannerIndex = Convert.ToInt32(objLastBannerIndex);
                }

                intCounter = 1;
                while (intCounter <= arrBanners.Count && returnValue.Count != Banners)
                {
                    // manage the rotation
                    intLastBannerIndex++;
                    if (intLastBannerIndex > (arrBanners.Count - 1))
                    {
                        intLastBannerIndex = 0;
                    }

                    // get the banner object
                    objBanner = (BannerInfo)arrBanners[intLastBannerIndex];

                    // check criteria
                    blnValid = true;
                    if (Null.IsNull(objBanner.StartDate) == false && DateTime.Now < objBanner.StartDate)
                    {
                        blnValid = false;
                    }
                    if (blnValid)
                    {
                        switch (objBanner.Criteria)
                        {
                            case 0: // AND = cancel the banner when the Impressions expire

                                if (objBanner.Impressions < objBanner.Views && objBanner.Impressions != 0)
                                {
                                    blnValid = false;
                                }
                                break;
                            case 1: // OR = cancel the banner if either the EndDate OR Impressions expire

                                if ((objBanner.Impressions < objBanner.Views && objBanner.Impressions != 0) || (DateTime.Now > objBanner.EndDate && Null.IsNull(objBanner.EndDate) == false))
                                {
                                    blnValid = false;
                                }
                                break;
                        }
                    }

                    // add to return collection
                    if (blnValid)
                    {
                        returnValue.Add(objBanner);

                        // update banner ( these values are persisted to the cache )
                        objBanner.Views++;
                        if (Null.IsNull(objBanner.StartDate))
                        {
                            objBanner.StartDate = DateTime.Now;
                        }
                        if (Null.IsNull(objBanner.EndDate) && objBanner.Views >= objBanner.Impressions && objBanner.Impressions != 0)
                        {
                            objBanner.EndDate = DateTime.Now;
                        }
                        // update database
                        DataProvider.Instance().UpdateBannerViews(objBanner.BannerId, objBanner.StartDate, objBanner.EndDate);
                    }

                    intCounter++;
                }

                // save last index for rotation
                DataCache.SetCache(strCacheKey + "LastIndex", intLastBannerIndex);
            }

            return returnValue;
        }

        public void UpdateBannerClickThrough(int BannerId, int VendorId)
        {
            DataProvider.Instance().UpdateBannerClickThrough(BannerId, VendorId);
        }

        public string FormatBanner(int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height, string BannerSource, string HomeDirectory)
        {
            string strBanner = "";

            string strWindow = "_new";
            if (Globals.GetURLType(URL) == TabType.Tab)
            {
                strWindow = "_self";
            }

            string strURL = "";
            if (BannerId != -1)
            {
                strURL = Globals.ApplicationPath + "/Admin/Vendors/BannerClickThrough.aspx?BannerId=" + BannerId.ToString() + "&VendorId=" + VendorId.ToString();
            }
            else
            {
                strURL = URL;
            }

            switch (BannerTypeId)
            {
                case (int)BannerType.Text:

                    strBanner += "<a href=\"" + strURL + "\" class=\"NormalBold\" target=\"" + strWindow + "\"><u>" + BannerName + "</u></a><br>";
                    strBanner += "<span class=\"Normal\">" + Description + "</span><br>";
                    if (ImageFile != "")
                    {
                        URL = ImageFile;
                    }
                    if (URL.IndexOf("://") != -1)
                    {
                        URL = URL.Substring(URL.IndexOf("://") + 3);
                    }
                    strBanner += "<a href=\"" + strURL + "\" class=\"NormalRed\" target=\"" + strWindow + "\">" + URL + "</a>";
                    break;
                case (int)BannerType.Script:

                    strBanner += Description;
                    break;
                default:

                    if (ImageFile.IndexOf("://") == -1 && ImageFile.StartsWith("/") == false)
                    {
                        if (ImageFile.ToLower().IndexOf(".swf") == -1)
                        {
                            strBanner += "<a href=\"" + strURL + "\" target=\"" + strWindow + "\">";
                            switch (BannerSource)
                            {
                                case "L": // local

                                    strBanner += FormatImage(HomeDirectory + ImageFile, Width, Height, BannerName, Description);
                                    break;
                                case "G": // global

                                    strBanner += FormatImage(Globals.HostPath + ImageFile, Width, Height, BannerName, Description);
                                    break;
                            }
                            strBanner += "</a>";
                        }
                        else // flash
                        {
                            switch (BannerSource)
                            {
                                case "L": // local

                                    strBanner += FormatFlash(HomeDirectory + ImageFile, Width, Height);
                                    break;
                                case "G": // global

                                    strBanner += FormatFlash(Globals.HostPath + ImageFile, Width, Height);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (ImageFile.ToLower().IndexOf(".swf") == -1)
                        {
                            strBanner += "<a href=\"" + strURL + "\" target=\"" + strWindow + "\">";
                            strBanner += FormatImage(ImageFile, Width, Height, BannerName, Description);
                            strBanner += "</a>";
                        }
                        else // flash
                        {
                            strBanner += FormatFlash(ImageFile, Width, Height);
                        }
                    }
                    break;
            }

            return strBanner;
        }

        private string FormatImage(string File, int Width, int Height, string BannerName, string Description)
        {
            string Image = "";

            Image += "<img src=\"" + File + "\" border=\"0\"";
            if (Description != "")
            {
                Image += " alt=\"" + Description + "\"";
            }
            else
            {
                Image += " alt=\"" + BannerName + "\"";
            }
            if (Width > 0)
            {
                Image += " width=\"" + Width.ToString() + "\"";
            }
            if (Height > 0)
            {
                Image += " height=\"" + Height.ToString() + "\"";
            }
            Image += "\">";

            return Image;
        }

        private string FormatFlash(string File, int Width, int Height)
        {
            string Flash = "";

            Flash += "<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=4,0,2,0\" width=\"" + Width.ToString() + "\" height=\"" + Height.ToString() + "\">";
            Flash += "<param name=movie value=\"" + File + "\">";
            Flash += "<param name=quality value=high>";
            Flash += "<embed src=\"" + File + "\" quality=high pluginspage=\"http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash\" type=\"application/x-shockwave-flash\" width=\"" + Width.ToString() + "\" height=\"" + Height.ToString() + "\">";
            Flash += "</embed>";
            Flash += "</object>";

            return Flash;
        }
    }
}