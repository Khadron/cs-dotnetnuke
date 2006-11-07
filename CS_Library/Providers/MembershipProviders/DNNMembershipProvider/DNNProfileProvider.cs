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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership.Data;

namespace DotNetNuke.Security.Profile
{
    /// <summary>
    /// The DNNProfileProvider overrides the default ProfileProvider to provide
    /// a purely DotNetNuke implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/29/2006	Created
    /// </history>
    public class DNNProfileProvider : ProfileProvider
    {
        private static DotNetNuke.Security.Membership.Data.DataProvider dataProvider;

        /// <summary>
        /// Gets whether the Provider Properties can be edited
        /// </summary>
        /// <returns>A Boolean</returns>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        public override bool CanEditProviderProperties
        {
            get
            {
                return true;
            }
        }

        static DNNProfileProvider()
        {
            DNNProfileProvider.dataProvider = DataProvider.Instance();
        }

        /// <summary>
        /// Gets the Profile properties for the User
        /// </summary>
        /// <param name="portalId">The Id of the portal.</param>
        /// <param name="objProfile">The profile.</param>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        private void GetProfileProperties(int portalId, IDataReader dr, UserProfile objProfile)
        {
            int definitionId;
            ProfilePropertyDefinition profProperty;
            ProfilePropertyDefinitionCollection properties;
            properties = ProfileController.GetPropertyDefinitionsByPortal( portalId );

            //Iterate through the Profile properties
            try
            {
                while( dr.Read() )
                {
                    definitionId = Convert.ToInt32( dr["PropertyDefinitionId"] );
                    profProperty = properties.GetById( definitionId );
                    if( profProperty != null )
                    {
                        profProperty.PropertyValue = Convert.ToString( dr["PropertyValue"] );
                        profProperty.Visibility = (UserVisibilityMode)dr["Visibility"];
                    }
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }

            //Add the properties to the profile
            foreach( ProfilePropertyDefinition tempLoopVar_profProperty in properties )
            {
                profProperty = tempLoopVar_profProperty;
                objProfile.ProfileProperties.Add( profProperty );
            }
        }

        /// <summary>
        /// GetUserProfile retrieves the UserProfile information from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user whose Profile information we are retrieving.</param>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        public override void GetUserProfile(ref UserInfo user)
        {
            int portalId;
            if( user.IsSuperUser )
            {
                portalId = Common.Globals.glbSuperUserAppName;
            }
            else
            {
                portalId = user.PortalID;
            }

            IDataReader dr = dataProvider.GetUserProfile( user.UserID );

            UserProfile objUserProfile = new UserProfile();
            GetProfileProperties( portalId, dr, objUserProfile );

            //Clear IsDirty Flag
            objUserProfile.ClearIsDirty();
            user.Profile = objUserProfile;
        }

        /// <summary>
        /// UpdateUserProfile persists a user's Profile to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        public override void UpdateUserProfile(UserInfo user)
        {
            ProfilePropertyDefinitionCollection properties = user.Profile.ProfileProperties;

            foreach( ProfilePropertyDefinition profProperty in properties )
            {
                if( ( profProperty.PropertyValue != null ) && ( profProperty.IsDirty ) )
                {
                    dataProvider.UpdateProfileProperty( Null.NullInteger, user.UserID, profProperty.PropertyDefinitionId, profProperty.PropertyValue, (int)profProperty.Visibility, DateTime.Now );
                }
            }
        }
    }
}