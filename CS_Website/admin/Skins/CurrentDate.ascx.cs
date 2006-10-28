using System;
using System.Diagnostics;
using DotNetNuke.Entities.Users;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class CurrentDate : SkinObjectBase
    {
        // private members
        private string _cssClass;
        private string _dateFormat;

        // protected controls

        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

        public string DateFormat
        {
            get
            {
                return _dateFormat;
            }
            set
            {
                _dateFormat = value;
            }
        }

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            // public attributes
            if( CssClass != "" )
            {
                lblDate.CssClass = CssClass;
            }

            UserTime objUserTime = new UserTime();
            if( DateFormat != "" )
            {
                lblDate.Text = Strings.Format( objUserTime.CurrentUserTime, DateFormat );
            }
            else
            {
                lblDate.Text = objUserTime.CurrentUserTime.ToLongDateString();
            }
        }
    }
}