#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Security.Authentication
{
    public class GroupInfo : RoleInfo, IAuthenticationObjectBase
    {
        private ArrayList mAuthenticationMember;
        private ArrayList mDNNMember;
        private bool mDNNPopulated;
        private string mGUID;
        private bool mIsPopulated;
        private string mLocation;
        private ArrayList mMembers;
        private string mProcessLog;

        public GroupInfo()
        {
            this.mGUID = "";
            this.mLocation = "";
            this.mIsPopulated = false;
            this.mMembers = new ArrayList();
            this.mAuthenticationMember = new ArrayList();
            this.mDNNPopulated = false;
            this.mDNNMember = new ArrayList();
            this.mProcessLog = "";
        }

        public ArrayList AuthenticationMember
        {
            get
            {
                return this.mAuthenticationMember;
            }
        }

        public virtual string GUID
        {
            get
            {
                return this.mGUID;
            }
            set
            {
                this.mGUID = value;
            }
        }

        public bool IsPopulated
        {
            get
            {
                return this.mIsPopulated;
            }
            set
            {
                this.mIsPopulated = value;
            }
        }

        public virtual string Location
        {
            get
            {
                return this.mLocation;
            }
            set
            {
                this.mLocation = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.RoleName;
            }
        }

        public virtual ObjectClass ObjectClass
        {
            get
            {
                return ObjectClass.group;
            }
        }
    }
}