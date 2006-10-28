using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CheckBoxColumn control provides a Check Box column for a Data Grid
    /// </Summary>
    public class CheckBoxColumn : TemplateColumn
    {

        public event DNNDataGridCheckedColumnEventHandler CheckedChanged
        {
            add
            {
                this.CheckedChangedEvent += value;
            }
            remove
            {
                this.CheckedChangedEvent -= value;
            }
        }
        private DNNDataGridCheckedColumnEventHandler CheckedChangedEvent;
        private bool mAutoPostBack;
        private bool mChecked;
        private string mDataField;
        private bool mEnabled;
        private string mEnabledField;
        private bool mHeaderCheckBox;

        /// <Summary>
        /// Gets and sets whether the column fires a postback when any check box is changed
        /// </Summary>
        public bool AutoPostBack
        {
            get
            {
                return this.mAutoPostBack;
            }
            set
            {
                this.mAutoPostBack = value;
            }
        }

        /// <Summary>
        /// Gets and sets whether the checkbox is checked (unless DataBound)
        /// </Summary>
        public bool Checked
        {
            get
            {
                return this.mChecked;
            }
            set
            {
                this.mChecked = value;
            }
        }

        /// <Summary>The Data Field that the column should bind to changed</Summary>
        public string DataField
        {
            get
            {
                return this.mDataField;
            }
            set
            {
                this.mDataField = value;
            }
        }

        /// <Summary>
        /// An flag that indicates whether the checkboxes are enabled (this is overridden if
        /// the EnabledField is set) changed
        /// </Summary>
        public bool Enabled
        {
            get
            {
                return this.mEnabled;
            }
            set
            {
                this.mEnabled = value;
            }
        }

        /// <Summary>
        /// The Data Field that determines whether the checkbox is Enabled changed
        /// </Summary>
        public string EnabledField
        {
            get
            {
                return this.mEnabledField;
            }
            set
            {
                this.mEnabledField = value;
            }
        }

        /// <Summary>
        /// A flag that indicates whether there is a checkbox in the Header that sets all
        /// the checkboxes
        /// </Summary>
        public bool HeaderCheckBox
        {
            get
            {
                return this.mHeaderCheckBox;
            }
            set
            {
                this.mHeaderCheckBox = value;
            }
        }

        /// <Summary>Constructs the CheckBoxColumn</Summary>
        public CheckBoxColumn() : this( false )
        {
        }

        /// <Summary>
        /// Constructs the CheckBoxColumn, with an optional AutoPostBack (where each change
        /// of state of a check box causes a Post Back)
        /// </Summary>
        /// <Param name="autoPostBack">Optional set the checkboxes to postback</Param>
        public CheckBoxColumn( bool autoPostBack )
        {
            this.mAutoPostBack = true;
            this.mChecked = false;
            this.mDataField = Null.NullString;
            this.mEnabled = true;
            this.mEnabledField = Null.NullString;
            this.mHeaderCheckBox = false;
            this.AutoPostBack = autoPostBack;
        }

        /// <Summary>Creates a CheckBoxColumnTemplate</Summary>
        /// <Returns>A CheckBoxColumnTemplate</Returns>
        private CheckBoxColumnTemplate CreateTemplate( ListItemType type )
        {
            bool isDesignMode = false;
            if (HttpContext.Current == null)
            {
                isDesignMode = true;
            }

            CheckBoxColumnTemplate template = new CheckBoxColumnTemplate(type);
            if (type != ListItemType.Header)
            {
                template.AutoPostBack = AutoPostBack;
            }
            template.Checked = Checked;
            template.DataField = DataField;
            template.Enabled = Enabled;
            template.EnabledField = EnabledField;

            template.CheckedChanged += new DNNDataGridCheckedColumnEventHandler(OnCheckedChanged);
            if (type == ListItemType.Header)
            {
                template.Text = this.HeaderText;
                template.AutoPostBack = true;
            }

            template.DesignMode = isDesignMode;

            return template;
        }

        /// <Summary>Initialises the Column</Summary>
        public override void Initialize()
        {
            this.ItemTemplate = CreateTemplate(ListItemType.Item);
            this.EditItemTemplate = CreateTemplate(ListItemType.EditItem);
            this.HeaderTemplate = CreateTemplate(ListItemType.Header);

            if (HttpContext.Current == null)
            {
                this.HeaderStyle.Font.Names = new string[] { "Tahoma, Verdana, Arial" };
                this.HeaderStyle.Font.Size = new FontUnit("10pt");
                this.HeaderStyle.Font.Bold = true;
            }

            this.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        /// <Summary>
        /// Centralised Event that is raised whenever a check box is changed.
        /// </Summary>
        private void OnCheckedChanged( object sender, DNNDataGridCheckChangedEventArgs e )
        {
            //Add the column to the Event Args
            e.Column = this;

            if (CheckedChangedEvent != null)
            {
                CheckedChangedEvent(sender, e);
            }
        }
    }
}