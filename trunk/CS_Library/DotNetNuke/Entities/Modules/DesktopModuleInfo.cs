namespace DotNetNuke.Entities.Modules
{
    public class DesktopModuleInfo
    {
        private string _BusinessControllerClass;
        private string _Description;
        private int _DesktopModuleID;
        private string _FolderName;
        private string _FriendlyName;
        private bool _IsAdmin;
        private bool _IsPremium;
        private string _ModuleName;
        private int _SupportedFeatures;
        private string _Version;

        public string BusinessControllerClass
        {
            get
            {
                return this._BusinessControllerClass;
            }
            set
            {
                this._BusinessControllerClass = value;
            }
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public int DesktopModuleID
        {
            get
            {
                return this._DesktopModuleID;
            }
            set
            {
                this._DesktopModuleID = value;
            }
        }

        public string FolderName
        {
            get
            {
                return this._FolderName;
            }
            set
            {
                this._FolderName = value;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this._FriendlyName;
            }
            set
            {
                this._FriendlyName = value;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return this._IsAdmin;
            }
            set
            {
                this._IsAdmin = value;
            }
        }

        public bool IsPortable
        {
            get
            {
                return this.GetFeature( DesktopModuleSupportedFeature.IsPortable );
            }
            set
            {
                this.UpdateFeature( DesktopModuleSupportedFeature.IsPortable, value );
            }
        }

        public bool IsPremium
        {
            get
            {
                return this._IsPremium;
            }
            set
            {
                this._IsPremium = value;
            }
        }

        public bool IsSearchable
        {
            get
            {
                return this.GetFeature( DesktopModuleSupportedFeature.IsSearchable );
            }
            set
            {
                this.UpdateFeature( DesktopModuleSupportedFeature.IsSearchable, value );
            }
        }

        public bool IsUpgradeable
        {
            get
            {
                return this.GetFeature( DesktopModuleSupportedFeature.IsUpgradeable );
            }
            set
            {
                this.UpdateFeature( DesktopModuleSupportedFeature.IsUpgradeable, value );
            }
        }

        public string ModuleName
        {
            get
            {
                return this._ModuleName;
            }
            set
            {
                this._ModuleName = value;
            }
        }

        public int SupportedFeatures
        {
            get
            {
                return this._SupportedFeatures;
            }
            set
            {
                this._SupportedFeatures = value;
            }
        }

        public string Version
        {
            get
            {
                return this._Version;
            }
            set
            {
                this._Version = value;
            }
        }

        private void ClearFeature( DesktopModuleSupportedFeature Feature )
        {
            //And with the 1's complement of Feature to Clear the Feature flag
            this.SupportedFeatures = ( (int)( ( (DesktopModuleSupportedFeature)this.SupportedFeatures ) & ( ~ Feature ) ) );
        }

        private bool GetFeature( DesktopModuleSupportedFeature Feature )
        {
            bool isSet = false;
            //And with the Feature to see if the flag is set
            if( ( ( (DesktopModuleSupportedFeature)this.SupportedFeatures ) & Feature ) != Feature )
            {
                return isSet;
            }
            else
            {
                return true;
            }
        }

        private void SetFeature( DesktopModuleSupportedFeature Feature )
        {
            //Or with the Feature to Set the Feature flag
            this.SupportedFeatures = ( (int)( ( (DesktopModuleSupportedFeature)this.SupportedFeatures ) | Feature ) );
        }

        private void UpdateFeature( DesktopModuleSupportedFeature Feature, bool IsSet )
        {
            if( IsSet )
            {
                this.SetFeature( Feature );
                return;
            }
            this.ClearFeature( Feature );
        }
    }
}