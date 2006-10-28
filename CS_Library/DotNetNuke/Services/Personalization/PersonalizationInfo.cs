using System.Collections;

namespace DotNetNuke.Services.Personalization
{
    public class PersonalizationInfo
    {
        private bool _IsModified;
        private int _PortalId;
        private Hashtable _Profile;
        private int _UserId;

        public bool IsModified
        {
            get
            {
                return this._IsModified;
            }
            set
            {
                this._IsModified = value;
            }
        }

        public int PortalId
        {
            get
            {
                return this._PortalId;
            }
            set
            {
                this._PortalId = value;
            }
        }

        public Hashtable Profile
        {
            get
            {
                return this._Profile;
            }
            set
            {
                this._Profile = value;
            }
        }

        public int UserId
        {
            get
            {
                return this._UserId;
            }
            set
            {
                this._UserId = value;
            }
        }
    }
}