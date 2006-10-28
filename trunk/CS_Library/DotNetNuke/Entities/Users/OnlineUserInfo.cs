namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The OnlineUserInfo class provides an Entity for an online user
    /// </Summary>
    public class OnlineUserInfo : BaseUserInfo
    {
        private int _UserID;

        /// <Summary>Gets and sets the User Id for this online user</Summary>
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
    }
}