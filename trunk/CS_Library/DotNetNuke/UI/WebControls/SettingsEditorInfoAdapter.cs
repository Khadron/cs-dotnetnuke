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