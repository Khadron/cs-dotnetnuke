using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The UserProfile class provides a Business Layer entity for the Users Profile
    /// </Summary>
    public class UserProfile
    {
        //Name properties
        private const string cPrefix = "Prefix";
        private const string cFirstName = "FirstName";
        private const string cMiddleName = "MiddleName";
        private const string cLastName = "LastName";
        private const string cSuffix = "Suffix";

        //Address Properties
        private const string cUnit = "Unit";
        private const string cStreet = "Street";
        private const string cCity = "City";
        private const string cRegion = "Region";
        private const string cCountry = "Country";
        private const string cPostalCode = "PostalCode";

        //Phone contact
        private const string cTelephone = "Telephone";
        private const string cCell = "Cell";
        private const string cFax = "Fax";

        //Online contact
        private const string cWebsite = "Website";
        private const string cIM = "IM";

        //Preferences
        private const string cTimeZone = "TimeZone";
        private const string cPreferredLocale = "PreferredLocale";

        private bool _IsDirty;
        private bool _ObjectHydrated;

        // collection to store all profile properties.
        private ProfilePropertyDefinitionCollection _profileProperties;

        public UserProfile()
        {
        }

        /// <summary>
        /// Gets and sets the Cell/Mobile Phone
        /// </summary>
        public string Cell
        {
            get
            {
                return GetPropertyValue(cCell);
            }
            set
            {
                SetProfileProperty(cCell, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the City part of the Address
        /// </summary>
        public string City
        {
            get
            {
                return GetPropertyValue(cCity);
            }
            set
            {
                SetProfileProperty(cCity, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Country part of the Address
        /// </summary>
        public string Country
        {
            get
            {
                return GetPropertyValue(cCountry);
            }
            set
            {
                SetProfileProperty(cCountry, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Fax Phone
        /// </summary>
        public string Fax
        {
            get
            {
                return GetPropertyValue(cFax);
            }
            set
            {
                SetProfileProperty(cFax, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the First Name
        /// </summary>
        public string FirstName
        {
            get
            {
                return GetPropertyValue(cFirstName);
            }
            set
            {
                SetProfileProperty(cFirstName, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Full Name
        /// </summary>
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Gets and sets the Instant Messenger Handle
        /// </summary>
        public string IM
        {
            get
            {
                return GetPropertyValue(cIM);
            }
            set
            {
                SetProfileProperty(cIM, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the property has been changed
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
        }

        /// <summary>
        /// Gets and sets the Last Name
        /// </summary>
        public string LastName
        {
            get
            {
                return GetPropertyValue(cLastName);
            }
            set
            {
                SetProfileProperty(cLastName, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets whether the object has been Hydrated
        /// </summary>
        public bool ObjectHydrated
        {
            get
            {
                return _ObjectHydrated;
            }
            set
            {
                _ObjectHydrated = value;
            }
        }

        /// <summary>
        /// Gets and sets the PostalCode part of the Address
        /// </summary>
        public string PostalCode
        {
            get
            {
                return GetPropertyValue(cPostalCode);
            }
            set
            {
                SetProfileProperty(cPostalCode, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Preferred Locale
        /// </summary>
        public string PreferredLocale
        {
            get
            {
                return GetPropertyValue(cPreferredLocale);
            }
            set
            {
                SetProfileProperty(cPreferredLocale, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Collection of Profile Properties
        /// </summary>
        public ProfilePropertyDefinitionCollection ProfileProperties
        {
            get
            {
                if (_profileProperties == null)
                {
                    _profileProperties = new ProfilePropertyDefinitionCollection();
                }
                return _profileProperties;
            }
        }

        /// <summary>
        /// Gets and sets the Region part of the Address
        /// </summary>
        public string Region
        {
            get
            {
                return GetPropertyValue(cRegion);
            }
            set
            {
                SetProfileProperty(cRegion, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Street part of the Address
        /// </summary>
        public string Street
        {
            get
            {
                return GetPropertyValue(cStreet);
            }
            set
            {
                SetProfileProperty(cStreet, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Telephone
        /// </summary>
        public string Telephone
        {
            get
            {
                return GetPropertyValue(cTelephone);
            }
            set
            {
                SetProfileProperty(cTelephone, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Unit part of the Address
        /// </summary>
        public string Unit
        {
            get
            {
                return GetPropertyValue(cUnit);
            }
            set
            {
                SetProfileProperty(cUnit, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the TimeZone
        /// </summary>
        public int TimeZone
        {
            get
            {
                int retValue = Null.NullInteger;
                string propValue = GetPropertyValue(cTimeZone);
                if (propValue != null)
                {
                    retValue = int.Parse(propValue);
                }
                return retValue;
            }
            set
            {
                SetProfileProperty(cTimeZone, value.ToString());
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Website
        /// </summary>
        public string Website
        {
            get
            {
                return GetPropertyValue(cWebsite);
            }
            set
            {
                SetProfileProperty(cWebsite, value);
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Clears the IsDirty Flag
        /// </summary>
        public void ClearIsDirty()
        {
            _IsDirty = false;
            foreach (ProfilePropertyDefinition profProperty in ProfileProperties)
            {
                profProperty.ClearIsDirty();
            }
        }

        /// <summary>
        /// Gets a Profile Property from the Profile
        /// </summary>
        /// <remarks></remarks>
        /// <param name="propName">The name of the property to retrieve.</param>
        public ProfilePropertyDefinition GetProperty(string propName)
        {
            return ProfileProperties[propName];
        }

        /// <summary>
        /// Gets a Profile Property Value from the Profile
        /// </summary>
        /// <remarks></remarks>
        /// <param name="propName">The name of the propoerty to retrieve.</param>
        public string GetPropertyValue(string propName)
        {
            //Declare ProfileProperty
            string propValue = Null.NullString;
            ProfilePropertyDefinition profileProp = GetProperty(propName);

            if (profileProp != null)
            {
                propValue = profileProp.PropertyValue;
            }

            return propValue;
        }

        /// <summary>
        /// Initialises the Profile with an empty collection of profile properties
        /// </summary>
        /// <remarks></remarks>
        /// <param name="portalId">The name of the property to retrieve.</param>
        public void InitialiseProfile(int portalId)
        {
            _profileProperties = ProfileController.GetPropertyDefinitionsByPortal(portalId);
        }

        /// <summary>
        /// Sets a Profile Property Value in the Profile
        /// </summary>
        /// <remarks></remarks>
        /// <param name="propName">The name of the propoerty to set.</param>
        /// <param name="propValue">The value of the propoerty to set.</param>
        public void SetProfileProperty(string propName, string propValue)
        {
            //Declare ProfileProperty
            ProfilePropertyDefinition profileProp = GetProperty(propName);

            if (profileProp != null)
            {
                profileProp.PropertyValue = propValue;

                //Set the IsDirty flag
                if (profileProp.IsDirty)
                {
                    _IsDirty = true;
                }
            }
        }
    }
}