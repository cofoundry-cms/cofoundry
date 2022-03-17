namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// Find a role with the specified role code, returning a <see cref="RoleDetails"/> 
/// projection if one is found, otherwise <see langword="null"/>. Roles only
/// have a RoleCode if they have been generated from code rather than the GUI. 
/// For GUI generated roles use <see cref="GetRoleDetailsByIdQuery"/>.
/// </para>
/// <para>
/// Roles are cached, so repeat uses of this query is inexpensive.
/// </para>
/// </summary>
public class GetRoleDetailsByRoleCodeQuery : IQuery<RoleDetails>
{
    public GetRoleDetailsByRoleCodeQuery()
    {
    }

    /// <summary>
    /// Initializes the query with the specified <paramref name="roleCode"/>
    /// </summary>
    /// <param name="roleCode">The code to find a matching role with. Codes are 3 characters long (fixed length).</param>
    public GetRoleDetailsByRoleCodeQuery(string roleCode)
    {
        RoleCode = roleCode;
    }

    /// <summary>
    /// The code to find a matching role with. Roles only have a RoleCode 
    /// if they have been generated from code rather than the GUI. For GUI generated roles
    /// use <see cref="GetRoleDetailsByIdQuery"/>. Codes are 3 characters long (fixed length).
    /// </summary>
    public string RoleCode { get; set; }
}
