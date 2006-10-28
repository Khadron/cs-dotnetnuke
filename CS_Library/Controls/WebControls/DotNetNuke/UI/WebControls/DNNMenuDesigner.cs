using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    public class DNNMenuDesigner : ControlDesigner
    {
        //--- This class allows us to render the design time mode with custom HTML ---'
        public override string GetDesignTimeHtml()
        {
            // Component is the instance of the component or control that
            // this designer object is associated with. This property is
            // inherited from System.ComponentModel.ComponentDesigner.
            DNNMenu objMenu = (DNNMenu)Component;

            if( objMenu.ID.Length > 0 )
            {
                StringWriter sw = new StringWriter();
                HtmlTextWriter tw = new HtmlTextWriter( sw );
                Label objText = new Label();

                objText.CssClass = objMenu.MenuBarCssClass + " " + objMenu.DefaultNodeCssClass;
                objText.Text = objMenu.ID;

                if( objMenu.Orientation == Orientation.Horizontal )
                {
                    objText.Width = new Unit( "100%" );
                    //objText.Height = New Unit(objMenu)
                }
                else
                {
                    objText.Height = new Unit( 500 ); //---not sure why 100% doesn't work here ---' 'Unit("100%")
                    //objText.Width = Unit.Empty
                }
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