using System;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNDataGrid control provides an Enhanced Data Grid, that supports other
    /// column types
    /// </Summary>
    public class DNNDataGrid : DataGrid
    {

        public event DNNDataGridCheckedColumnEventHandler ItemCheckedChanged
        {
            add
            {
                this.ItemCheckedChangedEvent += value;
            }
            remove
            {
                this.ItemCheckedChangedEvent -= value;
            }
        }
        private DNNDataGridCheckedColumnEventHandler ItemCheckedChangedEvent;

        protected override void CreateControlHierarchy( bool useDataSource )
        {
            base.CreateControlHierarchy( useDataSource );
        }

        /// <Summary>Called when the grid is Data Bound</Summary>
        protected override void OnDataBinding( EventArgs e )
        {
            foreach (DataGridColumn column in this.Columns)
            {
                if (column.GetType() == typeof(CheckBoxColumn))
                {
                    //Manage CheckBox column events
                    CheckBoxColumn cbColumn = (CheckBoxColumn)column;
                    cbColumn.CheckedChanged += new DNNDataGridCheckedColumnEventHandler(OnItemCheckedChanged);
                }
            }
        }

        /// <Summary>
        /// Centralised Event that is raised whenever a check box is changed.
        /// </Summary>
        private void OnItemCheckedChanged( object sender, DNNDataGridCheckChangedEventArgs e )
        {
            if (ItemCheckedChangedEvent != null)
            {
                ItemCheckedChangedEvent(sender, e);
            }
        }

        protected override void PrepareControlHierarchy()
        {
            base.PrepareControlHierarchy();
        }
    }
}