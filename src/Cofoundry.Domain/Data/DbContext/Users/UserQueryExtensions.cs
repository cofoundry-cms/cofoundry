using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class UserQueryExtensions
    {
        public static IQueryable<User> FilterById(this IQueryable<User> users, int id)
        {
            var user = users
                .Where(u => u.UserId == id && !u.IsDeleted);

            return user;
        }

        public static IQueryable<User> FilterActive(this IQueryable<User> users)
        {
            var user = users
                .Where(u => !u.IsDeleted);

            return user;
        }

        public static IQueryable<User> FilterByUserArea(this IQueryable<User> users, string userArea)
        {
            var user = users
                .Where(u => u.UserAreaCode == userArea);

            return user;
        }
    }
}
