namespace Cofoundry.Domain;

/// <summary>
/// Finds a user with a specific username, returning <see langword="null"/> if the 
/// user could not be found. A user always has a username, however it may just
/// be a copy of the email address if the <see cref="IUserAreaDefinition.UseEmailAsUsername"/>
/// setting is set to true.
/// </summary>
public class GetUserMicroSummaryByUsernameQuery : IQuery<UserMicroSummary>
{
    public GetUserMicroSummaryByUsernameQuery() { }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="username">This query must be run against a specific user area.</param>
    /// <param name="userAreaCode">The username to use to locate the user.</param>
    public GetUserMicroSummaryByUsernameQuery(string username, string userAreaCode)
    {
        Username = username;
        UserAreaCode = userAreaCode;
    }

    /// <summary>
    /// This query must be run against a specific user area.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The username to use to locate the user. The value will be normalized
    /// before making the comparison.
    /// </summary>
    public string Username { get; set; }
}
