using System.Collections;
using System.Xml.Serialization;

namespace DotNetNuke.Services.EventQueue.Config
{
    [XmlInclude(typeof(SubscriberInfo))]
    public class EventQueueSubscribers : Hashtable
    {
        public void Add(SubscriberInfo es)
        {
            this.Add(es.ID, es);
        }

        public virtual SubscriberInfo this[string key]
        {
            get
            {
                return ((SubscriberInfo)base[key]);
            }
            set
            {
                base[key] = value;
            }
        }
    }
}