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