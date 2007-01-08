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