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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Wizards
{
    /// <Summary>
    /// The Wizard class defines a custom base class inherited by all
    /// Wizard controls.  Wizard itself inherits from PortalModuleBase.
    /// </Summary>
    public class Wizard : PortalModuleBase
    {

        public event WizardEventHandler AfterPageChanged
        {
            add
            {
                this.AfterPageChangedEvent += value;
            }
            remove
            {
                this.AfterPageChangedEvent -= value;
            }
        }

        public event WizardCancelEventHandler BeforePageChanged
        {
            add
            {
                this.BeforePageChangedEvent += value;
            }
            remove
            {
                this.BeforePageChangedEvent -= value;
            }
        }

        public event WizardEventHandler FinishWizard
        {
            add
            {
                this.FinishWizardEvent += value;
            }
            remove
            {
                this.FinishWizardEvent -= value;
            }
        }
        private WizardEventHandler AfterPageChangedEvent;

        private WizardCancelEventHandler BeforePageChangedEvent;
        private LinkButton cmdBack;
        private ImageButton cmdBackIcon;
        private LinkButton cmdCancel;
        private ImageButton cmdCancelIcon;
        private LinkButton cmdFinish;
        private ImageButton cmdFinishIcon;
        private LinkButton cmdHelp;
        private ImageButton cmdHelpIcon;
        private LinkButton cmdNext;
        private ImageButton cmdNextIcon;

        private WizardEventHandler FinishWizardEvent;
        //Header Controls
        private Image imgIcon;
        private Label lblHelpTitle;

        //Footer Controls
        private Label lblPages;
        private Label lblTitle;
        private bool m_EnableBack = true;
        private bool m_EnableFinish = true;
        private bool m_EnableNext = true;
        private WizardPage m_FailurePage;

        private WizardPageCollection m_Pages = new WizardPageCollection();
        private WizardPage m_SuccessPage;

        //Help Panel
        private HtmlGenericControl WizardHelp;
        private HtmlTableCell WizardHelpPane;

        

        public int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                {
                    ViewState["CurrentPage"] = 0;
                }
                return ((int)ViewState["CurrentPage"]);
            }
            set
            {
                this.ViewState["CurrentPage"] = value;
            }
        }

        public bool EnableBack
        {
            get
            {
                return this.m_EnableBack;
            }
            set
            {
                this.m_EnableBack = value;
            }
        }

        public bool EnableFinish
        {
            get
            {
                return this.m_EnableFinish;
            }
            set
            {
                this.m_EnableFinish = value;
            }
        }

        public bool EnableNext
        {
            get
            {
                return this.m_EnableNext;
            }
            set
            {
                this.m_EnableNext = value;
            }
        }

        public int FinishPage
        {
            get
            {
                if (ViewState["FinishPage"] == null)
                {
                    ViewState["FinishPage"] = 0;
                }
                return ((int)ViewState["FinishPage"]);
            }
            set
            {
                this.ViewState["FinishPage"] = value;
            }
        }

        

        public WizardPageCollection Pages
        {
            get
            {
                return this.m_Pages;
            }
        }

        public WizardPage SuccessPage
        {
            get
            {
                return this.m_SuccessPage;
            }
            set
            {
                this.m_SuccessPage = value;
            }
        }

        

        public Wizard()
        {
            base.Init += new EventHandler( this.Page_Init );
            base.Load += new EventHandler( this.Page_Load );
            this.m_Pages = new WizardPageCollection();
            this.m_EnableBack = true;
            this.m_EnableNext = true;
            this.m_EnableFinish = true;
        }
        /// <summary>
        /// AddCommandButton adds a Button to the Button Panel
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="icon">The ImageButton to Add</param>
        ///	<param name="link">The LinkButton to Add</param>
        ///	<param name="Key">The Resource Key for the Text/Help</param>
        ///	<param name="imageUrl">The Image Url</param>
        ///	<param name="causesValidation">Flag set the button to not cause validation</param>
        private HtmlTable AddCommandButton( ref ImageButton icon, ref LinkButton link, string Key, string imageUrl, bool causesValidation )
        {
            //First Create the Container for the Button
            HtmlTable table = new HtmlTable();
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell container = new HtmlTableCell();
            table.Attributes.Add("class", "WizardButton");

            table.Rows.Add(row);
            row.Cells.Add(container);

            //Set the Icon properties
            icon = new ImageButton();
            icon.ImageUrl = "~/images/" + imageUrl;
            icon.ToolTip = Localization.Localization.GetString(Key + ".Help");

            //Set the link properties
            link = new LinkButton();
            link.CssClass = "CommandButton";
            link.Text = Localization.Localization.GetString(Key);
            link.ToolTip = Localization.Localization.GetString(Key + ".Help");
            link.CausesValidation = causesValidation;

            //Ad the controls to the Container
            container.Controls.Add(icon);
            container.Controls.Add(new LiteralControl("&nbsp;"));
            container.Controls.Add(link);

            //Return the Control
            return table;
        }

        /// <Summary>AddFooter adds the Footer to the Wizard</Summary>
        private void AddFooter()
        {
            //get Wizard Table
            HtmlTable Wizard = (HtmlTable)this.FindControl("Wizard");

            //Create Footer Row
            HtmlTableRow FooterRow = new HtmlTableRow();
            HtmlTableCell WizardFooter = new HtmlTableCell();
            WizardFooter.ColSpan = 2;
            WizardFooter.Width = "100%";

            HtmlTable table = new HtmlTable();
            table.Width = "100%";
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell;

            //Add the Page x of y cell
            cell = new HtmlTableCell();
            lblPages = new Label();
            lblPages.CssClass = "SubHead";
            cell.Align = "Left";
            cell.Width = "200";
            cell.Controls.Add(lblPages);
            row.Cells.Add(cell);

            //Add the Spacer Cell
            cell = new HtmlTableCell();
            cell.Width = "160";
            row.Cells.Add(cell);

            //Add the CommandButtons to the Footer
            cell = new HtmlTableCell();
            cell.Controls.Add(AddCommandButton(ref cmdBackIcon, ref cmdBack, "cmdBack", "lt.gif", false));
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Controls.Add(AddCommandButton(ref cmdNextIcon, ref cmdNext, "cmdNext", "rt.gif", true));
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Controls.Add(AddCommandButton(ref cmdFinishIcon, ref cmdFinish, "cmdFinish", "end.gif", true));
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Controls.Add(AddCommandButton(ref cmdCancelIcon, ref cmdCancel, "cmdCancel", "cancel.gif", false));
            row.Cells.Add(cell);

            table.Rows.Add(row);
            WizardFooter.Controls.Add(table);

            //Add the event Handlers
            cmdNext.Command += new CommandEventHandler(this.NextPage);
            cmdNextIcon.Command += new CommandEventHandler(this.NextPage);
            cmdBack.Command += new CommandEventHandler(this.PreviousPage);
            cmdBackIcon.Command += new CommandEventHandler(this.PreviousPage);
            cmdFinish.Command += new CommandEventHandler(this.Finish);
            cmdFinishIcon.Command += new CommandEventHandler(this.Finish);
            cmdCancel.Command += new CommandEventHandler(this.Cancel);
            cmdCancelIcon.Command += new CommandEventHandler(this.Cancel);

            WizardFooter.Attributes.Add("class", "WizardFooter");
            WizardFooter.Height = "22";
            WizardFooter.VAlign = "middle";
            FooterRow.Cells.Add(WizardFooter);
            Wizard.Rows.Add(FooterRow);
        }

        /// <Summary>AddHeader adds the Header to the Wizard</Summary>
        private void AddHeader()
        {
            HtmlTable Wizard = (HtmlTable)this.FindControl("Wizard");
            HtmlTableRow HeaderRow = new HtmlTableRow();

            HtmlTableCell WizardHeader = new HtmlTableCell();
            WizardHeader.ColSpan = 2;
            WizardHeader.Width = "100%";

            HtmlTable headerTable = new HtmlTable();
            headerTable.Width = "100%";
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell;

            cell = new HtmlTableCell();
            cell.Width = "40";
            imgIcon = new Image();
            cell.Controls.Add(imgIcon);
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Width = "15";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Width = "400";
            cell.VAlign = "middle";
            lblTitle = new Label();
            lblTitle.CssClass = "Head";
            cell.Controls.Add(lblTitle);
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.Width = "15";
            row.Cells.Add(cell);

            //Add the Help Button to the Header
            cell = new HtmlTableCell();
            cell.Align = "right";
            cell.Controls.Add(AddCommandButton(ref cmdHelpIcon, ref cmdHelp, "cmdHelp", "help.gif", false));
            row.Cells.Add(cell);

            headerTable.Rows.Add(row);
            WizardHeader.Controls.Add(headerTable);

            //Add the event Handlers
            cmdHelp.Command += new CommandEventHandler(this.ShowHelp);
            cmdHelpIcon.Command += new CommandEventHandler(this.ShowHelp);

            WizardHeader.Attributes.Add("class", "WizardHeader");
            HeaderRow.Cells.Add(WizardHeader);
            Wizard.Rows.Insert(0, HeaderRow);
        }

        /// <Summary>AddPage adds a Wizard Page to the Control</Summary>
        /// <Param name="title">The Page's Title</Param>
        /// <Param name="icon">The Page's Icon</Param>
        /// <Param name="ctl">The Page's Control</Param>
        /// <Param name="type">The type of the Wizard Page</Param>
        /// <Param name="help">The Page's Help Text</Param>
        public void AddPage( string title, string icon, Control ctl, WizardPageType type, string help )
        {
            switch (type)
            {
                case WizardPageType.Content:

                    this.Pages.Add(title, icon, ctl, help);
                    break;
                case WizardPageType.Failure:

                    m_FailurePage = new WizardPage(title, icon, ctl, type, help);
                    break;
                case WizardPageType.Success:

                    m_SuccessPage = new WizardPage(title, icon, ctl, type, help);
                    break;
            }
        }

        /// <Summary>AddPage adds a Wizard Page to the Control</Summary>
        /// <Param name="title">The Page's Title</Param>
        /// <Param name="icon">The Page's Icon</Param>
        /// <Param name="ctl">The Page's Control</Param>
        /// <Param name="help">The Page's Help Text</Param>
        public void AddPage( string title, string icon, Control ctl, string help )
        {
            this.AddPage( title, icon, ctl, WizardPageType.Content, help );
        }

        /// <Summary>AddPage adds a Wizard Page to the Control</Summary>
        /// <Param name="title">The Page's Title</Param>
        /// <Param name="icon">The Page's Icon</Param>
        /// <Param name="ctl">The Page's Control</Param>
        public void AddPage( string title, string icon, Control ctl )
        {
            this.AddPage( title, icon, ctl, WizardPageType.Content, "" );
        }

        /// <Summary>
        /// Cancel runs when the Cancel Button is clicked and returns the user to the
        /// Previous Location
        /// </Summary>
        private void Cancel( object sender, CommandEventArgs e )
        {
            try
            {
                Response.Redirect("~/" + Globals.glbDefaultPage, true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <Summary>
        /// ConfigureWizard sets up the Wizard Framework (creates the structure and injects
        /// the buttons
        /// </Summary>
        private void ConfigureWizard()
        {
            //set up Wizard Table
            HtmlTable Wizard = (HtmlTable)this.FindControl("Wizard");
            Wizard.Attributes.Add("class", "Wizard");
            Wizard.CellPadding = 0;
            Wizard.CellSpacing = 0;
            Wizard.Border = 0;

            //set up Wizard Body
            HtmlTableCell WizardBody = (HtmlTableCell)this.FindControl("WizardBody");
            WizardBody.Attributes.Add("class", "WizardBody");
            WizardBody.VAlign = "top";

            //set up Help Cell
            HtmlTableRow WizardRow = (HtmlTableRow)WizardBody.Parent;
            WizardHelpPane = new HtmlTableCell();
            WizardHelpPane.Attributes.Add("class", "WizardHelp");
            WizardHelpPane.VAlign = "top";
            WizardHelpPane.Visible = false;

            HtmlTable table = new HtmlTable();
            table.Width = "180";
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();

            // Add Help Header
            lblHelpTitle = new Label();
            lblHelpTitle.CssClass = "SubHead";
            lblHelpTitle.Text = Localization.Localization.GetString("cmdHelp");
            cell.Controls.Add(lblHelpTitle);
            cell.Controls.Add(new LiteralControl("<hr>"));
            row.Cells.Add(cell);
            table.Rows.Add(row);

            // Add Help Cell
            row = new HtmlTableRow();
            cell = new HtmlTableCell();
            cell.VAlign = "top";
            WizardHelp = new HtmlGenericControl();
            WizardHelp.Attributes.Add("class", "WizardHelpText");
            cell.Controls.Add(WizardHelp);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            WizardHelpPane.Controls.Add(table);
            WizardRow.Cells.Add(WizardHelpPane);

            //Add the Header to the Wizard
            AddHeader();

            //Add the Footer to the Wizard
            AddFooter();
        }

        /// <Summary>DisplayCurrentPage displays the current selected Page</Summary>
        public void DisplayCurrentPage()
        {
            WizardPage wizPage = (WizardPage)Pages[CurrentPage];
            DisplayPage(CurrentPage, wizPage, EnableBack, EnableNext, EnableFinish);

            //Hide any Help that might be visible
            DisplayHelp(false);
        }

        /// <Summary>DisplayFailurePage displays the Wizards failure page</Summary>
        public void DisplayFailurePage( string message )
        {
            //First determine if a custom FailurePage has been set, and load the default
            //if not
            if (m_FailurePage == null)
            {
                WizardSuccess ctlSuccess = (WizardSuccess)this.LoadControl("~/admin/Wizards/Success.ascx");
                ctlSuccess.Message = message;
                ctlSuccess.Type = false;
                //get Wizard Body
                HtmlTableCell WizardBody = (HtmlTableCell)this.FindControl("WizardBody");
                WizardBody.Controls.Add(ctlSuccess);
                m_FailurePage = new WizardPage("Wizard Error", "", ctlSuccess, WizardPageType.Failure);
            }

            DisplayPage(-1, m_FailurePage, false, false, false);
        }

        /// <Summary>DisplayHelp shows/hides the Help Panel the buttons</Summary>
        /// <Param name="show">
        /// Flag that determines whether the Help Panel is displayed/hidden
        /// </Param>
        private void DisplayHelp( bool show )
        {
            if (show)
            {
                WizardPage wizPage = this.Pages[CurrentPage];
                lblHelpTitle.Text = Localization.Localization.GetString("Help") + " - " + wizPage.Title;
                WizardHelp.InnerHtml = wizPage.Help;
            }
            else
            {
                lblHelpTitle.Text = "";
                WizardHelp.InnerHtml = "";
            }

            WizardHelpPane.Visible = show;
        }

        /// <Summary>DisplayPage displays a specific Page</Summary>
        /// <Param name="wizPage">The Page to display</Param>
        public void DisplayPage( int pageNo, WizardPage wizPage, bool ShowBack, bool ShowNext, bool ShowFinish )
        {
            int iPage;
            Control ctl;

            if (wizPage == null)
            {
                wizPage = new WizardPage();
            }

            //Set the Wizard Body
            for (iPage = 0; iPage <= Pages.Count - 1; iPage++)
            {
                WizardPage pge = Pages[iPage];
                ctl = pge.Control;
                ctl.Visible = false;
            }
            ctl = wizPage.Control;
            ctl.Visible = true;

            //Set the Icons ImageUrl
            if (wizPage.Icon == "")
            {
                imgIcon.ImageUrl = "~/images/1x1.gif";
            }
            else
            {
                imgIcon.ImageUrl = wizPage.Icon;
            }

            //Set the Titles Text and Style
            lblTitle.Text = wizPage.Title;

            //Show/Hide the Back/Next/Finish Buttons
            if ((CurrentPage > 0) && ShowBack)
            {
                cmdBack.Enabled = true;
                cmdBackIcon.Enabled = true;
            }
            else
            {
                cmdBack.Enabled = false;
                cmdBackIcon.Enabled = false;
            }
            if ((CurrentPage < m_Pages.Count - 1) && ShowNext)
            {
                cmdNext.Enabled = true;
                cmdNextIcon.Enabled = true;
            }
            else
            {
                cmdNext.Enabled = false;
                cmdNextIcon.Enabled = false;
            }
            if ((CurrentPage >= FinishPage) && ShowFinish)
            {
                cmdFinish.Enabled = true;
                cmdFinishIcon.Enabled = true;
            }
            else
            {
                cmdFinish.Enabled = false;
                cmdFinishIcon.Enabled = false;
            }

            //Set the Help
            if (wizPage.Help == "")
            {
                cmdHelp.Enabled = false;
                cmdHelpIcon.Enabled = false;
            }
            else
            {
                cmdHelp.Enabled = true;
                cmdHelpIcon.Enabled = true;
            }

            //Set the Pages
            lblPages.Text = string.Format(Localization.Localization.GetString("Pages"), CurrentPage + 1, Pages.Count);
        }

        /// <Summary>DisplaySuccessPage displays the Wizards success page</Summary>
        public void DisplaySuccessPage( string message )
        {
            //First determine if a custom SuccessPage has been set, and load the default
            //if not
            if (m_SuccessPage == null)
            {
                WizardSuccess ctlSuccess = (WizardSuccess)this.LoadControl("~/admin/Wizards/Success.ascx");
                ctlSuccess.Message = message;
                //get Wizard Body
                HtmlTableCell WizardBody = (HtmlTableCell)this.FindControl("WizardBody");
                WizardBody.Controls.Add(ctlSuccess);
                m_SuccessPage = new WizardPage("Congratulations", "", ctlSuccess, WizardPageType.Success);
            }

            DisplayPage(-1, m_SuccessPage, false, false, false);

            //Hide any Help that might be visible
            DisplayHelp(false);

            //Disable Cancel to prevent ViewState issues
            cmdCancel.Enabled = true;
            cmdCancelIcon.Enabled = true;
        }

        /// <Summary>
        /// EnableCommand allows a Wizard to Enable/Disable any of the four Command options
        /// </Summary>
        /// <Param name="cmd">The WizardCommand to Enable/Display</Param>
        /// <Param name="enable">
        /// A flag that determines whether the Commadn is Enabled (true) or Disabled (False)
        /// </Param>
        public void EnableCommand( WizardCommand cmd, bool enable )
        {
            switch (cmd)
            {
                case WizardCommand.Cancel:

                    cmdCancel.Enabled = enable;
                    break;
                case WizardCommand.Finish:

                    cmdFinish.Enabled = enable;
                    break;
                case WizardCommand.NextPage:

                    cmdNext.Enabled = enable;
                    break;
                case WizardCommand.PreviousPage:

                    cmdBack.Enabled = enable;
                    break;
            }
        }

        /// <Summary>
        /// Finish runs when the Back Button is clicked and raises a FinishWizard Event
        /// </Summary>
        private void Finish( object sender, CommandEventArgs e )
        {
            //Create a New WizardEventArgs Object and trigger a Finish Event
            WizardEventArgs we = new WizardEventArgs(CurrentPage, Pages);
            OnFinishWizard(we);
        }

        /// <Summary>
        /// NextPage runs when the Next Button is clicked and raises a NextPage Event
        /// </Summary>
        private void NextPage( object sender, CommandEventArgs e )
        {
            //If we are not on the last page Create a New WizardEventArgs Object
            //and trigger a Previous Page Event
            if (CurrentPage < Pages.Count - 1)
            {
                int PrevPage = CurrentPage;
                WizardCancelEventArgs wce = new WizardCancelEventArgs(CurrentPage, PrevPage, Pages);
                OnBeforePageChanged(wce);
                if (wce.Cancel == false)
                {
                    //Event Not Cancelled by Event Consumer
                    CurrentPage++;
                    WizardEventArgs we = new WizardEventArgs(CurrentPage, PrevPage, Pages);
                    OnAfterPageChanged(we);
                }
            }

            DisplayCurrentPage();
        }

        protected virtual void OnAfterPageChanged(WizardEventArgs e)
        {
            //Invokes the delegates.
            if (AfterPageChangedEvent != null)
            {
                AfterPageChangedEvent(this, e);
            }
        }

        protected virtual void OnBeforePageChanged(WizardCancelEventArgs e)
        {
            //Invokes the delegates.
            if (BeforePageChangedEvent != null)
            {
                BeforePageChangedEvent(this, e);
            }
        }

        protected virtual void OnFinishWizard(WizardEventArgs e)
        {
            //Invokes the delegates.
            if (FinishWizardEvent != null)
            {
                FinishWizardEvent(this, e);
            }
        }

        /// <Summary>Page_Init runs when the Control is Constructed</Summary>
        private void Page_Init( object sender, EventArgs e )
        {
            this.ConfigureWizard();
        }

        /// <Summary>Page_Load runs when the Control is loaded</Summary>
        private void Page_Load( object sender, EventArgs e )
        {
        }

        /// <Summary>
        /// PreviousPage runs when the Back Button is clicked and raises a PreviousPage
        /// Event
        /// </Summary>
        private void PreviousPage( object sender, CommandEventArgs e )
        {
            if (CurrentPage > 0)
            {
                int PrevPage = CurrentPage;
                WizardCancelEventArgs wce = new WizardCancelEventArgs(CurrentPage, PrevPage, Pages);
                OnBeforePageChanged(wce);
                if (wce.Cancel == false)
                {
                    //Event Not Cancelled by Event Consumer
                    CurrentPage--;
                    WizardEventArgs we = new WizardEventArgs(CurrentPage, PrevPage, Pages);
                    OnAfterPageChanged(we);
                }
            }

            DisplayCurrentPage();
        }

        /// <Summary>ShowHelp runs when the Back Buttn is clicked</Summary>
        private void ShowHelp( object sender, CommandEventArgs e )
        {
            this.DisplayHelp( ( ! this.WizardHelpPane.Visible ) );
        }
    }
}