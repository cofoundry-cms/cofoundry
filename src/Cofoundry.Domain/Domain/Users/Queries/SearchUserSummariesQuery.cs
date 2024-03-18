﻿namespace Cofoundry.Domain;

/// <summary>
/// Searches users based on simple filter criteria and returns a paged result. 
/// </summary>
public class SearchUserSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<UserSummary>>
{
    /// <summary>
    /// Users are partitioned by UserArea and are treated as completely
    /// separate, therefore a UserAreaCode is required for the search.
    /// </summary>
    [Required]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// Filter by first or last name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filter by email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Filter by username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Filter by whether the account is active or not.
    /// </summary>
    public UserAccountStatusFilter AccountStatus { get; set; } = UserAccountStatusFilter.Active;
}
