namespace DotNetNuke.Security.Membership
{
    public enum PasswordUpdateStatus
    {
        Success = 0,
        PasswordMissing = 1,
        PasswordNotDifferent = 2,
        PasswordResetFailed = 3,
        PasswordInvalid = 4,
        PasswordMismatch = 5,
        InvalidPasswordAnswer = 6,
        InvalidPasswordQuestion = 7,
    }
}