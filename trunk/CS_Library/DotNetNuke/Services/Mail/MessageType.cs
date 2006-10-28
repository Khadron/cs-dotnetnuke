namespace DotNetNuke.Services.Mail
{
    public enum MessageType
    {
        PasswordReminder = 0,
        ProfileUpdated = 1,
        UserRegistrationAdmin = 2,
        UserRegistrationPrivate = 3,
        UserRegistrationPublic = 4,
        UserRegistrationVerified = 5,
    }
}