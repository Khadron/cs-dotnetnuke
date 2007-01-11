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
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The ModuleDefinitions PortalModuleBase is used to manage the modules
    /// attached to this portal
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ModuleDefinitions : PortalModuleBase, IActionable
    {
        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public void BindData()
        {
            if (!Page.IsPostBack)
            {
                GetFeatures();
            }

            if (Convert.ToString(Common.Globals.HostSettings["CheckUpgrade"]) == "N")
            {
                tabUpgrade.Visible = false;
                grdDefinitions.Columns[4].HeaderText = "";
            }

            // Get the portal's defs from the database
            DesktopModuleController objDesktopModules = new DesktopModuleController();

            ArrayList arr = objDesktopModules.GetDesktopModules();

            DesktopModuleInfo objDesktopModule = new DesktopModuleInfo();

            objDesktopModule.DesktopModuleID = - 2;
            objDesktopModule.FriendlyName = Localization.GetString( "SkinObjects" );
            objDesktopModule.Description = Localization.GetString( "SkinObjectsDescription" );
            objDesktopModule.IsPremium = false;

            arr.Insert( 0, objDesktopModule );

            //Localize Grid
            Localization.LocalizeDataGrid(ref grdDefinitions, this.LocalResourceFile );

            grdDefinitions.DataSource = arr;
            grdDefinitions.DataBind();
        }

        private void GetFeatures()
        {
            string strFileName = "Features.config";

            // if it does not yet exist, copy from \Config folder
            if (!(File.Exists(Globals.ApplicationMapPath + "\\" + strFileName)))
            {
                if (File.Exists(Globals.ApplicationMapPath + Globals.glbConfigFolder + strFileName))
                {
                    File.Copy(Globals.ApplicationMapPath + Globals.glbConfigFolder + strFileName, Globals.ApplicationMapPath + "\\" + strFileName, true);
                }
            }

            // open Features configuration file
            StreamReader objStreamReader = null;
            objStreamReader = File.OpenText(Globals.ApplicationMapPath + "\\" + strFileName);
            string strItems = objStreamReader.ReadToEnd();
            objStreamReader.Close();

            // load XML (RSS) document
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(strItems);
                //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#
                //                XmlNode xmlItem = null;
                XmlNodeList xmlItems = xmlDoc.SelectNodes("rss/channel/item");
                foreach (XmlNode xmlItem in xmlItems)
                {
                    cboSites.Items.Add(new ListItem(XmlUtils.GetNodeValue(xmlItem, "title", ""), XmlUtils.GetNodeValue(xmlItem, "link", "")));
                }
            }
            catch
            {
                // problem loading XML document
            }

            // hide Features functionality if none specified
            if (cboSites.Items.Count == 0)
            {
                tabFeatures.Visible = false;
            }

        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void cmdGo_Click(object sender, System.EventArgs e)
        {
            if (cboSites.SelectedItem != null)
            {
                Response.Write("<script>window.open('" + cboSites.SelectedItem.Value + "', 'new');</script>");
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Host, true, false );

                ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );

                string[] additionalParameters = new string[3];

                additionalParameters[0] = "mid=" + FileManagerModule.ModuleID;
                additionalParameters[1] = "ftype=" + UploadType.Module;
                additionalParameters[2] = "rtab=" + this.TabId;
                actions.Add( GetNextActionID(), Localization.GetString( "ModuleUpload.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", additionalParameters ), false, SecurityAccessLevel.Host, true, false );
                return actions;
            }
        }

        /// <summary>
        /// UpgradeStatusURL returns the imageurl for the upgrade button for the module
        /// </summary>
        public string UpgradeStatusURL(string Version, string ModuleName)
        {
            if (Convert.ToString(Common.Globals.HostSettings["CheckUpgrade"]) != "N" & ModuleName != "" & Version != "")
            {
                return Globals.glbUpgradeUrl + "/update.aspx?version=" + Version.Replace(".", "") + "&modulename=" + ModuleName;
            }
            else
            {
                return "~/spacer.gif";
            }
        }

        /// <summary>
        /// UpgradeURL returns the imageurl for the upgrade button for the module
        /// </summary>        
        public string UpgradeURL(string ModuleName)
        {
            if (Convert.ToString(Common.Globals.HostSettings["CheckUpgrade"]) != "N" & ModuleName != "")
            {
                return Globals.glbUpgradeUrl + "/Redirect.aspx?modulename=" + ModuleName;
            }
            else
            {
                return "";
            }
        }
    }
}