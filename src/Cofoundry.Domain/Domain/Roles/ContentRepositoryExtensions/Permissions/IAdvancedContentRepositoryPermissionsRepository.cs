namespace Cofoundry.Domain;

/// <summary>
/// Queries for permissions in the Cofoundry identity system.
/// </summary>
public interface IAdvancedContentRepositoryPermissionsRepository
{
    /// <summary>
    /// Returns all IPermission instances registered with Cofoundry.
    /// </summary>
    IAdvancedContentRepositoryGetAllPermissionsQueryBuilder GetAll();
}