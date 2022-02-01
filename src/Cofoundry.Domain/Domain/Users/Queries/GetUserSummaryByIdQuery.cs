using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user by a database id returning a <see cref="UserSummary"/> projection 
    /// if it is found, otherwise <see langword="null"/>.
    /// </summary>
    public class GetUserSummaryByIdQuery : IQuery<UserSummary>
    {
        public GetUserSummaryByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with the specified user id.
        /// </summary>
        /// <param name="userId">Database id of the user.</param>
        public GetUserSummaryByIdQuery(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Database id of the user to find.
        /// </summary>
        public int UserId { get; set; }
    }
}
