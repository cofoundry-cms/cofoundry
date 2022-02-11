using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserMicroSummary objects.
    /// </summary>
    public interface IUserMicroSummaryMapper
    {
        /// <summary>
        /// Maps the <see cref="UserMicroSummary"/> properties from a <see cref="User"/> record 
        /// into a new <typeparamref name="TModel"/> projection.
        /// </summary>
        /// <typeparam name="TModel"><see cref="UserMicroSummary"/> or dirived type.</typeparam>
        /// <param name="dbUser">User record from the database to map from.</param>
        TModel Map<TModel>(User dbUser) where TModel : UserMicroSummary, new();

        /// <summary>
        /// Maps a <see cref="User"/> record from the database into a <see cref="UserMicroSummary"/> 
        /// projection. If the record is <see langword="null"/> then <see langword="null"/> is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database to map from.</param>
        UserMicroSummary Map(User dbUser);
    }
}
