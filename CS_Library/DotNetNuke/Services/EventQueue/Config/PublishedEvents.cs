using System;
using System.Collections;
using System.Xml.Serialization;

namespace DotNetNuke.Services.EventQueue.Config
{
    [Serializable(), XmlInclude(typeof(PublishedEvent))]
    public class PublishedEvents : Hashtable
    {
        public void Add(PublishedEvent pe)
        {
            this.Add(pe.EventName, pe);
        }

        public virtual PublishedEvent this[string key]
        {
            get
            {
                return ((PublishedEvent)base[key]);
            }
            set
            {
                base[key] = value;
            }
        }
    }
}