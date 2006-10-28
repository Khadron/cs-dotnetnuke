using System;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The BaseUserInfo class provides a base Entity for an online user
    /// </Summary>
    public abstract class BaseUserInfo
    {
        private DateTime _CreationDate;
        private DateTime _LastActiveDate;
        private int _PortalID;
        private int _TabID;

        /// <Summary>Gets and sets the CreationDate for this online user</Summary>
        public DateTime CreationDate
        {
            get
            {
                return this._CreationDate;
            }
            set
            {
                this._CreationDate = value;
            }
        }

        /// <Summary>Gets and sets the LastActiveDate for this online user</Summary>
        public DateTime LastActiveDate
        {
            get
            {
                return this._LastActiveDate;
            }
            set
            {
                this._LastActiveDate = value;
            }
        }

        /// <Summary>Gets and sets the PortalId for this online user</Summary>
        public int PortalID
        {
            get
            {
                return this._PortalID;
            }
            set
            {
                this._PortalID = value;
            }
        }

        /// <Summary>Gets and sets the TabId for this online user</Summary>
        public int TabID
        {
            get
            {
                return this._TabID;
            }
            set
            {
                this._TabID = value;
            }
        }
    }
}