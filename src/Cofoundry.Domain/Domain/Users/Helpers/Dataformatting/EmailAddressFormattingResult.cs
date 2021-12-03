using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The result of a call to <see cref="IUserDataFormatter.FormatEmailAddress"/>
    /// which contains all the email address formats required to update the email 
    /// address on a <see cref="Data.User"/> record.
    /// </summary>
    public class EmailAddressFormattingResult
    {
        /// <summary>
        /// A cleaned up version of the email address which can be used to comminicate with the 
        /// user. The formatting of this version should not be altered so that any privacy protections
        /// are left in place e.g. tricks like "plus addressing" should not be removed.
        /// </summary>
        public string NormalizedEmailAddress { get; set; }

        /// <summary>
        /// A version of the email address that is used for uniqueness checks, which
        /// can differ to prevent duplicates from legitimate variants that resolve 
        /// to the same email inbox e.g. "plus addressing" or interchangeable domains.
        /// </summary>
        public string UniqueEmailAddress { get; set; }

        /// <summary>
        /// The domain name part of an email address (the part after the @). This is the
        /// version fo the domain found in <see cref="UniqueEmailAddress"/> i.e. it
        /// has been "uniquified".
        /// </summary>
        public EmailDomainName Domain { get; set; }
    }
}
