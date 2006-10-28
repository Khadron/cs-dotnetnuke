using System;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The UserRoleInfo class provides Business Layer model for a User/Role
    /// </Summary>
    public class UserRoleInfo : RoleInfo
    {
        private DateTime _EffectiveDate;
        private string _Email;
        private DateTime _ExpiryDate;
        private string _FullName;
        private bool _IsTrialUsed;
        private bool _Subscribed;
        private int _UserID;
        private int _UserRoleID;

        public DateTime EffectiveDate
        {
            get
            {
                return this._EffectiveDate;
            }
            set
            {
                this._EffectiveDate = value;
            }
        }

        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
            }
        }

        public DateTime ExpiryDate
        {
            get
            {
                return this._ExpiryDate;
            }
            set
            {
                this._ExpiryDate = value;
            }
        }

        public string FullName
        {
            get
            {
                return this._FullName;
            }
            set
            {
                this._FullName = value;
            }
        }

        public bool IsTrialUsed
        {
            get
            {
                return this._IsTrialUsed;
            }
            set
            {
                this._IsTrialUsed = value;
            }
        }

        public bool Subscribed
        {
            get
            {
                return this._Subscribed;
            }
            set
            {
                this._Subscribed = value;
            }
        }

        public int UserID
        {
            get
            {
                return this._UserID;
            }
            set
            {
                this._UserID = value;
            }
        }

        public int UserRoleID
        {
            get
            {
                return this._UserRoleID;
            }
            set
            {
                this._UserRoleID = value;
            }
        }
    }
}