using System.Collections;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.UserControls
{
    public class ListItemComparer : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            ListItem a = (ListItem)x;
            ListItem b = (ListItem)y;
            CaseInsensitiveComparer c = new CaseInsensitiveComparer();
            return c.Compare(a.Text, b.Text);
        }
    }
}