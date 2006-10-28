using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The FieldEditorControl control provides a Control to display Profile Properties.
    /// </Summary>
    [ToolboxData( "<{0}:FieldEditorControl runat=server></{0}:FieldEditorControl>" )]
    public class FieldEditorControl : WebControl, INamingContainer
    {

        public event PropertyChangedEventHandler ItemChanged
        {
            add
            {
                this.ItemChangedEvent += value;
            }
            remove
            {
                this.ItemChangedEvent -= value;
            }
        }
        private string _DataField;
        private object _DataSource;
        private Style _EditControlStyle;
        private Unit _EditControlWidth;
        private PropertyEditorMode _EditMode;
        private EditControl _Editor;
        private EditorDisplayMode _EditorDisplayMode;
        private IEditorInfoAdapter _EditorInfoAdapter;
        private bool _EnableClientValidation;
        private Style _ErrorStyle;
        private Hashtable _FieldNames;
        private Style _HelpStyle;
        private bool _IsDirty;
        private bool _IsValid;
        private Style _LabelStyle;
        private Unit _LabelWidth;
        private string _LocalResourceFile;
        private string _RequiredUrl;
        private bool _ShowRequired;
        private bool _ShowVisibility;
        private StandardEditorInfoAdapter _StdAdapter;
        private bool _Validated;
        private Style _VisibilityStyle;
        private PropertyChangedEventHandler ItemChangedEvent;
        private ValidatorCollection Validators;

        /// <Summary>
        /// Gets and sets the value of the Field/property that this control displays
        /// </Summary>
        [Browsable( true ), DescriptionAttribute( "Enter the name of the field that is data bound to the Control." ), DefaultValueAttribute( "" ), CategoryAttribute( "Data" )]
        public string DataField
        {
            get
            {
                return this._DataField;
            }
            set
            {
                this._DataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the DataSource that is bound to this control
        /// </Summary>
        [BrowsableAttribute( false )]
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
        [DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), DescriptionAttribute( "Set the Style for the Edit Control." ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), BrowsableAttribute( true ), CategoryAttribute( "Styles" )]
        public Style EditControlStyle
        {
            get
            {
                return this._EditControlStyle;
            }
        }

        /// <Summary>Gets and sets the width of the Edit Control Column</Summary>
        [BrowsableAttribute( true ), DescriptionAttribute( "Set the Width for the Edit Control." ), CategoryAttribute( "Appearance" )]
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

        /// <Summary>gets and sets whether the Visibility control is used</Summary>
        public EditControl Editor
        {
            get
            {
                return this._Editor;
            }
        }

        /// <Summary>Gets and sets whether the control uses Divs or Tables</Summary>
        public EditorDisplayMode EditorDisplayMode
        {
            get
            {
                return this._EditorDisplayMode;
            }
            set
            {
                this._EditorDisplayMode = value;
            }
        }

        /// <Summary>Gets and sets the Factory used to create the Control</Summary>
        public IEditorInfoAdapter EditorInfoAdapter
        {
            get
            {
                if (_EditorInfoAdapter == null)
                {
                    if (_StdAdapter == null)
                    {
                        _StdAdapter = new StandardEditorInfoAdapter(DataSource, DataField);
                    }
                    return _StdAdapter;
                }
                else
                {
                    return _EditorInfoAdapter;
                }
            }
            set
            {
                this._EditorInfoAdapter = value;
            }
        }

        /// <Summary>
        /// Gets and sets a flag indicating whether the Validators should use client-side
        /// validation
        /// </Summary>
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
        [DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), DescriptionAttribute( "Set the Style for the Error Text." ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), CategoryAttribute( "Styles" ), BrowsableAttribute( true )]
        public Style ErrorStyle
        {
            get
            {
                return this._ErrorStyle;
            }
        }

        /// <Summary>Gets the value of the Label Style</Summary>
        [PersistenceModeAttribute( PersistenceMode.InnerProperty ), DescriptionAttribute( "Set the Style for the Help Text." ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), BrowsableAttribute( true ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), CategoryAttribute( "Styles" )]
        public Style HelpStyle
        {
            get
            {
                return this._HelpStyle;
            }
        }

        /// <Summary>Gets whether any of the properties have been changed</Summary>
        public bool IsDirty
        {
            get
            {
                return this._IsDirty;
            }
        }

        /// <Summary>Gets whether all of the properties are Valid</Summary>
        public bool IsValid
        {
            get
            {
                if (!_Validated)
                {
                    Validate();
                }
                return _IsValid;
            }
        }

        /// <Summary>Gets the value of the Label Style</Summary>
        [BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Label Text" ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), CategoryAttribute( "Styles" )]
        public Style LabelStyle
        {
            get
            {
                return this._LabelStyle;
            }
        }

        /// <Summary>Gets and sets the width of the Label Column</Summary>
        [CategoryAttribute( "Appearance" ), DescriptionAttribute( "Set the Width for the Label Control." ), BrowsableAttribute( true )]
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

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                if (EditorDisplayMode == EditorDisplayMode.Div)
                {
                    return HtmlTextWriterTag.Div;
                }
                else
                {
                    return HtmlTextWriterTag.Span;
                }
            }
        }

        protected override string TagName
        {
            get
            {
                if (EditorDisplayMode == EditorDisplayMode.Div)
                {
                    return "div";
                }
                else
                {
                    return "span";
                }
            }
        }

        /// <Summary>Gets the value of the Visibility Style</Summary>
        [DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), DescriptionAttribute( "Set the Style for the Visibility Control" ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), BrowsableAttribute( true ), PersistenceModeAttribute( PersistenceMode.InnerProperty ), CategoryAttribute( "Styles" )]
        public Style VisibilityStyle
        {
            get
            {
                return this._VisibilityStyle;
            }
        }

        public FieldEditorControl()
        {
            this._EditorDisplayMode = EditorDisplayMode.Div;
            this._EditControlStyle = new Style();
            this._ErrorStyle = new Style();
            this._HelpStyle = new Style();
            this._LabelStyle = new Style();
            this._VisibilityStyle = new Style();
            this._ShowRequired = true;
            this._ShowVisibility = false;
            this.Validators = new ValidatorCollection();
            this._IsValid = true;
            this._Validated = false;
        }

        /// <Summary>BuildEditor creates the editor part of the Control</Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private EditControl BuildEditor( EditorInfo editInfo )
        {
            EditControl propEditor = EditControlFactory.CreateEditControl(editInfo);
            propEditor.ControlStyle.CopyFrom(EditControlStyle);
            propEditor.LocalResourceFile = LocalResourceFile;
            if (editInfo.ControlStyle != null)
            {
                propEditor.ControlStyle.CopyFrom(editInfo.ControlStyle);
            }
            propEditor.ValueChanged += new PropertyChangedEventHandler(this.ValueChanged);
            if (propEditor is DNNListEditControl)
            {
                DNNListEditControl listEditor = (DNNListEditControl)propEditor;
                listEditor.ItemChanged += new PropertyChangedEventHandler(this.ListItemChanged);
            }

            _Editor = propEditor;

            return propEditor;
        }

        /// <Summary>BuildLabel creates the label part of the Control</Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private PropertyLabelControl BuildLabel( EditorInfo editInfo )
        {
            PropertyLabelControl propLabel = new PropertyLabelControl();
            propLabel.ID = editInfo.Name + "_Label";
            propLabel.HelpStyle.CopyFrom(HelpStyle);
            propLabel.LabelStyle.CopyFrom(LabelStyle);
            string strValue = Convert.ToString(editInfo.Value);
            if (editInfo.EditMode == PropertyEditorMode.Edit || (editInfo.Required && strValue == ""))
            {
                propLabel.ShowHelp = true;
            }
            else
            {
                propLabel.ShowHelp = false;
            }
            propLabel.Caption = editInfo.Name;
            propLabel.ResourceKey = editInfo.ResourceKey;
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                propLabel.Width = LabelWidth;
            }

            return propLabel;
        }

        /// <Summary>
        /// BuildValidators creates the validators part of the Control
        /// </Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private Image BuildRequiredIcon( EditorInfo editInfo )
        {
            Image img = null;

            string strValue = Convert.ToString(editInfo.Value);
            if (ShowRequired && editInfo.Required && (editInfo.EditMode == PropertyEditorMode.Edit || (editInfo.Required && strValue == "")))
            {
                img = new Image();
                if (RequiredUrl == Null.NullString)
                {
                    img.ImageUrl = "~/images/required.gif";
                }
                else
                {
                    img.ImageUrl = RequiredUrl;
                }
                img.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required");
            }

            return img;
        }

        /// <Summary>
        /// BuildVisibility creates the visibility part of the Control
        /// </Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private VisibilityControl BuildVisibility( EditorInfo editInfo )
        {
            VisibilityControl visControl = null;

            if (ShowVisibility)
            {
                visControl = new VisibilityControl();
                visControl.ID = this.ID + "_vis";
                visControl.Caption = Localization.GetString("Visibility");
                visControl.Name = editInfo.Name;
                visControl.Value = editInfo.Visibility;
                visControl.ControlStyle.CopyFrom(VisibilityStyle);
                visControl.VisibilityChanged += new PropertyChangedEventHandler(this.VisibilityChanged);
            }

            return visControl;
        }

        /// <Summary>
        /// GetOppositeSide finds the opposite side (ie if LabelMode is left it returns right)
        /// </Summary>
        /// <Param name="labelMode">The LabelMode for this control</Param>
        private string GetOppositeSide( LabelMode labelMode )
        {
            switch( labelMode )
            {
                case LabelMode.Left:
                    {
                        return "right";
                    }
                case LabelMode.Right:
                    {
                        return "left";
                    }
                case LabelMode.Top:
                    {
                        return "bottom";
                    }
                case LabelMode.Bottom:
                    {
                        return "top";
                    }
            }
            return string.Empty;
        }

        /// <Summary>BuildDiv creates the Control as a Div</Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private void BuildDiv( EditorInfo editInfo )
        {
            HtmlGenericControl divLabel = null;

            if (editInfo.LabelMode != LabelMode.None)
            {
                divLabel = new HtmlGenericControl("div");
                string style = "float: " + editInfo.LabelMode.ToString().ToLower();
                if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
                {
                    style += "; width: " + LabelWidth.ToString();
                }
                divLabel.Attributes.Add("style", style);
                divLabel.Controls.Add(BuildLabel(editInfo));
            }

            HtmlGenericControl divEdit = new HtmlGenericControl("div");
            string side = GetOppositeSide(editInfo.LabelMode);
            if (side.Length > 0)
            {
                string style = "float: " + side;
                style += "; width: " + EditControlWidth.ToString();
                divEdit.Attributes.Add("style", style);
            }

            EditControl propEditor = BuildEditor(editInfo);
            VisibilityControl visibility = BuildVisibility(editInfo);
            if (visibility != null)
            {
                visibility.Attributes.Add("style", "float: right;");
                divEdit.Controls.Add(visibility);
            }
            divEdit.Controls.Add(propEditor);
            Image requiredIcon = BuildRequiredIcon(editInfo);
            divEdit.Controls.Add(propEditor);
            if (requiredIcon != null)
            {
                divEdit.Controls.Add(requiredIcon);
            }

            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Top)
            {
                Controls.Add(divLabel);
                Controls.Add(divEdit);
            }
            else
            {
                Controls.Add(divEdit);
                if (divLabel != null)
                {
                    Controls.Add(divLabel);
                }
            }

            //Build the Validators
            BuildValidators(editInfo, propEditor.ID);

            if (Validators.Count > 0)
            {
                //Add the Validators to the editor cell
                foreach (BaseValidator validator in Validators)
                {
                    validator.Width = this.Width;
                    Controls.Add(validator);
                }
            }
        }

        /// <Summary>BuildTable creates the Control as a Table</Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private void BuildTable( EditorInfo editInfo )
        {
            Table tbl = new Table();
            TableCell labelCell = new TableCell();
            TableCell editorCell = new TableCell();

            //Build Label Cell
            labelCell.VerticalAlign = VerticalAlign.Top;
            labelCell.Controls.Add(BuildLabel(editInfo));
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                labelCell.Width = LabelWidth;
            }

            //Build Editor Cell
            editorCell.VerticalAlign = VerticalAlign.Top;
            EditControl propEditor = BuildEditor(editInfo);
            Image requiredIcon = BuildRequiredIcon(editInfo);
            editorCell.Controls.Add(propEditor);
            if (requiredIcon != null)
            {
                editorCell.Controls.Add(requiredIcon);
            }
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                editorCell.Width = EditControlWidth;
            }

            //Add cells to table
            TableRow editorRow = new TableRow();
            TableRow labelRow = new TableRow();
            if (editInfo.LabelMode == LabelMode.Bottom || editInfo.LabelMode == LabelMode.Top || editInfo.LabelMode == LabelMode.None)
            {
                editorCell.ColumnSpan = 2;
                editorRow.Cells.Add(editorCell);
                if (editInfo.LabelMode == LabelMode.Bottom || editInfo.LabelMode == LabelMode.Top)
                {
                    labelCell.ColumnSpan = 2;
                    labelRow.Cells.Add(labelCell);
                }
                if (editInfo.LabelMode == LabelMode.Top)
                {
                    tbl.Rows.Add(labelRow);
                }
                tbl.Rows.Add(editorRow);
                if (editInfo.LabelMode == LabelMode.Bottom)
                {
                    tbl.Rows.Add(labelRow);
                }
            }
            else if (editInfo.LabelMode == LabelMode.Left)
            {
                editorRow.Cells.Add(labelCell);
                editorRow.Cells.Add(editorCell);
                tbl.Rows.Add(editorRow);
            }
            else if (editInfo.LabelMode == LabelMode.Right)
            {
                editorRow.Cells.Add(editorCell);
                editorRow.Cells.Add(labelCell);
                tbl.Rows.Add(editorRow);
            }

            //Build the Validators
            BuildValidators(editInfo, propEditor.ID);

            TableRow validatorsRow = new TableRow();
            TableCell validatorsCell = new TableCell();
            validatorsCell.ColumnSpan = 2;
            //Add the Validators to the editor cell
            foreach (BaseValidator validator in Validators)
            {
                validatorsCell.Controls.Add(validator);
            }
            validatorsRow.Cells.Add(validatorsCell);
            tbl.Rows.Add(validatorsRow);

            //Add the Table to the Controls Collection
            Controls.Add(tbl);
        }

        /// <Summary>
        /// BuildValidators creates the validators part of the Control
        /// </Summary>
        /// <Param name="editInfo">The EditorInfo object for this control</Param>
        private void BuildValidators( EditorInfo editInfo, string targetId )
        {
            //Add Required Validators
            if (editInfo.Required)
            {
                RequiredFieldValidator reqValidator = new RequiredFieldValidator();
                reqValidator.ID = editInfo.Name + "_Req";
                reqValidator.ControlToValidate = targetId;
                reqValidator.Display = ValidatorDisplay.Dynamic;
                reqValidator.ControlStyle.CopyFrom(ErrorStyle);
                reqValidator.EnableClientScript = EnableClientValidation;
                reqValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required");
                Validators.Add(reqValidator);
            }

            //Add Regular Expression Validators
            if (editInfo.ValidationExpression != "")
            {
                RegularExpressionValidator regExValidator = new RegularExpressionValidator();
                regExValidator.ID = editInfo.Name + "_RegEx";
                regExValidator.ControlToValidate = targetId;
                regExValidator.ValidationExpression = editInfo.ValidationExpression;
                regExValidator.Display = ValidatorDisplay.Dynamic;
                regExValidator.ControlStyle.CopyFrom(ErrorStyle);
                regExValidator.EnableClientScript = EnableClientValidation;
                regExValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Validation");
                Validators.Add(regExValidator);
            }
        }

        /// <Summary>
        /// CreateEditor creates the control collection for this control
        /// </Summary>
        protected virtual void CreateEditor()
        {
            EditorInfo editInfo = EditorInfoAdapter.CreateEditControl();
            if (editInfo.EditMode == PropertyEditorMode.Edit)
            {
                editInfo.EditMode = EditMode;
            }

            if (EditorDisplayMode == EditorDisplayMode.Div)
            {
                BuildDiv(editInfo);
            }
            else
            {
                BuildTable(editInfo);
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

            //Create the editor
            CreateEditor();

            //Set flag so CreateChildConrols should not be invoked later in control's lifecycle
            ChildControlsCreated = true;
        }

        /// <Summary>Runs when an Item in the List Is Changed</Summary>
        protected virtual void ListItemChanged( object sender, PropertyEditorEventArgs e )
        {
            if (ItemChangedEvent != null)
            {
                ItemChangedEvent(this, e);
            }
        }

        /// <Summary>Validates the data, and sets the IsValid Property</Summary>
        public virtual void Validate()
        {
            _IsValid = Editor.IsValid;

            if (_IsValid)
            {
                IEnumerator valEnumerator = Validators.GetEnumerator();
                while (valEnumerator.MoveNext())
                {
                    IValidator validator = (IValidator)valEnumerator.Current;
                    if (!validator.IsValid)
                    {
                        _IsValid = false;
                        break;
                    }
                }

                _Validated = true;
            }
        }

        /// <Summary>Runs when the Value of a Property changes</Summary>
        protected virtual void ValueChanged( object sender, PropertyEditorEventArgs e )
        {
            this._IsDirty = this.EditorInfoAdapter.UpdateValue( e );
        }

        /// <Summary>Runs when the Visibility of a Property changes</Summary>
        protected virtual void VisibilityChanged( object sender, PropertyEditorEventArgs e )
        {
            this._IsDirty = this.EditorInfoAdapter.UpdateVisibility( e );
        }
    }
}