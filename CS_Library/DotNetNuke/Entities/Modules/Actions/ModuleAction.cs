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
using DotNetNuke.Security;

namespace DotNetNuke.Entities.Modules.Actions
{
    /// <Summary>
    /// Each Module Action represents a separate functional action as defined by the
    /// associated module.
    /// </Summary>
    public class ModuleAction
    {
        private ModuleActionCollection _actions;
        private string _clientScript;
        private string _commandArgument;
        private string _commandName;
        private string _icon;
        private int _id;
        private bool _newwindow;
        private SecurityAccessLevel _secure;
        private string _title;
        private string _url;
        private bool _useActionEvent;
        private bool _visible;

        public ModuleAction( int ID ) : this( ID, "", "", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName ) : this( ID, Title, CmdName, "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg ) : this( ID, Title, CmdName, CmdArg, "", "", "", false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon ) : this( ID, Title, CmdName, CmdArg, Icon, "", "", false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url ) : this( ID, Title, CmdName, CmdArg, Icon, Url, "", false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript ) : this( ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, false, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent ) : this( ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, SecurityAccessLevel.Anonymous, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure ) : this( ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, true, false )
        {
        }

        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible ) : this( ID, Title, CmdName, CmdArg, Icon, Url, ClientScript, UseActionEvent, Secure, Visible, false )
        {
        }

        /// <Summary>
        /// Initializes a new instance of the  class using the specified parameters
        /// </Summary>
        /// <Param name="ID">This is the identifier to use for this action.</Param>
        /// <Param name="Title">
        /// This is the title that will be displayed for this action
        /// </Param>
        /// <Param name="CmdName">
        /// The command name passed to the client when this action is clicked.
        /// </Param>
        /// <Param name="CmdArg">
        /// The command argument passed to the client when this action is clicked.
        /// </Param>
        /// <Param name="Icon">The URL of the Icon to place next to this action</Param>
        /// <Param name="Url">
        /// The destination URL to redirect the client browser when this action is clicked.
        /// </Param>
        /// <Param name="UseActionEvent">
        /// Determines whether client will receive an event notification
        /// </Param>
        /// <Param name="Secure">
        /// The security access level required for access to this action
        /// </Param>
        /// <Param name="Visible">Whether this action will be displayed</Param>
        public ModuleAction( int ID, string Title, string CmdName, string CmdArg, string Icon, string Url, string ClientScript, bool UseActionEvent, SecurityAccessLevel Secure, bool Visible, bool NewWindow )
        {
            this._id = ID;
            this._title = Title;
            this._commandName = CmdName;
            this._commandArgument = CmdArg;
            this._icon = Icon;
            this._url = Url;
            this._clientScript = ClientScript;
            this._useActionEvent = UseActionEvent;
            this._secure = Secure;
            this._visible = Visible;
            this._newwindow = NewWindow;
        }

        /// <Summary>
        /// The Actions property allows the user to create a heirarchy of actions, with
        /// each action having sub-actions.
        /// </Summary>
        public ModuleActionCollection Actions
        {
            get
            {
                if( this._actions != null )
                {
                    return this._actions;
                }
                this._actions = new ModuleActionCollection();
                return this._actions;
            }
            set
            {
                this._actions = value;
            }
        }

        /// <Summary>
        /// Gets or sets javascript which will be run in the clients browser
        /// when the associated Module menu Action is selected. prior to a postback.
        /// </Summary>
        public string ClientScript
        {
            get
            {
                return this._clientScript;
            }
            set
            {
                this._clientScript = value;
            }
        }

        /// <Summary>
        /// A Module Action CommandArgument provides additional information and
        /// complements the CommandName.
        /// </Summary>
        public string CommandArgument
        {
            get
            {
                return this._commandArgument;
            }
            set
            {
                this._commandArgument = value;
            }
        }

        /// <Summary>
        /// A Module Action CommandName represents a string used by the ModuleTitle to notify
        /// the parent module that a given Module Action was selected in the Module Menu.
        /// </Summary>
        public string CommandName
        {
            get
            {
                return this._commandName;
            }
            set
            {
                this._commandName = value;
            }
        }

        /// <Summary>
        /// Gets or sets the URL for the icon file that is displayed for the given .
        /// </Summary>
        public string Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                this._icon = value;
            }
        }

        /// <Summary>
        /// A Module Action ID is a identifier that can be used in a Module Action Collection
        /// to find a specific Action.
        /// </Summary>
        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        /// <Summary>
        /// Gets or sets a value that determines if a new window is opened when the
        /// DoAction() method is called.
        /// </Summary>
        public bool NewWindow
        {
            get
            {
                return this._newwindow;
            }
            set
            {
                this._newwindow = value;
            }
        }

        /// <Summary>
        /// Gets or sets the value indicating the  that is required to access this .
        /// </Summary>
        public SecurityAccessLevel Secure
        {
            get
            {
                return this._secure;
            }
            set
            {
                this._secure = value;
            }
        }

        /// <Summary>
        /// Gets or sets the string that is displayed in the Module Menu
        /// that represents a given menu action.
        /// </Summary>
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
            }
        }

        /// <Summary>
        /// Gets or sets the URL to which the user is redirected when the
        /// associated Module Menu Action is selected.
        /// </Summary>
        public string Url
        {
            get
            {
                return this._url;
            }
            set
            {
                this._url = value;
            }
        }

        /// <Summary>
        /// Gets or sets a value that determines if a local ActionEvent is fired when the
        /// contains a URL.
        /// </Summary>
        public bool UseActionEvent
        {
            get
            {
                return this._useActionEvent;
            }
            set
            {
                this._useActionEvent = value;
            }
        }

        /// <Summary>
        /// Gets or sets whether the current action should be displayed.
        /// </Summary>
        public bool Visible
        {
            get
            {
                return this._visible;
            }
            set
            {
                this._visible = value;
            }
        }

        /// <Summary>
        /// Determines whether the action node contains any child actions.
        /// </Summary>
        /// <Returns>
        /// True if child actions exist, false if child actions do not exist.
        /// </Returns>
        public bool HasChildren()
        {
            return ( this.Actions.Count > 0 );
        }
    }
}