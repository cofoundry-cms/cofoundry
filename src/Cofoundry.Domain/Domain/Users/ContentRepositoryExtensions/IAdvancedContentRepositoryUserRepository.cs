using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the User entity.
    /// </summary>
    public interface IAdvancedContentRepositoryUserRepository
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

        /// <summary>
        /// Determines if a username is unique within a specific UserArea.
        /// Usernames only have to be unique per UserArea.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<bool> IsUsernameUnique(IsUsernameUniqueQuery query);

        #endregion

        #region commands

        /// <summary>
        /// A generic user creation command for use with Cofoundry users and
        /// other non-Cofoundry users. Does not send any email notifications.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddAsync(AddUserCommand command);

        /// <summary>
        /// Adds a user to the Cofoundry user area and sends a welcome notification
        /// containing a generated password which must be changed at first login.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created user.</returns>
        Task<int> AddCofoundryUserAsync(AddCofoundryUserCommand command);

        /// <summary>
        /// A generic user update command for use with Cofoundry users and
        /// other non-Cofoundry users.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateUserAsync(UpdateUserCommand command);

        /// <summary>
        /// Marks a user as deleted in the database (soft delete).
        /// </summary>
        /// <param name="userId">UserId of the role to delete.</param>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Updates the user account of the currently logged in user.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command);

        /// <summary>
        /// Updates the password of the currently logged in user, using the
        /// OldPassword field to authenticate the request.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateCurrentUserPasswordAsync(UpdateCurrentUserPasswordCommand command);

        #endregion
    }
}
