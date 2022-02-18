using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a user in the Cofoundry custom identity system. Users can be partitioned into
    /// different 'User Areas', which enables the identity system used by the Cofoundry administration area 
    /// to be reused for other purposes, but this isn't a common scenario and often there will only be the 
    /// Cofoundry user area.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Database id of the <see cref="User"/>.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The first name is not required.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is not required.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// An optional display-friendly name. This is capped at 150 characters to
        /// match the <see cref="Username"/>, which may be used as the username in 
        /// some cases. If <see cref="UsernameOptions.UseAsDisplayName"/> is set to
        /// <see langword="true"/> then this field will be a copy of the <see cref="Username"/>
        /// field.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The users primary email address whcih can be used to comminicate with the user.
        /// This can be optional depending on the user area settings, if 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to <see langword="true"/>
        /// then this field is required and is used in the <see cref="Username"/> field. 
        /// The formatting of this version should not be altered so that any privacy protections
        /// are left in place e.g. tricks like "plus addressing" should not be removed.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A copy of <see cref="Email"/> that is used for uniqueness checks, which
        /// can differ to prevent duplicates from legitimate variants that resolve 
        /// to the same email inbox e.g. "plus addressing" or interchangeable domains.
        /// </summary>
        public string UniqueEmail { get; set; }

        /// <summary>
        /// The username that is used as the user identifier e.g. "JArnold" or "jarnold@example.com". 
        /// It is always required and depending on the user area settings this might be a normalized 
        /// copy of the email address. Although this version of the username does go through a normalization 
        /// process it is generally unaltered, whereas the <see cref="UniqueUsername"/> field is formatted by a
        /// "uniquification" process which can be more involved, because that field used for 
        /// comparisons when signing in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// A copy of <see cref="Username"/> that is formatted to standardize casing and any other
        /// required formatting irregularities e.g. "jarnold" or "jarnold@example.com". This field 
        /// is used for uniqueness checks and user lookups.
        /// </summary>
        public string UniqueUsername { get; set; }

        /// <summary>
        /// The domain name associated with the users <see cref="UniqueEmail"/> if one is supplied.
        /// Note that the email domain may not be a direct mapping to the value in <see cref="Email"/> 
        /// e.g. the user's mail provider maps multiple domains to the same host e.g. "googlemail.com"
        /// and "gmail.com" may be consolidated depending on configuration.
        /// </summary>
        public int? EmailDomainId { get; set; }

        /// <summary>
        /// The domain name associated with the users <see cref="UniqueEmail"/> if one is supplied.
        /// Note that the email domain may not be a direct mapping to the value in <see cref="Email"/> 
        /// e.g. the user's mail provider maps multiple domains to the same host e.g. "googlemail.com"
        /// and "gmail.com" may be consolidated depending on configuration.
        /// </summary>
        public EmailDomain EmailDomain { get; set; }

        /// <summary>
        /// The users hashed password value.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Cofoundry supports upgradable password hashing and this integer value
        /// maps to the <see cref="Domain.PasswordHashVersion"/> enum to 
        /// tell us which hash function type to use. An integer outside of the 
        /// <see cref="Domain.PasswordHashVersion"/> enum range
        /// can be used to set up a completed custom hash provider.
        /// </summary>
        public int? PasswordHashVersion { get; set; }

        /// <summary>
        /// <para>
        /// A random string that gets updated when key user identity fields are changed, such 
        /// as a password or username. During user session validation this field is checked to
        /// detect any changes and invalidate any out of date sessions. For example, if I am signed in 
        /// into the admin panel on a Latop and a PC, and change my password on the Laptop session, then
        /// I would be signed out of the session on the PC.
        /// </para>
        /// <para>
        /// This field is synonymous with the SecurityStamp field in ASP.NET Identity.
        /// </para>
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Used for soft deletes so we can maintain old relations and
        /// archive data. Deleted accounts are anonymized and are therefore
        /// not recoverable.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Setting <see cref="DeactivatedDate"/> deactivates a user account, 
        /// preventing them from logging in or taking any actions, but does
        /// not remove any data or prevent them from being queried.
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }

        /// <summary>
        /// The date and time the password was last changed or the that the password
        /// was first set (account create date)
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        /// <summary>
        /// The date and time the user last signed in, if ever.
        /// </summary>
        public DateTime? LastSignInDate { get; set; }

        /// <summary>
        /// The date and time the user signed-in in before the <see cref="LastSignInDate"/>.
        /// </summary>
        /// <remarks>
        /// We can consider dropping this now we have a sign in auditing table, I don't think it's
        /// used for anything else
        /// </remarks>
        public DateTime? PreviousSignInDate { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// The <see cref="Role"/> that this user is assigned to. The role is required
        /// and determines the permissions available to the user
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The <see cref="Role"/> that this user is assigned to. The role is 
        /// required and determines the permissions available to the user
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// The Cofoundry user system can be partitioned into user areas. Each
        /// user area has a 3 letter code, e.g. The Cofoundry admin area code is
        /// 'COF'.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The Cofoundry user system can be partitioned into user areas. This enables
        /// reuse of user functionality to create custom sign in areas in your application.
        /// </summary>
        public virtual UserArea UserArea { get; set; }

        /// <summary>
        /// There can be only one (system account). This is an account that
        /// can be impersonated and used to import data or in special
        /// cases used to elevate privileges
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// A generic verification date that can be used to mark an account as verified
        /// or activated. One common way of verification is via an email sign-up notification.
        /// Given that account verification is not tied to any specified data, it is not automatically
        /// cleared when for example an email address is updated. The management of this property
        /// is left to the implementor.
        /// </summary>
        public DateTime? AccountVerifiedDate { get; set; }

        /// <summary>
        /// The date and time the user was created
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The id of the user that created this user. Nullable
        /// to allow the first user to be created.
        /// </summary>
        public int? CreatorId { get; set; }

        /// <summary>
        /// The user that created this user. Nullable
        /// to allow the first user to be created.
        /// </summary>
        public virtual User Creator { get; set; }

        /// <summary>
        /// <see langword="true"/> if the user is active and not deleted. The system 
        /// user account will return <see langword="true"/>, to exlude it use <see cref="CanSignIn"/>
        /// instead.
        /// </summary>
        public bool IsEnabled()
        {
            return !DeletedDate.HasValue && !DeactivatedDate.HasValue;
        }

        /// <summary>
        /// <see langword="true"/> if the user is allowed to be signed in i.e. is active, not
        /// deleted and not the system user.
        /// </summary>
        public bool CanSignIn()
        {
            return !IsSystemAccount && !DeletedDate.HasValue && !DeactivatedDate.HasValue;
        }
    }
}