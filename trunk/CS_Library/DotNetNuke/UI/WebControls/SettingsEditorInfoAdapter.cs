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
using System.Collections;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The SettingsEditorInfoAdapter control provides a factory for creating the
    /// appropriate EditInfo object
    /// </Summary>
    public class SettingsEditorInfoAdapter : IEditorInfoAdapter
    {
        private object DataMember;
        private object DataSource;
        private string FieldName;

        public SettingsEditorInfoAdapter( object dataSource, object dataMember, string fieldName )
        {
            this.DataMember = dataMember;
            this.DataSource = dataSource;
            this.FieldName = fieldName;
        }

        public virtual EditorInfo CreateEditControl()
        {
            SettingInfo info = (SettingInfo)DataMember;
            EditorInfo editInfo = new EditorInfo();

            //Get the Name of the property
            editInfo.Name = info.Name;

            //Get the Category
            editInfo.Category = string.Empty;

            //Get Value Field
            editInfo.Value = info.Value;

            //Get the type of the property
            editInfo.Type = info.Type.AssemblyQualifiedName;

            //Get Editor Field
            editInfo.Editor = info.Editor;

            //Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left;

            //Get Required Field
            editInfo.Required = false;

            //Set ResourceKey Field
            editInfo.ResourceKey = editInfo.Name;

            //Get Style
            editInfo.ControlStyle = new Style();

            //Get Validation Expression Field
            editInfo.ValidationExpression = string.Empty;

            return editInfo;
        }

        public virtual bool UpdateValue( PropertyEditorEventArgs e )
        {
            string key;
            string name = e.Name;
            object oldValue = e.OldValue;
            object newValue = e.Value;
            object stringValue = e.StringValue;
            bool _IsDirty = Null.NullBoolean;

            Hashtable settings = (Hashtable)DataSource;
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                key = Convert.ToString(settingsEnumerator.Key);
                //Do we have the item in the Hashtable being changed
                if (key == name)
                {
                    //Set the Value property to the new value
                    if (!(newValue == oldValue))
                    {
                        settings[key] = newValue;
                        _IsDirty = true;
                        break;
                    }
                }
            }

            return _IsDirty;
        }

        public virtual bool UpdateVisibility( PropertyEditorEventArgs e )
        {
            return true;
        }
    }
}