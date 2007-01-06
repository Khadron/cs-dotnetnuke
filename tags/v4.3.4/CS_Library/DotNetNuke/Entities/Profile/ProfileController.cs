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
using System.Collections;
using System.Data;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Profile;

namespace DotNetNuke.Entities.Profile
{
    /// <Summary>
    /// The ProfileController class provides Business Layer methods for profiles and
    /// for profile property Definitions
    /// </Summary>
    public class ProfileController
    {
        public const string PROPERTIES_CACHEKEY = "Profile.Property.Definition";
        private static int _orderCounter;
        private static ProfileProvider profileProvider = ProfileProvider.Instance();

        private static DataProvider provider = DataProvider.Instance();

        /// <summary>
        /// Adds a Property Defintion to the Data Store
        /// </summary>
        /// <param name="definition">An ProfilePropertyDefinition object</param>
        /// <returns>The Id of the definition (or if negative the errorcode of the error)</returns>
        public static int AddPropertyDefinition(ProfilePropertyDefinition definition)
        {
            return provider.AddPropertyDefinition(definition.PortalId, definition.ModuleDefId, definition.DataType, definition.DefaultValue, definition.PropertyCategory, definition.PropertyName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length);
        }

        /// <summary>
        /// Fills a ProfilePropertyDefinitionCollection from a DataReader
        /// </summary>
        /// <param name="dr">An IDataReader object</param>
        /// <returns>The ProfilePropertyDefinitionCollection</returns>
        private static ProfilePropertyDefinitionCollection FillCollection(IDataReader dr)
        {
            ArrayList arrDefinitions = CBO.FillCollection(dr, typeof(ProfilePropertyDefinition));
            ProfilePropertyDefinitionCollection definitionsCollection = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in arrDefinitions)
            {
                definition.ClearIsDirty();
                definitionsCollection.Add(definition);
            }
            return definitionsCollection;
        }

        /// <summary>
        /// Gets a Property Defintion from the Data Store by id
        /// </summary>
        /// <param name="definitionId">The id of the ProfilePropertyDefinition object to retrieve</param>
        /// <returns>The ProfilePropertyDefinition object</returns>
        public static ProfilePropertyDefinition GetPropertyDefinition(int definitionId)
        {
            return ((ProfilePropertyDefinition)CBO.FillObject(provider.GetPropertyDefinition(definitionId), typeof(ProfilePropertyDefinition)));
        }

        /// <summary>
        /// Gets a Property Defintion from the Data Store by name
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <param name="name">The name of the ProfilePropertyDefinition object to retrieve</param>
        /// <returns>The ProfilePropertyDefinition object</returns>
        public static ProfilePropertyDefinition GetPropertyDefinitionByName(int portalId, string name)
        {
            return ((ProfilePropertyDefinition)CBO.FillObject(provider.GetPropertyDefinitionByName(portalId, name), typeof(ProfilePropertyDefinition)));
        }

        /// <summary>
        /// Gets a collection of Property Defintions from the Data Store by category
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <param name="category">The category of the Property Defintions to retrieve</param>
        /// <returns>A ProfilePropertyDefinitionCollection object</returns>
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByCategory(int portalId, string category)
        {
            return FillCollection(provider.GetPropertyDefinitionsByCategory(portalId, category));
        }

        /// <summary>
        /// Gets a collection of Property Defintions from the Data Store by portal
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <returns>A ProfilePropertyDefinitionCollection object</returns>
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByPortal(int portalId)
        {
            return FillCollection(provider.GetPropertyDefinitionsByPortal(portalId));
        }

        /// <summary>
        /// Updates a User's Profile
        /// </summary>
        /// <param name="objUser">The use to update</param>
        /// <param name="profileProperties">The collection of profile properties</param>
        /// <returns>The updated User</returns>
        public static UserInfo UpdateUserProfile(UserInfo objUser, ProfilePropertyDefinitionCollection profileProperties)
        {
            //Iterate through the Definitions
            foreach (ProfilePropertyDefinition propertyDefinition in profileProperties)
            {
                string propertyName = propertyDefinition.PropertyName;
                string propertyValue = propertyDefinition.PropertyValue;

                if (propertyDefinition.IsDirty)
                {
                    //Update Profile
                    objUser.Profile.SetProfileProperty(propertyName, propertyValue);
                }
            }

            UpdateUserProfile(objUser);

            return objUser;
        }

        /// <summary>
        /// Validates the Profile properties for the User (determines if all required properties
        /// have been set)
        /// </summary>
        /// <param name="portalId">The Id of the portal.</param>
        /// <param name="objProfile">The profile.</param>
        public static bool ValidateProfile(int portalId, UserProfile objProfile)
        {
            bool isValid = true;

            foreach (ProfilePropertyDefinition propertyDefinition in objProfile.ProfileProperties)
            {
                if (propertyDefinition.Required && propertyDefinition.PropertyValue == Null.NullString)
                {
                    isValid = false;
                    goto endOfForLoop;
                }
            }
        endOfForLoop:

            return isValid;
        }

