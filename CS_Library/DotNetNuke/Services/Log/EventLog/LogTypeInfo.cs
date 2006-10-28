namespace DotNetNuke.Services.Log.EventLog
{
    public class LogTypeInfo
    {
        private string _LogTypeCSSClass;
        private string _LogTypeDescription;
        private string _LogTypeFriendlyName;
        private string _LogTypeKey;
        private string _LogTypeOwner;

        public string LogTypeCSSClass
        {
            get
            {
                return this._LogTypeCSSClass;
            }
            set
            {
                this._LogTypeCSSClass = value;
            }
        }

        public string LogTypeDescription
        {
            get
            {
                return this._LogTypeDescription;
            }
            set
            {
                this._LogTypeDescription = value;
            }
        }

        public string LogTypeFriendlyName
        {
            get
            {
                return this._LogTypeFriendlyName;
            }
            set
            {
                this._LogTypeFriendlyName = value;
            }
        }

        public string LogTypeKey
        {
            get
            {
                return this._LogTypeKey;
            }
            set
            {
                this._LogTypeKey = value;
            }
        }

        public string LogTypeOwner
        {
            get
            {
                return this._LogTypeOwner;
            }
            set
            {
                this._LogTypeOwner = value;
            }
        }
    }
}