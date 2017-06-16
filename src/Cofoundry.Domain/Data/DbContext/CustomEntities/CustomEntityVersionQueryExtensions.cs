using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CustomEntityVersionQueryExtensions
    {
        public static IQueryable<CustomEntityVersion> FilterByCustomEntityId(this IQueryable<CustomEntityVersion> customEntities, int id)
        {
            var result = customEntities
                .Where(e => e.CustomEntityId == id);

            return result;
        }

        public static IQueryable<CustomEntityVersion> FilterByCustomEntityDefinitionCode(this IQueryable<CustomEntityVersion> customEntities, string customEntityDefinitionCode)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == customEntityDefinitionCode);

            return result;
        }

        public static IQueryable<CustomEntityVersion> FilterByActiveLocales(this IQueryable<CustomEntityVersion> customEntities)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == null || e.CustomEntity.Locale.IsActive);

            return result;
        }

        public static IQueryable<CustomEntityVersion> FilterByLocale(this IQueryable<CustomEntityVersion> customEntities, int? localeId)
        {
            var result = customEntities
                .Where(e => e.CustomEntity.LocaleId == localeId);

            return result;
        }
    }
}
