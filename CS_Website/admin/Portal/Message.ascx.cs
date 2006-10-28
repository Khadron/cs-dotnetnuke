using System;
using System.Diagnostics;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Common.Controls
{
    /// <summary>
    /// The Message is used to display Messages.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    public partial class Message : PortalModuleBase
    {
        
        private void InitializeComponent()
        {
            this.ID = "Message";
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}