        /// <summary>
        /// Adds a single default property definition
        /// </summary>
        /// <param name="PortalId">Id of the Portal</param>
        /// <param name="category">Category of the Property</param>
        /// <param name="name">Name of the Property</param>
        private static void AddDefaultDefinition(int PortalId, string category, string name, string strType, int length, ListEntryInfoCollection types)
        {
            ListEntryInfo typeInfo = types.Item("DataType." + strType);
            if (typeInfo == null)
            {
                typeInfo = types.Item("DataType.Unknown");
            }

            ProfilePropertyDefinition propertyDefinition = new ProfilePropertyDefinition();
            propertyDefinition.DataType = typeInfo.EntryID;
            propertyDefinition.DefaultValue = "";
            propertyDefinition.ModuleDefId = Null.NullInteger;
            propertyDefinition.PortalId = PortalId;
            propertyDefinition.PropertyCategory = category;
            propertyDefinition.PropertyName = name;
            propertyDefinition.Required = false;
            propertyDefinition.Visible = true;
            propertyDefinition.Length = length;

            _orderCounter += 2;

            propertyDefinition.ViewOrder = _orderCounter;

            AddPropertyDefinition(propertyDefinition);
        }

        /// <summary>
        /// Adds the default property definitions for a portal
        /// </summary>
        /// <param name="PortalId">Id of the Portal</param>
        public static void AddDefaultDefinitions(int PortalId)
        {
            _orderCounter = 1;

            ListController objListController = new ListController();
            ListEntryInfoCollection dataTypes = objListController.GetListEntryInfoCollection("DataType");

            AddDefaultDefinition(PortalId, "Name", "Prefix", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "FirstName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "MiddleName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "LastName", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "Suffix", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Unit", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Street", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "City", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Region", "Region", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Country", "Country", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "PostalCode", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Telephone", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Cell", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Fax", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Website", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "IM", "Text", 50, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "Biography", "RichText", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "TimeZone", "TimeZone", 0, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "PreferredLocale", "Locale", 0, dataTypes);
        }

        /// <summary>
        /// Clears the Profile Definitions Cache
        /// </summary>
        /// <param name="PortalId">Id of the Portal</param>
        public static void ClearProfileDefinitionCache(int PortalId)
        {
            //Clear the Cache and refresh the Properties collection
            string strKey = PROPERTIES_CACHEKEY + "." + PortalId.ToString();
            DataCache.RemoveCache(strKey);
        }

        /// <summary>
        /// Deletes a Property Defintion from the Data Store
        /// </summary>
        /// <param name="definitionId">The id of the ProfilePropertyDefinition object to delete</param>
        public static void DeletePropertyDefinition(int definitionId)
        {
            provider.DeletePropertyDefinition(definitionId);
        }

        /// <summary>
        /// Deletes a Property Defintion from the Data Store
        /// </summary>
        /// <param name="definition">The ProfilePropertyDefinition object to delete</param>
        public static void DeletePropertyDefinition(ProfilePropertyDefinition definition)
        {
            DeletePropertyDefinition(definition.PropertyDefinitionId);
        }

        /// <summary>
        /// Gets the Profile Information for the User
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The user whose Profile information we are retrieving.</param>
        public static void GetUserProfile(ref UserInfo objUser)
        {
            profileProvider.GetUserProfile(ref objUser);
        }

        /// <summary>
        /// Updates a Property Defintion in the Data Store
        /// </summary>
        /// <param name="definition">The ProfilePropertyDefinition object to update</param>
        public static void UpdatePropertyDefinition(ProfilePropertyDefinition definition)
        {
            provider.UpdatePropertyDefinition(definition.PropertyDefinitionId, definition.DataType, definition.DefaultValue, definition.PropertyCategory, definition.PropertyName, definition.Required, definition.ValidationExpression, definition.ViewOrder, definition.Visible, definition.Length);
        }

        /// <summary>
        /// Updates a User's Profile
        /// </summary>
        /// <param name="objUser">The use to update</param>
        /// <remarks>
        /// </remarks>
        public static void UpdateUserProfile(UserInfo objUser)
        {
            string UserInfoCacheKey = UserController.CacheKey(objUser.PortalID, objUser.Username);

            //Update the User Profile
            if (objUser.Profile.IsDirty)
            {
                profileProvider.UpdateUserProfile(objUser);
            }

            //Remove the UserInfo from the Cache, as it has been modified
            DataCache.RemoveCache(UserInfoCacheKey);
        }
    }
}