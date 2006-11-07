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
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Communications;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Containers;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins
{
    /// <Summary>Skin is the base for the Skins</Summary>
    public class Skin : UserControlBase
    {
        private ArrayList _actionEventListeners;
        private string CONTAINERLOAD_ERROR = Localization.GetString( "ContainerLoad.Error" );
        private string CONTRACTEXPIRED_ERROR = Localization.GetString( "ContractExpired.Error" );
        private string CRITICAL_ERROR = Localization.GetString( "CriticalError.Error" );
        private string MODULEACCESS_ERROR = Localization.GetString( "ModuleAccess.Error" );
        private string MODULEADD_ERROR = Localization.GetString( "ModuleAdd.Error" );
        private string MODULELOAD_ERROR = Localization.GetString( "ModuleLoad.Error" );
        private string MODULELOAD_WARNING = Localization.GetString( "ModuleLoadWarning.Error" );
        private string MODULELOAD_WARNINGTEXT = Localization.GetString( "ModuleLoadWarning.Text" );
        private ModuleCommunicate objCommunicator = new ModuleCommunicate();

        //Localized Strings
        private string PANE_LOAD_ERROR = Localization.GetString( "PaneNotFound.Error" );
        private string TABACCESS_ERROR = Localization.GetString( "TabAccess.Error" );

        public ArrayList ActionEventListeners
        {
            get
            {
                if( this._actionEventListeners != null )
                {
                    return this._actionEventListeners;
                }
                this._actionEventListeners = new ArrayList();
                return this._actionEventListeners;
            }
            set
            {
                this._actionEventListeners = value;
            }
        }

        public string SkinPath
        {
            get
            {
                return ( this.TemplateSourceDirectory + "/" );
            }
        }

        public Skin()
        {
            base.Init += new EventHandler( this.Page_Init );
        }

        public static ModuleMessage GetModuleMessageControl( string Heading, string Message, string IconImage )
        {
            //Use this to get a module message control
            //with a custom image for an icon
            ModuleMessage objModuleMessage;
            Skin s = new Skin();
            objModuleMessage = (ModuleMessage)s.LoadControl( "~/admin/skins/ModuleMessage.ascx" );
            objModuleMessage.Heading = Heading;
            objModuleMessage.Text = Message;
            objModuleMessage.IconImage = IconImage;
            return objModuleMessage;
        }

        public static ModuleMessage GetModuleMessageControl( string Heading, string Message, ModuleMessage.ModuleMessageType objModuleMessageType )
        {
            //Use this to get a module message control
            //with a standard DotNetNuke icon
            Skin skin = new Skin();
            ModuleMessage moduleMessage = ( (ModuleMessage)skin.LoadControl( "~/admin/skins/ModuleMessage.ascx" ) );
            moduleMessage.Heading = Heading;
            moduleMessage.Text = Message;
            moduleMessage.IconType = objModuleMessageType;
            return moduleMessage;
        }

        public static Skin GetParentSkin( PortalModuleBase objModule )
        {
            Control MyParent = objModule.Parent;
            bool FoundSkin = false;
            while( MyParent != null )
            {
                if( MyParent is Skin )
                {
                    FoundSkin = true;
                    break;
                }
                MyParent = MyParent.Parent;
            }
            if( FoundSkin )
            {
                return ( (Skin)MyParent );
            }
            else
            {
                return null;
            }
        }

        private UserControl LoadContainer( string ContainerPath, Control objPane )
        {
            //sanity check to ensure skin not loaded accidentally
            if( ContainerPath.ToLower().IndexOf( "/skins/" ) != -1 || ContainerPath.ToLower().IndexOf( "/skins\\" ) != -1 || ContainerPath.ToLower().IndexOf( "\\skins\\" ) != -1 || ContainerPath.ToLower().IndexOf( "\\skins/" ) != -1 )
            {
                throw ( new Exception() );
            }
            UserControl ctlContainer = null;

            try
            {
                if( ContainerPath.ToLower().IndexOf( Globals.ApplicationPath.ToLower() ) != -1 )
                {
                    ContainerPath = ContainerPath.Remove( 0, Globals.ApplicationPath.Length );
                }
                ctlContainer = (UserControl)LoadControl( "~" + ContainerPath );
                // call databind so that any server logic in the container is executed
                ctlContainer.DataBind();
            }
            catch( Exception exc )
            {
                // could not load user control
                ModuleLoadException lex = new ModuleLoadException( MODULELOAD_ERROR, exc );
                if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == true || PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == true )
                {
                    // only display the error to administrators
                    objPane.Controls.Add( new ErrorContainer( PortalSettings, string.Format( CONTAINERLOAD_ERROR, ContainerPath ), lex ).Container );
                }
                Exceptions.LogException( lex );
            }

            return ctlContainer;
        }

        public static void AddModuleMessage( PortalModuleBase objPortalModuleBase, string Heading, string Message, ModuleMessage.ModuleMessageType objModuleMessageType )
        {
            if( objPortalModuleBase != null )
            {
                if( Message != "" )
                {
                    PlaceHolder MessagePlaceHolder = (PlaceHolder)objPortalModuleBase.Parent.FindControl( "MessagePlaceHolder" );
                    if( MessagePlaceHolder != null )
                    {
                        MessagePlaceHolder.Visible = true;
                        ModuleMessage objModuleMessage;
                        objModuleMessage = GetModuleMessageControl( Heading, Message, objModuleMessageType );
                        MessagePlaceHolder.Controls.Add( objModuleMessage );
                        //CType(objPortalModuleBase.Page, CDefault).ScrollToControl(MessagePlaceHolder.Parent)       'scroll to error message
                    }
                }
            }
        }

        public static void AddModuleMessage( PortalModuleBase objPortalModuleBase, string Heading, string Message, string IconSrc )
        {
            if( objPortalModuleBase != null )
            {
                if( Message != "" )
                {
                    PlaceHolder MessagePlaceHolder = (PlaceHolder)objPortalModuleBase.Parent.FindControl( "MessagePlaceHolder" );
                    if( MessagePlaceHolder != null )
                    {
                        MessagePlaceHolder.Visible = true;
                        ModuleMessage objModuleMessage;
                        objModuleMessage = GetModuleMessageControl( Heading, Message, IconSrc );
                        MessagePlaceHolder.Controls.Add( objModuleMessage );
                    }
                }
            }
        }

        public static void AddModuleMessage( PortalModuleBase objPortalModuleBase, string Message, ModuleMessage.ModuleMessageType objModuleMessageType )
        {
            AddModuleMessage( objPortalModuleBase, "", Message, objModuleMessageType );
        }

        public static void AddPageMessage( Page objPage, string Heading, string Message, string IconSrc )
        {
            if( Message != "" )
            {
                Control ContentPane = (Control)objPage.FindControl( Globals.glbDefaultPane );
                if( ContentPane != null )
                {
                    ModuleMessage objModuleMessage;
                    objModuleMessage = GetModuleMessageControl( Heading, Message, IconSrc );
                    ContentPane.Controls.AddAt( 0, objModuleMessage );
                }
            }
        }

        public static void AddPageMessage( Skin objSkin, string Heading, string Message, ModuleMessage.ModuleMessageType objModuleMessageType )
        {
            if (Message != "")
            {
                Control ContentPane = (Control)objSkin.FindControl(Globals.glbDefaultPane);
                if (ContentPane != null)
                {
                    ModuleMessage objModuleMessage;
                    objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType);
                    ContentPane.Controls.AddAt(0, objModuleMessage);
                }
            }
        }

        public static void AddPageMessage( Page objPage, string Heading, string Message, ModuleMessage.ModuleMessageType objModuleMessageType )
        {
            if (Message != "")
            {
                Control ContentPane = (Control)objPage.FindControl(Globals.glbDefaultPane);
                if (ContentPane != null)
                {
                    ModuleMessage objModuleMessage;
                    objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType);
                    ContentPane.Controls.AddAt(0, objModuleMessage);
                }
            }
        }

        public static void AddPageMessage( Skin objSkin, string Heading, string Message, string IconSrc )
        {
            if (Message != "")
            {
                Control ContentPane = (Control)objSkin.FindControl(Globals.glbDefaultPane);
                if (ContentPane != null)
                {
                    ModuleMessage objModuleMessage;
                    objModuleMessage = GetModuleMessageControl(Heading, Message, IconSrc);
                    ContentPane.Controls.AddAt(0, objModuleMessage);
                }
            }
        }

        private void CollapseUnusedPanes()
        {
            //This method sets the width to "0" on panes that have no modules.
            //This preserves the integrity of the HTML syntax so we don't have to set
            //the visiblity of a pane to false. Setting the visibility of a pane to
            //false where there are colspans and rowspans can render the skin incorrectly.

            Control ctlPane;
            string strPane;
            foreach (string tempLoopVar_strPane in PortalSettings.ActiveTab.Panes)
            {
                strPane = tempLoopVar_strPane;
                ctlPane = this.FindControl(strPane);
                if (ctlPane != null)
                {
                    if (!ctlPane.HasControls())
                    {
                        //This pane has no controls so set the width to 0
                        HtmlControl objHtmlControl = (HtmlControl)ctlPane;
                        objHtmlControl.Attributes["width"] = "0";
                        if (objHtmlControl.Attributes["style"] != null)
                        {
                            objHtmlControl.Attributes.Remove("style");
                        }
                    }
                    else if (ctlPane.Controls.Count == 1)
                    {
                        //this pane has one control, check to see if it's the pane name label
                        if (ctlPane.Controls[0].GetType() == typeof(Label))
                        {
                            //the only control in this pane is some type of label
                            if (((Label)ctlPane.Controls[0]).Text.LastIndexOf(ctlPane.ID) > 0)
                            {
                                //the "pane name" is the only control in this pane
                                //so, since there are no other controls, resize the pane to width="0"
                                HtmlControl objHtmlControl = (HtmlControl)ctlPane;
                                objHtmlControl.Attributes["width"] = "0";
                                if (objHtmlControl.Attributes["style"] != null)
                                {
                                    objHtmlControl.Attributes.Remove("style");
                                }

                                //hide the pane name label
                                Label objLabel = (Label)ctlPane.Controls[0];
                                objLabel.Visible = false;
                            }
                        }
                    }
                }
            }
        }

       
        public void InjectModule( Control objPane, ModuleInfo objModule, PortalSettings PortalSettings )
        {
            bool bSuccess = true;

            try
            {
                //Get a reference to the Page
                CDefault DefaultPage = (CDefault)Page;
                PortalModuleBase objPortalModuleBase = null;

                // load container control
                UserControl ctlContainer = null;

                SkinController objSkins = new SkinController();

                //Save the current ContainerSrc/Path (in case we are in "Preview" mode)
                string strOldContainerSource = objModule.ContainerSrc;
                string strOldContainerPath = objModule.ContainerPath;

                // container preview
                int PreviewModuleId = -1;
                if (Request.QueryString["ModuleId"] != null)
                {
                    PreviewModuleId = int.Parse(Request.QueryString["ModuleId"]);
                }
                if ((Request.QueryString["ContainerSrc"] != null) && (objModule.ModuleID == PreviewModuleId || PreviewModuleId == -1))
                {
                    objModule.ContainerSrc = SkinController.FormatSkinSrc(Globals.QueryStringDecode(Request.QueryString["ContainerSrc"]) + ".ascx", PortalSettings);
                    ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                }

                // load user container ( based on cookie )
                if (ctlContainer == null)
                {
                    if (Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()] != null)
                    {
                        if (Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()].Value != "")
                        {
                            objModule.ContainerSrc = SkinController.FormatSkinSrc(Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()].Value + ".ascx", PortalSettings);
                            ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                        }
                    }
                }

                if (ctlContainer == null)
                {
                    // if the module specifies that no container should be used
                    if (objModule.DisplayTitle == false)
                    {
                        // always display container if the current user is the administrator or the module is being used in an admin case
                        bool blnDisplayTitle = (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString())) || Globals.IsAdminSkin(PortalSettings.ActiveTab.IsAdminTab);
                        // unless the administrator has selected the Page Preview option
                        if (blnDisplayTitle == true)
                        {
                            if (Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] != null)
                            {
                                blnDisplayTitle = !bool.Parse(Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()].Value);
                            }
                        }
                        if (blnDisplayTitle == false)
                        {
                            objModule.ContainerSrc = SkinController.FormatSkinSrc("[G]" + SkinInfo.RootContainer + "/_default/No Container.ascx", PortalSettings);
                            ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                        }
                    }
                }

                if (ctlContainer == null)
                {
                    // if this is not a container assigned to a module
                    if (objModule.ContainerSrc == PortalSettings.ActiveTab.ContainerSrc)
                    {
                        // look for a container specification in the skin pane
                        if (objPane is HtmlControl)
                        {
                            HtmlControl objHtmlControl = (HtmlControl)objPane;
                            if (objHtmlControl.Attributes["ContainerSrc"] != null)
                            {
                                if ((objHtmlControl.Attributes["ContainerType"] != null) && (objHtmlControl.Attributes["ContainerName"] != null))
                                {
                                    // legacy container specification in skin pane
                                    objModule.ContainerSrc = "[" + objHtmlControl.Attributes["ContainerType"] + "]" + SkinInfo.RootContainer + "/" + objHtmlControl.Attributes["ContainerName"] + "/" + objHtmlControl.Attributes["ContainerSrc"];
                                }
                                else
                                {
                                    // 3.0 container specification in skin pane
                                    objModule.ContainerSrc = objHtmlControl.Attributes["ContainerSrc"];
                                }
                                objModule.ContainerSrc = SkinController.FormatSkinSrc(objModule.ContainerSrc, PortalSettings);
                                ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                            }
                        }
                    }
                }

                // else load assigned container
                if (ctlContainer == null)
                {
                    if (Globals.IsAdminSkin(PortalSettings.ActiveTab.IsAdminTab))
                    {
                        SkinInfo objSkin = SkinController.GetSkin(SkinInfo.RootContainer, PortalSettings.PortalId, SkinType.Admin);
                        if (objSkin != null)
                        {
                            objModule.ContainerSrc = objSkin.SkinSrc;
                        }
                        else
                        {
                            objModule.ContainerSrc = "";
                        }
                    }

                    if (objModule.ContainerSrc != "")
                    {
                        objModule.ContainerSrc = SkinController.FormatSkinSrc(objModule.ContainerSrc, PortalSettings);
                        ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                    }
                }

                // error loading container - load default
                if (ctlContainer == null)
                {
                    if (Globals.IsAdminSkin(PortalSettings.ActiveTab.IsAdminTab))
                    {
                        objModule.ContainerSrc = Globals.HostPath + SkinInfo.RootContainer + Globals.glbDefaultContainerFolder + Globals.glbDefaultAdminContainer;
                    }
                    else
                    {
                        objModule.ContainerSrc = Globals.HostPath + SkinInfo.RootContainer + Globals.glbDefaultContainerFolder + Globals.glbDefaultContainer;
                    }
                    ctlContainer = LoadContainer(objModule.ContainerSrc, objPane);
                }

                // set container path
                objModule.ContainerPath = SkinController.FormatSkinPath(objModule.ContainerSrc);

                string ID;
                Hashtable objCSSCache = null;
                if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                {
                    objCSSCache = (Hashtable)DataCache.GetCache("CSS");
                }
                if (objCSSCache == null)
                {
                    objCSSCache = new Hashtable();
                }

                // container package style sheet
                ID = Globals.CreateValidID(objModule.ContainerPath);
                if (objCSSCache.ContainsKey(ID) == false)
                {
                    if (File.Exists(Server.MapPath(objModule.ContainerPath) + "container.css"))
                    {
                        objCSSCache[ID] = objModule.ContainerPath + "container.css";
                    }
                    else
                    {
                        objCSSCache[ID] = "";
                    }
                    if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                    {
                        DataCache.SetCache("CSS", objCSSCache);
                    }
                }
                if (objCSSCache[ID].ToString() != "")
                {
                    DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString());
                }

                // container file style sheet
                ID = Globals.CreateValidID(objModule.ContainerSrc.Replace(".ascx", ".css"));
                if (objCSSCache.ContainsKey(ID) == false)
                {
                    if (File.Exists(Server.MapPath(objModule.ContainerSrc.Replace(".ascx", ".css"))))
                    {
                        objCSSCache[ID] = objModule.ContainerSrc.Replace(".ascx", ".css");
                    }
                    else
                    {
                        objCSSCache[ID] = "";
                    }
                    if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                    {
                        DataCache.SetCache("CSS", objCSSCache);
                    }
                }
                if (objCSSCache[ID].ToString() != "")
                {
                    DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString());
                }

                if (!Globals.IsAdminControl())
                {
                    // inject an anchor tag to allow navigation to the module content
                    objPane.Controls.Add(new LiteralControl("<a name=\"" + objModule.ModuleID.ToString() + "\"></a>"));
                }

                // get container pane
                Control objCell = ctlContainer.FindControl(Globals.glbDefaultPane);
                string EditText = "";
                bool DisplayOptions = false;

                if (objCell != null)
                {
                    // set container content pane display properties ( could be <TD>,<DIV>,<SPAN>,<P> )
                    if (objCell is HtmlContainerControl)
                    {
                        if (objModule.Alignment != "")
                        {
                            ((HtmlContainerControl)objCell).Attributes.Add("align", objModule.Alignment);
                        }
                        if (objModule.Color != "")
                        {
                            ((HtmlContainerControl)objCell).Style["background"] = objModule.Color;
                        }
                        if (objModule.Border != "")
                        {
                            ((HtmlContainerControl)objCell).Style["border-top"] = objModule.Border + "px #000000 solid";
                            ((HtmlContainerControl)objCell).Style["border-bottom"] = objModule.Border + "px #000000 solid";
                            ((HtmlContainerControl)objCell).Style["border-right"] = objModule.Border + "px #000000 solid";
                            ((HtmlContainerControl)objCell).Style["border-left"] = objModule.Border + "px #000000 solid";
                        }

                        // display visual indicator if module is only visible to administrators
                        if (Globals.IsAdminControl() == false && PortalSettings.ActiveTab.IsAdminTab == false)
                        {
                            if ((objModule.StartDate >= DateTime.Now || objModule.EndDate <= DateTime.Now) || (objModule.AuthorizedViewRoles.ToLower() == ";" + PortalSettings.AdministratorRoleName.ToLower() + ";"))
                            {
                                ((HtmlContainerControl)objCell).Style["border-top"] = "2px #FF0000 solid";
                                ((HtmlContainerControl)objCell).Style["border-bottom"] = "2px #FF0000 solid";
                                ((HtmlContainerControl)objCell).Style["border-right"] = "2px #FF0000 solid";
                                ((HtmlContainerControl)objCell).Style["border-left"] = "2px #FF0000 solid";
                                objCell.Controls.Add(new LiteralControl("<span class=\"NormalRed\"><center>" + Localization.GetString("ModuleVisibleAdministrator.Text") + "</center></span>"));
                            }
                        }
                    }

                    if (!Globals.IsAdminControl())
                    {
                        // inject a start comment around the module content
                        objCell.Controls.Add(new LiteralControl("<!-- Start_Module_" + objModule.ModuleID.ToString() + " -->"));
                    }

                    // inject the header
                    if (objModule.Header != "")
                    {
                        Label objLabel = new Label();
                        objLabel.Text = objModule.Header;
                        objLabel.CssClass = "Normal";
                        objCell.Controls.Add(objLabel);
                    }

                    // inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
                    PlaceHolder MessagePlaceholder = new PlaceHolder();
                    MessagePlaceholder.ID = "MessagePlaceHolder";
                    MessagePlaceholder.Visible = false;
                    objCell.Controls.Add(MessagePlaceholder);

                    // create a wrapper panel control for the module content min/max
                    Panel objPanel = new Panel();
                    objPanel.ID = "ModuleContent";

                    // module content visibility options
                    bool blnContent = true;
                    if (Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()] != null)
                    {
                        blnContent = bool.Parse(Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()].Value);
                    }
                    if (Request.QueryString["content"] != null)
                    {
                        switch (Request.QueryString["Content"].ToLower())
                        {
                            case "1":
                                blnContent = true;
                                break;

                            case "true":

                                blnContent = true;
                                break;
                            case "0":
                                blnContent = false;
                                break;

                            case "false":

                                blnContent = false;
                                break;
                        }
                    }
                    if (Globals.IsAdminControl() == true || PortalSettings.ActiveTab.IsAdminTab)
                    {
                        blnContent = true;
                    }

                    // try to load the module user control
                    try
                    {
                        if (blnContent)
                        {
                            // if the module has caching and the user does not have EDIT permissions
                            if (objModule.CacheTime != 0 && PortalSecurity.HasEditPermissions(objModule.ModuleID) == false) // use output caching
                            {
                                objPortalModuleBase = new PortalModuleBase();
                            }
                            else // load the control dynamically
                            {
                                objPortalModuleBase = (PortalModuleBase)this.LoadControl("~/" + objModule.ControlSrc);
                            }

                            // set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                            // this is necessary for the Localization in PageBase
                            objPortalModuleBase.ID = Path.GetFileNameWithoutExtension(objModule.ControlSrc);
                        }
                        else // content placeholder
                        {
                            objPortalModuleBase = (PortalModuleBase)this.LoadControl("~/admin/Portal/NoContent.ascx");
                        }

                        //check for IMC
                        objCommunicator.LoadCommunicator(objPortalModuleBase);

                        // add module settings
                        objPortalModuleBase.ModuleConfiguration = objModule;
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                        bSuccess = false;
                    }
                    catch (Exception exc)
                    {
                        bSuccess = false;
                        objPortalModuleBase = (PortalModuleBase)this.LoadControl("~/admin/Portal/NoContent.ascx");

                        //' add module settings
                        objPortalModuleBase.ModuleConfiguration = objModule;

                        if (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true)
                        {
                            // only display the error to administrators
                            if (objPortalModuleBase == null)
                            {
                                Exceptions.ProcessModuleLoadException(MODULELOAD_ERROR, objPanel, exc);
                            }
                            else
                            {
                                Exceptions.ProcessModuleLoadException(objPortalModuleBase, exc);
                            }
                        }
                    }

                    // module user control processing
                    if (objPortalModuleBase != null)
                    {
                        // inject the module into the panel
                        objPanel.Controls.Add(objPortalModuleBase);
                    }

                    // inject the panel into the container pane
                    objCell.Controls.Add(objPanel);

                    // disable legacy controls in module
                    if (objPortalModuleBase != null)
                    {
                        // force the CreateChildControls() event to fire for the PortalModuleBase ( the timing is critical for output caching )
                        objPortalModuleBase.FindControl("");

                        Panel objModuleContent = (Panel)objPortalModuleBase.FindControl("ModuleContent");
                        if (objModuleContent != null)
                        {
                            objModuleContent.Visible = false;
                        }
                    }

                    // inject the footer
                    if (objModule.Footer != "")
                    {
                        Label objLabel = new Label();
                        objLabel.Text = objModule.Footer;
                        objLabel.CssClass = "Normal";
                        objCell.Controls.Add(objLabel);
                    }

                    // inject an end comment around the module content
                    if (!Globals.IsAdminControl())
                    {
                        objPanel.Controls.Add(new LiteralControl("<!-- End_Module_" + objModule.ModuleID.ToString() + " -->"));
                    }
                }

                // set container id to an explicit short name to reduce page payload
                ctlContainer.ID = "ctr";
                // make the container id unique for the page
                if (objPortalModuleBase != null && objPortalModuleBase.ModuleId > -1) //Can't have ID with a - (dash) in it, should only be for admin modules, where they are the only container, so don't need unique name
                {
                    ctlContainer.ID += objPortalModuleBase.ModuleId.ToString();
                }

                if ((objPortalModuleBase != null) && Globals.IsLayoutMode() && Globals.IsAdminControl() == false)
                {
                    //provide Drag-N-Drop capabilities
                    Panel ctlDragDropContainer = new Panel();
                    Control ctlTitle = ctlContainer.FindControl("dnnTitle"); //'Assume that the title control is named dnnTitle.  If this becomes an issue we could loop through the controls looking for the title type of skin object
                    ctlDragDropContainer.ID = ctlContainer.ID + "_DD";
                    objPane.Controls.Add(ctlDragDropContainer);

                    // inject the container into the page pane - this triggers the Pre_Init() event for the user control
                    ctlDragDropContainer.Controls.Add(ctlContainer);

                    if (ctlTitle != null)
                    {
                        if (ctlTitle.Controls.Count > 0)
                        {
                            // if multiple title controls, use the first instance
                            ctlTitle = ctlTitle.Controls[0];
                        }
                    }

                    // enable drag and drop
                    if (ctlDragDropContainer != null && ctlTitle != null) //The title ID is actually the first child so we need to make sure at least one child exists
                    {
                        DNNClientAPI.EnableContainerDragAndDrop(ctlTitle, ctlDragDropContainer, objPortalModuleBase.ModuleId);
                        ClientAPI.RegisterPostBackEventHandler(objCell, "MoveToPane", new ClientAPIPostBackControl.PostBackEvent(ModuleMoveToPanePostBack), false);
                    }
                }
                else
                {
                    // inject the container into the page pane - this triggers the Pre_Init() event for the user control
                    objPane.Controls.Add(ctlContainer);
                }

                //Process the Action Controls
                ProcessActionControls(objPortalModuleBase, ctlContainer);

                // process the base class module properties
                if (objPortalModuleBase != null)
                {
                    // module stylesheet
                    ID = Globals.CreateValidID(Globals.ApplicationPath + "/" + objModule.ControlSrc.Substring(0, objModule.ControlSrc.LastIndexOf("/")));
                    if (objCSSCache.ContainsKey(ID) == false)
                    {
                        if (File.Exists(Server.MapPath(Globals.ApplicationPath + "/" + objModule.ControlSrc.Substring(0, objModule.ControlSrc.LastIndexOf("/") + 1)) + "module.css"))
                        {
                            objCSSCache[ID] = Globals.ApplicationPath + "/" + objModule.ControlSrc.Substring(0, objModule.ControlSrc.LastIndexOf("/") + 1) + "module.css";
                        }
                        else
                        {
                            objCSSCache[ID] = "";
                        }
                        if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                        {
                            DataCache.SetCache("CSS", objCSSCache);
                        }
                    }
                    if (objCSSCache[ID].ToString() != "")
                    {
                        //Add it to beginning of style list
                        DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString(), true);
                    }
                }

                // display collapsible page panes
                if (objPane.Visible == false)
                {
                    objPane.Visible = true;
                }

                //Reset the ContainerSource/Path (in case we are in "Preview" mode)
                objModule.ContainerPath = strOldContainerPath;
                objModule.ContainerSrc = strOldContainerSource;
            }
            catch (Exception exc)
            {
                ModuleLoadException lex;
                lex = new ModuleLoadException(string.Format(MODULEADD_ERROR, objPane.ID.ToString()), exc);
                if (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true)
                {
                    // only display the error to administrators
                    objPane.Controls.Add(new ErrorContainer(PortalSettings, MODULELOAD_ERROR, lex).Container);
                }
                Exceptions.LogException(lex);
                bSuccess = false;
                throw (lex);
            }

            if (!bSuccess)
            {
                throw (new ModuleLoadException());
            }
        }

        public void ModuleAction_Click( object sender, ActionEventArgs e )
        {
            //Search through the listeners
            ModuleActionEventListener Listener;
            foreach (ModuleActionEventListener tempLoopVar_Listener in ActionEventListeners)
            {
                Listener = tempLoopVar_Listener;

                //If the associated module has registered a listener
                if (e.ModuleConfiguration.ModuleID == Listener.ModuleID)
                {
                    //Invoke the listener to handle the ModuleAction_Click event
                    Listener.ActionEvent.Invoke(sender, e);
                }
            }
        }

        private void ModuleMoveToPanePostBack( ClientAPIPostBackEventArgs args )
        {
            PortalSettings PortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true)
            {
                int intModuleID = Convert.ToInt32(args.EventArguments["moduleid"]);
                string strPaneName = args.EventArguments["pane"].ToString();
                int intOrder = Convert.ToInt32(args.EventArguments["order"]);
                ModuleController objModules = new ModuleController();

                objModules.UpdateModuleOrder(PortalSettings.ActiveTab.TabID, intModuleID, intOrder, strPaneName);
                objModules.UpdateTabModuleOrder(PortalSettings.ActiveTab.TabID, PortalSettings.ActiveTab.PortalID);

                // Redirect to the same page to pick up changes
                Response.Redirect(Request.RawUrl, true);
            }
        }

        private void InitializeComponent()
        {
        }

        private void Page_Init( object sender, EventArgs e )
        {
            InitializeComponent();

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = null;
            Control ctlPane;
            bool blnLayoutMode = Globals.IsLayoutMode();

            bool bSuccess = true;

            // iterate page controls
            Control ctlControl;
            HtmlControl objHtmlControl;
            foreach (Control tempLoopVar_ctlControl in this.Controls)
            {
                ctlControl = tempLoopVar_ctlControl;
                // load the skin panes
                if (ctlControl is HtmlControl)
                {
                    objHtmlControl = (HtmlControl)ctlControl;
                    if (objHtmlControl.ID != null)
                    {
                        switch (objHtmlControl.TagName.ToUpper())
                        {
                            case "TD":
                                // content pane
                                if (ctlControl.ID != "ControlPanel")
                                {
                                    PortalSettings.ActiveTab.Panes.Add(ctlControl.ID);
                                }
                                break;

                            case "DIV":
                                // content pane
                                if (ctlControl.ID != "ControlPanel")
                                {
                                    PortalSettings.ActiveTab.Panes.Add(ctlControl.ID);
                                }
                                break;

                            case "SPAN":
                                // content pane
                                if (ctlControl.ID != "ControlPanel")
                                {
                                    PortalSettings.ActiveTab.Panes.Add(ctlControl.ID);
                                }
                                break;

                            case "P":

                                // content pane
                                if (ctlControl.ID != "ControlPanel")
                                {
                                    PortalSettings.ActiveTab.Panes.Add(ctlControl.ID);
                                }
                                break;
                        }
                    }
                }
            }

            //if querystring dnnprintmode=true, controlpanel will not be shown
            if (Request.QueryString["dnnprintmode"] != "true")
            {
                // ControlPanel processing
                if (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName.ToString()) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true)
                {
                    UserControl objControlPanel = null;
                    if (Convert.ToString(PortalSettings.HostSettings["ControlPanel"]) != "")
                    {
                        // load custom control panel
                        objControlPanel = (UserControl)LoadControl("~/" + Convert.ToString(PortalSettings.HostSettings["ControlPanel"]));
                    }
                    if (objControlPanel == null)
                    {
                        // load default control panel
                        objControlPanel = (UserControl)LoadControl("~/" + Globals.glbDefaultControlPanel);
                    }
                    // inject ControlPanel control into skin
                    ctlPane = this.FindControl("ControlPanel");
                    if (ctlPane == null)
                    {
                        HtmlForm objForm = (HtmlForm)this.Parent.FindControl("Form");
                        objForm.Controls.AddAt(0, objControlPanel);
                    }
                    else
                    {
                        ctlPane.Controls.Add(objControlPanel);
                    }
                }
            }

            if (!Globals.IsAdminControl()) // master module
            {
                if (PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AuthorizedRoles))
                {
                    // check portal expiry date
                    bool blnExpired = false;
                    if (PortalSettings.ExpiryDate != Null.NullDate)
                    {
                        if (Convert.ToDateTime(PortalSettings.ExpiryDate) < DateTime.Now && PortalSettings.ActiveTab.ParentId != PortalSettings.AdminTabId && PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                        {
                            blnExpired = true;
                        }
                    }
                    if (!blnExpired)
                    {
                        if ((PortalSettings.ActiveTab.StartDate < DateTime.Now && PortalSettings.ActiveTab.EndDate > DateTime.Now) || blnLayoutMode == true)
                        {
                            // process panes
                            if (blnLayoutMode)
                            {
                                string strPane;
                                foreach (string tempLoopVar_strPane in PortalSettings.ActiveTab.Panes)
                                {
                                    strPane = tempLoopVar_strPane;
                                    ctlPane = this.FindControl(strPane);
                                    ctlPane.Visible = true;

                                    // display pane border
                                    if (ctlPane is HtmlContainerControl)
                                    {
                                        ((HtmlContainerControl)ctlPane).Style["border-top"] = "1px #CCCCCC dotted";
                                        ((HtmlContainerControl)ctlPane).Style["border-bottom"] = "1px #CCCCCC dotted";
                                        ((HtmlContainerControl)ctlPane).Style["border-right"] = "1px #CCCCCC dotted";
                                        ((HtmlContainerControl)ctlPane).Style["border-left"] = "1px #CCCCCC dotted";
                                    }

                                    // display pane name
                                    Label ctlLabel = new Label();
                                    ctlLabel.Text = "<center>" + strPane + "</center><br>";
                                    ctlLabel.CssClass = "SubHead";
                                    ctlPane.Controls.AddAt(0, ctlLabel);
                                }
                            }

                            // dynamically populate the panes with modules
                            if (PortalSettings.ActiveTab.Modules.Count > 0)
                            {
                                // loop through each entry in the configuration system for this tab
                                foreach (ModuleInfo tempLoopVar_objModule in PortalSettings.ActiveTab.Modules)
                                {
                                    objModule = tempLoopVar_objModule;

                                    // if user is allowed to view module and module is not deleted
                                    if (PortalSecurity.IsInRoles(objModule.AuthorizedViewRoles) == true && objModule.IsDeleted == false)
                                    {
                                        // if current date is within module display schedule or user is admin
                                        if ((objModule.StartDate < DateTime.Now && objModule.EndDate > DateTime.Now) || blnLayoutMode == true)
                                        {
                                            // modules which are displayed on all tabs should not be displayed on the Admin or Super tabs
                                            if (objModule.AllTabs == false || PortalSettings.ActiveTab.IsAdminTab == false)
                                            {
                                                Control parent = this.FindControl(objModule.PaneName);

                                                if (parent == null)
                                                {
                                                    // the pane specified in the database does not exist for this skin
                                                    // insert the module into the default pane instead
                                                    parent = this.FindControl(Globals.glbDefaultPane);
                                                }

                                                if (parent != null)
                                                {
                                                    // try to localize admin modules
                                                    if (PortalSettings.ActiveTab.IsAdminTab)
                                                    {
                                                        objModule.ModuleTitle = Localization.LocalizeControlTitle(objModule.ModuleTitle, objModule.ControlSrc, "");
                                                    }

                                                    //try to inject the module into the skin
                                                    try
                                                    {
                                                        InjectModule(parent, objModule, PortalSettings);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        bSuccess = false;
                                                    }
                                                }
                                                else // no ContentPane in skin
                                                {
                                                    ModuleLoadException lex;
                                                    lex = new ModuleLoadException(PANE_LOAD_ERROR);
                                                    Controls.Add(new ErrorContainer(PortalSettings, MODULELOAD_ERROR, lex).Container);
                                                    Exceptions.LogException(lex);
                                                    Information.Err().Clear();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Skin.AddPageMessage(this, "", TABACCESS_ERROR, ModuleMessage.ModuleMessageType.YellowWarning);
                        }
                    }
                    else
                    {
                        Skin.AddPageMessage(this, "", string.Format(CONTRACTEXPIRED_ERROR, PortalSettings.PortalName, Globals.GetMediumDate(PortalSettings.ExpiryDate.ToString()), PortalSettings.Email), ModuleMessage.ModuleMessageType.RedError);
                    }
                }
                else
                {
                    Response.Redirect(Globals.AccessDeniedURL(TABACCESS_ERROR), true);
                }
            }
            else // slave module
            {
                int ModuleId = -1;
                string Key = "";

                // get ModuleId
                if (!Information.IsNothing(Request.QueryString["mid"]))
                {
                    ModuleId = int.Parse(Request.QueryString["mid"]);
                }

                // get ControlKey
                if (!Information.IsNothing(Request.QueryString["ctl"]))
                {
                    Key = Request.QueryString["ctl"];
                }

                // initialize moduleid for modulesettings
                if (!Information.IsNothing(Request.QueryString["moduleid"]) && (Key.ToLower() == "module" || Key.ToLower() == "help"))
                {
                    ModuleId = int.Parse(Request.QueryString["moduleid"]);
                }

                if (ModuleId != -1)
                {
                    // get master module security settings
                    objModule = objModules.GetModule(ModuleId, PortalSettings.ActiveTab.TabID);
                    if (objModule != null)
                    {
                        if (objModule.InheritViewPermissions)
                        {
                            objModule.AuthorizedViewRoles = PortalSettings.ActiveTab.AuthorizedRoles;
                        }
                    }
                }
                if (objModule == null)
                {
                    // initialize object not related to a module
                    objModule = new ModuleInfo();
                    objModule.ModuleID = ModuleId;
                    objModule.ModuleDefID = -1;
                    objModule.TabID = PortalSettings.ActiveTab.TabID;
                    objModule.AuthorizedEditRoles = "";
                    objModule.AuthorizedViewRoles = "";
                }

                // initialize moduledefid for modulesettings
                if (!Information.IsNothing(Request.QueryString["moduleid"]) && (Key.ToLower() == "module" || Key.ToLower() == "help"))
                {
                    objModule.ModuleDefID = -1;
                }

                // override slave module settings
                if (Request.QueryString["dnnprintmode"] != "true")
                {
                    objModule.ModuleTitle = "";
                }
                objModule.Header = "";
                objModule.Footer = "";
                objModule.StartDate = DateTime.MinValue;
                objModule.EndDate = DateTime.MaxValue;
                objModule.PaneName = Globals.glbDefaultPane;
                objModule.Visibility = VisibilityState.None;
                objModule.Color = "";
                if (Request.QueryString["dnnprintmode"] != "true")
                {
                    objModule.Alignment = "center";
                }
                objModule.Border = "";
                objModule.DisplayTitle = true;
                objModule.DisplayPrint = false;
                objModule.DisplaySyndicate = false;

                // get portal container for slave module
                SkinInfo objSkin = SkinController.GetSkin(SkinInfo.RootContainer, PortalSettings.PortalId, SkinType.Portal);
                if (objSkin != null)
                {
                    objModule.ContainerSrc = objSkin.SkinSrc;
                }
                else
                {
                    objModule.ContainerSrc = "[G]" + SkinInfo.RootContainer + Globals.glbDefaultContainerFolder + Globals.glbDefaultContainer;
                }
                objModule.ContainerSrc = SkinController.FormatSkinSrc(objModule.ContainerSrc, PortalSettings);
                objModule.ContainerPath = SkinController.FormatSkinPath(objModule.ContainerSrc);

                // get the pane
                Control parent = this.FindControl(objModule.PaneName);

                // load the controls
                ModuleControlController objModuleControls = new ModuleControlController();
                ModuleControlInfo objModuleControl;
                int intCounter;

                ArrayList arrModuleControls = objModuleControls.GetModuleControlsByKey(Key, objModule.ModuleDefID);

                for (intCounter = 0; intCounter <= arrModuleControls.Count - 1; intCounter++)
                {
                    objModuleControl = (ModuleControlInfo)arrModuleControls[intCounter];

                    // initialize control values
                    objModule.ModuleControlId = objModuleControl.ModuleControlID;
                    objModule.ControlSrc = objModuleControl.ControlSrc;
                    objModule.ControlType = objModuleControl.ControlType;
                    objModule.IconFile = objModuleControl.IconFile;
                    objModule.HelpUrl = objModuleControl.HelpURL;

                    if (!Null.IsNull(objModuleControl.ControlTitle))
                    {
                        // try to localize control title
                        objModule.ModuleTitle = Localization.LocalizeControlTitle(objModuleControl.ControlTitle, objModule.ControlSrc, Key);
                    }

                    // verify that the current user has access to this control
                    bool blnAuthorized = true;
                    switch (objModule.ControlType)
                    {
                        case SecurityAccessLevel.Anonymous: // anonymous

                            break;
                        case SecurityAccessLevel.View: // view

                            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) == false && PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == false)
                            {
                                if (!PortalSecurity.IsInRoles(objModule.AuthorizedViewRoles))
                                {
                                    blnAuthorized = false;
                                }
                            }
                            break;
                        case SecurityAccessLevel.Edit: // edit

                            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) == false && PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == false)
                            {
                                if (!PortalSecurity.IsInRoles(objModule.AuthorizedViewRoles))
                                {
                                    blnAuthorized = false;
                                }
                                else
                                {
                                    if (!PortalSecurity.HasEditPermissions(objModule.ModulePermissions))
                                    {
                                        blnAuthorized = false;
                                    }
                                }
                            }
                            break;
                        case SecurityAccessLevel.Admin: // admin

                            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) == false && PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == false)
                            {
                                blnAuthorized = false;
                            }
                            break;
                        case SecurityAccessLevel.Host: // host

                            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                            if (!objUserInfo.IsSuperUser)
                            {
                                blnAuthorized = false;
                            }
                            break;
                    }

                    if (blnAuthorized)
                    {
                        //try to inject the module into the skin
                        try
                        {
                            InjectModule(parent, objModule, PortalSettings);
                        }
                        catch (Exception)
                        {
                            bSuccess = false;
                        }
                    }
                    else
                    {
                        Response.Redirect(Globals.AccessDeniedURL(MODULEACCESS_ERROR), true);
                    }
                }
            }

            if (!blnLayoutMode)
            {
                CollapseUnusedPanes();
            }

            if (Request.QueryString["error"] != null)
            {
                Skin.AddPageMessage(this, CRITICAL_ERROR, Server.HtmlEncode(Request.QueryString["error"]), ModuleMessage.ModuleMessageType.RedError);
            }

            if (!(PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) == true || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString()) == true))
            {
                // only display the warning to non-administrators (adminsitrators will see the errors)
                if (!bSuccess)
                {
                    Skin.AddPageMessage(this, MODULELOAD_WARNING, string.Format(MODULELOAD_WARNINGTEXT, PortalSettings.Email), ModuleMessage.ModuleMessageType.YellowWarning);
                }
            }
        }

        private void ProcessActionControls( PortalModuleBase objPortalModuleBase, Control objContainer )
        {
            ActionBase objActions;

            foreach (Control objChildControl in objContainer.Controls)
            {
                // check if control is an action control
                if (objChildControl is ActionBase)
                {
                    objActions = (ActionBase)objChildControl;
                    objActions.PortalModule = objPortalModuleBase;
                    objActions.MenuActions.AddRange(objPortalModuleBase.Actions);
                    objActions.Action += new ActionEventHandler(ModuleAction_Click);
                }

                if (objChildControl.HasControls())
                {
                    // recursive call for child controls
                    ProcessActionControls(objPortalModuleBase, objChildControl);
                }
            }
        }

        public void RegisterModuleActionEvent( int ModuleID, ActionEventHandler e )
        {
            this.ActionEventListeners.Add( new ModuleActionEventListener( ModuleID, e ) );
        }
    }
}