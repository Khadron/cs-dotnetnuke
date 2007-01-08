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
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls.Design;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The PropertyEditorControl control provides a way to display and edit any
    /// properties of any Info class
    /// </Summary>
    [PersistChildren( true ), ToolboxDataAttribute( "<{0}:PropertyEditorControl runat=server></{0}:PropertyEditorControl>" ), Designer( typeof( PropertyEditorControlDesigner ) )]
    public class PropertyEditorControl : WebControl, INamingContainer
    {
        private bool _AutoGenerate;
        private object _DataSource;
        private Style _EditControlStyle;
        private Unit _EditControlWidth;
        private PropertyEditorMode _EditMode;
        private EditorDisplayMode _DisplayMode;
        private bool _EnableClientValidation;
        private Style _ErrorStyle;
        private ArrayList _Fields;
        private GroupByMode _GroupByMode;
        private bool _GroupHeaderIncludeRule;
        private Style _GroupHeaderStyle;
        private string _Groups;
        private Style _HelpStyle;
        private bool _ItemChanged;
        private Style _LabelStyle;
        private Unit _LabelWidth;
        private string _LocalResourceFile;
        private string _RequiredUrl;
        private Hashtable _Sections;
        private bool _ShowRequired;
        private bool _ShowVisibility;
        private PropertySortType _SortMode;
        private IEnumerable _UnderlyingDataSource;
        private Style _VisibilityStyle;

        /// <Summary>
        /// Gets and sets whether the editor Autogenerates its editors
        /// </Summary>
        [CategoryAttribute( "Behavior" )]
        public bool AutoGenerate
        {
            get
            {
                return this._AutoGenerate;
            }
            set
            {
                this._AutoGenerate = value;
            }
        }

        /// <Summary>
        /// Gets and sets the DataSource that is bound to this control
        /// </Summary>
        [CategoryAttribute( "Data" ), BrowsableAttribute( false )]
        public object DataSource
        {
            get
            {
                return this._DataSource;
            }
            set
            {
                this._DataSource = value;
            }
        }

        /// <Summary>Gets the value of the Field Style</Summary>
        [CategoryAttribute( "Styles" ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), DescriptionAttribute( "Set the Style for the Edit Control." ), BrowsableAttribute( true ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content )]
        public Style EditControlStyle
        {
            get
            {
                return this._EditControlStyle;
            }
        }

        /// <Summary>Gets and sets the width of the Edit Control Column</Summary>
        [DescriptionAttribute( "Set the Width for the Edit Control." ), BrowsableAttribute( true ), CategoryAttribute( "Appearance" )]
        public Unit EditControlWidth
        {
            get
            {
                return this._EditControlWidth;
            }
            set
            {
                this._EditControlWidth = value;
            }
        }

        /// <Summary>Gets and sets the Edit Mode of the Editor</Summary>
        [CategoryAttribute( "Appearance" )]
        public PropertyEditorMode EditMode
        {
            get
            {
                return this._EditMode;
            }
            set
            {
                this._EditMode = value;
            }
        }

        public EditorDisplayMode DisplayMode
        {
            get
            {
                return _DisplayMode;
            }
            set
            {
                _DisplayMode = value;
            }
        }

        /// <Summary>
        /// Gets and sets a flag indicating whether the Validators should use client-side
        /// validation
        /// </Summary>
        [CategoryAttribute( "Behavior" )]
        public bool EnableClientValidation
        {
            get
            {
                return this._EnableClientValidation;
            }
            set
            {
                this._EnableClientValidation = value;
            }
        }

        /// <Summary>Gets the value of the Error Style</Summary>
        [PersistenceModeAttribute( PersistenceMode.InnerProperty ), CategoryAttribute( "Styles" ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Error Text." ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content )]
        public Style ErrorStyle
        {
            get
            {
                return this._ErrorStyle;
            }
        }

        /// <Summary>
        /// Gets a collection of fields to display if AutoGenerate is false. Or the
        /// collection of fields generated if AutoGenerate is true.
        /// </Summary>
        [PersistenceModeAttribute( PersistenceMode.InnerProperty ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), CategoryAttribute( "Behavior" )]
        public ArrayList Fields
        {
            get
            {
                return this._Fields;
            }
        }

        /// <Summary>Gets and sets the grouping mode</Summary>
        [CategoryAttribute( "Appearance" )]
        public GroupByMode GroupByMode
        {
            get
            {
                return this._GroupByMode;
            }
            set
            {
                this._GroupByMode = value;
            }
        }

        /// <Summary>Gets and sets whether to add a <hr> to the Group Header</Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Appearance" ), DescriptionAttribute( "Set whether to include a rule <hr> in the Group Header." )]
        public bool GroupHeaderIncludeRule
        {
            get
            {
                return this._GroupHeaderIncludeRule;
            }
            set
            {
                this._GroupHeaderIncludeRule = value;
            }
        }

        /// <Summary>Gets the value of the Group Header Style</Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Styles" ), DescriptionAttribute( "Set the Style for the Group Header Control." ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content )]
        public Style GroupHeaderStyle
        {
            get
            {
                return this._GroupHeaderStyle;
            }
        }

        /// <Summary>Gets and sets the grouping order</Summary>
        [CategoryAttribute( "Appearance" )]
        public string Groups
        {
            get
            {
                return this._Groups;
            }
            set
            {
                this._Groups = value;
            }
        }

        /// <Summary>Gets the value of the Label Style</Summary>
        [DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Help Text." ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), CategoryAttribute( "Styles" )]
        public Style HelpStyle
        {
            get
            {
                return this._HelpStyle;
            }
        }

        /// <Summary>Gets whether any of the properties have been changed</Summary>
        [BrowsableAttribute( false )]
        public bool IsDirty
        {
            get
            {
                foreach( FieldEditorControl editor in this.Fields )
                {
                    if( editor.IsDirty )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <Summary>Gets whether all of the properties are Valid</Summary>
        [BrowsableAttribute( false )]
        public bool IsValid
        {
            get
            {
                foreach( FieldEditorControl editor in this.Fields )
                {
                    if( ! editor.IsValid )
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <Summary>Gets the value of the Label Style</Summary>
        [CategoryAttribute( "Styles" ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Label Text" ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content )]
        public Style LabelStyle
        {
            get
            {
                return this._LabelStyle;
            }
        }

        /// <Summary>Gets and sets the width of the Label Column</Summary>
        [CategoryAttribute( "Appearance" ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Width for the Label Control." )]
        public Unit LabelWidth
        {
            get
            {
                return this._LabelWidth;
            }
            set
            {
                this._LabelWidth = value;
            }
        }

        /// <Summary>Gets and sets the Local Resource File for the Control</Summary>
        public string LocalResourceFile
        {
            get
            {
                return this._LocalResourceFile;
            }
            set
            {
                this._LocalResourceFile = value;
            }
        }

        /// <Summary>Gets and sets the Url of the Required Image</Summary>
        public string RequiredUrl
        {
            get
            {
                return this._RequiredUrl;
            }
            set
            {
                this._RequiredUrl = value;
            }
        }

        /// <Summary>gets and sets whether the Required icon is used</Summary>
        public bool ShowRequired
        {
            get
            {
                return this._ShowRequired;
            }
            set
            {
                this._ShowRequired = value;
            }
        }

        /// <Summary>gets and sets whether the Visibility control is used</Summary>
        [CategoryAttribute( "Appearance" )]
        public bool ShowVisibility
        {
            get
            {
                return this._ShowVisibility;
            }
            set
            {
                this._ShowVisibility = value;
            }
        }

        /// <Summary>Gets and sets whether to sort properties.</Summary>
        [CategoryAttribute( "Appearance" )]
        public PropertySortType SortMode
        {
            get
            {
                return this._SortMode;
            }
            set
            {
                this._SortMode = value;
            }
        }

        /// <Summary>Gets the Underlying DataSource</Summary>
        protected virtual IEnumerable UnderlyingDataSource
        {
            get
            {
                if( this._UnderlyingDataSource != null )
                {
                    return this._UnderlyingDataSource;
                }
                this._UnderlyingDataSource = this.GetProperties();
                return this._UnderlyingDataSource;
            }
        }

        /// <Summary>Gets the value of the Visibility Style</Summary>
        [PersistenceModeAttribute( PersistenceMode.InnerProperty ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Visibility Control" ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), CategoryAttribute( "Styles" )]
        public Style VisibilityStyle
        {
            get
            {
                return this._VisibilityStyle;
            }
        }

        public PropertyEditorControl()
        {
            this._AutoGenerate = true;
            this._EditControlStyle = new Style();
            this._ErrorStyle = new Style();
            this._GroupHeaderStyle = new Style();
            this._HelpStyle = new Style();
            this._LabelStyle = new Style();
            this._VisibilityStyle = new Style();
            this._ItemChanged = false;
            this._ShowRequired = true;
            this._ShowVisibility = false;
            this._Groups = Null.NullString;
            this._Fields = new ArrayList();
        }

        /// <Summary>GetCategory gets the Category of an object</Summary>
        protected virtual string GetCategory( object obj )
        {
            PropertyInfo objProperty = (PropertyInfo)obj;
            string _Category = Null.NullString;

            //Get Category Field
            object[] categoryAttributes = objProperty.GetCustomAttributes(typeof(CategoryAttribute), true);
            if (categoryAttributes.Length > 0)
            {
                CategoryAttribute category = (CategoryAttribute)categoryAttributes[0];
                _Category = category.Category;
            }

            return _Category;
        }

        /// <Summary>
        /// GetGroups gets an array of Groups/Categories from the DataSource
        /// </Summary>
        protected virtual string[] GetGroups( IEnumerable arrObjects )
        {
            ArrayList arrGroups = new ArrayList();
            string[] strGroups;

            foreach (PropertyInfo objProperty in arrObjects)
            {
                object[] categoryAttributes = objProperty.GetCustomAttributes(typeof(CategoryAttribute), true);
                if (categoryAttributes.Length > 0)
                {
                    CategoryAttribute category = (CategoryAttribute)categoryAttributes[0];

                    if (!arrGroups.Contains(category.Category))
                    {
                        arrGroups.Add(category.Category);
                    }
                }
            }

            strGroups = new string[arrGroups.Count - 1 + 1];
            for (int i = 0; i < arrGroups.Count; i++)
            {
                strGroups[i] = arrGroups[i].ToString();
            }
            return strGroups;
        }

        /// <Summary>GetProperties returns an array of PropertyInfo</Summary>
        /// <Returns>
        /// An array of PropertyInfo objects for the current DataSource object.
        /// </Returns>
        private PropertyInfo[] GetProperties()
        {
            if (DataSource != null)
            {
                //TODO:  We need to add code to support using the cache in the future

                BindingFlags Bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

                PropertyInfo[] Properties = DataSource.GetType().GetProperties(Bindings);

                //Apply sort method
                switch (SortMode)
                {
                    case PropertySortType.Alphabetical:

                        Array.Sort(Properties, new PropertyNameComparer());
                        break;
                    case PropertySortType.Category:

                        Array.Sort(Properties, new PropertyCategoryComparer());
                        break;
                    case PropertySortType.SortOrderAttribute:

                        Array.Sort(Properties, new PropertySortOrderComparer());
                        break;
                }

                return Properties;
            }
            else
            {
                return null;
            }
        }

        /// <Summary>
        /// GetRowVisibility determines the Visibility of a row in the table
        /// </Summary>
        /// <Param name="obj">The property</Param>
        protected virtual bool GetRowVisibility( object obj )
        {
            PropertyInfo objProperty = (PropertyInfo)obj;

            bool isVisible = true;
            object[] browsableAttributes = objProperty.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (browsableAttributes.Length > 0)
            {
                BrowsableAttribute browsable = (BrowsableAttribute)browsableAttributes[0];
                if (!browsable.Browsable)
                {
                    isVisible = false;
                }
            }

            return isVisible;
        }

        /// <summary>
        /// AddEditorRow builds a sigle editor row and adds it to the Table
        /// </summary>
        /// <remarks>This method is protected so that classes that inherit from
        /// PropertyEditor can modify how the Row is displayed</remarks>
        /// <param name="tbl">The Table Control to add the row to</param>
        /// <param name="obj">an object</param>
        protected virtual void AddEditorRow( ref Table tbl, object obj )
        {
            PropertyInfo objProperty = (PropertyInfo)obj;

            AddEditorRow(ref tbl, objProperty.Name, new StandardEditorInfoAdapter(DataSource, objProperty.Name));
        }

        protected void AddEditorRow( ref Table tbl, string name, IEditorInfoAdapter adapter )
        {
            TableRow row = new TableRow();
            tbl.Rows.Add(row);

            TableCell cell = new TableCell();
            row.Cells.Add(cell);

            //Create a FieldEditor for this Row
            FieldEditorControl editor = new FieldEditorControl();
            editor.DataSource = DataSource;
            editor.EditorInfoAdapter = adapter;
            editor.DataField = name;
            editor.EditorDisplayMode = DisplayMode;
            editor.EditorDisplayMode = EditorDisplayMode.Div;
            editor.EnableClientValidation = EnableClientValidation;
            editor.EditMode = EditMode;
            editor.LabelWidth = LabelWidth;
            editor.LabelStyle.CopyFrom(LabelStyle);
            editor.HelpStyle.CopyFrom(HelpStyle);
            editor.ErrorStyle.CopyFrom(ErrorStyle);
            editor.VisibilityStyle.CopyFrom(VisibilityStyle);
            editor.EditControlStyle.CopyFrom(EditControlStyle);
            editor.EditControlWidth = EditControlWidth;
            editor.LocalResourceFile = LocalResourceFile;
            editor.RequiredUrl = RequiredUrl;
            editor.ShowRequired = ShowRequired;
            editor.ShowVisibility = ShowVisibility;
            editor.Width = Width;
            editor.ItemChanged += new PropertyChangedEventHandler(this.ListItemChanged);
            editor.DataBind();
            Fields.Add(editor);
            cell.Controls.Add(editor);
        }

        /// <Summary>
        /// AddFields adds the fields that have beend defined in design mode (Autogenerate=false)
        /// </Summary>
        /// <Param name="tbl">The Table Control to add the row to</Param>
        protected virtual void AddFields( Table tbl )
        {
            foreach (FieldEditorControl editor in Fields)
            {
                TableRow row = new TableRow();
                tbl.Rows.Add(row);

                TableCell cell = new TableCell();
                row.Cells.Add(cell);

                editor.EditorDisplayMode = DisplayMode;
                editor.EnableClientValidation = EnableClientValidation;
                editor.EditMode = EditMode;
                editor.LabelWidth = LabelWidth;
                editor.LabelStyle.CopyFrom(LabelStyle);
                editor.HelpStyle.CopyFrom(HelpStyle);
                editor.ErrorStyle.CopyFrom(ErrorStyle);
                editor.EditControlStyle.CopyFrom(EditControlStyle);
                editor.EditControlWidth = EditControlWidth;
                editor.RequiredUrl = RequiredUrl;
                editor.ShowRequired = ShowRequired;
                editor.ShowVisibility = ShowVisibility;
                editor.Width = Width;

                editor.DataSource = DataSource;
                editor.DataBind();

                cell.Controls.Add(editor);
            }
        }

        /// <summary>
        /// AddHeader builds a group header
        /// </summary>
        /// <remarks>This method is protected so that classes that inherit from
        /// PropertyEditor can modify how the Header is displayed</remarks>
        /// <param name="tbl">The Table Control that contains the group</param>
        /// <param name="header">the header</param>
        protected virtual void AddHeader( ref Table tbl, string header )
        {
            Panel panel = new Panel();

            Image icon = new Image();
            icon.ID = "ico" + header;
            icon.EnableViewState = false;

            Label label = new Label();
            label.ID = "lbl" + header;
            label.Text = header;
            label.EnableViewState = false;
            label.ControlStyle.CopyFrom(GroupHeaderStyle);

            panel.Controls.Add(icon);
            panel.Controls.Add(label);

            if (GroupHeaderIncludeRule)
            {
                panel.Controls.Add(new LiteralControl("<hr noshade size=\"1\"/>"));
            }

            Controls.Add(panel);

            //Get the Hashtable
            if (_Sections == null)
            {
                _Sections = new Hashtable();
            }
            _Sections[icon] = tbl;
        }

        /// <Summary>CreateEditor creates the control collection.</Summary>
        protected virtual void CreateEditor()
        {

            Table tbl = null;
            string[] arrGroups = new string[0];

            Controls.Clear();

            if (Groups.Length > 0)
            {
                arrGroups = Groups.Split(',');
            }
            else if (GroupByMode != GroupByMode.None)
            {
                arrGroups = GetGroups(UnderlyingDataSource);
            }

            if (!AutoGenerate)
            {
                //Create a new table
                tbl = new Table();
                tbl.ID = "tbl";

                AddFields(tbl);

                //Add the Table to the Controls Collection
                Controls.Add(tbl);
            }
            else
            {
                Fields.Clear();
                if (arrGroups.Length > 0)
                {
                    foreach (string strGroup in arrGroups)
                    {
                        if (GroupByMode == UI.WebControls.GroupByMode.Section)
                        {
                            //Create a new table
                            tbl = new Table();
                            tbl.ID = "tbl" + strGroup;

                            foreach (object obj in UnderlyingDataSource)
                            {
                                if (GetCategory(obj) == strGroup.Trim())
                                {
                                    //Add the Editor Row to the Table
                                    if (GetRowVisibility(obj))
                                    {
                                        if (tbl.Rows.Count == 0)
                                        {
                                            //Add a Header
                                            AddHeader(ref tbl, strGroup);
                                        }
                                        AddEditorRow(ref tbl, obj);
                                    }
                                }
                            }

                            //Add the Table to the Controls Collection (if it has any rows)
                            if (tbl.Rows.Count > 0)
                            {
                                Controls.Add(tbl);
                            }
                        }
                    }
                }
                else
                {
                    //Create a new table
                    tbl = new Table();
                    tbl.ID = "tbl";

                    foreach (object obj in UnderlyingDataSource)
                    {
                        //Add the Editor Row to the Table
                        if (GetRowVisibility(obj))
                        {
                            AddEditorRow(ref tbl, obj);
                        }
                    }


                    //Add the Table to the Controls Collection
                    Controls.Add(tbl);
                }
            }
        }

        /// <Summary>Binds the controls to the DataSource</Summary>
        public override void DataBind()
        {
            //Invoke OnDataBinding so DataBinding Event is raised
            base.OnDataBinding(EventArgs.Empty);

            //Clear Existing Controls
            Controls.Clear();

            //Clear Child View State as controls will be loaded from DataSource
            ClearChildViewState();

            //Start Tracking ViewState
            TrackViewState();

            //Create the Editor
            CreateEditor();

            //Set flag so CreateChildConrols should not be invoked later in control's lifecycle
            ChildControlsCreated = true;
        }

        /// <Summary>Runs when an Item in the List Is Changed</Summary>
        protected virtual void ListItemChanged( object sender, PropertyEditorEventArgs e )
        {
            this._ItemChanged = true;
        }

        /// <Summary>Runs just before the control is rendered</Summary>
        protected override void OnPreRender( EventArgs e )
        {
            if (_ItemChanged)
            {
                //Rebind the control to the DataSource to make sure that the dependent
                //editors are updated
                DataBind();
            }

            //Find the Min/Max buttons
            if (GroupByMode == GroupByMode.Section && (_Sections != null))
            {
                foreach (DictionaryEntry key in _Sections)
                {
                    Table tbl = (Table)key.Value;
                    Image icon = (Image)key.Key;
                    DNNClientAPI.EnableMinMax(icon, tbl, false, Page.ResolveUrl("~/images/minus.gif"), Page.ResolveUrl("~/images/plus.gif"), DNNClientAPI.MinMaxPersistanceType.Page);
                }
            }
            base.OnPreRender(e);
        }
    }
}