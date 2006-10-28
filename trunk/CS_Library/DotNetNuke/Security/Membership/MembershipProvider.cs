using System.Collections;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;

namespace DotNetNuke.Security.Membership
{
    public abstract class MembershipProvider
    {
        // singleton reference to the instantiated object
        private static MembershipProvider objProvider = null;

        // constructor
        static MembershipProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (MembershipProvider)Reflection.CreateObject("members");
        }

        // return the provider
        public static MembershipProvider Instance()
        {
            return objProvider;
        }

        public abstract bool CanEditProviderProperties { get; }
        public abstract int MaxInvalidPasswordAttempts { get; set; }
        public abstract int MinPasswordLength { get; set; }
        public abstract int MinNonAlphanumericCharacters { get; set; }
        public abstract int PasswordAttemptWindow { get; set; }
        public abstract PasswordFormat PasswordFormat { get; set; }
        public abstract bool PasswordResetEnabled { get; set; }
        public abstract bool PasswordRetrievalEnabled { get; set; }
        public abstract string PasswordStrengthRegularExpression { get; set; }
        public abstract bool RequiresQuestionAndAnswer { get; set; }

        //Users
        public abstract bool ChangePassword(UserInfo user, string oldPassword, string newPassword);
        public abstract bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer);
        public abstract UserCreateStatus CreateUser(ref UserInfo user);
        public abstract bool DeleteUser(UserInfo user);
        public abstract string GeneratePassword();
        public abstract string GeneratePassword(int length);
        public abstract string GetPassword(UserInfo user, string passwordAnswer);
        public abstract UserInfo GetUser(int portalId, int userId, bool isHydrated);
        public abstract UserInfo GetUserByUserName(int portalId, string username, bool isHydrated);
        public abstract int GetUserCountByPortal(int portalId);
        public abstract void GetUserMembership(ref UserInfo user);
        public abstract ArrayList GetUnAuthorizedUsers(int portalId, bool isHydrated);
        public abstract ArrayList GetUsers(int portalId, bool isHydrated, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByEmail(int portalId, bool isHydrated, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByUserName(int portalId, bool isHydrated, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByProfileProperty(int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords);
        public abstract string ResetPassword(UserInfo user, string passwordAnswer);
        public abstract bool UnLockUser(UserInfo user);
        public abstract void UpdateUser(UserInfo user);
        public abstract UserInfo UserLogin(int portalId, string username, string password, string verificationCode, ref UserLoginStatus loginStatus);

        //Users Online
        public abstract void DeleteUsersOnline(int TimeWindow);
        public abstract ArrayList GetOnlineUsers(int PortalId);
        public abstract bool IsUserOnline(UserInfo user);
        public abstract void UpdateUsersOnline(Hashtable UserList);

        //Legacy
        public abstract void TransferUsersToMembershipProvider();
    }
}