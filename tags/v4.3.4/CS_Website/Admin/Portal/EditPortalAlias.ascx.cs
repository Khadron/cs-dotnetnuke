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
using System.Diagnostics;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Portals
{
    /// <summary>
    /// The EditPortalAlias PortalModuleBase is used to edit a portal alias
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/17/2005	documented
    /// </history>
    public partial class EditPortalAlias : PortalModuleBase
    {
        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        private void BindData()
        {
            if( Request.QueryString["paid"] != null )
            {
                PortalAliasInfo objPortalAliasInfo;
                int intPortalAliasID = Convert.ToInt32( Request.QueryString["paid"] );

                PortalAliasController p = new PortalAliasController();
                objPortalAliasInfo = p.GetPortalAliasByPortalAliasID( intPortalAliasID );

                ViewState.Add( "PortalAliasID", intPortalAliasID );
                ViewState.Add( "PortalID", objPortalAliasInfo.PortalID );

                if( ! UserInfo.IsSuperUser )
                {
                    if( objPortalAliasInfo.PortalID != PortalSettings.PortalId )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, "You do not have access to view this Portal Alias.", ModuleMessageType.RedError );
                        return;
                    }
                }

                txtAlias.Text = objPortalAliasInfo.HTTPAlias;
                SetDeleteVisibility( objPortalAliasInfo.PortalID );
            }
            else if( Request.QueryString["pid"] != "" )
            {
                if( UserInfo.IsSuperUser )
                {
                    ViewState.Add( "PortalID", Convert.ToInt32( Request.QueryString["pid"] ) );
                }
                SetDeleteVisibility( Convert.ToInt32( Request.QueryString["pid"] ) );
            }
            else
            {
                ViewState.Add( "PortalID", PortalSettings.PortalId );
                SetDeleteVisibility( PortalSettings.PortalId );
            }
        }

        /// <summary>
        /// SetDeleteVisibility determines whether the Delete button should be displayed
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        private void SetDeleteVisibility( int PortalID )
        {
            PortalAliasCollection colPortalAlias;
            PortalAliasController p = new PortalAliasController();
            colPortalAlias = p.GetPortalAliasByPortalID( PortalID );
            //Disallow delete if there is only one portal alias
            if( colPortalAlias.Count <= 1 )
            {
                cmdDelete.Visible = false;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    // Store URL Referrer to return to portal
                    if( Request.UrlReferrer != null )
                    {
                        ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                    }
                    else
                    {
                        ViewState["UrlReferrer"] = "";
                    }

                    BindData();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            try
            {
                int intPortalAliasID;
                intPortalAliasID = Convert.ToInt32( ViewState["PortalAliasID"] );
                PortalAliasInfo objPortalAliasInfo;
                PortalAliasController p = new PortalAliasController();
                objPortalAliasInfo = p.GetPortalAliasByPortalAliasID( intPortalAliasID );

                if( ! UserInfo.IsSuperUser )
                {
                    if( objPortalAliasInfo.PortalID != PortalSettings.PortalId )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "AccessDenied", this.LocalResourceFile ), ModuleMessageType.RedError );
                        return;
                    }
                }
                p.DeletePortalAlias( intPortalAliasID );

                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/17/2005	documented
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            try
            {
                string strAlias = txtAlias.Text;
                if( !String.IsNullOrEmpty(strAlias) )
                {
                    if( strAlias.IndexOf( "://" ) != - 1 )
                    {
                        strAlias = strAlias.Remove( 0, strAlias.IndexOf( "://" ) + 3 );
                    }
                    if( strAlias.IndexOf( "\\\\" ) != - 1 )
                    {
                        strAlias = strAlias.Remove( 0, strAlias.IndexOf( "\\\\" ) + 2 );
                    }

                    PortalAliasController p = new PortalAliasController();
                    if( ViewState["PortalAliasID"] != null )
                    {
                        PortalAliasInfo objPortalAliasInfo = new PortalAliasInfo();
                        objPortalAliasInfo.PortalAliasID = Convert.ToInt32( ViewState["PortalAliasID"] );
                        objPortalAliasInfo.PortalID = Convert.ToInt32( ViewState["PortalID"] );
                        objPortalAliasInfo.HTTPAlias = strAlias;
                        p.UpdatePortalAliasInfo( objPortalAliasInfo );
                    }
                    else
                    {
                        PortalAliasInfo objPortalAliasInfo;
                        objPortalAliasInfo = p.GetPortalAlias( strAlias, Convert.ToInt32( ViewState["PortalAliasID"] ) );
                        if( objPortalAliasInfo == null )
                        {
                            objPortalAliasInfo = new PortalAliasInfo();
                            objPortalAliasInfo.PortalID = Convert.ToInt32( ViewState["PortalID"] );
                            objPortalAliasInfo.HTTPAlias = strAlias;
                            p.AddPortalAlias( objPortalAliasInfo );
                        }
                        else
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DuplicateAlias", this.LocalResourceFile ), ModuleMessageType.RedError );
                            return;
                        }
                    }
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Success", this.LocalResourceFile ), ModuleMessageType.GreenSuccess );
                    Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }


    }
}