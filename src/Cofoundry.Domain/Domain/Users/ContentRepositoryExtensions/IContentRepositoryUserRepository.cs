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
        /// Retieve a user by a unique database id.
        /// </summary>
        IContentRepositoryUserByIdQueryBuilder GetById(int userId);

        /// <summary>
        /// Search for users, returning paged lists of data.
        /// </summary>
        IContentRepositoryUserSearchQueryBuilder Search();

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        Task<bool> IsUsernameUniqueAsync(IsUsernameUniqueQuery query);

        #endregion

        #region commands

        /// <summary>
        /// A generic user creation command for use with Cofoundry users and
        /// other non-Cofoundry users. Does not send any email notifications.
        /// </summary>
        Task AddAsync(AddUserCommand command);

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        Task UpdateUserAsync(UpdateUserCommand command);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        Task UpdateCurrentUserPasswordAsync(UpdateCurrentUserPasswordCommand command);

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">Id of the user to delete.</param>
        Task DeleteUserAsync(int userId);

        #endregion
    }
}
