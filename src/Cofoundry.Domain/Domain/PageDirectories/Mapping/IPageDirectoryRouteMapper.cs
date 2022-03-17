using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// A mapper for mapping <see cref="PageDirectoryRoute"/> projections.
/// </summary>
public interface IPageDirectoryRouteMapper
{
    /// <summary>
    /// Maps a <see cref="PageDirectory"/> data from the database to a
    /// collection of <see cref="PageDirectoryRoute"/> projections.
    /// </summary>
    /// <param name="dbPageDirectories">
    /// Entity Framework query results to map. The query must include the 
    /// <see cref="PageDirectory.PageDirectoryLocales"/> and <see cref="PageDirectory.AccessRules"/> relations.
    /// </param>
    ICollection<PageDirectoryRoute> Map(IReadOnlyCollection<PageDirectory> dbPageDirectories);
}
