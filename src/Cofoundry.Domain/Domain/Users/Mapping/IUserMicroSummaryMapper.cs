using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to UserMicroSummary objects.
/// </summary>
public interface IUserMicroSummaryMapper
{
    /// <summary>
    /// Maps a <see cref="User"/> record from the database into a <see cref="UserMicroSummary"/> 
    /// projection. If the record is <see langword="null"/> then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="dbUser">User record from the database to map from.</param>
    [return: NotNullIfNotNull(nameof(dbUser))]
    UserMicroSummary? Map(User? dbUser);
}
