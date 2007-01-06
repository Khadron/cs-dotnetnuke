#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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