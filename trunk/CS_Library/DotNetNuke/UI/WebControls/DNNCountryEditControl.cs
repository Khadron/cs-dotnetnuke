using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNCountryEditControl control provides a standard UI component for editing
    /// Countries
    /// </Summary>
    [ToolboxData( "<{0}:DNNCountryEditControl runat=server></{0}:DNNCountryEditControl>" )]
    public class DNNCountryEditControl : DNNListEditControl
    {
        /// <Summary>Constructs a DNNCountryEditControl</Summary>
        public DNNCountryEditControl()
        {
            this.AutoPostBack = true;
            this.ListName = "Country";
            this.ParentKey = "";
            this.TextField = ListBoundField.Text;
            this.ValueField = ListBoundField.Text;
        }
    }
}