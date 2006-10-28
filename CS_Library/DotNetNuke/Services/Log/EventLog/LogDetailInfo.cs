using System;
using System.Xml.Serialization;

namespace DotNetNuke.Services.Log.EventLog
{
    [Serializable(), XmlRoot("property")]
    public class LogDetailInfo
    {
        private string _PropertyName;
        private string _PropertyValue;

        public LogDetailInfo()
            : this("", "")
        {
        }

        public LogDetailInfo(string name, string value)
        {
            _PropertyName = name;
            _PropertyValue = value;
        }

        [XmlElement("name")]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                _PropertyName = value;
            }
        }

        [XmlElement("value")]
        public string PropertyValue
        {
            get
            {
                return _PropertyValue;
            }
            set
            {
                _PropertyValue = value;
            }
        }

        public override string ToString()
        {
            return "<b>" + PropertyName + "</b>: " + PropertyValue + ";&nbsp;";
        }
    }
}