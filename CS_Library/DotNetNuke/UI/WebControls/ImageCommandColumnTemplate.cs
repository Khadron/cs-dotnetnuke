using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The ImageCommandColumnTemplate provides a Template for the ImageCommandColumn
    /// </Summary>
    public class ImageCommandColumnTemplate : ITemplate
    {
        private string mCommandName;
        private bool mDesignMode;
        private ImageCommandColumnEditMode mEditMode;
        private string mImageURL;
        private ListItemType mItemType;
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

        /// <Summary>Gets or sets the Design Mode of the Column</Summary>
        public bool DesignMode
        {
            get
            {
                return this.mDesignMode;
            }
            set
            {
                this.mDesignMode = value;
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

        /// <Summary>The type of Template to Create</Summary>
        public ListItemType ItemType
        {
            get
            {
                return this.mItemType;
            }
            set
            {
                this.mItemType = value;
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

        /// <Summary>
        /// An flag that indicates whether the buttons are visible (this is overridden if
        /// the VisibleField is set) changed
        /// </Summary>
        public bool Visible
        {
            get
            {
                return this.mVisible;
            }
            set
            {
                this.mVisible = value;
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

        public ImageCommandColumnTemplate( ListItemType itemType )
        {
            this.mEditMode = ImageCommandColumnEditMode.Command;
            this.mItemType = ListItemType.Item;
            this.mShowImage = true;
            this.mVisible = true;
            this.ItemType = itemType;
        }

        public ImageCommandColumnTemplate() : this( ListItemType.Item )
        {
        }

        /// <Summary>Gets whether theButton is visible</Summary>
        /// <Param name="container">The parent container (DataGridItem)</Param>
        private bool GetIsVisible( DataGridItem container )
        {
            bool isVisible;
            if (VisibleField != "")
            {
                isVisible = Convert.ToBoolean(DataBinder.Eval(container.DataItem, VisibleField));
            }
            else
            {
                isVisible = Visible;
            }

            return isVisible;
        }

        /// <Summary>Gets the value of the key</Summary>
        /// <Param name="container">The parent container (DataGridItem)</Param>
        private int GetValue( DataGridItem container )
        {
            int keyValue = Null.NullInteger;
            if (KeyField != "")
            {
                keyValue = Convert.ToInt32(DataBinder.Eval(container.DataItem, KeyField));
            }

            return keyValue;
        }

        /// <Summary>
        /// InstantiateIn instantiates the template (implementation of ITemplate)
        /// </Summary>
        /// <Param name="container">The parent container (DataGridItem)</Param>
        public virtual void InstantiateIn( Control container )
        {
            switch (ItemType)
            {
                case ListItemType.Item:

                    if (EditMode == ImageCommandColumnEditMode.URL)
                    {
                        //Add a Hyperlink
                        HyperLink hypLink = new HyperLink();
                        hypLink.ToolTip = Text;
                        if (ImageURL != "" && ShowImage)
                        {
                            Image img = new Image();
                            if (DesignMode)
                            {
                                img.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                img.ImageUrl = ImageURL;
                            }
                            hypLink.Controls.Add(img);
                            img.ToolTip = Text;
                        }
                        else
                        {
                            hypLink.Text = Text;
                        }
                        hypLink.DataBinding += new EventHandler(Item_DataBinding);
                        container.Controls.Add(hypLink);
                    }
                    else
                    {
                        //Add Image Button
                        if (ImageURL != "" && ShowImage)
                        {
                            ImageButton colIcon = new ImageButton();
                            if (DesignMode)
                            {
                                colIcon.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                colIcon.ImageUrl = ImageURL;
                            }
                            colIcon.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colIcon, OnClickJS);
                            }
                            colIcon.CommandName = CommandName;
                            colIcon.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colIcon);
                        }

                        //Add Link Button
                        if (Text != "" && !ShowImage)
                        {
                            LinkButton colLink = new LinkButton();
                            colLink.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colLink, OnClickJS);
                            }
                            colLink.CommandName = CommandName;
                            colLink.Text = Text;
                            colLink.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colLink);
                        }
                    }
                    break;

                case ListItemType.AlternatingItem:

                    if (EditMode == ImageCommandColumnEditMode.URL)
                    {
                        //Add a Hyperlink
                        HyperLink hypLink = new HyperLink();
                        hypLink.ToolTip = Text;
                        if (ImageURL != "" && ShowImage)
                        {
                            Image img = new Image();
                            if (DesignMode)
                            {
                                img.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                img.ImageUrl = ImageURL;
                            }
                            hypLink.Controls.Add(img);
                            img.ToolTip = Text;
                        }
                        else
                        {
                            hypLink.Text = Text;
                        }
                        hypLink.DataBinding += new EventHandler(Item_DataBinding);
                        container.Controls.Add(hypLink);
                    }
                    else
                    {
                        //Add Image Button
                        if (ImageURL != "" && ShowImage)
                        {
                            ImageButton colIcon = new ImageButton();
                            if (DesignMode)
                            {
                                colIcon.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                colIcon.ImageUrl = ImageURL;
                            }
                            colIcon.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colIcon, OnClickJS);
                            }
                            colIcon.CommandName = CommandName;
                            colIcon.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colIcon);
                        }

                        //Add Link Button
                        if (Text != "" && !ShowImage)
                        {
                            LinkButton colLink = new LinkButton();
                            colLink.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colLink, OnClickJS);
                            }
                            colLink.CommandName = CommandName;
                            colLink.Text = Text;
                            colLink.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colLink);
                        }
                    }
                    break;

                case ListItemType.SelectedItem:

                    if (EditMode == ImageCommandColumnEditMode.URL)
                    {
                        //Add a Hyperlink
                        HyperLink hypLink = new HyperLink();
                        hypLink.ToolTip = Text;
                        if (ImageURL != "" && ShowImage)
                        {
                            Image img = new Image();
                            if (DesignMode)
                            {
                                img.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                img.ImageUrl = ImageURL;
                            }
                            hypLink.Controls.Add(img);
                            img.ToolTip = Text;
                        }
                        else
                        {
                            hypLink.Text = Text;
                        }
                        hypLink.DataBinding += new EventHandler(Item_DataBinding);
                        container.Controls.Add(hypLink);
                    }
                    else
                    {
                        //Add Image Button
                        if (ImageURL != "" && ShowImage)
                        {
                            ImageButton colIcon = new ImageButton();
                            if (DesignMode)
                            {
                                colIcon.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                colIcon.ImageUrl = ImageURL;
                            }
                            colIcon.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colIcon, OnClickJS);
                            }
                            colIcon.CommandName = CommandName;
                            colIcon.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colIcon);
                        }

                        //Add Link Button
                        if (Text != "" && !ShowImage)
                        {
                            LinkButton colLink = new LinkButton();
                            colLink.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colLink, OnClickJS);
                            }
                            colLink.CommandName = CommandName;
                            colLink.Text = Text;
                            colLink.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colLink);
                        }
                    }
                    break;

                case ListItemType.EditItem:

                    if (EditMode == ImageCommandColumnEditMode.URL)
                    {
                        //Add a Hyperlink
                        HyperLink hypLink = new HyperLink();
                        hypLink.ToolTip = Text;
                        if (ImageURL != "" && ShowImage)
                        {
                            Image img = new Image();
                            if (DesignMode)
                            {
                                img.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                img.ImageUrl = ImageURL;
                            }
                            hypLink.Controls.Add(img);
                            img.ToolTip = Text;
                        }
                        else
                        {
                            hypLink.Text = Text;
                        }
                        hypLink.DataBinding += new EventHandler(Item_DataBinding);
                        container.Controls.Add(hypLink);
                    }
                    else
                    {
                        //Add Image Button
                        if (ImageURL != "" && ShowImage)
                        {
                            ImageButton colIcon = new ImageButton();
                            if (DesignMode)
                            {
                                colIcon.ImageUrl = ImageURL.Replace("~/", "../../");
                            }
                            else
                            {
                                colIcon.ImageUrl = ImageURL;
                            }
                            colIcon.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colIcon, OnClickJS);
                            }
                            colIcon.CommandName = CommandName;
                            colIcon.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colIcon);
                        }

                        //Add Link Button
                        if (Text != "" && !ShowImage)
                        {
                            LinkButton colLink = new LinkButton();
                            colLink.ToolTip = Text;
                            if (OnClickJS != "")
                            {
                                ClientAPI.AddButtonConfirm(colLink, OnClickJS);
                            }
                            colLink.CommandName = CommandName;
                            colLink.Text = Text;
                            colLink.DataBinding += new EventHandler(Item_DataBinding);
                            container.Controls.Add(colLink);
                        }
                    }
                    break;

                case ListItemType.Footer:
                    container.Controls.Add(new LiteralControl(Text));
                    break;

                case ListItemType.Header:

                    container.Controls.Add(new LiteralControl(Text));
                    break;
            }
        }

        /// <Summary>
        /// Item_DataBinding runs when an Item of type ListItemType.Item is being data-bound
        /// </Summary>
        /// <Param name="sender">The object that triggers the event</Param>
        /// <Param name="e">An EventArgs object</Param>
        private void Item_DataBinding( object sender, EventArgs e )
        {
            DataGridItem container;
            int keyValue = Null.NullInteger;

            if (EditMode == ImageCommandColumnEditMode.URL)
            {
                HyperLink hypLink = (HyperLink)sender;
                container = (DataGridItem)hypLink.NamingContainer;
                keyValue = GetValue(container);
                if (NavigateURLFormatString != "")
                {
                    hypLink.NavigateUrl = string.Format(NavigateURLFormatString, keyValue);
                }
                else
                {
                    hypLink.NavigateUrl = keyValue.ToString();
                }
            }
            else
            {
                //Bind Image Button
                if (ImageURL != "" && ShowImage)
                {
                    ImageButton colIcon = (ImageButton)sender;
                    container = (DataGridItem)colIcon.NamingContainer;
                    keyValue = GetValue(container);
                    colIcon.CommandArgument = keyValue.ToString();
                    colIcon.Visible = GetIsVisible(container);
                }

                //Bind Link Button
                if (Text != "" && !ShowImage)
                {
                    LinkButton colLink = (LinkButton)sender;
                    container = (DataGridItem)colLink.NamingContainer;
                    keyValue = GetValue(container);
                    colLink.CommandArgument = keyValue.ToString();
                    colLink.Visible = GetIsVisible(container);
                }
            }
        }
    }
}