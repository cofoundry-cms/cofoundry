using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the User entity.
    /// </summary>
    public interface IContentRepositoryUserRepository
    {
        #region queries

        /// <summary>
        /// Retrieve the currently logged in user. If
        /// there are multiple users then this only applies to the
        /// UserArea set as the default schema.
        /// </summary>
        IContentRepositoryCurrentUserQueryBuilder GetCurrent();

        /// <summary>
        /// Retrieve a user by a unique database id.
        /// </summary>
        /// <param name="userId">UserId of the user to get.</param>
        IContentRepositoryUserByIdQueryBuilder GetById(int userId);

        /// <summary>
        /// Search for users, returning paged lists of data.
        /// </summary>
        IContentRepositoryUserSearchQueryBuilder Search();

        #endregion
    }
}
