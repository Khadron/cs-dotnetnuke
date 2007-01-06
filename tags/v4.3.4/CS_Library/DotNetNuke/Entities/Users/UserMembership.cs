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
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Entities.Users
{
    /// <summary>
    /// The UserMembership class provides Business Layer model for the Users Membership
    /// related properties
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class UserMembership
    {
        private bool _Approved = true;
        private DateTime _CreatedDate;
        private string _Email;
        private bool _IsOnLine;
        private DateTime _LastActivityDate;
        private DateTime _LastLockoutDate;
        private DateTime _LastLoginDate;
        private DateTime _LastPasswordChangeDate;
        private bool _LockedOut = false;
        private bool _ObjectHydrated;
        private string _Password;
        private string _PasswordQuestion;
        private bool _UpdatePassword;
        private string _Username;

        /// <summary>
        /// Gets and sets whether the User is Approved
        /// </summary>
        [SortOrder(9)]
        public bool Approved
        {
            get
            {
                return _Approved;
            }
            set
            {
                _Approved = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the User's Creation Date
        /// </summary>
        [SortOrder(1), IsReadOnly(true)]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets whether the User Is Online
        /// </summary>
        [SortOrder(7)]
        public bool IsOnLine
        {
            get
            {
                return _IsOnLine;
            }
            set
            {
                _IsOnLine = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Last Activity Date of the User
        /// </summary>
        [SortOrder(3), IsReadOnly(true)]
        public DateTime LastActivityDate
        {
            get
            {
                return _LastActivityDate;
            }
            set
            {
                _LastActivityDate = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Last Lock Out Date of the User
        /// </summary>
        [SortOrder(5), IsReadOnly(true)]
        public DateTime LastLockoutDate
        {
            get
            {
                return _LastLockoutDate;
            }
            set
            {
                _LastLockoutDate = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Last Login Date of the User
        /// </summary>
        [SortOrder(2), IsReadOnly(true)]
        public DateTime LastLoginDate
        {
            get
            {
                return _LastLoginDate;
            }
            set
            {
                _LastLoginDate = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the Last Password Change Date of the User
        /// </summary>
        [SortOrder(4), IsReadOnly(true)]
        public DateTime LastPasswordChangeDate
        {
            get
            {
                return _LastPasswordChangeDate;
            }
            set
            {
                _LastPasswordChangeDate = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets whether the user is locked out
        /// </summary>
        [SortOrder(8)]
        public bool LockedOut
        {
            get
            {
                return _LockedOut;
            }
            set
            {
                _LockedOut = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets whether the object is hydrated
        /// </summary>
        [Browsable(false)]
        public bool ObjectHydrated
        {
            get
            {
                return _ObjectHydrated;
            }
            set
            {
                _ObjectHydrated = value;
                if (!ObjectHydrated)
                {
                    _ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the User's Password
        /// </summary>
        [Browsable(false)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets the User's Password Question
        /// </summary>
        [Browsable(false)]
        public string PasswordQuestion
        {
            get
            {
                return _PasswordQuestion;
            }
            set
            {
                _PasswordQuestion = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        /// <summary>
        /// Gets and sets a flag that determines whether the password should be updated
        /// </summary>
        [SortOrder(10)]
        public bool UpdatePassword
        {
            get
            {
                return _UpdatePassword;
            }
            set
            {
                _UpdatePassword = value;
            }
        }

        /// <summary>
        /// Gets and sets the User's Email Address
        /// </summary>
        [Browsable(false)]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }

        [Browsable(false)]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                if (!ObjectHydrated)
                {
                    ObjectHydrated = true;
                }
            }
        }
    }
}