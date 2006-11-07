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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The ImageCommandColumn control provides an Image Command (or Hyperlink) column
    /// for a Data Grid
    /// </Summary>
    public class ImageCommandColumn : TemplateColumn
    {
        private string mCommandName;
        private ImageCommandColumnEditMode mEditMode;
        private string mImageURL;
        private string mKeyField;
        private string mNavigateURL;
        private string mNavigateURLFormatString;
        private string mOnClickJS;
        private bool mShowImage;
        private string mText;
        private bool mVisible;
        private string mVisibleField;

        /// <Summary>Gets or sets the CommandName for the Column</Summary>
        public string CommandName
        {
            get
            {
                return this.mCommandName;
            }
            set
            {
                this.mCommandName = value;
            }
        }

        /// <Summary>Gets or sets the CommandName for the Column</Summary>
        public ImageCommandColumnEditMode EditMode
        {
            get
            {
                return this.mEditMode;
            }
            set
            {
                this.mEditMode = value;
            }
        }

        /// <Summary>Gets or sets the URL of the Image</Summary>
        public string ImageURL
        {
            get
            {
                return this.mImageURL;
            }
            set
            {
                this.mImageURL = value;
            }
        }

        /// <Summary>The Key Field that provides a Unique key to the data Item</Summary>
        public string KeyField
        {
            get
            {
                return this.mKeyField;
            }
            set
            {
                this.mKeyField = value;
            }
        }

        /// <Summary>
        /// Gets or sets the URL of the Link (unless DataBinding through KeyField)
        /// </Summary>
        public string NavigateURL
        {
            get
            {
                return this.mNavigateURL;
            }
            set
            {
                this.mNavigateURL = value;
            }
        }

        /// <Summary>Gets or sets the URL Formatting string</Summary>
        public string NavigateURLFormatString
        {
            get
            {
                return this.mNavigateURLFormatString;
            }
            set
            {
                this.mNavigateURLFormatString = value;
            }
        }

        /// <Summary>Javascript text to attach to the OnClick Event</Summary>
        public string OnClickJS
        {
            get
            {
                return this.mOnClickJS;
            }
            set
            {
                this.mOnClickJS = value;
            }
        }

        /// <Summary>Gets or sets whether an Image is displayed</Summary>
        public bool ShowImage
        {
            get
            {
                return this.mShowImage;
            }
            set
            {
                this.mShowImage = value;
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

        /// <Summary>An flag that indicates whether the buttons are visible.</Summary>
        public string VisibleField
        {
            get
            {
                return this.mVisibleField;
            }
            set
            {
                this.mVisibleField = value;
            }
        }

        public ImageCommandColumn()
        {
            this.mEditMode = ImageCommandColumnEditMode.Command;
            this.mShowImage = true;
            this.mVisible = true;
        }

        /// <Summary>Creates a ImageCommandColumnTemplate</Summary>
        /// <Returns>A ImageCommandColumnTemplate</Returns>
        private ImageCommandColumnTemplate CreateTemplate( ListItemType type )
        {
            bool isDesignMode = false;
            if (HttpContext.Current == null)
            {
                isDesignMode = true;
            }
            ImageCommandColumnTemplate template = new ImageCommandColumnTemplate(type);
            if (type != ListItemType.Header)
            {
                template.ImageURL = ImageURL;
                if (!isDesignMode)
                {
                    template.CommandName = CommandName;
                    template.VisibleField = VisibleField;
                    template.KeyField = KeyField;
                }
            }
            template.EditMode = EditMode;
            template.NavigateURL = NavigateURL;
            template.NavigateURLFormatString = NavigateURLFormatString;
            template.OnClickJS = OnClickJS;
            template.ShowImage = ShowImage;
            template.Visible = Visible;

            if (type == ListItemType.Header)
            {
                template.Text = this.HeaderText;
            }
            else
            {
                template.Text = Text;
            }

            //Set Design Mode to True
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
    }
}