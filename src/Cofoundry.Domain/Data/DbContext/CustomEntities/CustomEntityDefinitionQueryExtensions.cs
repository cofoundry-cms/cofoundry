using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CustomEntityDefinitionQueryExtensions
    {
        public static IQueryable<CustomEntityDefinition> FilterByCode(this IQueryable<CustomEntityDefinition> customEntityDefinitions, string code)
        {
            var result = customEntityDefinitions
                .Where(i => i.CustomEntityDefinitionCode == code);

            return result;
        }
    }
}
