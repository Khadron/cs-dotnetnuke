using System;
using System.Collections;
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CollectionEditorInfoAdapter control provides an Adapter for Collection Onjects
    /// </Summary>
    public class CollectionEditorInfoAdapter : IEditorInfoAdapter
    {
        private object DataSource;
        private string FieldName;
        private Hashtable FieldNames;
        private string Name;

        public CollectionEditorInfoAdapter( object dataSource, string name, string fieldName, Hashtable fieldNames )
        {
            this.DataSource = dataSource;
            this.FieldName = fieldName;
            this.FieldNames = fieldNames;
            this.Name = name;
        }

        public virtual EditorInfo CreateEditControl()
        {
            return this.GetEditorInfo();
        }

        /// <Summary>GetEditorInfo builds an EditorInfo object for a propoerty</Summary>
        private EditorInfo GetEditorInfo()
        {
            string CategoryDataField = Convert.ToString(FieldNames["Category"]);
            string EditorDataField = Convert.ToString(FieldNames["Editor"]);
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string RequiredDataField = Convert.ToString(FieldNames["Required"]);
            string TypeDataField = Convert.ToString(FieldNames["Type"]);
            string ValidationExpressionDataField = Convert.ToString(FieldNames["ValidationExpression"]);
            string ValueDataField = Convert.ToString(FieldNames["Value"]);
            string VisibilityDataField = Convert.ToString(FieldNames["Visibility"]);

            EditorInfo editInfo = new EditorInfo();
            PropertyInfo objProperty;

            //Get the Name of the property
            editInfo.Name = string.Empty;
            //Get Name Field
            if (NameDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(NameDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Name = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }

            //Get the Category of the property
            editInfo.Category = string.Empty;
            //Get Category Field
            if (CategoryDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(CategoryDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Category = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }

            //Get Value Field
            editInfo.Value = string.Empty;
            if (ValueDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(ValueDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Value = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }

            //Get the type of the property
            editInfo.Type = "System.String";
            if (TypeDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(TypeDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Type = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }

            //Get Editor Field
            editInfo.Editor = "DotNetNuke.UI.WebControls.TextEditControl, DotNetNuke";
            if (EditorDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(EditorDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Editor = EditorInfo.GetEditor(Convert.ToInt32(objProperty.GetValue(DataSource, null)));
                }
            }

            //Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left;

            //Get Required Field
            editInfo.Required = false;
            if (RequiredDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(RequiredDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Required = Convert.ToBoolean(objProperty.GetValue(DataSource, null));
                }
            }

            //Set ResourceKey Field
            editInfo.ResourceKey = editInfo.Name;
            editInfo.ResourceKey = string.Format("{0}_{1}", Name, editInfo.Name);

            //Get Style
            editInfo.ControlStyle = new Style();

            //Get Visibility Field
            editInfo.Visibility = UserVisibilityMode.AllUsers;
            if (VisibilityDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(VisibilityDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Visibility = (UserVisibilityMode)objProperty.GetValue(DataSource, null);
                }
            }

            //Get Validation Expression Field
            editInfo.ValidationExpression = string.Empty;
            if (ValidationExpressionDataField != "")
            {
                objProperty = DataSource.GetType().GetProperty(ValidationExpressionDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.ValidationExpression = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }

            return editInfo;
        }

        public virtual bool UpdateValue( PropertyEditorEventArgs e )
        {
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string ValueDataField = Convert.ToString(FieldNames["Value"]);
            PropertyInfo objProperty;
            string PropertyName = "";
            string name = e.Name;
            object oldValue = e.OldValue;
            object newValue = e.Value;
            object stringValue = e.StringValue;
            bool _IsDirty = Null.NullBoolean;

            //Get the Name Property
            objProperty = DataSource.GetType().GetProperty(NameDataField);
            if (objProperty != null)
            {
                PropertyName = Convert.ToString(objProperty.GetValue(DataSource, null));

                //Do we have the item in the IEnumerable Collection being changed
                if (PropertyName == name)
                {
                    //Get the Value Property
                    objProperty = DataSource.GetType().GetProperty(ValueDataField);

                    //Set the Value property to the new value
                    if (!(newValue == oldValue))
                    {
                        if (objProperty.PropertyType.FullName == "System.String")
                        {
                            objProperty.SetValue(DataSource, stringValue, null);
                        }
                        else
                        {
                            objProperty.SetValue(DataSource, newValue, null);
                        }
                        _IsDirty = true;
                    }
                }
            }

            return _IsDirty;
        }

        public virtual bool UpdateVisibility( PropertyEditorEventArgs e )
        {
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string VisibilityDataField = Convert.ToString(FieldNames["Visibility"]);
            PropertyInfo objProperty;
            string PropertyName = "";
            string name = e.Name;
            object newValue = e.Value;
            bool _IsDirty = Null.NullBoolean;

            //Get the Name Property
            objProperty = DataSource.GetType().GetProperty(NameDataField);
            if (objProperty != null)
            {
                PropertyName = Convert.ToString(objProperty.GetValue(DataSource, null));

                //Do we have the item in the IEnumerable Collection being changed
                if (PropertyName == name)
                {
                    //Get the Visibility Property
                    objProperty = DataSource.GetType().GetProperty(VisibilityDataField);

                    //Set the Visibility property to the new value
                    objProperty.SetValue(DataSource, newValue, null);
                    _IsDirty = true;
                }
            }

            return _IsDirty;
        }
    }
}