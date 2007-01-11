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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Containers
{
    /// <Summary>ActionBase is the base for the Action objects</Summary>
    public abstract class ActionBase : SkinObjectBase
    {

        public event ActionEventHandler Action
        {
            add
            {
                this.ActionEvent += value;
            }
            remove
            {
                this.ActionEvent -= value;
            }
        }
        private bool _editMode;
        private ModuleInfo _moduleConfiguration;
        private PortalModuleBase _portalModule;
        private ActionEventHandler ActionEvent;
        protected bool m_adminControl;
        protected bool m_adminModule;
        protected ModuleAction m_menuActionRoot;
        protected ModuleActionCollection m_menuActions;
        protected bool m_supportsIcons;
        protected bool m_tabPreview;

        public ModuleAction ActionRoot
        {
            get
            {
                if( this.m_menuActionRoot != null )
                {
                    return this.m_menuActionRoot;
                }
                this.m_menuActionRoot = new ModuleAction( this.GetNextActionID(), " ", "", "", "action.gif" );
                return this.m_menuActionRoot;
            }
        }

        public bool EditMode
        {
            get
            {
                if( this.PortalModule.ModuleConfiguration == null )
                {
                    return this._editMode;
                }
                this._editMode = ( PortalSecurity.IsInRoles( this.PortalModule.PortalSettings.AdministratorRoleName ) | PortalSecurity.IsInRoles( this.PortalModule.PortalSettings.ActiveTab.AdministratorRoles.ToString() ) );
                return this._editMode;
            }
        }

        [Obsolete( "This property has been obsoleted: Use EditMode" )]
        protected bool m_editMode
        {
            get
            {
                return this._editMode;
            }
            set
            {
                this._editMode = value;
            }
        }

        [ObsoleteAttribute( "This property has been obsoleted: Use ModuleConfiguration" )]
        protected ModuleInfo m_moduleConfiguration
        {
            get
            {
                return this.ModuleConfiguration;
            }
            set
            {
                this._moduleConfiguration = value;
            }
        }

        [ObsoleteAttribute( "This property has been obsoleted: Use PortalModule" )]
        protected PortalModuleBase m_PortalModule
        {
            get
            {
                return this.PortalModule;
            }
            set
            {
                this.PortalModule = value;
            }
        }

        public ModuleActionCollection MenuActions
        {
            get
            {
                if( this.m_menuActions != null )
                {
                    return this.m_menuActions;
                }
                this.m_menuActions = new ModuleActionCollection();
                return this.m_menuActions;
            }
        }

        public ModuleInfo ModuleConfiguration
        {
            get
            {
                this._moduleConfiguration = this.PortalModule.ModuleConfiguration;
                return this._moduleConfiguration;
            }
        }

        public PortalModuleBase PortalModule
        {
            get
            {
                return this._portalModule;
            }
            set
            {
                this._portalModule = value;
                if( ( this.Request.Cookies[( "_Tab_Admin_Preview" + this.PortalModule.PortalSettings.PortalId )] == null ) || this.PortalModule.PortalSettings.ActiveTab.IsAdminTab )
                {
                    return;
                }
                this.m_tabPreview = bool.Parse( this.Request.Cookies[( "_Tab_Admin_Preview" + this.PortalModule.PortalSettings.PortalId )].Value );
            }
        }

        public bool SupportsIcons
        {
            get
            {
                return this.m_supportsIcons;
            }
        }

        public ActionBase()
        {
            Init += new EventHandler( this.Page_Init );
            Load += new EventHandler( this.Page_Load );
            this._editMode = false;
            this.m_adminControl = false;
            this.m_adminModule = false;
            this.m_supportsIcons = true;
            this.m_tabPreview = false;
        }

        public ModuleAction GetAction( int Index )
        {
            return this.GetAction( Index, this.ActionRoot );
        }

        public ModuleAction GetAction( int Index, ModuleAction ParentAction )
        {
            ModuleAction retAction = null;
            if (ParentAction != null)
            {
                foreach (ModuleAction tempLoopVar_modaction in ParentAction.Actions)
                {
                    ModuleAction modaction = tempLoopVar_modaction;
                    if (modaction.ID == Index)
                    {
                        retAction = modaction;
                        break;
                    }
                    if (modaction.HasChildren())
                    {
                        ModuleAction ChildAction = GetAction(Index, modaction);
                        if (ChildAction != null)
                        {
                            retAction = ChildAction;
                            break;
                        }
                    }
                }


            }
            return retAction;
        }

        protected bool DisplayControl(DNNNodeCollection objNodes)
        {
            if (objNodes != null && objNodes.Count > 0 && m_tabPreview == false)
            {
                DNNNode objRootNode = objNodes[0];
                if (objRootNode.HasNodes && objRootNode.DNNNodes.Count == 0)
                {
                    //if has pending node then display control
                    return true;
                }
                else if (objRootNode.DNNNodes.Count > 0)
                {
                    //verify that at least one child is not a break
                    foreach (DNNNode childNode in objRootNode.DNNNodes)
                    {
                        if (!childNode.IsBreak)
                        {
                            //Found a child so make Visible
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected int GetNextActionID()
        {
            int retValue = Null.NullInteger;
            if (PortalModule != null)
            {
                retValue = PortalModule.GetNextActionID();
            }
            return retValue;
        }

        [ObsoleteAttribute( "This function has been obsoleted: Use the no parameters version GetNextActionID()" )]
        protected int GetNextActionID( ModuleAction ModAction, int Level )
        {
            return this.PortalModule.GetNextActionID();
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon )
        {
            this.AddAction( Title, CmdName, CmdArg, Icon, "", false, SecurityAccessLevel.Anonymous, false, false );
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon, string Url )
        {
            this.AddAction( Title, CmdName, CmdArg, Icon, Url, false, SecurityAccessLevel.Anonymous, false, false );
        }

        public void AddAction( string Title, string CmdName )
        {
            this.AddAction( Title, CmdName, "", "", "", false, SecurityAccessLevel.Anonymous, false, false );
        }

        public void AddAction( string Title, string CmdName, string CmdArg )
        {
            this.AddAction( Title, CmdName, CmdArg, "", "", false, SecurityAccessLevel.Anonymous, false, false );
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent, SecurityAccessLevel Secure, bool visible )
        {
            this.AddAction( Title, CmdName, CmdArg, Icon, Url, UseActionEvent, Secure, visible, false );
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent, SecurityAccessLevel Secure, bool visible, bool NewWindow )
        {
            this.MenuActions.Add( this.GetNextActionID(), Title, CmdName, CmdArg, Icon, Url, UseActionEvent, Secure, visible, NewWindow );
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent )
        {
            this.AddAction( Title, CmdName, CmdArg, Icon, Url, UseActionEvent, SecurityAccessLevel.Anonymous, false, false );
        }

        public void AddAction( string Title, string CmdName, string CmdArg, string Icon, string Url, bool UseActionEvent, SecurityAccessLevel Secure )
        {
            this.AddAction( Title, CmdName, CmdArg, Icon, Url, UseActionEvent, Secure, false, false );
        }

        private void ClearCache( ModuleAction Command )
        {
            // synchronize cache
            PortalModule.SynchronizeModule();

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }

        private void Delete( ModuleAction Command )
        {
            ModuleController objModules = new ModuleController();

            ModuleInfo objModule = objModules.GetModule(int.Parse(Command.CommandArgument), PortalModule.TabId);
            if (objModule != null)
            {
                objModules.DeleteTabModule(PortalModule.TabId, int.Parse(Command.CommandArgument));

                UserInfo m_UserInfo = UserController.GetCurrentUserInfo();
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog(objModule, this.PortalSettings, m_UserInfo.UserID, "", EventLogController.EventLogType.MODULE_SENT_TO_RECYCLE_BIN);
            }

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }

        private void DoAction( ModuleAction Command )
        {
            if (Command.NewWindow)
            {
                Response.Write("<script>window.open('" + Command.Url + "','_blank')</script>");
            }
            else
            {
                Response.Redirect(Command.Url, true);
            }
        }

        private void MoveToPane( ModuleAction Command )
        {
            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleOrder(PortalModule.TabId, PortalModule.ModuleConfiguration.ModuleID, -1, Command.CommandArgument);
            objModules.UpdateTabModuleOrder(PortalModule.TabId, PortalModule.PortalId);

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }

        private void MoveUpDown( ModuleAction Command )
        {
            ModuleController objModules = new ModuleController();
            switch (Command.CommandName)
            {
                case ModuleActionType.MoveTop:

                    objModules.UpdateModuleOrder(PortalModule.TabId, PortalModule.ModuleConfiguration.ModuleID, 0, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveUp:

                    objModules.UpdateModuleOrder(PortalModule.TabId, PortalModule.ModuleConfiguration.ModuleID, PortalModule.ModuleConfiguration.ModuleOrder - 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveDown:

                    objModules.UpdateModuleOrder(PortalModule.TabId, PortalModule.ModuleConfiguration.ModuleID, PortalModule.ModuleConfiguration.ModuleOrder + 3, Command.CommandArgument);
                    break;
                case ModuleActionType.MoveBottom:

                    objModules.UpdateModuleOrder(PortalModule.TabId, PortalModule.ModuleConfiguration.ModuleID, (PortalModule.ModuleConfiguration.PaneModuleCount * 2) + 1, Command.CommandArgument);
                    break;
            }

            objModules.UpdateTabModuleOrder(PortalModule.TabId, PortalModule.PortalId);

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, true);
        }

        protected virtual void OnAction( ActionEventArgs e )
        {
            if (ActionEvent != null)
            {
                ActionEvent(this, e);
            }
        }

        protected void Page_Init( object sender, EventArgs e )
        {
        }

        /// <Summary>Page_Load runs when the class is loaded</Summary>
        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                ActionRoot.Actions.AddRange(MenuActions);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public void ProcessAction( string actionID )
        {
            int actionNumber;
            
            if (Int32.TryParse(actionID, out actionNumber))
            {
                ModuleAction action = GetAction(actionNumber);
                if (action.CommandName == ModuleActionType.ModuleHelp)
                {
                    DoAction(action);
                }
                else if (action.CommandName == ModuleActionType.OnlineHelp)
                {
                    DoAction(action);
                }
                else if (action.CommandName == ModuleActionType.ModuleSettings)
                {
                    DoAction(action);
                }
                else if (action.CommandName == ModuleActionType.DeleteModule)
                {
                    Delete(action);
                }
                else if (action.CommandName == ModuleActionType.PrintModule)
                {
                    DoAction(action);
                }
                else if (action.CommandName == ModuleActionType.ClearCache)
                {
                    ClearCache(action);
                }
                else if (action.CommandName == ModuleActionType.MovePane)
                {
                    MoveToPane(action);
                }
                else if (action.CommandName == ModuleActionType.MoveTop)
                {
                    MoveUpDown(action);
                }
                else if (action.CommandName == ModuleActionType.MoveUp)
                {
                    MoveUpDown(action);
                }
                else if (action.CommandName == ModuleActionType.MoveDown)
                {
                    MoveUpDown(action);
                }
                else if (action.CommandName == ModuleActionType.MoveBottom)
                {
                    MoveUpDown(action);
                }
                else
                {
                    // custom action

                    if (action.Url.Length > 0 && action.UseActionEvent == false)
                    {
                        DoAction(action);
                    }
                    else
                    {
                        OnAction(new ActionEventArgs(action, ModuleConfiguration));
                    }
                }
            }
        }
    }
}