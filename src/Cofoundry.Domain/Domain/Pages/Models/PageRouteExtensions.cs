using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class PageRouteExtensions
    {
        public static IEnumerable<PageRoute> FilterByDirectory(this IEnumerable<PageRoute> routes, WebDirectoryRoute directory, int? localeId = null)
        {
            return routes
                .Where(r => (r.WebDirectory == null && directory == null) || r.WebDirectory == directory)
                .Where(r => (r.Locale == null && !localeId.HasValue) || r.Locale.LocaleId == localeId)
                ;
        }
    }
}
