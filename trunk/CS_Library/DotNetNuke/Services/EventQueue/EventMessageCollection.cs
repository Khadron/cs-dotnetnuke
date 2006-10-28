using System.Collections;

namespace DotNetNuke.Services.EventQueue
{
    public class EventMessageCollection : CollectionBase
    {
        public void Add(EventMessage message)
        {
            this.InnerList.Add(message);
        }

        public virtual EventMessage this[int index]
        {
            get
            {
                return ((EventMessage)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
    }
}