using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Host
{
    public class HostSettingsController
    {
        public IDataReader GetHostSetting( string SettingName )
        {
            return DataProvider.Instance().GetHostSetting( SettingName );
        }

        public IDataReader GetHostSettings()
        {
            return DataProvider.Instance().GetHostSettings();
        }

        public void UpdateHostSetting( string SettingName, string SettingValue )
        {
            UpdateHostSetting( SettingName, SettingValue, false );
        }

        public void UpdateHostSetting( string SettingName, string SettingValue, bool SettingIsSecure )
        {
            IDataReader dr = DataProvider.Instance().GetHostSetting( SettingName );
            if( dr.Read() )
            {
                DataProvider.Instance().UpdateHostSetting( SettingName, SettingValue, SettingIsSecure );
            }
            else
            {
                DataProvider.Instance().AddHostSetting( SettingName, SettingValue, SettingIsSecure );
            }
            dr.Close();

            // clear host cache
            DataCache.ClearHostCache( false );
        }
    }
}