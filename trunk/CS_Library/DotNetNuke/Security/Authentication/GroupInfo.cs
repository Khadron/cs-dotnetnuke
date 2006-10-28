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