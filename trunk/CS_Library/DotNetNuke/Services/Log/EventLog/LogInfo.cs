using System;
using System.Text;
using System.Xml.Serialization;

namespace DotNetNuke.Services.Log.EventLog
{
    [XmlRoot("log", IsNullable = false)]
    public class LogInfo
    {
        private bool _BypassBuffering;
        private string _LogConfigID;
        private DateTime _LogCreateDate;
        private long _LogCreateDateNum;
        private string _LogFileID;
        private string _LogGUID;
        private int _LogPortalID;
        private string _LogPortalName;
        private LogProperties _LogProperties;
        private string _LogServerName;
        private string _LogTypeKey;
        private int _LogUserID;
        private string _LogUserName;

        [XmlAttributeAttribute("BypassBuffering")]
        public bool BypassBuffering
        {
            get
            {
                return _BypassBuffering;
            }
            set
            {
                _BypassBuffering = value;
            }
        }

        [XmlAttributeAttribute("LogConfigID")]
        public string LogConfigID
        {
            get
            {
                return _LogConfigID;
            }
            set
            {
                _LogConfigID = value;
            }
        }

        [XmlAttributeAttribute("LogCreateDate")]
        public DateTime LogCreateDate
        {
            get
            {
                return _LogCreateDate;
            }
            set
            {
                _LogCreateDate = value;
            }
        }

        [XmlAttributeAttribute("LogCreateDateNum")]
        public long LogCreateDateNum
        {
            get
            {
                return _LogCreateDateNum;
            }
            set
            {
                _LogCreateDateNum = value;
            }
        }

        [XmlAttributeAttribute("LogFileID")]
        public string LogFileID
        {
            get
            {
                return _LogFileID;
            }
            set
            {
                _LogFileID = value;
            }
        }

        [XmlAttributeAttribute("LogGUID")]
        public string LogGUID
        {
            get
            {
                return _LogGUID;
            }
            set
            {
                _LogGUID = value;
            }
        }

        [XmlAttributeAttribute("LogPortalID")]
        public int LogPortalID
        {
            get
            {
                return _LogPortalID;
            }
            set
            {
                _LogPortalID = value;
            }
        }

        [XmlAttributeAttribute("LogPortalName")]
        public string LogPortalName
        {
            get
            {
                return _LogPortalName;
            }
            set
            {
                _LogPortalName = value;
            }
        }

        [XmlArray("properties"), XmlArrayItem(ElementName = "property", Type = typeof(LogDetailInfo))]
        public LogProperties LogProperties
        {
            get
            {
                return _LogProperties;
            }
            set
            {
                _LogProperties = value;
            }
        }

        [XmlAttributeAttribute("LogServerName")]
        public string LogServerName
        {
            get
            {
                return _LogServerName;
            }
            set
            {
                _LogServerName = value;
            }
        }

        [XmlAttributeAttribute("LogTypeKey")]
        public string LogTypeKey
        {
            get
            {
                return _LogTypeKey;
            }
            set
            {
                _LogTypeKey = value;
            }
        }

        [XmlAttributeAttribute("LogUserID")]
        public int LogUserID
        {
            get
            {
                return _LogUserID;
            }
            set
            {
                _LogUserID = value;
            }
        }

        [XmlAttributeAttribute("LogUserName")]
        public string LogUserName
        {
            get
            {
                return _LogUserName;
            }
            set
            {
                _LogUserName = value;
            }
        }

        public LogInfo()
        {
            _LogGUID = Guid.NewGuid().ToString();
            _BypassBuffering = false;
            LogProperties = new LogProperties();
            _LogPortalID = -1;
            _LogPortalName = "";
            _LogUserID = -1;
            _LogUserName = "";
        }

        public static bool IsSystemType(string PropName)
        {
            switch (PropName)
            {
                case "LogGUID":
                    return true;

                case "LogFileID":
                    return true;

                case "LogTypeKey":
                    return true;

                case "LogCreateDate":
                    return true;

                case "LogCreateDateNum":
                    return true;

                case "LogPortalID":
                    return true;

                case "LogPortalName":
                    return true;

                case "LogUserID":
                    return true;

                case "LogUserName":
                    return true;

                case "BypassBuffering":
                    return true;

                case "LogServerName":

                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("<b>LogGUID:</b> " + LogGUID + "<br>");
            str.Append("<b>LogType:</b> " + LogTypeKey + "<br>");
            str.Append("<b>UserID:</b> " + LogUserID.ToString() + "<br>");
            str.Append("<b>Username:</b> " + LogUserName + "<br>");
            str.Append("<b>PortalID:</b> " + LogPortalID.ToString() + "<br>");
            str.Append("<b>PortalName:</b> " + LogPortalName + "<br>");
            str.Append("<b>CreateDate:</b> " + LogCreateDate.ToString() + "<br>");
            str.Append("<b>ServerName:</b> " + LogServerName + "<br>");
            str.Append(LogProperties.ToString());
            return str.ToString();
        }

        public void AddProperty(string PropertyName, string PropertyValue)
        {
            try
            {
                if (PropertyName.Length > 50)
                {
                    PropertyName = PropertyName.Substring(0, 50);
                }
                if (PropertyValue.Length > 500)
                {
                    PropertyValue = "(TRUNCATED TO 500 CHARS): " + PropertyValue.Substring(0, 500);
                }
                LogDetailInfo objLogDetailInfo = new LogDetailInfo();
                objLogDetailInfo.PropertyName = PropertyName;
                objLogDetailInfo.PropertyValue = PropertyValue;
                _LogProperties.Add(objLogDetailInfo);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.ProcessPageLoadException(exc);
            }
        }
    }
}