namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The AnonymousUserInfo class provides an Entity for an anonymous user
    /// </Summary>
    public class AnonymousUserInfo : BaseUserInfo
    {
        private string _UserID;

        /// <Summary>Gets and sets the User Id for this online user</Summary>
        public string UserID
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
    }
}