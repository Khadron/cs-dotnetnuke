using System;
using System.Xml.Serialization;

namespace DotNetNuke.Services.EventQueue.Config
{
    [Serializable(), XmlRoot( "PublishedEvent" )]
    public class PublishedEvent
    {
        private string _eventName;
        private string _subscribers;

        public string EventName
        {
            get
            {
                return this._eventName;
            }
            set
            {
                this._eventName = value;
            }
        }

        public string Subscribers
        {
            get
            {
                return this._subscribers;
            }
            set
            {
                this._subscribers = value;
            }
        }
    }
}