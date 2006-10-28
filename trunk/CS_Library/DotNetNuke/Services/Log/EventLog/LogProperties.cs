using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace DotNetNuke.Services.Log.EventLog
{
    [Serializable(), XmlInclude(typeof(LogDetailInfo))]
    public class LogProperties : ArrayList
    {
        public string Summary
        {
            get
            {
                return this.ToString().Substring(0, 75);
            }
        }

        public override string ToString()
        {
            StringBuilder t = new StringBuilder();
            int i;
            for (i = 0; i <= this.Count - 1; i++)
            {
                LogDetailInfo ldi = (LogDetailInfo)this[i];
                t.Append(ldi.ToString());
            }
            return t.ToString();
        }
    }
}