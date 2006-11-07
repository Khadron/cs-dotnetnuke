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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Entities.Modules
{
    /// <Summary>
    /// The PortalModuleBase class defines a custom base class inherited by all
    /// desktop portal modules within the Portal.
    /// The PortalModuleBase class defines portal specific properties
    /// that are used by the portal framework to correctly display portal modules
    /// </Summary>
    public class PortalModuleBase : UserControlBase
    {
        private ModuleActionCollection _actions;
        private string _cachedOutput = "";
        private string _helpfile;
        private string _helpurl;
        private int _isEditable = 0;
        private string _localResourceFile;
        private ModuleInfo _moduleConfiguration;
        private int _nextActionId = -1;
        private Hashtable _settings;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleActionCollection Actions
        {
            get
            {
                if (_actions == null)
                {
                    LoadActions();
                }
                return _actions;
            }
            set
            {
                _actions = value;
            }
        }

        /// <summary>
        /// The CacheDirectory property is used to return the location of the "Cache"
        /// Directory for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string CacheDirectory
        {
            get
            {
                return PortalSettings.HomeDirectoryMapPath + "Cache";
            }
        }

        /// <summary>
        /// The CacheFileName property is used to store the FileName for this Module's
        /// Cache
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string CacheFileName
        {
            get
            {
                return CacheDirectory + "\\" + Globals.CleanFileName(CacheKey) + ".htm";
            }
        }

        /// <summary>
        /// The CacheKey property is used to calculate a "unique" cache key
        /// entry to be used to store/retrieve the portal module's content
        /// from the ASP.NET Cache. Note that cache key allows two versions of the module
        /// content to be stored - one for anonymous and one for authenticated users.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string CacheKey
        {
            get
            {
                string strCacheKey = "TabModule:";
                strCacheKey += TabModuleId.ToString() + ":";
                strCacheKey += Thread.CurrentThread.CurrentCulture.ToString();
                return strCacheKey;
            }
        }

        /// <summary>
        /// The CacheMethod property is used to store the Method used for this Module's
        /// Cache
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string CacheMethod
        {
            get
            {
                return Convert.ToString(Globals.HostSettings["ModuleCaching"]);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control ContainerControl
        {
            get
            {
                return Globals.FindControlRecursive(this, "ctr" + ModuleId.ToString());
            }
        }

        public string ControlName
        {
            get
            {
                return this.GetType().Name.Replace("_", ".");
            }
        }

        /// <summary>
        /// The EditMode property is used to determine whether the user is in the
        /// Administrator role
        /// Cache
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool EditMode
        {
            get
            {
                return PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString());
            }
        }

        [ObsoleteAttribute("The HelpFile() property was deprecated in version 2.2. Help files are now stored in the /App_LocalResources folder beneath the module with the following resource key naming convention: ModuleHelp.Text")]
        public string HelpFile
        {
            get
            {
                return _helpfile;
            }
            set
            {
                _helpfile = value;
            }
        }

        public string HelpURL
        {
            get
            {
                return _helpurl;
            }
            set
            {
                _helpurl = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditable
        {
            get
            {
                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (_isEditable == 0)
                {
                    bool blnPreview = false;
                    HttpRequest Request = HttpContext.Current.Request;
                    if (Request.Cookies["_Tab_Admin_Preview" + PortalId.ToString()] != null)
                    {
                        blnPreview = bool.Parse(Request.Cookies["_Tab_Admin_Preview" + PortalId.ToString()].Value);
                    }
                    if (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId || PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                    {
                        blnPreview = false;
                    }

                    bool blnHasModuleEditPermissions = false;
                    if (_moduleConfiguration != null)
                    {
                        blnHasModuleEditPermissions = (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles) == true) || (PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles) == true) || (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) == true);
                    }

                    if (blnPreview == false && blnHasModuleEditPermissions == true)
                    {
                        _isEditable = 1;
                    }
                    else
                    {
                        _isEditable = 2;
                    }
                }

                return _isEditable == 1;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                string fileRoot;

                if (_localResourceFile == "")
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" + this.ID;
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                _localResourceFile = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return _moduleConfiguration;
            }
            set
            {
                _moduleConfiguration = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ModuleId
        {
            get
            {
                if (_moduleConfiguration != null)
                {
                    return Convert.ToInt32(_moduleConfiguration.ModuleID);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public string ModulePath
        {
            get
            {
                return this.TemplateSourceDirectory + "/";
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalAliasInfo PortalAlias
        {
            get
            {
                return PortalSettings.PortalAlias;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PortalId
        {
            get
            {
                return PortalSettings.PortalId;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = PortalSettings.GetTabModuleSettings(new Hashtable(PortalSettings.GetModuleSettings(ModuleId)), new Hashtable(PortalSettings.GetTabModuleSettings(TabModuleId)));
                }
                return _settings;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabId
        {
            get
            {
                if (_moduleConfiguration != null)
                {
                    return Convert.ToInt32(_moduleConfiguration.TabID);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabModuleId
        {
            get
            {
                if (_moduleConfiguration != null)
                {
                    return Convert.ToInt32(_moduleConfiguration.TabModuleID);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int UserId
        {
            get
            {
                int returnValue;
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    returnValue = UserInfo.UserID;
                }
                else
                {
                    returnValue = -1;
                }
                return returnValue;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserInfo UserInfo
        {
            get
            {
                return UserController.GetCurrentUserInfo();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl()
        {
            return EditUrl("", "", "Edit");
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string ControlKey)
        {
            return EditUrl("", "", ControlKey);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue)
        {
            return EditUrl(KeyName, KeyValue, "Edit");
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey)
        {
            string key = ControlKey;

            if (key == "")
            {
                key = "Edit";
            }

            if (KeyName != "" && KeyValue != "")
            {
                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, "mid=" + ModuleId.ToString(), KeyName + "=" + KeyValue);
            }
            else
            {
                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, "mid=" + ModuleId.ToString());
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey, params string[] AdditionalParameters)
        {
            string key = ControlKey;

            if (key == "")
            {
                key = "Edit";
            }

            if (KeyName != "" && KeyValue != "")
            {
                string[] parameters = new string[AdditionalParameters.Length + 1 + 1];

                parameters[0] = "mid=" + ModuleId.ToString();
                parameters[1] = KeyName + "=" + KeyValue;

                for (int i = 0; i <= AdditionalParameters.Length - 1; i++)
                {
                    parameters[i + 2] = AdditionalParameters[i];
                }

                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, parameters);
            }
            else
            {
                string[] parameters = new string[AdditionalParameters.Length + 1];

                parameters[0] = "mid=" + ModuleId.ToString();

                for (int i = 0; i <= AdditionalParameters.Length - 1; i++)
                {
                    parameters[i + 1] = AdditionalParameters[i];
                }

                return Globals.NavigateURL(PortalSettings.ActiveTab.TabID, key, parameters);
            }
        }

        private int GetActionsCount(int count, ModuleActionCollection actions)
        {
            foreach (ModuleAction action in actions)
            {
                if (action.HasChildren())
                {
                    count += action.Actions.Count;

                    //Recursively call to see if this collection has any child actions that would affect the count
                    count = GetActionsCount(count, action.Actions);
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the Next Action ID
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int GetNextActionID()
        {
            _nextActionId++;
            return _nextActionId;
        }

        public bool HasModulePermission(string PermissionKey)
        {
            return ModulePermissionController.HasModulePermission(this.ModuleConfiguration.ModulePermissions, PermissionKey);
        }

        /// <summary>
        /// Helper method that can be used to add an ActionEventHandler to the Skin for this
        /// Module Control
        /// </summary>
        /// <remarks>
        /// </remarks>
        protected void AddActionHandler(ActionEventHandler e)
        {
            //This finds a reference to the containing skin
            UI.Skins.Skin ParentSkin = UI.Skins.Skin.GetParentSkin(this);
            //We should always have a ParentSkin, but need to make sure
            if (ParentSkin != null)
            {
                //Register our EventHandler as a listener on the ParentSkin so that it may tell us
                //when a menu has been clicked.
                ParentSkin.RegisterModuleActionEvent(this.ModuleId, e);
            }
        }

        /// <summary>
        /// The CreateChildControls method is called when the ASP.NET Page Framework
        /// determines that it is time to instantiate a server control.
        /// This method and attempts to resolve any previously cached output of the portal
        /// module from the ASP.NET cache.
        /// If it doesn't find cached output from a previous request, then it will instantiate
        /// and add the portal modules UserControl instance into the page tree.
        /// </summary>
        /// <remarks>
        /// </remarks>
        protected override void CreateChildControls()
        {
            if (_moduleConfiguration != null)
            {
                // if user does not have EDIT rights for the module ( content editors can not see cached versions of modules )
                if (PortalSecurity.HasEditPermissions(_moduleConfiguration.ModuleID) == false)
                {
                    // Attempt to resolve previously cached content
                    if (_moduleConfiguration.CacheTime != 0)
                    {
                        if (CacheMethod != "D")
                        {
                            _cachedOutput = Convert.ToString(DataCache.GetCache(CacheKey));
                        }
                        else // cache from disk
                        {
                            if (File.Exists(CacheFileName))
                            {
                                FileInfo cacheFile = new FileInfo(CacheFileName);
                                if (cacheFile.CreationTime.AddSeconds(_moduleConfiguration.CacheTime) >= DateTime.Now)
                                {
                                    //Load from Cache
                                    StreamReader objStreamReader;
                                    objStreamReader = cacheFile.OpenText();
                                    _cachedOutput = objStreamReader.ReadToEnd();
                                    objStreamReader.Close();
                                }
                                else
                                {
                                    //Cache Expired so delete it
                                    cacheFile.Delete();
                                }
                            }
                        }
                    }

                    // If no cached content is found, then instantiate and add the portal
                    // module user control into the portal's page server control tree
                    if (_cachedOutput == "" && _moduleConfiguration.CacheTime > 0)
                    {
                        base.CreateChildControls();

                        PortalModuleBase objPortalModuleBase = (PortalModuleBase)Page.LoadControl(_moduleConfiguration.ControlSrc);
                        objPortalModuleBase.ModuleConfiguration = this.ModuleConfiguration;

                        // In skin.vb, the call to Me.Controls.Add(objPortalModuleBase) calls CreateChildControls() therefore
                        // we need to indicate the control has already been created. We will manipulate the CacheTime property for this purpose.
                        objPortalModuleBase.ModuleConfiguration.CacheTime = -(objPortalModuleBase.ModuleConfiguration.CacheTime);

                        this.Controls.Add(objPortalModuleBase);
                    }
                    else
                    {
                        // restore the CacheTime property in preparation for the Render() event
                        if (_moduleConfiguration.CacheTime < 0)
                        {
                            _moduleConfiguration.CacheTime = -(_moduleConfiguration.CacheTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// LoadActions loads the Actions collections
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void LoadActions()
        {
            _actions = new ModuleActionCollection();

            //check if module Implements Entities.Modules.IActionable interface
            if (this is IActionable)
            {
                // load module actions
                ModuleActionCollection ModuleActions = ((IActionable)this).ModuleActions;

                _actions.AddRange(ModuleActions);
                _actions.Add(GetNextActionID(), "~", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false);
                foreach (ModuleAction action in _actions)
                {
                    if (action.Icon == "")
                    {
                        action.Icon = "edit.gif";
                    }
                }
            }

            //Make sure the Next Action Id counter is correct
            int actionCount = GetActionsCount(_actions.Count, _actions);
            if (_nextActionId != actionCount)
            {
                _nextActionId = actionCount;
            }

            // check if module implements IPortable interface
            if (this.ModuleConfiguration.IsPortable && !Globals.IsAdminControl() && this.ModuleConfiguration.BusinessControllerClass != "")
            {
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.ImportModule, Localization.GlobalResourceFile), "", "", "rt.gif", Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ImportModule", "moduleid=" + ModuleId.ToString()), "", false, SecurityAccessLevel.Admin, EditMode, false);
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.ExportModule, Localization.GlobalResourceFile), "", "", "lt.gif", Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ExportModule", "moduleid=" + ModuleId.ToString()), "", false, SecurityAccessLevel.Admin, EditMode, false);
            }

            //If TypeOf objPortalModuleBase Is ISearchable Then
            if (this.ModuleConfiguration.IsSearchable && !Globals.IsAdminControl() && this.ModuleConfiguration.BusinessControllerClass != "")
            {
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.SyndicateModule, Localization.GlobalResourceFile), ModuleActionType.SyndicateModule, "", "xml.gif", Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "", "moduleid=" + ModuleId.ToString()).Replace(Globals.glbDefaultPage, "RSS.aspx"), "", false, SecurityAccessLevel.Anonymous, true, true);
            }

            // help module actions available to content editors and administrators
            if (ModuleConfiguration.ControlType != SecurityAccessLevel.Anonymous && Request.QueryString["ctl"] != "Help")
            {
                SetHelpVisibility();
            }

            //Add Print Action
            if (ModuleConfiguration.DisplayPrint)
            {
                // print module action available to everyone
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.PrintModule, Localization.GlobalResourceFile), ModuleActionType.PrintModule, "", "print.gif", Globals.NavigateURL(TabId, "", "mid=" + ModuleId.ToString(), "SkinSrc=" + Globals.QueryStringEncode("[G]" + SkinInfo.RootSkin + "/" + Globals.glbHostSkinFolder + "/" + "No Skin"), "ContainerSrc=" + Globals.QueryStringEncode("[G]" + SkinInfo.RootContainer + "/" + Globals.glbHostSkinFolder + "/" + "No Container"), "dnnprintmode=true"), "", false, SecurityAccessLevel.Anonymous, true, true);
            }

            // core module actions only available to administrators
            if (EditMode == true && ModuleConfiguration.IsAdmin == false && Globals.IsAdminControl() == false)
            {
                // module settings
                _actions.Add(GetNextActionID(), "~", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false);
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.ModuleSettings, Localization.GlobalResourceFile), ModuleActionType.ModuleSettings, "", "settings.gif", Globals.NavigateURL(TabId, "Module", "ModuleId=" + ModuleId.ToString()), false, SecurityAccessLevel.Admin, true, false);
                _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.DeleteModule, Localization.GlobalResourceFile), ModuleActionType.DeleteModule, ModuleConfiguration.ModuleID.ToString(), "delete.gif", "", "confirm(\'" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteModule.Confirm")) + "\')", false, SecurityAccessLevel.Admin, true, false);
                if (ModuleConfiguration.CacheTime != 0)
                {
                    _actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.ClearCache, Localization.GlobalResourceFile), ModuleActionType.ClearCache, ModuleConfiguration.ModuleID.ToString(), "restore.gif", "", false, SecurityAccessLevel.Admin, true, false);
                }

                // module movement
                _actions.Add(GetNextActionID(), "~", "", "", "", "", false, SecurityAccessLevel.Anonymous, true, false);
                ModuleAction MoveActionRoot = new ModuleAction(GetNextActionID(), Localization.GetString(ModuleActionType.MoveRoot, Localization.GlobalResourceFile), "", "", "", "", "", false, SecurityAccessLevel.Admin, EditMode);

                // move module up/down
                if (ModuleConfiguration != null)
                {
                    SetMoveMenuVisibility(MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveTop, Localization.GlobalResourceFile), ModuleActionType.MoveTop, ModuleConfiguration.PaneName, "top.gif", "", false, SecurityAccessLevel.Admin, EditMode, false));
                    SetMoveMenuVisibility(MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveUp, Localization.GlobalResourceFile), ModuleActionType.MoveUp, ModuleConfiguration.PaneName, "up.gif", "", false, SecurityAccessLevel.Admin, EditMode, false));
                    SetMoveMenuVisibility(MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveDown, Localization.GlobalResourceFile), ModuleActionType.MoveDown, ModuleConfiguration.PaneName, "dn.gif", "", false, SecurityAccessLevel.Admin, EditMode, false));
                    SetMoveMenuVisibility(MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveBottom, Localization.GlobalResourceFile), ModuleActionType.MoveBottom, ModuleConfiguration.PaneName, "bottom.gif", "", false, SecurityAccessLevel.Admin, EditMode, false));
                }

                // move module to pane
                int intItem;
                for (intItem = 0; intItem <= PortalSettings.ActiveTab.Panes.Count - 1; intItem++)
                {
                    SetMoveMenuVisibility(MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MovePane, Localization.GlobalResourceFile) + " " + Convert.ToString(PortalSettings.ActiveTab.Panes[intItem]), ModuleActionType.MovePane, Convert.ToString(PortalSettings.ActiveTab.Panes[intItem]), "move.gif", "", false, SecurityAccessLevel.Admin, EditMode, false));
                }
                ModuleAction ma;
                foreach (ModuleAction tempLoopVar_ma in MoveActionRoot.Actions)
                {
                    ma = tempLoopVar_ma;
                    if (ma.Visible)
                    {
                        _actions.Add(MoveActionRoot);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The Render method is called when the ASP.NET Page Framework
        /// determines that it is time to render content into the page output stream.
        /// This method and captures the output generated by the portal module user control
        /// It then adds this content into the ASP.NET Cache for future requests.
        /// </summary>
        /// <remarks>
        /// </remarks>
        protected override void Render(HtmlTextWriter output)
        {
            if (_moduleConfiguration != null)
            {
                // If no caching is specified or in admin mode, render the child tree and return
                if (_moduleConfiguration.CacheTime == 0 || (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true))
                {
                    base.Render(output);
                }
                else //output caching enabled
                {
                    // If no cached output was found from a previous request, render
                    // child controls into a TextWriter, and then cache the results
                    if (_cachedOutput == "")
                    {
                        StringWriter tempWriter = new StringWriter();
                        base.Render(new HtmlTextWriter(tempWriter));
                        _cachedOutput = tempWriter.ToString();

                        if (CacheMethod != "D")
                        {
                            DataCache.SetCache(CacheKey, _cachedOutput, DateTime.Now.AddSeconds(_moduleConfiguration.CacheTime));
                        }
                        else // cache to disk
                        {
                            try
                            {
                                if (!Directory.Exists(CacheDirectory))
                                {
                                    Directory.CreateDirectory(CacheDirectory);
                                }

                                StreamWriter objStream;
                                objStream = File.CreateText(CacheFileName);
                                objStream.WriteLine(_cachedOutput);
                                objStream.Close();
                            }
                            catch
                            {
                                // error writing to disk
                            }
                        }
                    }

                    // Output the user control's content
                    output.Write(_cachedOutput);
                }
            }
            else
            {
                base.Render(output);
            }
        }

        /// <summary>
        /// SetHelpVisibility Adds the Help actions to the Action Menu
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void SetHelpVisibility()
        {
            //Add Help Menu Action
            ModuleAction helpAction = new ModuleAction(GetNextActionID());
            helpAction.Title = Localization.GetString(ModuleActionType.ModuleHelp, Localization.GlobalResourceFile);
            helpAction.CommandName = ModuleActionType.ModuleHelp;
            helpAction.CommandArgument = "";
            helpAction.Icon = "help.gif";
            helpAction.Url = Globals.NavigateURL(TabId, "Help", "ctlid=" + ModuleConfiguration.ModuleControlId.ToString(), "moduleid=" + ModuleId);
            helpAction.Secure = SecurityAccessLevel.Edit;
            helpAction.Visible = true;
            helpAction.NewWindow = false;
            helpAction.UseActionEvent = true;
            _actions.Add(helpAction);

            //Add OnLine Help Action
            string helpUrl = Globals.GetOnLineHelp(ModuleConfiguration.HelpUrl, ModuleConfiguration);
            if (!Null.IsNull(helpUrl))
            {
                //Add OnLine Help menu action
                helpAction = new ModuleAction(GetNextActionID());
                helpAction.Title = Localization.GetString(ModuleActionType.OnlineHelp, Localization.GlobalResourceFile);
                helpAction.CommandName = ModuleActionType.OnlineHelp;
                helpAction.CommandArgument = "";
                helpAction.Icon = "help.gif";
                helpAction.Url = Globals.FormatHelpUrl(helpUrl, PortalSettings, ModuleConfiguration.FriendlyName);
                helpAction.Secure = SecurityAccessLevel.Edit;
                helpAction.UseActionEvent = true;
                helpAction.Visible = true;
                helpAction.NewWindow = true;
                _actions.Add(helpAction);
            }
        }

        /// <summary>
        /// SetMoveMenuVisibility Adds the Move actions to the Action Menu
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void SetMoveMenuVisibility(ModuleAction Action)
        {
            switch (Action.CommandName)
            {
                case ModuleActionType.MoveTop:

                    Action.Visible = (ModuleConfiguration.ModuleOrder != 0) && (ModuleConfiguration.PaneModuleIndex > 0) && EditMode;
                    break;
                case ModuleActionType.MoveUp:

                    Action.Visible = (ModuleConfiguration.ModuleOrder != 0) && (ModuleConfiguration.PaneModuleIndex > 0) && EditMode;
                    break;
                case ModuleActionType.MoveDown:

                    Action.Visible = (ModuleConfiguration.ModuleOrder != 0) && (ModuleConfiguration.PaneModuleIndex < (ModuleConfiguration.PaneModuleCount - 1)) && EditMode;
                    break;
                case ModuleActionType.MoveBottom:

                    Action.Visible = (ModuleConfiguration.ModuleOrder != 0) && (ModuleConfiguration.PaneModuleIndex < (ModuleConfiguration.PaneModuleCount - 1)) && EditMode;
                    break;
                case ModuleActionType.MovePane:

                    Action.Visible = (ModuleConfiguration.PaneName.ToLower() != Action.CommandArgument.ToLower()) && EditMode;
                    break;
            }
        }

        public void SynchronizeModule()
        {
            if (CacheMethod != "D")
            {
                DataCache.RemoveCache(CacheKey);
            }
            else // disk caching
            {
                if (File.Exists(CacheFileName))
                {
                    try
                    {
                        File.Delete(CacheFileName);
                    }
                    catch
                    {
                        // error deleting file from disk
                    }
                }
            }
        }
    }
}