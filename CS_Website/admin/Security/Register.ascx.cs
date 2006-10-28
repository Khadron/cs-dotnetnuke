using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The Register PortalModuleBase is used to register Users
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    ///     [cnurse]    10/06/2004  System Messages now handled by Localization
    /// </history>
    public partial class Register : PortalModuleBase
    {
        private int Services = 0;
        private int RoleID = - 1;

        /// <summary>
        /// BindData binds the data from the DB to the controls
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            ModuleController objModules = new ModuleController();
            PortalController objPortalController = new PortalController();

            userControl.ModuleId = objModules.GetModuleByDefinition( PortalId, "Site Settings" ).ModuleID;
            userControl.StartTabIndex = 1;
            addressUser.ModuleId = objModules.GetModuleByDefinition( PortalId, "Site Settings" ).ModuleID;
            addressUser.StartTabIndex = 9;

            if( Services == 1 )
            {
                UserRow.Visible = false;
                PasswordManagementRow.Visible = false;

                RoleController objRoles = new RoleController();
                grdServices.DataSource = objRoles.GetUserRoles( PortalId );
                grdServices.DataBind();

                if( grdServices.Items.Count != 0 )
                {
                    lblServices.Text = string.Format(Localization.GetString("PleaseRegister", this.LocalResourceFile), Globals.GetPortalDomainName(PortalAlias.HTTPAlias, Request, true) + "/" + Globals.glbDefaultPage, TabId);
                }
                else
                {
                    grdServices.Visible = false;
                    lblServices.Text = Localization.GetString( "MembershipNotOffered", this.LocalResourceFile );
                }
                lblServices.Visible = true;

                grdServices.Columns[ 0 ].Visible = false; // subscribe
                grdServices.Columns[ 9 ].Visible = false; // expiry date

                ServicesRow.Visible = true;
            }
            else
            {
                UserRow.Visible = true;

                //Populate the timezone combobox (look up timezone translations based on currently set culture)
                Localization.LoadTimeZoneDropDownList( cboTimeZone, ( (PageBase)Page ).PageCulture.Name, Convert.ToString( PortalSettings.TimeZoneOffset ) );
                Localization.LoadCultureDropDownList( cboLocale, CultureDropDownTypes.NativeName, ( (PageBase)Page ).PageCulture.Name );
                if( cboLocale.Items.Count == 1 )
                {
                    cboLocale.Enabled = false;
                }

                if( Request.IsAuthenticated )
                {
                    lblRegister.Text = Localization.GetString( "RegisterNote", this.LocalResourceFile );
                    cmdRegister.Text = Localization.GetString( "cmdUpdate" );

                    PasswordManagementRow.Visible = true;
                    userControl.ShowPassword = false;

                    if( UserInfo.UserID >= 0 )
                    {
                        userControl.FirstName = UserInfo.FirstName;
                        userControl.LastName = UserInfo.LastName;
                        userControl.UserName = UserInfo.Username;
                        userControl.Email = UserInfo.Email;
                        userControl.IM = UserInfo.Profile.IM;
                        userControl.Website = UserInfo.Profile.Website;
                        if( cboTimeZone.Items.FindByValue( UserInfo.Profile.TimeZone.ToString() ) != null )
                        {
                            cboTimeZone.ClearSelection();
                            cboTimeZone.Items.FindByValue( UserInfo.Profile.TimeZone.ToString() ).Selected = true;
                        }

                        addressUser.Unit = UserInfo.Profile.Unit;
                        addressUser.Street = UserInfo.Profile.Street;
                        addressUser.City = UserInfo.Profile.City;
                        addressUser.Region = UserInfo.Profile.Region;
                        addressUser.Country = UserInfo.Profile.Country;
                        addressUser.Postal = UserInfo.Profile.PostalCode;
                        addressUser.Telephone = UserInfo.Profile.Telephone;
                        addressUser.Fax = UserInfo.Profile.Fax;
                        addressUser.Cell = UserInfo.Profile.Cell;
                        if( cboLocale.Items.FindByValue( UserInfo.Profile.PreferredLocale ) != null )
                        {
                            cboLocale.ClearSelection();
                            cboLocale.Items.FindByValue( UserInfo.Profile.PreferredLocale ).Selected = true;
                        }
                    }

                    RoleController objRoles = new RoleController();

                    grdServices.DataSource = objRoles.GetUserRoles( PortalId, UserInfo.UserID );
                    grdServices.DataBind();

                    if( UserInfo.IsSuperUser )
                    {
                        cmdUnregister.Visible = false;
                        ServicesRow.Visible = false;
                    }
                    else
                    {
                        // if no service available then hide options
                        ServicesRow.Visible = grdServices.Items.Count > 0;
                    }
                }
                else
                {
                    switch( PortalSettings.UserRegistration )
                    {
                        case (int)Globals.PortalRegistrationType.PrivateRegistration:

                            lblRegister.Text = Localization.GetString( "PrivateMembership", this.LocalResourceFile );
                            break;
                        case (int)Globals.PortalRegistrationType.PublicRegistration:

                            lblRegister.Text = Localization.GetString( "PublicMembership", this.LocalResourceFile );
                            break;
                        case (int)Globals.PortalRegistrationType.VerifiedRegistration:

                            lblRegister.Text = Localization.GetString( "VerifiedMembership", this.LocalResourceFile );
                            break;
                    }
                    lblRegister.Text += Localization.GetString( "Required", this.LocalResourceFile );
                    cmdRegister.Text = Localization.GetString( "cmdRegister", this.LocalResourceFile );

                    cmdUnregister.Visible = false;
                    ServicesRow.Visible = false;
                    PasswordManagementRow.Visible = false;
                    userControl.ShowPassword = true;
                }
            }
        }

        /// <summary>
        /// FormatURL correctly formats a URL
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL()
        {
            string strServerPath = Null.NullString;
            try
            {
                strServerPath = Request.ApplicationPath;
                if( ! strServerPath.EndsWith( "/" ) )
                {
                    strServerPath += "/";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return strServerPath + "Register.aspx?tabid=" + TabId;
        }

        /// <summary>
        /// FormatPrice formats the Fee amount and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="price">The price to format</param>
        ///	<returns>The correctly formatted price</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPrice( float price )
        {
            string _FormatPrice = Null.NullString;
            try
            {
                if( price != Null.NullSingle )
                {
                    _FormatPrice = price.ToString( "##0.00" );
                }
                else
                {
                    _FormatPrice = "";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPrice;
        }

        /// <summary>
        /// FormatPeriod formats the Period and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="period">The period to format</param>
        ///	<returns>The correctly formatted period</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPeriod( int period )
        {
            string _FormatPeriod = Null.NullString;
            try
            {
                if( period != Null.NullInteger )
                {
                    _FormatPeriod = period.ToString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPeriod;
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry date and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="DateTime">The date to format</param>
        ///	<returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatExpiryDate( DateTime DateTime )
        {
            string _FormatExpiry = Null.NullString;
            try
            {
                if( ! Null.IsNull( DateTime ) )
                {
                    _FormatExpiry = DateTime.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatExpiry;
        }

        /// <summary>
        /// ServiceText gets the Service Text (Cancel or Subscribe)
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="Subscribed">The service state</param>
        ///	<returns>The correctly formatted text</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string ServiceText( bool Subscribed )
        {
            string _ServiceText = Null.NullString;
            try
            {
                if( ! Subscribed )
                {
                    _ServiceText = Localization.GetString( "Subscribe", this.LocalResourceFile );
                }
                else
                {
                    _ServiceText = Localization.GetString( "Unsubscribe", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _ServiceText;
        }

        /// <summary>
        /// ServiceURL correctly formats the Subscription url
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strKeyName">The key name for the service</param>
        ///	<param name="strKeyValue">The key value for the service</param>
        ///	<param name="objServiceFee">The service fee</param>
        ///	<param name="Subscribed">The service state</param>
        ///	<returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string ServiceURL( string strKeyName, string strKeyValue, object objServiceFee, bool Subscribed )
        {
            string _ServiceUrl = Null.NullString;
            try
            {
                double dblServiceFee = 0;
                string ctlRegister = "";

                if(  objServiceFee != DBNull.Value  )
                {
                    dblServiceFee = Convert.ToDouble( objServiceFee );
                }

                if( PortalSettings.UserTabId != - 1 )
                {
                    // user defined tab
                    ctlRegister = "";
                }
                else
                {
                    // admin tab
                    ctlRegister = "Register";
                }

                if( ! Subscribed )
                {
                    if( dblServiceFee > 0 )
                    {
                        _ServiceUrl = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue;
                    }
                    else
                    {
                        _ServiceUrl = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue );
                    }
                }
                else // cancel
                {
                    if( dblServiceFee > 0 )
                    {
                        _ServiceUrl = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue + "&cancel=1";
                    }
                    else
                    {
                        _ServiceUrl = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue, "cancel=1" );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return _ServiceUrl;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Verify if portal has a customized registration page
                if (!Null.IsNull(PortalSettings.UserTabId) && Globals.IsAdminControl())
                {
                    // user page exists and trying to access this control directly with url param -> not allowed
                    Response.Redirect( Globals.NavigateURL( PortalSettings.UserTabId ) );
                }

                // Verify that the current user has access to this page
                if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false )
                {
                    Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                }

                if( ( Request.QueryString["Services"] != null ) )
                {
                    Services = int.Parse( Request.QueryString["Services"] );
                }

                // free subscriptions
                if( ( Request.QueryString["RoleID"] != null ) )
                {
                    RoleID = int.Parse( Request.QueryString["RoleID"] );

                    RoleController objRoles = new RoleController();

                    RoleInfo objRole = objRoles.GetRole( RoleID, PortalSettings.PortalId );

                    if( objRole.IsPublic && objRole.ServiceFee == 0.0 )
                    {
                        objRoles.UpdateUserRole( PortalId, UserInfo.UserID, RoleID, Convert.ToBoolean( ( Request.QueryString["cancel"] != null ) ? true : false ) );

                        if( PortalSettings.UserTabId != - 1 )
                        {
                            // user defined tab
                            Response.Redirect( Globals.NavigateURL( PortalSettings.UserTabId ), true );
                        }
                        else
                        {
                            // admin tab
                            Response.Redirect( Globals.NavigateURL( "Register" ), true );
                        }
                    }
                    else
                    {
                        // EVENTLOGGER
                    }
                }

                //Localize the Headers
                Localization.LocalizeDataGrid( ref grdServices, this.LocalResourceFile );

                // If this is the first visit to the page, bind the role data to the datalist
                if( Page.IsPostBack == false )
                {
                    ClientAPI.AddButtonConfirm( cmdUnregister, Localization.GetString( "CancelConfirm", this.LocalResourceFile ) );

                    BindData();

                    try
                    {
                        Globals.SetFormFocus(userControl);
                    }
                    catch
                    {
                        //control not there or error setting focus
                    }

                    // Store URL Referrer to return to portal
                    if( Request.UrlReferrer != null )
                    {
                        ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                    }
                    else
                    {
                        ViewState["UrlReferrer"] = "";
                    }
                }

                lblRegistration.Text = Localization.GetSystemMessage( PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS" );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdRegister_Click runs when the Register Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [cnurse]    10/06/2004  System Messages now handled by Localization
        /// </history>
        protected void cmdRegister_Click( Object sender, EventArgs E )
        {
            try
            {
                // Only attempt a save/update if all form fields on the page are valid
                if( Page.IsValid == true )
                {
                    // Add New User to Portal User Database
                    PortalSecurity objSecurity = new PortalSecurity();
                    ModuleController objModules = new ModuleController();
                    UserInfo objUserInfo;

                    objUserInfo = UserController.GetUserByName( PortalId, userControl.UserName, false );

                    if( Request.IsAuthenticated )
                    {
                        if( userControl.Password != "" || userControl.Confirm != "" )
                        {
                            if( userControl.Password != userControl.Confirm )
                            {
                                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "PasswordMatchFailure", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                            }
                            return;
                        }

                        string Username = null;

                        if( UserInfo.UserID >= 0 )
                        {
                            Username = UserInfo.Username;
                        }

                        //if a user is found with that username and the username isn't our current user's username
                        if( objUserInfo != null && userControl.UserName != Username )
                        {
                            //username already exists in DB so show user an error
                            UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "RegistrationFailure", this.LocalResourceFile ), userControl.UserName ), ModuleMessage.ModuleMessageType.YellowWarning );
                        }
                        else
                        {
                            //update the user
                            objUserInfo.UserID = UserInfo.UserID;
                            objUserInfo.PortalID = PortalId;
                            objUserInfo.Profile.FirstName = userControl.FirstName;
                            objUserInfo.Profile.LastName = userControl.LastName;
                            objUserInfo.Profile.Unit = addressUser.Unit;
                            objUserInfo.Profile.Street = addressUser.Street;
                            objUserInfo.Profile.City = addressUser.City;
                            objUserInfo.Profile.Region = addressUser.Region;
                            objUserInfo.Profile.PostalCode = addressUser.Postal;
                            objUserInfo.Profile.Country = addressUser.Country;
                            objUserInfo.Profile.Telephone = addressUser.Telephone;
                            objUserInfo.Email = userControl.Email;
                            objUserInfo.Username = userControl.UserName;
                            objUserInfo.Profile.Cell = addressUser.Cell;
                            objUserInfo.Profile.Fax = addressUser.Fax;
                            objUserInfo.Profile.IM = userControl.IM;
                            objUserInfo.Profile.Website = userControl.Website;
                            objUserInfo.Profile.PreferredLocale = cboLocale.SelectedItem.Value;
                            objUserInfo.Profile.TimeZone = Convert.ToInt32( cboTimeZone.SelectedItem.Value );

                            //Update the User
                            UserController.UpdateUser( PortalId, objUserInfo );

                            //store preferredlocale in cookie
                            Localization.SetLanguage( objUserInfo.Profile.PreferredLocale );

                            //clear the portal cache if current user is portal administrator (catch email adress changes)
                            if( UserInfo.UserID == PortalSettings.AdministratorId )
                            {
                                DataCache.ClearPortalCache( PortalId, true );
                            }
                            // Redirect browser back to home page
                            Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
                        }
                    }
                    else
                    {
                        UserCreateStatus userCreateStatus;

                        //if a user is found with that username, error.
                        //this prevents you from adding a username
                        //with the same name as a superuser.
                        if( objUserInfo != null )
                        {
                            //username already exists in DB so show user an error
                            UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "RegistrationFailure", this.LocalResourceFile ), userControl.UserName ), ModuleMessage.ModuleMessageType.YellowWarning );
                            return;
                        }

                        int AffiliateId = Null.NullInteger;
                        if( Request.Cookies[ "AffiliateId" ] != null )
                        {
                            AffiliateId = int.Parse( Request.Cookies[ "AffiliateId" ].Value );
                        }

                        UserInfo objNewUser = new UserInfo();
                        objNewUser.PortalID = PortalId;
                        objNewUser.Profile.FirstName = userControl.FirstName;
                        objNewUser.Profile.LastName = userControl.LastName;
                        objNewUser.Profile.Unit = addressUser.Unit;
                        objNewUser.Profile.Street = addressUser.Street;
                        objNewUser.Profile.City = addressUser.City;
                        objNewUser.Profile.Region = addressUser.Region;
                        objNewUser.Profile.PostalCode = addressUser.Postal;
                        objNewUser.Profile.Country = addressUser.Country;
                        objNewUser.Profile.Telephone = addressUser.Telephone;
                        objNewUser.Email = userControl.Email;
                        objNewUser.Username = userControl.UserName;
                        objNewUser.Membership.Password = userControl.Password;
                        objNewUser.Membership.Approved = Convert.ToBoolean( PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.PublicRegistration ? false : true );
                        objNewUser.AffiliateID = AffiliateId;
                        objNewUser.Profile.Cell = addressUser.Cell;
                        objNewUser.Profile.Fax = addressUser.Fax;
                        objNewUser.Profile.IM = userControl.IM;
                        objNewUser.Profile.Website = userControl.Website;
                        objNewUser.Profile.PreferredLocale = cboLocale.SelectedItem.Value;
                        objNewUser.Profile.TimeZone = Convert.ToInt32( cboTimeZone.SelectedItem.Value );

                        userCreateStatus = UserController.CreateUser( ref objNewUser );

                        if( userCreateStatus == UserCreateStatus.Success )
                        {
                            EventLogController objEventLog = new EventLogController();
                            objEventLog.AddLog( objNewUser, PortalSettings, UserId, userControl.UserName, EventLogController.EventLogType.USER_CREATED );

                            // send notification to portal administrator of new user registration
                            Mail.SendMail( PortalSettings.Email, PortalSettings.Email, "", Localization.GetSystemMessage( PortalSettings.DefaultLanguage, PortalSettings, "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT", objNewUser ), Localization.GetSystemMessage( PortalSettings.DefaultLanguage, PortalSettings, "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY", objNewUser ), "", "", "", "", "", "" );

                            // complete registration
                            string strMessage = "";
                            if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.PrivateRegistration )
                            {
                                Mail.SendMail( PortalSettings.Email, userControl.Email, "", Localization.GetSystemMessage( objNewUser.Profile.PreferredLocale, PortalSettings, "EMAIL_USER_REGISTRATION_PRIVATE_SUBJECT", objNewUser ), Localization.GetSystemMessage( objNewUser.Profile.PreferredLocale, PortalSettings, "EMAIL_USER_REGISTRATION_PRIVATE_BODY", objNewUser ), "", "", "", "", "", "" );
                                //show a message that a portal administrator has to verify the user credentials
                                strMessage = string.Format( Localization.GetString( "PrivateConfirmationMessage", this.LocalResourceFile ), userControl.Email );
                            }
                            else if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.PublicRegistration )
                            {
                                UserLoginStatus loginStatus = 0;
                                UserController.UserLogin( PortalSettings.PortalId, userControl.UserName, userControl.Password, "", PortalSettings.PortalName, "", ref loginStatus, false );
                            }
                            else if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.VerifiedRegistration )
                            {
                                Mail.SendMail( PortalSettings.Email, userControl.Email, "", Localization.GetSystemMessage( objNewUser.Profile.PreferredLocale, PortalSettings, "EMAIL_USER_REGISTRATION_PUBLIC_SUBJECT", objNewUser ), Localization.GetSystemMessage( objNewUser.Profile.PreferredLocale, PortalSettings, "EMAIL_USER_REGISTRATION_PUBLIC_BODY", objNewUser ), "", "", "", "", "", "" );
                                //show a message that an email has been send with the registration details
                                strMessage = string.Format( Localization.GetString( "VerifiedConfirmationMessage", this.LocalResourceFile ), userControl.Email );
                            }

                            // affiliate
                            if( ! Null.IsNull( AffiliateId ) )
                            {
                                AffiliateController objAffiliates = new AffiliateController();
                                objAffiliates.UpdateAffiliateStats( AffiliateId, 0, 1 );
                            }

                            //store preferredlocale in cookie
                            Localization.SetLanguage( objNewUser.Profile.PreferredLocale );

                            if( strMessage.Length != 0 )
                            {
                                UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess );
                                UserRow.Visible = false;
                                cmdRegister.Visible = false;
                                cmdUnregister.Visible = false;
                                cmdCancel.Visible = false;
                            }
                            else
                            {
                                // Redirect browser back to home page
                                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
                            }
                        }
                        else // registration error
                        {
                            UI.Skins.Skin.AddModuleMessage( this, UserController.GetUserCreateStatus( userCreateStatus ), ModuleMessage.ModuleMessageType.RedError );
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdRSVP_Click runs when the Subscribe to RSVP Code Roles Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/19/2006  created
        /// </history>
        protected void cmdRSVP_Click( object sender, EventArgs e )
        {
            //Get the RSVP code
            string code = txtRSVPCode.Text;

            if( code != "" )
            {
                //Get the roles from the Database
                RoleController objRoles = new RoleController();
                RoleInfo objRole;
                ArrayList arrRoles = objRoles.GetPortalRoles( PortalSettings.PortalId );

                //Parse the roles
                foreach( RoleInfo tempLoopVar_objRole in arrRoles )
                {
                    objRole = tempLoopVar_objRole;
                    if( objRole.RSVPCode == code )
                    {
                        //Subscribe User to Role
                        objRoles.UpdateUserRole( PortalId, UserInfo.UserID, objRole.RoleID );
                    }
                }
            }

            //Reset RSVP Code field
            txtRSVPCode.Text = "";

            BindData();
        }

        /// <summary>
        /// cmdUnregister_Click runs when the UnRegister Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUnregister_Click( object sender, EventArgs e )
        {
            try
            {
                UserInfo user = UserInfo;
                UserController.DeleteUser( ref user, true, false );

                Response.Redirect( "~/Admin/Security/Logoff.aspx?portalid=" + PortalSettings.PortalId.ToString() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdatePassword_Click runs when the Update Pasword Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdatePassword_Click( Object sender, EventArgs e )
        {
            if( txtOldPassword.Text == "" )
            {
                MessageCell.Controls.Add( UI.Skins.Skin.GetModuleMessageControl( "", Localization.GetString( "OldPassword", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning ) );
                return;
            }
            if( txtNewPassword.Text == "" || txtNewConfirm.Text == "" )
            {
                MessageCell.Controls.Add( UI.Skins.Skin.GetModuleMessageControl( "", Localization.GetString( "NewPassword", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning ) );
                return;
            }
            if( txtNewPassword.Text != txtNewConfirm.Text )
            {
                MessageCell.Controls.Add( UI.Skins.Skin.GetModuleMessageControl( "", Localization.GetString( "PasswordMatchFailure", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning ) );
                return;
            }

            if( UserController.ChangePassword( UserInfo, txtOldPassword.Text, txtNewPassword.Text ) )
            {
                //Success
                MessageCell.Controls.Add( UI.Skins.Skin.GetModuleMessageControl( "", Localization.GetString( "PasswordChanged", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess ) );
            }
            else
            {
                //Fail
                MessageCell.Controls.Add( UI.Skins.Skin.GetModuleMessageControl( "", Localization.GetString( "PasswordUpdateFailed", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning ) );
            }
        }
    }
}