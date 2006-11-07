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
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Containers
{
    public partial class ActionButton : ActionBase
    {
        private string _buttonSeparator = "&nbsp;&nbsp;";
        private string _commandName = "";
        private string _cssClass = "CommandButton";
        private bool _displayLink = true;
        private bool _displayIcon = false;
        private string _iconFile;

        /// <summary>
        /// Gets or sets the Command Name
        /// </summary>
        /// <remarks>Maps to ModuleActionType in DotNetNuke.Entities.Modules.Actions</remarks>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
        public string CommandName
        {
            get
            {
                return _commandName;
            }
            set
            {
                _commandName = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSS Class
        /// </summary>
        /// <remarks>Defaults to 'CommandButton'</remarks>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
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

        /// <summary>
        /// Gets or sets whether the link is displayed
        /// </summary>
        /// <remarks>Defaults to True</remarks>
        /// <value>A Boolean</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
        public bool DisplayLink
        {
            get
            {
                return _displayLink;
            }
            set
            {
                _displayLink = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the icon is displayed
        /// </summary>
        /// <remarks>Defaults to False</remarks>
        /// <value>A Boolean</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
        public bool DisplayIcon
        {
            get
            {
                return _displayIcon;
            }
            set
            {
                _displayIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the Icon used
        /// </summary>
        /// <remarks>Defaults to the icon defined in Action</remarks>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
        public string IconFile
        {
            get
            {
                return _iconFile;
            }
            set
            {
                _iconFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the Separator between Buttons
        /// </summary>
        /// <remarks>Defaults to 2 non-breaking spaces</remarks>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	6/29/2005	Documented
        /// </history>
        public string ButtonSeparator
        {
            get
            {
                return _buttonSeparator;
            }
            set
            {
                _buttonSeparator = value;
            }
        }

        private void GetClientScriptURL( ModuleAction Action, WebControl control )
        {
            if( Action.ClientScript.Length > 0 )
            {
                string Script = Action.ClientScript;

                int JSPos = Script.ToLower().IndexOf( "javascript:" );
                if( JSPos > - 1 )
                {
                    Script = Script.Substring( JSPos + 11 );
                }

                string FormatScript = "javascript: return {0};";

                control.Attributes.Add( "onClick", string.Format( FormatScript, Script ) );
            }
        }

        private void LinkAction_Click( object sender, EventArgs e )
        {
            try
            {
                ProcessAction( ( (LinkButton)sender ).ID.Substring( 3 ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                UserInfo _UserInfo = UserController.GetCurrentUserInfo();

                foreach( ModuleAction action in this.MenuActions )
                {
                    if( action.CommandName == CommandName )
                    {
                        if( action.Visible && PortalSecurity.HasNecessaryPermission( action.Secure, PortalSettings, PortalModule.ModuleConfiguration, _UserInfo.UserID.ToString() ) )
                        {
                            bool blnPreview = false;
                            if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] != null )
                            {
                                blnPreview = bool.Parse( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()].Value );
                            }
                            if( blnPreview == false || ( action.Secure == SecurityAccessLevel.Anonymous || action.Secure == SecurityAccessLevel.View ) )
                            {
                                if( action.CommandName == ModuleActionType.PrintModule && PortalModule.ModuleConfiguration.DisplayPrint == false )
                                {
                                    DisplayIcon = false;
                                    DisplayLink = false;
                                }
                                if( action.CommandName == ModuleActionType.SyndicateModule && PortalModule.ModuleConfiguration.DisplaySyndicate == false )
                                {
                                    DisplayIcon = false;
                                    DisplayLink = false;
                                }

                                if( DisplayIcon && ( action.Icon != "" || IconFile != "" ) )
                                {
                                    Image ModuleActionIcon = new Image(); //New ImageButton
                                    LinkButton ModuleActionLink = new LinkButton();
                                    if( IconFile != "" )
                                    {
                                        ModuleActionIcon.ImageUrl = PortalModule.ModuleConfiguration.ContainerPath.Substring( 0, PortalModule.ModuleConfiguration.ContainerPath.LastIndexOf( "/" ) + 1 ) + IconFile;
                                    }
                                    else
                                    {
                                        ModuleActionIcon.ImageUrl = "~/images/" + action.Icon;
                                    }
                                    ModuleActionIcon.ToolTip = action.Title;
                                    ModuleActionLink.ID = "ico" + action.ID.ToString();
                                    ModuleActionLink.CausesValidation = false;
                                    ModuleActionLink.EnableViewState = false;
                                    GetClientScriptURL( action, ModuleActionLink );

                                    ModuleActionLink.Click += new EventHandler( LinkAction_Click ); //IconAction_Click

                                    ModuleActionLink.Controls.Add( ModuleActionIcon );
                                    this.Controls.Add( ModuleActionLink );
                                }

                                if( DisplayLink )
                                {
                                    LinkButton ModuleActionLink = new LinkButton();
                                    ModuleActionLink.Text = action.Title;
                                    ModuleActionLink.ToolTip = action.Title;
                                    ModuleActionLink.CssClass = CssClass;
                                    ModuleActionLink.ID = "lnk" + action.ID.ToString();
                                    ModuleActionLink.CausesValidation = false;
                                    ModuleActionLink.EnableViewState = false;
                                    GetClientScriptURL( action, ModuleActionLink );

                                    ModuleActionLink.Click += new EventHandler( LinkAction_Click );

                                    this.Controls.Add( ModuleActionLink );
                                }

                                this.Controls.Add( new LiteralControl( ButtonSeparator ) );
                            }
                        }
                    }
                }

                // set visibility
                if( this.Controls.Count > 0 )
                {
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}