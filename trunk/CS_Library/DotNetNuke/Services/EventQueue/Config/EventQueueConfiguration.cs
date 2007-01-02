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
using System.IO;
using System.Text;
using System.Web.Caching;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.EventQueue.Config
{
    public class EventQueueConfiguration
    {
        private EventQueueSubscribers _eventQueueSubscribers;
        private PublishedEvents _publishedEvents;

        public EventQueueSubscribers EventQueueSubscribers
        {
            get
            {
                return _eventQueueSubscribers;
            }
            set
            {
                _eventQueueSubscribers = value;
            }
        }

        public PublishedEvents PublishedEvents
        {
            get
            {
                return _publishedEvents;
            }
            set
            {
                _publishedEvents = value;
            }
        }

        public EventQueueConfiguration()
        {
            _publishedEvents = new PublishedEvents();
            _eventQueueSubscribers = new EventQueueSubscribers();
        }

        public static EventQueueConfiguration GetConfig()
        {
            EventQueueConfiguration config = (EventQueueConfiguration)DataCache.GetCache("EventQueueConfig");

            if (config == null)
            {
                string filePath = Globals.HostMapPath + "EventQueue\\EventQueue.config";
                if (File.Exists(filePath))
                {
                    config = new EventQueueConfiguration();
                    // Deserialize into EventQueueConfiguration
                    StreamReader oStreamReader = File.OpenText(filePath);
                    config.Deserialize(oStreamReader.ReadToEnd());
                    oStreamReader.Close();
                }
                else
                {
                    //make a default config file
                    SubscriberInfo si = new SubscriberInfo("DNN Core");
                    PublishedEvent e = new PublishedEvent();
                    e.EventName = "Application_Start";
                    e.Subscribers = si.ID.ToString();
                    config = new EventQueueConfiguration();
                    config.PublishedEvents = new PublishedEvents();
                    config.PublishedEvents.Add(e);
                    config.EventQueueSubscribers = new EventQueueSubscribers();
                    config.EventQueueSubscribers.Add(si);
                    StreamWriter oStream = File.CreateText(filePath);
                    oStream.WriteLine(config.Serialize());
                    oStream.Close();
                }
                if (File.Exists(filePath))
                {
                    // Create a dependency on the config file
                    CacheDependency dep = new CacheDependency(filePath);

                    // Set back into Cache
                    DataCache.SetCache("EventQueueConfig", config, dep);
                }
            }

            return config;
        }

        public string Serialize()
        {
            StringBuilder configXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            configXML.Append("\r\n");
            configXML.Append("<EventQueueConfig>");
            configXML.Append("\r\n");
            configXML.Append('\t');
            configXML.Append("<PublishedEvents>");
            configXML.Append("\r\n");
            foreach (string key in this.PublishedEvents.Keys)
            {
                configXML.Append('\t', 2);
                configXML.Append("<Event>");
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                configXML.Append("<EventName>");
                configXML.Append(this.PublishedEvents[key].EventName);
                configXML.Append("</EventName>");
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                if (this.PublishedEvents[key].Subscribers.Length > 0)
                {
                    configXML.Append("<Subscribers>");
                    configXML.Append(this.PublishedEvents[key].Subscribers);
                    configXML.Append("</Subscribers>");
                }
                else
                {
                    configXML.Append("<Subscribers/>");
                }
                configXML.Append("\r\n");
                configXML.Append('\t', 2);
                configXML.Append("</Event>");
                configXML.Append("\r\n");
            }
            configXML.Append('\t');
            configXML.Append("</PublishedEvents>");
            configXML.Append("\r\n");
            configXML.Append('\t');
            configXML.Append("<EventQueueSubscribers>");
            configXML.Append("\r\n");

            foreach (string key in this.EventQueueSubscribers.Keys)
            {
                configXML.Append('\t', 2);
                configXML.Append("<Subscriber>");
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                configXML.Append("<ID>");
                configXML.Append(this.EventQueueSubscribers[key].ID);
                configXML.Append("</ID>");
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                if (this.EventQueueSubscribers[key].Name.Length > 0)
                {
                    configXML.Append("<Name>");
                    configXML.Append(this.EventQueueSubscribers[key].Name);
                    configXML.Append("</Name>");
                }
                else
                {
                    configXML.Append("<Name/>");
                }
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                if (this.EventQueueSubscribers[key].Address.Length > 0)
                {
                    configXML.Append("<Address>");
                    configXML.Append(this.EventQueueSubscribers[key].Address);
                    configXML.Append("</Address>");
                }
                else
                {
                    configXML.Append("<Address/>");
                }
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                if (this.EventQueueSubscribers[key].Description.Length > 0)
                {
                    configXML.Append("<Description>");
                    configXML.Append(this.EventQueueSubscribers[key].Description);
                    configXML.Append("</Description>");
                }
                else
                {
                    configXML.Append("<Description/>");
                }
                configXML.Append("\r\n");
                configXML.Append('\t', 3);
                if (this.EventQueueSubscribers[key].PrivateKey.Length > 0)
                {
                    configXML.Append("<PrivateKey>");
                    configXML.Append(this.EventQueueSubscribers[key].PrivateKey);
                    configXML.Append("</PrivateKey>");
                }
                else
                {
                    configXML.Append("<PrivateKey/>");
                }
                configXML.Append("\r\n");
                configXML.Append('\t', 2);
                configXML.Append("</Subscriber>");
                configXML.Append("\r\n");
            }
            configXML.Append('\t');
            configXML.Append("</EventQueueSubscribers>");
            configXML.Append("\r\n");
            configXML.Append("</EventQueueConfig>");
            return configXML.ToString();
        }

        public void Deserialize(string configXml)
        {
            if (!String.IsNullOrEmpty(configXml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(configXml);
                foreach (XmlElement xmlItem in xmlDoc.SelectNodes("/EventQueueConfig/PublishedEvents/Event"))
                {
                    PublishedEvent oPublishedEvent = new PublishedEvent();
                    oPublishedEvent.EventName = xmlItem.SelectSingleNode("EventName").InnerText;
                    oPublishedEvent.Subscribers = xmlItem.SelectSingleNode("Subscribers").InnerText;
                    this.PublishedEvents.Add(oPublishedEvent);
                }
                foreach (XmlElement xmlItem in xmlDoc.SelectNodes("/EventQueueConfig/EventQueueSubscribers/Subscriber"))
                {
                    SubscriberInfo oSubscriberInfo = new SubscriberInfo();
                    oSubscriberInfo.ID = xmlItem.SelectSingleNode("ID").InnerText;
                    oSubscriberInfo.Name = xmlItem.SelectSingleNode("Name").InnerText;
                    oSubscriberInfo.Address = xmlItem.SelectSingleNode("Address").InnerText;
                    oSubscriberInfo.Description = xmlItem.SelectSingleNode("Description").InnerText;
                    oSubscriberInfo.PrivateKey = xmlItem.SelectSingleNode("PrivateKey").InnerText;
                    this.EventQueueSubscribers.Add(oSubscriberInfo);
                }
            }
        }
    }
}