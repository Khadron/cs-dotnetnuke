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
using System.ComponentModel;
using DotNetNuke.Common;
using DotNetNuke.Entities.Profile;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The UserInfo class provides Business Layer model for Users
    /// </Summary>
    /// <summary>
    /// The UserInfo class provides Business Layer model for Users
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class UserInfo
    {
        private int _UserID;
        private string _Username;
        private string _DisplayName;
        private string _FirstName;
        private string _LastName;
        private string _FullName;
        private string _Email;
        private int _PortalID;
        private bool _IsSuperUser;
        private int _AffiliateID;
        private UserMembership _Membership;
        private UserProfile _Profile;
        private string[] _Roles;

        public UserInfo()
        {
            _UserID = -1;
            _PortalID = -1;
            _IsSuperUser = false;
            _AffiliateID = -1;
            _Membership = new UserMembership();
            _Profile = new UserProfile();
        }

        /// <summary>
        /// Gets and sets the AffiliateId for this user
        /// </summary>
        [Browsable(false)]
        public int AffiliateID
        {
            get
            {
                return _AffiliateID;
            }
            set
            {
                _AffiliateID = value;
            }
        }

        /// <summary>
        /// Gets and sets the Display Name
        /// </summary>
        [SortOrder(3), Required(true), MaxLength(128)]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }

        /// <summary>
        /// Gets and sets the Email Address
        /// </summary>
        [SortOrder(4), MaxLength(256), Required(true), RegularExpressionValidator("[\\w\\.-]+(\\+[\\w-]*)?@([\\w-]+\\.)+[\\w-]+")]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;

                //Continue to set the membership Property in case developers have used this
                //in their own code
                this.Membership.Email = value;
            }
        }

        /// <summary>
        /// Gets and sets the First Name
        /// </summary>
        [SortOrder(1), MaxLength(50), Required(true)]
        public string FirstName
        {
            get
            {
                if (_FirstName == "")
                {
                    //Try Profile
                    _FirstName = Profile.FirstName;
                }
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }

        /// <summary>
        /// Gets and sets whether the User is a SuperUser
        /// </summary>
        [Browsable(false)]
        public bool IsSuperUser
        {
            get
            {
                return _IsSuperUser;
            }
            set
            {
                _IsSuperUser = value;
            }
        }

        /// <summary>
        /// Gets and sets the Last Name
        /// </summary>
        [SortOrder(2), MaxLength(50), Required(true)]
        public string LastName
        {
            get
            {
                if (_LastName == "")
                {
                    //Try profile
                    _LastName = Profile.LastName;
                }
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }

        /// <summary>
        /// Gets and sets the Membership object
        /// </summary>
        [Browsable(false)]
        public UserMembership Membership
        {
            get
            {
                //implemented progressive hydration
                //this object will be hydrated on demand
                if (!_Membership.ObjectHydrated && this.Username != null && this.Username.Length > 0)
                {
                    _Membership.ObjectHydrated = true;
                    UserInfo userInfo1 = this;
                    UserController.GetUserMembership(ref userInfo1);
                }
                return _Membership;
            }
            set
            {
                _Membership = value;
                _Membership.ObjectHydrated = true;
            }
        }

        /// <summary>
        /// Gets and sets the PortalId
        /// </summary>
        [Browsable(false)]
        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

        /// <summary>
        /// Gets and sets the Profile Object
        /// </summary>
        [Browsable(false)]
        public UserProfile Profile
        {
            get
            {
                //implemented progressive hydration
                //this object will be hydrated on demand
                if (!_Profile.ObjectHydrated && this.Username != null && this.Username.Length > 0)
                {
                    UserInfo userInfo1 = this;
                    ProfileController.GetUserProfile(ref userInfo1);
                    _Profile.ObjectHydrated = true;
                }
                return _Profile;
            }
            set
            {
                _Profile = value;
                _Profile.ObjectHydrated = true;
            }
        }

        /// <summary>
        /// Gets and sets the Roles for this User
        /// </summary>
        [Browsable(false)]
        public string[] Roles
        {
            get
            {
                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }

        /// <summary>
        /// Gets and sets the User Id
        /// </summary>
        [Browsable(false)]
        public int UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }

        /// <summary>
        /// Gets and sets the User Name
        /// </summary>
        [SortOrder(0), IsReadOnly(true), Required(true)]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;

                //Continue to set the membership Property in case developers have used this
                //in their own code
                this.Membership.Username = value;
            }
        }

        /// <summary>
        /// IsInRole determines whether the user is in the role passed
        /// </summary>
        /// <param name="role">The role to check</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        public bool IsInRole(string role)
        {
            if (IsSuperUser || role == Globals.glbRoleAllUsersName)
            {
                return true;
            }
            else
            {
                if (Roles != null)
                {
                    foreach (string strRole in Roles)
                    {
                        if (strRole == role)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        [Browsable(false), Obsolete("This property has been deprecated in favour of Display Name")]
        public string FullName
        {
            get
            {
                if (_FullName == "")
                {
                    //Build from component names
                    _FullName = FirstName + " " + LastName;
                }
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }
    }
}