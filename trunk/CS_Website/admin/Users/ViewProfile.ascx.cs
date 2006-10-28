using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The ViewProfile UserModuleBase is used to view a Users Profile
    /// </summary>
    /// <history>
    /// 	[cnurse]	05/02/2006   Created
    /// </history>
    public partial class ViewProfile : UserModuleBase
    {
        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            // get userid from encrypted ticket
            UserId = Null.NullInteger;
            if( Context.Request.QueryString["userticket"] != null )
            {
                UserId = int.Parse( UrlUtils.DecryptParameter( Context.Request.QueryString["userticket"] ) );
            }

            ctlProfile.ID = "Profile";
            ctlProfile.UserId = UserId;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //Before we bind the Profile to the editor we need to "update" the visibility data
                ProfilePropertyDefinitionCollection properties = ctlProfile.UserProfile.ProfileProperties;

                foreach( ProfilePropertyDefinition profProperty in properties )
                {
                    if( profProperty.Visible )
                    {
                        //Check Visibility
                        if( profProperty.Visibility == UserVisibilityMode.AdminOnly )
                        {
                            //Only Visible if Admin (or self)
                            profProperty.Visible = IsAdmin || IsUser;
                        }
                        else if( profProperty.Visibility == UserVisibilityMode.MembersOnly )
                        {
                            //Only Visible if Is a Member (ie Authenticated)
                            profProperty.Visible = Request.IsAuthenticated;
                        }
                    }
                }

                //Bind the profile information to the control
                ctlProfile.DataBind();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}