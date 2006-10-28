using System.Web.UI;
using DotNetNuke.Common.Lists;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The ProfileEditorControl control provides a Control to display Profile
    /// Properties.
    /// </Summary>
    [ToolboxData( "<{0}:ProfileEditorControl runat=server></{0}:ProfileEditorControl>" )]
    public class ProfileEditorControl : CollectionEditorControl
    {
        /// <Summary>CreateEditor creates the control collection.</Summary>
        protected override void CreateEditor()
        {
            CategoryDataField = "PropertyCategory";
            EditorDataField = "DataType";
            NameDataField = "PropertyName";
            RequiredDataField = "Required";
            ValidationExpressionDataField = "ValidationExpression";
            ValueDataField = "PropertyValue";
            VisibleDataField = "Visible";
            VisibilityDataField = "Visibility";
            LengthDataField = "Length";

            base.CreateEditor();

            //We need to wire up the RegionControl to the CountryControl
            foreach( FieldEditorControl editor in Fields )
            {
                if( editor.Editor is DNNRegionEditControl )
                {
                    ListEntryInfo country = null;

                    foreach( FieldEditorControl checkEditor in Fields )
                    {
                        if( checkEditor.Editor is DNNCountryEditControl )
                        {
                            DNNCountryEditControl countryEdit = (DNNCountryEditControl)checkEditor.Editor;
                            ListController objListController = new ListController();
                            ListEntryInfoCollection countries = objListController.GetListEntryInfoCollection( "Country" );
                            foreach( ListEntryInfo checkCountry in countries )
                            {
                                if( checkCountry.Text == countryEdit.Value.ToString() )
                                {
                                    country = checkCountry;
                                    break;
                                }
                            }
                        }
                    }

                    //Create a ListAttribute for the Region
                    string countryKey;
                    if( country != null )
                    {
                        countryKey = "Country." + country.Value;
                    }
                    else
                    {
                        countryKey = "Country.Unknown";
                    }

                    object[] attributes;
                    attributes = new object[1];
                    attributes[0] = new ListAttribute( "Region", countryKey, ListBoundField.Text, ListBoundField.Text );
                    editor.Editor.CustomAttributes = attributes;
                }
            }        
        }
    }
}