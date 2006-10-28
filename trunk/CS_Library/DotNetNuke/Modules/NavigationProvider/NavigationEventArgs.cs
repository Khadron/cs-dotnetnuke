using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.NavigationProvider
{
    public class NavigationEventArgs
    {
        public string ID;
        public DNNNode Node;

        public NavigationEventArgs( string strID, DNNNode objNode )
        {
            this.ID = strID;
            this.Node = objNode;
        }
    }
}