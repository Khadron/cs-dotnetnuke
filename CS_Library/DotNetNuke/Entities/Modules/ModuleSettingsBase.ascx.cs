using System.Collections;
using System.ComponentModel;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleSettingsBase : PortalModuleBase
    {
        private int _moduleId;
        private Hashtable _moduleSettings;
        private Hashtable _settings;
        private int _tabModuleId;
        private Hashtable _tabModuleSettings;

        public new int ModuleId
        {
            get
            {
                return _moduleId;
            }
            set
            {
                _moduleId = value;
            }
        }

        public Hashtable ModuleSettings
        {
            get
            {
                if (_moduleSettings == null)
                {
                    //Get ModuleSettings
                    _moduleSettings = Portals.PortalSettings.GetModuleSettings(ModuleId);
                }
                return _moduleSettings;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Hashtable Settings
        {
            get
            {
                if (_settings == null)
                {
                    //Merge the TabModuleSettings and ModuleSettings
                    _settings = Portals.PortalSettings.GetTabModuleSettings(new Hashtable(ModuleSettings), new Hashtable(TabModuleSettings));
                }
                return _settings;
            }
        }

        public new int TabModuleId
        {
            get
            {
                return _tabModuleId;
            }
            set
            {
                _tabModuleId = value;
            }
        }

        public Hashtable TabModuleSettings
        {
            get
            {
                if (_tabModuleSettings == null)
                {
                    //Get TabModuleSettings
                    _tabModuleSettings = Portals.PortalSettings.GetTabModuleSettings(TabModuleId);
                }
                return _tabModuleSettings;
            }
        }

        public virtual void LoadSettings()
        {
        }

        public virtual void UpdateSettings()
        {
        }
    }
}