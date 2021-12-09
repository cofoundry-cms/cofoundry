using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Describes a password policy using both a short <see cref="Description"/> 
    /// and a more detailed list of <see cref="Criteria"/>.
    /// </summary>
    public class PasswordPolicyDescription
    {
        /// <summary>
        /// A brief description of the policy highlighting the
        /// main criteria e.g. "Passwords must be between 10 and 300 characters.".
        /// This description is designed to be displayed to users to help them choose
        /// their new password.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A full list of password requirements extracted from each of
        /// the validators in the new password policy e.g. "Must be at least 10 characters.", 
        /// "Must be at less than 300 characters.". A developer may choose to list the requirements 
        /// in full to help a user choose their new password, however the list can include a tedious 
        /// list of edge cases such as "Password must not be the same as your current password.".
        /// </summary>
        public ICollection<string> Criteria { get; set; }

        /// <summary>
        /// A collection of HTML attributes that describe the policy e.g. "minlength", "maxlength"
        /// or "passwordrules".
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; }
    }
}
