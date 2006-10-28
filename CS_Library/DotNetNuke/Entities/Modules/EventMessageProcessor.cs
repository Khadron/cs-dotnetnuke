using System;
using DotNetNuke.Framework;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Entities.Modules
{
    public class EventMessageProcessor : EventMessageProcessorBase
    {
        public override bool ProcessMessage( EventMessage message )
        {
            try
            {
                switch( message.Attributes["ProcessCommand"].ToString().ToUpper() )
                {
                    case "UPDATESUPPORTEDFEATURES":

                        UpdateSupportedFeatures( message );
                        break;
                    case "UPGRADEMODULE":

                        UpgradeModule( message );
                        break;
                    default:

                        break;
                }
            }
            catch( Exception ex )
            {
                message.ExceptionMessage = ex.Message;
                return false;
            }
            return true;
        }

        private void UpdateSupportedFeatures( EventMessage message )
        {
            object oObject = Reflection.CreateObject( message.Attributes["BusinessControllerClass"].ToString(), "" );
            UpdateSupportedFeatures( oObject, Convert.ToInt32( message.Attributes["DesktopModuleId"] ) );
        }

        private void UpdateSupportedFeatures( object oObject, int desktopModuleId )
        {
            int SupportedFeatures = 0;

            if( oObject is IPortable )
            {
                SupportedFeatures = SupportedFeatures + (int)DesktopModuleSupportedFeature.IsPortable;
            }

            if( oObject is ISearchable )
            {
                SupportedFeatures = SupportedFeatures + (int)DesktopModuleSupportedFeature.IsSearchable;
            }

            if( oObject is IUpgradeable )
            {
                SupportedFeatures = SupportedFeatures + (int)DesktopModuleSupportedFeature.IsUpgradeable;
            }

            DesktopModuleController oDesktopModuleController = new DesktopModuleController();
            DesktopModuleInfo oDesktopModule = oDesktopModuleController.GetDesktopModule( desktopModuleId );
            oDesktopModule.SupportedFeatures = SupportedFeatures;
            oDesktopModuleController.UpdateDesktopModule( oDesktopModule );
        }

        private void UpgradeModule( EventMessage message )
        {
            string BusinessController = message.Attributes["BusinessControllerClass"].ToString();
            object oObject = Reflection.CreateObject( BusinessController, BusinessController );
            EventLogController objEventLog = new EventLogController();
            LogInfo objEventLogInfo;
            if( oObject is IUpgradeable )
            {
                // get the list of applicable versions
                string[] UpgradeVersions = message.Attributes["UpgradeVersionsList"].ToString().Split( ",".ToCharArray() );
                foreach( string Version in UpgradeVersions )
                {
                    //call the IUpgradeable interface for the module/version
                    string upgradeResults = ( (IUpgradeable)oObject ).UpgradeModule( Version );
                    //log the upgrade results
                    objEventLogInfo = new LogInfo();
                    objEventLogInfo.AddProperty( "Module Upgraded", BusinessController );
                    objEventLogInfo.AddProperty( "Version", Version );
                    objEventLogInfo.AddProperty( "Results", upgradeResults );
                    objEventLogInfo.LogTypeKey = EventLogController.EventLogType.MODULE_UPDATED.ToString();
                    objEventLog.AddLog( objEventLogInfo );
                }
            }
            UpdateSupportedFeatures( oObject, Convert.ToInt32( message.Attributes["DesktopModuleId"] ) );
        }
    }
}