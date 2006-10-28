using System;
using System.Web.UI.Design;

namespace DotNetNuke.UI.Design.WebControls
{
    public class DNNTreeDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            string designTimeHTML = null;

            try
            {
                designTimeHTML = "<Div>This is a PlaceHolder.<br>Additional design-time support will be <br>added in future versions</Div>";
            }
            catch( Exception e )
            {
                designTimeHTML = GetErrorDesignTimeHtml( e );
            }
            return designTimeHTML;
        }
    }
}