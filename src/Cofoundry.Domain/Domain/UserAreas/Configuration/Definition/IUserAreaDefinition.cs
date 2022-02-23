namespace Cofoundry.Domain
{
    /// <summary>
    /// Implementing <see cref="IUserAreaDefinition"/> allows you to define a custom user area that is completely 
    /// separate to other user areas, but can take advantage of the same tools for handling and managing 
    /// users, registrations and sign in. This is what the Cofoundry admin panel uses for user accounts,
    /// other examples might be a client area or members only area of your website. The 
    /// username for a user must be unique for each user area, but the same username can exist
    /// in different user areas which allows a person to be a member of each user area. User areas
    /// are very distinct partitions and shouldn't be used for something where Roles and Permissions 
    /// might be more appropriate (e.g. different levels of membership).
    /// </summary>
    public interface IUserAreaDefinition
    {
        /// <summary>
        /// A unique 3 letter code identifying this user area. The cofoundry 
        /// user are uses the code "COF" so you can pick anything else!
        /// </summary>
        string UserAreaCode { get; }

        /// <summary>
        /// Display name of the area, used in the Cofoundry admin panel
        /// as the navigation link to manage your users. This should be singular
        /// because "Users" is appended to the link text.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if users in this area can sign in using a password. If this
        /// is <see langword="false"/> the password field will be <see langword="null"/> 
        /// and sign in will typically be via SSO or some other method. If this is 
        /// <see langword="true"/> then the email field is mandatory, because it is
        /// required for account recovery.
        /// </summary>
        bool AllowPasswordSignIn { get; }

        /// <summary>
        /// Indicates whether the user should sign in using thier email address
        /// as the username. If this is <see langword="false"/> then a separate
        /// username can be provided to sign in with.
        /// </summary>
        bool UseEmailAsUsername { get; }

        /// <summary>
        /// The path to redirect to if a user is not signed in, which is typically a
        /// page where the user can sign in. The path to the denied resource is appended to the
        /// query string of the <see cref="SignInPath"/> using the parameter name "ReturnUrl".
        /// If set to <see langword="null"/> then a 403 (Forbidden) error page will be 
        /// returned instead.
        /// </summary>
        string SignInPath { get; }

        /// <summary>
        /// Cofoundry creates an auth scheme for each user area, but only one can be the 
        /// default. In an application with multiple user areas a client can be signed in
        /// to multiple users at the same time, so the default scheme dictates which user
        /// area is authenticated by default when querying the "current user" and is therefore
        /// also use for checking permissions. It's rare that a site would implement more 
        /// than one custom user area, so in most cases this should be set to <see langword="true"/>.
        /// </summary>
        bool IsDefaultAuthScheme { get; }

        /// <summary>
        /// Configure any additional optional settings.
        /// </summary>
        /// <param name="options">
        /// The current configuration with any values from configuration providers (e.g. appsettings.json) applied.
        /// </param>
        void ConfigureOptions(UserAreaOptions options);
    }
}
