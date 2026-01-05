namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{User}"/>.
/// </summary>
public static class UserQueryExtensions
{
    extension(IQueryable<User> users)
    {
        /// <summary>
        /// Filters the result to include only the user with the specified UserId
        /// </summary>
        public IQueryable<User> FilterById(int id)
        {
            var user = users.Where(u => u.UserId == id);

            return user;
        }

        /// <summary>
        /// Filters the collection to only include users who are not deleted (i.e. not deleted).
        /// This will not filter out inactive users.
        /// </summary>
        public IQueryable<User> FilterNotDeleted()
        {
            var user = users.Where(u => !u.DeletedDate.HasValue);

            return user;
        }

        /// <summary>
        /// Filters the collection to only include users who have an active account that has not 
        /// been deleted. This includes the system user account; to exclude it use 
        /// <see cref="FilterCanSignIn"/> instead.
        /// </summary>
        public IQueryable<User> FilterEnabled()
        {
            var user = users.Where(u => !u.DeletedDate.HasValue && !u.DeactivatedDate.HasValue);

            return user;
        }

        /// <summary>
        /// Returns only users that are allowed to be signed in i.e. is active, not
        /// deleted and is not the system user.
        /// </summary>
        public IQueryable<User> FilterCanSignIn()
        {
            var user = users.Where(u =>
                !u.IsSystemAccount
                && !u.DeletedDate.HasValue
                && !u.DeactivatedDate.HasValue);

            return user;
        }

        /// <summary>
        /// Filters the collection to only exclude the system user account.
        /// </summary>
        public IQueryable<User> FilterNotSystemAccount()
        {
            var user = users.Where(u => !u.IsSystemAccount);

            return user;
        }

        /// <summary>
        /// Filters the collection to only include users whoa re assigned to the specified user area
        /// </summary>
        public IQueryable<User> FilterByUserArea(string userArea)
        {
            var user = users.Where(u => u.UserAreaCode == userArea);

            return user;
        }

        /// <summary>
        /// Includes the required entities to map a <see cref="Domain.UserSummary"/> projection.
        /// </summary>
        public IQueryable<User> IncludeForSummary()
        {
            var user = users
                .Include(u => u.Role)
                .Include(u => u.Creator);

            return user;
        }
    }
}
