using System;
using System.Collections;

namespace DotNetNuke.HttpModules.Config
{
    [Serializable()]
    public class RewriterRuleCollection : CollectionBase
    {
        public void Add( RewriterRule r )
        {
            this.InnerList.Add( r );
        }

        public virtual RewriterRule this[ int index ]
        {
            get
            {
                return ( (RewriterRule)List[index] );
            }
            set
            {
                List[index] = value;
            }
        }
    }
}