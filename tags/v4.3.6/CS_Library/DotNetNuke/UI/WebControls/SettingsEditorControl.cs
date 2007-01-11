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
        private Hashtable _CustomEditors;
        private Hashtable _Visibility;
        private IEnumerable _UnderlyingDataSource;

        /// <Summary>
        /// Gets and sets the CustomEditors that are used by this control
        /// </Summary>
        [Browsable( false )]
        public Hashtable CustomEditors
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

        /// <summary>
        /// Gets and sets the Visibility values that are used by this control
        /// </summary>
        /// <value>The CustomEditors object</value>
        /// <history>
        /// 	[cnurse]	08/21/2006	Created
        /// </history>
        public Hashtable Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
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
        protected override bool GetRowVisibility(object obj)
        {
            SettingInfo info = (SettingInfo)obj;
            bool _IsVisible = true;

            if ((Visibility != null) && (Visibility[info.Name] != null))
            {
                _IsVisible = Convert.ToBoolean(Visibility[info.Name]);
            }

            return _IsVisible;
        }

        /// <Summary>
        /// GetSettings converts the DataSource into an ArrayList (IEnumerable)
        /// </Summary>
        private ArrayList GetSettings()
        {
            Hashtable settings = (Hashtable)DataSource;
            ArrayList arrSettings = new ArrayList();
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                SettingInfo info = new SettingInfo(settingsEnumerator.Key, settingsEnumerator.Value);
                if ((CustomEditors != null) && (CustomEditors[settingsEnumerator.Key] != null))
                {
                    info.Editor = Convert.ToString(CustomEditors[settingsEnumerator.Key]);
                }
                arrSettings.Add(info);
            }

            arrSettings.Sort(new SettingNameComparer());

            return arrSettings;

        }

        protected override void AddEditorRow( ref Table tbl, object obj )
        {
            SettingInfo info = (SettingInfo)obj;

            AddEditorRow( ref tbl, info.Name, new SettingsEditorInfoAdapter( DataSource, obj, this.ID ) );
        }
    }
}