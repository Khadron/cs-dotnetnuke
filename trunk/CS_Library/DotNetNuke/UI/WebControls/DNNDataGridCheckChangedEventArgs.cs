using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNDataGridCheckChangedEventArgs class is a cusom EventArgs class for
    /// handling Event Args from the CheckChanged event in a CheckBox Column
    /// </Summary>
    public class DNNDataGridCheckChangedEventArgs : DataGridItemEventArgs
    {
        private bool mChecked;
        private CheckBoxColumn mColumn;
        private string mField;
        private bool mIsAll;

        public DNNDataGridCheckChangedEventArgs( DataGridItem item, bool isChecked, string field, bool isAll ) : base( item )
        {
            this.mIsAll = false;
            this.mChecked = isChecked;
            this.mIsAll = isAll;
            this.mField = field;
        }

        public DNNDataGridCheckChangedEventArgs( DataGridItem item, bool isChecked, string field ) : this( item, isChecked, field, false )
        {
        }

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

        public CheckBoxColumn Column
        {
            get
            {
                return this.mColumn;
            }
            set
            {
                this.mColumn = value;
            }
        }

        public string Field
        {
            get
            {
                return this.mField;
            }
            set
            {
                this.mField = value;
            }
        }

        public bool IsAll
        {
            get
            {
                return this.mIsAll;
            }
            set
            {
                this.mIsAll = value;
            }
        }
    }
}