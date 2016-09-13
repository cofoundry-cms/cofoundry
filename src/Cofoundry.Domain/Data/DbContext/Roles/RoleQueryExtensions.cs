using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class RoleQueryExtensions
    {
        public static IQueryable<Role> FilterById(this IQueryable<Role> roles, int id)
        {
            var role = roles
                .Where(u => u.RoleId == id);

            return role;
        }
    }
}
