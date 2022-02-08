namespace Cofoundry.Domain
{
    /// <summary>
    /// User information relating to a user sign in request. This includes the bare-minimum
    /// of data to resolve a sign in request, including properties that may need action taken
    /// such as <see cref="RequirePasswordChange"/>.
    /// </summary>
    public class UserSignInInfo
    {
        /// <summary>
        /// Database id of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Code identifier of the user area this user belongs to. Each user 
        /// must be assigned to a user area (but not more than one).
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// True if the account has been marked as as verified or activated.  One common way 
        /// of verification is via an email sign-up notification.
        /// </summary>>
        public bool IsAccountVerified { get; set; }

        /// <summary>
        /// True if the password hash version is out of date. If this true then the password 
        /// needs updating with the latest hash.
        /// </summary>
        public bool PasswordRehashNeeded { get; set; }
    }
}
