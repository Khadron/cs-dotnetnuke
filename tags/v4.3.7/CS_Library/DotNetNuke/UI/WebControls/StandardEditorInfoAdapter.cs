#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The StandardEditorInfoAdapter control provides an Adapter for standard datasources
    /// </Summary>
    public class StandardEditorInfoAdapter : IEditorInfoAdapter
    {
        private object DataSource;
        private string FieldName;

        public StandardEditorInfoAdapter( object dataSource, string fieldName )
        {
            this.DataSource = dataSource;
            this.FieldName = fieldName;
        }

        public virtual EditorInfo CreateEditControl()
        {
            EditorInfo editInfo = null;

            PropertyInfo objProperty = GetProperty(DataSource, FieldName);
            if (objProperty != null)
            {
                editInfo = GetEditorInfo(DataSource, objProperty);
            }

            return editInfo;
        }

        /// <Summary>GetEditorInfo builds an EditorInfo object for a propoerty</Summary>
        private EditorInfo GetEditorInfo( object dataSource, PropertyInfo objProperty )
        {
            EditorInfo editInfo = new EditorInfo();

            //Get the Name of the property
            editInfo.Name = objProperty.Name;

            //Get the value of the property
            object value = objProperty.GetValue(dataSource, null);
            if (Null.IsNull(value))
            {
                editInfo.Value = string.Empty; //Null.NullString
            }
            else
            {
                editInfo.Value = Convert.ToString(objProperty.GetValue(dataSource, null));
            }

            //Get the type of the property
            editInfo.Type = objProperty.PropertyType.AssemblyQualifiedName;

            //Get the Custom Attributes for the property
            editInfo.Attributes = objProperty.GetCustomAttributes(true);

            //Get Category Field
            editInfo.Category = string.Empty;
            object[] categoryAttributes = objProperty.GetCustomAttributes(typeof(CategoryAttribute), true);
            if (categoryAttributes.Length > 0)
            {
                CategoryAttribute category = (CategoryAttribute)categoryAttributes[0];
                editInfo.Category = category.Category;
            }

            //Get EditMode Field
            if (!objProperty.CanWrite)
            {
                editInfo.EditMode = PropertyEditorMode.View;
            }
            else
            {
                object[] readOnlyAttributes = objProperty.GetCustomAttributes(typeof(IsReadOnlyAttribute), true);
                if (readOnlyAttributes.Length > 0)
                {
                    IsReadOnlyAttribute readOnlyMode = (IsReadOnlyAttribute)readOnlyAttributes[0];
                    if (readOnlyMode.IsReadOnly)
                    {
                        editInfo.EditMode = PropertyEditorMode.View;
                    }
                }
            }

            //Get Editor Field
            editInfo.Editor = "UseSystemType";
            object[] editorAttributes = objProperty.GetCustomAttributes(typeof(EditorAttribute), true);
            if (editorAttributes.Length > 0)
            {
                EditorAttribute editor = null;
                for (int i = 0; i <= editorAttributes.Length - 1; i++)
                {
                    if (((EditorAttribute)editorAttributes[i]).EditorBaseTypeName.IndexOf("DotNetNuke.UI.WebControls.EditControl") >= 0)
                    {
                        editor = (EditorAttribute)editorAttributes[i];
                        break;
                    }
                }
                if (editor != null)
                {
                    editInfo.Editor = editor.EditorTypeName;
                }
            }

            //Get Required Field
            editInfo.Required = false;
            object[] requiredAttributes = objProperty.GetCustomAttributes(typeof(RequiredAttribute), true);
            if (requiredAttributes.Length > 0)
            {
                //The property may contain multiple edit mode types, so make sure we only use DotNetNuke editors.
                RequiredAttribute required = (RequiredAttribute)requiredAttributes[0];
                if (required.Required)
                {
                    editInfo.Required = true;
                }
            }

            //Get Css Style
            editInfo.ControlStyle = new Style();
            object[] StyleAttributes = objProperty.GetCustomAttributes(typeof(ControlStyleAttribute), true);
            if (StyleAttributes.Length > 0)
            {
                ControlStyleAttribute attribute = (ControlStyleAttribute)StyleAttributes[0];
                editInfo.ControlStyle.CssClass = attribute.CssClass;
                editInfo.ControlStyle.Height = attribute.Height;
                editInfo.ControlStyle.Width = attribute.Width;
            }

            //Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left;
            object[] labelModeAttributes = objProperty.GetCustomAttributes(typeof(LabelModeAttribute), true);
            if (labelModeAttributes.Length > 0)
            {
                LabelModeAttribute mode = (LabelModeAttribute)labelModeAttributes[0];
                editInfo.LabelMode = mode.Mode;
            }

            //Set ResourceKey Field
            editInfo.ResourceKey = string.Format("{0}_{1}", dataSource.GetType().Name, objProperty.Name);

            //Get Validation Expression Field
            editInfo.ValidationExpression = string.Empty;
            object[] regExAttributes = objProperty.GetCustomAttributes(typeof(RegularExpressionValidatorAttribute), true);
            if (regExAttributes.Length > 0)
            {
                //The property may contain multiple edit mode types, so make sure we only use DotNetNuke editors.
                RegularExpressionValidatorAttribute regExAttribute = (RegularExpressionValidatorAttribute)regExAttributes[0];
                editInfo.ValidationExpression = regExAttribute.Expression;
            }

            //Set Visibility
            editInfo.Visibility = UserVisibilityMode.AllUsers;

            return editInfo;
        }

        /// <Summary>GetProperty returns the property that is being "bound" to</Summary>
        private PropertyInfo GetProperty( object dataSource, string fieldName )
        {
            if (dataSource != null)
            {
                BindingFlags Bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                PropertyInfo objProperty = dataSource.GetType().GetProperty(fieldName, Bindings);
                return objProperty;
            }
            else
            {
                return null;
            }
        }

        public virtual bool UpdateValue( PropertyEditorEventArgs e )
        {
            object oldValue = e.OldValue;
            object newValue = e.Value;
            bool _IsDirty = Null.NullBoolean;

            //Update the DataSource
            if (DataSource != null)
            {
                PropertyInfo objProperty = DataSource.GetType().GetProperty(e.Name);
                if (objProperty != null)
                {
                    if (!(newValue == oldValue))
                    {
                        objProperty.SetValue(DataSource, newValue, null);
                        _IsDirty = true;
                    }
                }
            }

            return _IsDirty;
        }

        public virtual bool UpdateVisibility( PropertyEditorEventArgs e )
        {
            bool _IsDirty = Null.NullBoolean;
            return _IsDirty;
        }
    }
}