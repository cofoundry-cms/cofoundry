using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class UserQueryExtensions
    {
        /// <summary>
        /// Filters the result to include only the user with the specified UserId
        /// </summary>
        public static IQueryable<User> FilterById(this IQueryable<User> users, int id)
        {
            var user = users
                .Where(u => u.UserId == id && !u.IsDeleted);

            return user;
        }

        /// <summary>
        /// Filters the collection to only include users who have an active
        /// account (i.e. not deleted). This includes the system user account; to exclude
        /// it use <see cref="FilterCanLogIn"/> instead.
        /// </summary>
        public static IQueryable<User> FilterActive(this IQueryable<User> users)
        {
            var user = users
                .Where(u => !u.IsDeleted);

            return user;
        }

        /// <summary>
        /// Returns only users that are allowed to be logged in i.e. is not
        /// deleted and is not the system user.
        /// </summary>
        public static IQueryable<User> FilterCanLogIn(this IQueryable<User> users)
        {
            var user = users
                .Where(u => !u.IsSystemAccount && !u.IsDeleted);

            return user;
        }

        /// <summary>
        /// Filters the collection to only include users whoa re assigned to the specified user area
        /// </summary>
        public static IQueryable<User> FilterByUserArea(this IQueryable<User> users, string userArea)
        {
            var user = users
                .Where(u => u.UserAreaCode == userArea);

            return user;
        }

        /// <summary>
        /// Includes the required entities to map a <see cref="Domain.UserSummary"/> projection.
        /// </summary>
        public static IQueryable<User> IncludeForSummary(this IQueryable<User> users)
        {
            var user = users
                .Include(u => u.Role)
                .Include(u => u.Creator);

            return user;
        }
    }
}
