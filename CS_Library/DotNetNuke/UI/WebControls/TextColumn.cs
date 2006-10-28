using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>The TextColumn control provides a custom Text Column</Summary>
    public class TextColumn : TemplateColumn
    {
        private string mDataField;
        private string mText;
        private Unit mWidth;

        /// <Summary>The Data Field is the field that binds to the Text Column</Summary>
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

        /// <Summary>Gets or sets the Text (for Header/Footer Templates)</Summary>
        public string Text
        {
            get
            {
                return this.mText;
            }
            set
            {
                this.mText = value;
            }
        }

        /// <Summary>Gets or sets the Width of the Column</Summary>
        public Unit Width
        {
            get
            {
                return this.mWidth;
            }
            set
            {
                this.mWidth = value;
            }
        }

        /// <Summary>Creates a TextColumnTemplate</Summary>
        /// <Returns>A TextColumnTemplate</Returns>
        private TextColumnTemplate CreateTemplate( ListItemType type )
        {
            bool isDesignMode = false;
            if (HttpContext.Current == null)
            {
                isDesignMode = true;
            }

            TextColumnTemplate template = new TextColumnTemplate(type);
            if (type != ListItemType.Header)
            {
                template.DataField = DataField;
            }
            template.Width = Width;

            if (type == ListItemType.Header)
            {
                template.Text = this.HeaderText;
            }
            else
            {
                template.Text = Text;
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
                this.ItemStyle.Font.Names = new string[] { "Tahoma, Verdana, Arial" };
                this.ItemStyle.Font.Size = new FontUnit("10pt");
                this.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                this.HeaderStyle.Font.Names = new string[] { "Tahoma, Verdana, Arial" };
                this.HeaderStyle.Font.Size = new FontUnit("10pt");
                this.HeaderStyle.Font.Bold = true;
                this.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            }
        }
    }
}