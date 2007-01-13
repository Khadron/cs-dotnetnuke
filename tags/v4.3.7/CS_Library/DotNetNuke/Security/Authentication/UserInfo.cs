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
namespace DotNetNuke.Security.Authentication
{
    public class UserInfo : Entities.Users.UserInfo, IAuthenticationObjectBase
    {
        private string mAssistant;
        private bool mAuthenticationExists;
        private string mCName;
        private string mDepartment;
        private string mDistinguishedName;
        private string mGUID;
        private string mHomePhone;
        private bool mIsAuthenticated;
        private string mLocation;
        private string mManager;
        private string mPrincipalName;
        private string msAMAccountName;

        public UserInfo()
        {
            this.mGUID = "";
            this.mLocation = "";
            this.mCName = "";
            this.mPrincipalName = "";
            this.mDistinguishedName = "";
            this.msAMAccountName = "";
            this.mAuthenticationExists = false;
        }

        public string Assistant
        {
            get
            {
                return this.mAssistant;
            }
            set
            {
                this.mAssistant = value;
            }
        }

        public bool AuthenticationExists
        {
            get
            {
                return this.mAuthenticationExists;
            }
            set
            {
                this.mAuthenticationExists = value;
            }
        }

        public string CName
        {
            get
            {
                return this.mCName;
            }
            set
            {
                this.mCName = value;
            }
        }

        public string Department
        {
            get
            {
                return this.mDepartment;
            }
            set
            {
                this.mDepartment = value;
            }
        }

        public string DistinguishedName
        {
            get
            {
                return this.mDistinguishedName;
            }
            set
            {
                this.mDistinguishedName = value;
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

        public string HomePhone
        {
            get
            {
                return this.mHomePhone;
            }
            set
            {
                this.mHomePhone = value;
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

        public string Manager
        {
            get
            {
                return this.mManager;
            }
            set
            {
                this.mManager = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.sAMAccountName;
            }
        }

        public virtual ObjectClass ObjectClass
        {
            get
            {
                return ObjectClass.person;
            }
        }

        public string PrincipalName
        {
            get
            {
                return this.mPrincipalName;
            }
            set
            {
                this.mPrincipalName = value;
            }
        }

        public string sAMAccountName
        {
            get
            {
                return this.msAMAccountName;
            }
            set
            {
                this.msAMAccountName = value;
            }
        }
    }
}