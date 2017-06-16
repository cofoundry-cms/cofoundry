using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CustomEntityExtensions
    {
        public static IQueryable<CustomEntity> FilterById(this IQueryable<CustomEntity> customEntities, int id)
        {
            var result = customEntities
                .Where(i => i.CustomEntityId == id);

            return result;
        }
    }
}
