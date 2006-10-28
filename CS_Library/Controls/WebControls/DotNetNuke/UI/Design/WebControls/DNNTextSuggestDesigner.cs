using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Design.WebControls
{
    public class DNNTextSuggestDesigner : ControlDesigner
    {
        //--- This class allows us to render the design time mode with custom HTML ---'
        public override string GetDesignTimeHtml()
        {
            // Component is the instance of the component or control that
            // this designer object is associated with. This property is
            // inherited from System.ComponentModel.ComponentDesigner.
            DNNTextSuggest objTextSuggest = (DNNTextSuggest)Component;

            if( objTextSuggest.ID.Length > 0 )
            {
                StringWriter sw = new StringWriter();
                HtmlTextWriter tw = new HtmlTextWriter( sw );
                TextBox objText = new TextBox();
                objText.Text = objTextSuggest.Text;
                objText.RenderControl( tw );
                return sw.ToString();
            }
            else
            {
                return null;
            }
        }

        public override bool AllowResize
        {
            get
            {
                return false;
            }
        }
    }
}