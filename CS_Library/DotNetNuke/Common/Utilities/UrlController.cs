using System;
using System.Collections;
using DotNetNuke.Data;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Common.Utilities
{
    public class UrlController
    {

        public UrlInfo GetUrl( int PortalID, string Url )
        {
            return ( (UrlInfo)CBO.FillObject( DataProvider.Instance().GetUrl( PortalID, Url ), typeof( UrlInfo ) ) );
        }

        public ArrayList GetUrlLog( int PortalID, string Url, int ModuleId, DateTime StartDate, DateTime EndDate )
        {
            ArrayList arrUrlLog = null;

            UrlTrackingInfo objUrlTracking = GetUrlTracking( PortalID, Url, ModuleId );
            if( objUrlTracking != null )
            {
                arrUrlLog = CBO.FillCollection( DataProvider.Instance().GetUrlLog( objUrlTracking.UrlTrackingID, StartDate, EndDate ), typeof( UrlLogInfo ) );
            }

            return arrUrlLog;
        }
        public ArrayList GetUrls( int PortalID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetUrls( PortalID ), typeof( UrlInfo ) );
        }

        public UrlTrackingInfo GetUrlTracking( int PortalID, string Url, int ModuleId )
        {
            return ( (UrlTrackingInfo)CBO.FillObject( DataProvider.Instance().GetUrlTracking( PortalID, Url, ModuleId ), typeof( UrlTrackingInfo ) ) );
        }

        public void DeleteUrl( int PortalID, string Url )
        {
            DataProvider.Instance().DeleteUrl( PortalID, Url );
        }

        public void UpdateUrl( int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow )
        {
            UpdateUrl( PortalID, Url, UrlType, 0, Null.NullDate, Null.NullDate, LogActivity, TrackClicks, ModuleID, NewWindow );
        }

        public void UpdateUrl( int PortalID, string Url, string UrlType, int Clicks, DateTime LastClick, DateTime CreatedDate, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow )
        {
            if( Url != "" )
            {
                if( UrlType == "U" )
                {
                    if( GetUrl( PortalID, Url ) == null )
                    {
                        DataProvider.Instance().AddUrl( PortalID, Url );
                    }
                }

                UrlTrackingInfo objURLTracking = GetUrlTracking( PortalID, Url, ModuleID );
                if( objURLTracking == null )
                {
                    DataProvider.Instance().AddUrlTracking( PortalID, Url, UrlType, LogActivity, TrackClicks, ModuleID, NewWindow );
                }
                else
                {
                    DataProvider.Instance().UpdateUrlTracking( PortalID, Url, LogActivity, TrackClicks, ModuleID, NewWindow );
                }
            }
        }

        public void UpdateUrlTracking( int PortalID, string Url, int ModuleId, int UserID )
        {
            TabType UrlType = Globals.GetURLType( Url );
            if( UrlType == TabType.File && Url.ToLower().StartsWith( "fileid=" ) == false )
            {
                // to handle legacy scenarios before the introduction of the FileServerHandler
                FileController objFiles = new FileController();
                Url = "FileID=" + objFiles.ConvertFilePathToFileId( Url, PortalID );
            }

            UrlTrackingInfo objUrlTracking = GetUrlTracking( PortalID, Url, ModuleId );
            if( objUrlTracking != null )
            {
                DataProvider.Instance().UpdateUrlTrackingStats( PortalID, Url, ModuleId );
                if( objUrlTracking.LogActivity )
                {
                    DataProvider.Instance().AddUrlLog( objUrlTracking.UrlTrackingID, UserID );
                }
            }
        }
    }
}