#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Log.EventLog
{
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
            _LogProperties = new LogProperties();
            _LogPortalID = -1;
            _LogPortalName = "";
            _LogUserID = -1;
            _LogUserName = "";
        }
        
        public LogInfo(string content) : this()
        {
            this.Deserialize(content);
        }

        public void Deserialize(string content)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                if (reader.Read())
                {
                    ReadXml(reader);
                }
                reader.Close();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    switch (reader.Name)
                    {
                        case "LogGUID":
                            LogGUID = reader.ReadContentAsString();
                            break;
                        case "LogFileID":
                            LogFileID = reader.ReadContentAsString();
                            break;
                        case "LogTypeKey":
                            LogTypeKey = reader.ReadContentAsString();
                            break;
                        case "LogUserID":
                            LogUserID = reader.ReadContentAsInt();
                            break;
                        case "LogUserName":
                            LogUserName = reader.ReadContentAsString();
                            break;
                        case "LogPortalID":
                            LogPortalID = reader.ReadContentAsInt();
                            break;
                        case "LogPortalName":
                            LogPortalName = reader.ReadContentAsString();
                            break;
                        case "LogCreateDate":
                            LogCreateDate = System.DateTime.Parse(reader.ReadContentAsString());
                            break;
                        case "LogCreateDateNum":
                            LogCreateDateNum = reader.ReadContentAsLong();
                            break;
                        case "BypassBuffering":
                            BypassBuffering = bool.Parse(reader.ReadContentAsString());
                            break;
                        case "LogServerName":
                            LogServerName = reader.ReadContentAsString();
                            break;
                        case "LogConfigID":
                            LogConfigID = reader.ReadContentAsString();
                            break;
                    }
                }
            }

            //Check for LogProperties child node
            reader.Read();
            if (reader.NodeType == XmlNodeType.Element & reader.LocalName == "LogProperties")
            {
                reader.ReadStartElement("LogProperties");
                if (reader.ReadState != ReadState.EndOfFile & reader.NodeType != XmlNodeType.None & reader.LocalName != "")
                {
                    LogProperties.ReadXml(reader);
                }
            }
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

        public string Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;

            StringBuilder sb = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(sb, settings);
            WriteXml(writer);
            writer.Close();

            return sb.ToString();

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

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("log");
            writer.WriteAttributeString("LogGUID", LogGUID);
            writer.WriteAttributeString("LogFileID", LogFileID);
            writer.WriteAttributeString("LogTypeKey", LogTypeKey);
            writer.WriteAttributeString("LogUserID", LogUserID.ToString());
            writer.WriteAttributeString("LogUserName", LogUserName);
            writer.WriteAttributeString("LogPortalID", LogPortalID.ToString());
            writer.WriteAttributeString("LogPortalName", LogPortalName);
            writer.WriteAttributeString("LogCreateDate", LogCreateDate.ToString());
            writer.WriteAttributeString("LogCreateDateNum", LogCreateDateNum.ToString());
            writer.WriteAttributeString("BypassBuffering", BypassBuffering.ToString());
            writer.WriteAttributeString("LogServerName", LogServerName);
            writer.WriteAttributeString("LogConfigID", LogConfigID);

            LogProperties.WriteXml(writer);

            writer.WriteEndElement();

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