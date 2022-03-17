namespace Cofoundry.Domain;

/// <summary>
/// Finds a user with a specific email address returning <see langword="null"/> 
/// if the user could not be found. Note that if the user area does not use email 
/// addresses as the username then the email field is optional and may be empty.
/// </summary>
public class GetUserMicroSummaryByEmailQuery : IQuery<UserMicroSummary>
{
    public GetUserMicroSummaryByEmailQuery() { }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="email">
    /// The email address to use to locate the user. The value will be normalized
    /// before making the comparison.
    /// </param>
    /// <param name="userAreaCode">This query must be run against a specific user area.</param>
    public GetUserMicroSummaryByEmailQuery(string email, string userAreaCode)
    {
        Email = email;
        UserAreaCode = userAreaCode;
    }

    /// <summary>
    /// This query must be run against a specific user area.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The email address to use to locate the user.
    /// </summary>
    public string Email { get; set; }
}
