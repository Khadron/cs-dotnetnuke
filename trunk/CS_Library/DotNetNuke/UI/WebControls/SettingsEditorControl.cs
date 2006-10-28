using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The SettingsEditorControl control provides an Editor to edit DotNetNuke Settings
    /// </Summary>
    [ToolboxData( "<{0}:SettingsEditorControl runat=server></{0}:SettingsEditorControl>" )]
    public class SettingsEditorControl : PropertyEditorControl
    {
        private object _CustomEditors;
        private IEnumerable _UnderlyingDataSource;

        /// <Summary>
        /// Gets and sets the CustomEditors that are used by this control
        /// </Summary>
        [Browsable( false )]
        public object CustomEditors
        {
            get
            {
                return this._CustomEditors;
            }
            set
            {
                this._CustomEditors = value;
            }
        }

        /// <Summary>Gets the Underlying DataSource</Summary>
        protected override IEnumerable UnderlyingDataSource
        {
            get
            {
                if( _UnderlyingDataSource == null )
                {
                    _UnderlyingDataSource = GetSettings();
                }
                return _UnderlyingDataSource;
            }
        }

        /// <Summary>
        /// GetRowVisibility determines the Visibility of a row in the table
        /// </Summary>
        /// <Param name="obj">The property</Param>
        protected override bool GetRowVisibility( object obj )
        {
            return true;
        }

        /// <Summary>
        /// GetSettings converts the DataSource into an ArrayList (IEnumerable)
        /// </Summary>
        private ArrayList GetSettings()
        {
            Hashtable settings = (Hashtable)DataSource;
            Hashtable editors = (Hashtable)CustomEditors;
            ArrayList arrSettings = new ArrayList();
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while( settingsEnumerator.MoveNext() )
            {
                SettingInfo info = new SettingInfo( settingsEnumerator.Key, settingsEnumerator.Value );
                if( ( editors != null ) && ( editors[settingsEnumerator.Key] != null ) )
                {
                    info.Editor = Convert.ToString( editors[settingsEnumerator.Key] );
                }
                arrSettings.Add( info );
            }

            arrSettings.Sort( new SettingNameComparer() );

            return arrSettings;
        }

        protected override void AddEditorRow( ref Table tbl, object obj )
        {
            SettingInfo info = (SettingInfo)obj;

            AddEditorRow( ref tbl, info.Name, new SettingsEditorInfoAdapter( DataSource, obj, this.ID ) );
        }
    }
}